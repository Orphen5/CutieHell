﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour {

    private Transform player;
    private Player playerScript;
    private CapsuleCollider playerCapsuleCollider;

    private const float xSpeed = 2.5f;
    private const float ySpeed = 1.2f;
    private const float yMinLimit = -20f;
    private const float yMaxLimit = 40f;
    private const float lerpSpeed = 0.1f;
    private const float transitionTime = 0.55f;
    private float timeOnTransition = 10.0f;
    private Player.CameraState lastState;
    [HideInInspector]
    public float timeSinceLastAction = 0.0f;
    [HideInInspector]
    public bool slowAction, fastAction;

    private int collisionLayers;
    private float x;
    [HideInInspector]
    public float y;

    /* Player camera values */
    public float distance;
    public float cameraX;
    public float cameraY;
    public float focusDistance;
    public float focusX;
    public float focusY;
    public float fov;


    /* Turret camera values */
    public float t_distance;
    public float t_cameraY;
    public float t_fov;

    /*Canon camera values*/
    public float lerpTowardsCanonTargetDecal; 

    [Header("Fog setup")]
    public float fogDistance;
    public float yFogMin;
    public float yFogMax;

    private void Awake()
    {
        x = 0f;
        y = 0f;
        slowAction = fastAction = false;
        player = GameObject.Find("Player").transform;
        playerScript = player.GetComponent<Player>();
        playerCapsuleCollider = player.GetComponent<CapsuleCollider>();

        distance = 2.8f;
        cameraX = 0.45f;
        cameraY = 1.8f;
        focusDistance = 0.65f;
        focusX = 0.55f;
        focusY = 1.65f;
        fov = 60f;

        t_distance = -2.2f;
        t_cameraY = 0.6f;
        t_fov = 40f;
    }

    private void Start()
    {
        Vector2 angles = transform.eulerAngles;
        x = 35.5f;
        y = 0.0f;
        lastState = playerScript.cameraState;
    }

    private void Update()
    {
        RotateCamera();
    }

    private void RotateCamera()
    {
        if(!GameManager.instance.gameIsPaused)
        {
            if (InputManager.instance.GetRightStickLeft() || InputManager.instance.GetRightStickRight())
                x += xSpeed * InputManager.instance.GetRightStickHorizontalSqrValue();
            if (InputManager.instance.GetRightStickUp() || InputManager.instance.GetRightStickDown())
                y += ySpeed * InputManager.instance.GetRightStickVerticalSqrValue();

            if (playerScript.cameraState != lastState)
            {
                timeOnTransition = 0.0f;
            }
            switch (playerScript.cameraState)
            {
                case Player.CameraState.STILL:
                    break;
                case Player.CameraState.MOVE:
                    {

                        //if (this.transform.parent != null) this.transform.parent = null;
                        y = ClampAngle(y, yMinLimit, yMaxLimit);
                        Quaternion rotation = Quaternion.Euler(y, x, 0);
                        float noCollisionDistance = distance;

                        for (float zOffset = distance; zOffset >= 0.5f; zOffset -= 0.025f)
                        {
                            noCollisionDistance = zOffset;
                            Vector3 tempPos = rotation * new Vector3(cameraX, cameraY, -noCollisionDistance) + player.position;

                            if (DoubleViewingPosCheck(tempPos, zOffset))
                            {
                                break;
                            }
                        }
                        if (timeOnTransition < transitionTime)
                        {
                            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fov, 0.1f);
                            timeOnTransition += Time.deltaTime;
                            this.transform.position = Vector3.Lerp(this.transform.position, rotation * new Vector3(cameraX, cameraY, -noCollisionDistance) + player.position, timeOnTransition / 2f);
                        }
                        else
                        {
                            Camera.main.fieldOfView = fov;
                            Vector3 position = rotation * new Vector3(cameraX, cameraY, -noCollisionDistance) + player.position;
                            this.transform.position = position;
                        }
                        if (timeSinceLastAction < 0.5f)
                        {
                            timeSinceLastAction += Time.deltaTime;
                            if (fastAction)
                            {
                                SetPlayerDirection(rotation.eulerAngles.y, 0.7f);
                            }
                            else if(slowAction)
                            {
                                SetPlayerDirection(rotation.eulerAngles.y, 0.2f);
                            }
                            else
                            {
                                SetPlayerDirection(rotation.eulerAngles.y);//, playerScript.rb.velocity.magnitude / 10.0f);
                            }
                        }
                        else
                        {
                            fastAction = slowAction = false;
                        }
                        this.transform.LookAt(player.transform.position + rotation * Vector3.up * focusY + rotation * Vector3.right * focusX + rotation * Vector3.forward * focusDistance);
                    }

                    break;
                case Player.CameraState.WOLF:
                    break;
                case Player.CameraState.FOG:
                    {
                        y = ClampAngle(y, yFogMin, yFogMax);                        

                        Quaternion rotation = Quaternion.Euler(y, x, 0);
                        float noCollisionDistance = distance;

                        for (float zOffset = fogDistance; zOffset >= 0.5f; zOffset -= 0.025f)
                        {
                            noCollisionDistance = zOffset;
                            Vector3 tempPos = rotation * new Vector3(cameraX, cameraY, -noCollisionDistance) + player.position;

                            if (DoubleViewingPosCheck(tempPos, zOffset))
                            {
                                break;
                            }
                        }

                        if (timeOnTransition < transitionTime)
                        {
                            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, fov, 0.1f);
                            timeOnTransition += Time.deltaTime;
                            this.transform.position = Vector3.Lerp(this.transform.position, rotation * new Vector3(cameraX, cameraY, -noCollisionDistance) + player.position, timeOnTransition / 2f);
                        }
                        else
                        {
                            Camera.main.fieldOfView = fov;
                            this.transform.position = rotation * new Vector3(cameraX, cameraY, -noCollisionDistance) + player.position;
                        }

                        if (timeSinceLastAction < 0.5f)
                        {
                            timeSinceLastAction += Time.deltaTime;
                            if (fastAction)
                            {
                                SetPlayerDirection(rotation.eulerAngles.y, 0.7f);
                            }
                            else if (slowAction)
                            {
                                SetPlayerDirection(rotation.eulerAngles.y, 0.2f);
                            }
                            else
                            {
                                SetPlayerDirection(rotation.eulerAngles.y);//, playerScript.rb.velocity.magnitude / 10.0f);
                            }
                        }
                        else
                        {
                            fastAction = slowAction = false;
                        }
                        this.transform.LookAt(player.transform.position + rotation * Vector3.up * focusY + rotation * Vector3.right * focusX + rotation * Vector3.forward * focusDistance);
                    }
                    break;
                case Player.CameraState.BATTURRET:
                    {
                        y = ClampAngle(y, yMinLimit, yMaxLimit);
                        Quaternion rotation = Quaternion.Euler(y, x, 0);
                        playerScript.currentTrap.rotatingHead.rotation = rotation;
                        this.transform.SetParent(playerScript.currentTrap.rotatingHead);
                        if (timeOnTransition < transitionTime)
                        {
                            timeOnTransition += Time.deltaTime;
                            this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, new Vector3(0.0f, t_cameraY, t_distance), timeOnTransition / 2f);
                            Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, t_fov, 0.1f);
                        }
                        else
                        {
                            Camera.main.fieldOfView = t_fov;
                            this.transform.localPosition = new Vector3(0.0f, t_cameraY, t_distance);
                        }
                        this.transform.localRotation = Quaternion.identity;
                        SetPlayerDirection(rotation.eulerAngles.y);
                        break;
                    }
                case Player.CameraState.CANONTURRET:
                    {
                        if (playerScript.currentTrap.canonTargetDecal.gameObject.activeSelf)
                        {
                            Vector3 canonTargetDecalDirection = playerScript.currentTrap.canonTargetDecal.transform.position - transform.position;
                            playerScript.currentTrap.rotatingHead.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(canonTargetDecalDirection), Time.deltaTime * lerpTowardsCanonTargetDecal);
                            playerScript.currentTrap.canonTargetDecal.transform.rotation = Quaternion.Euler(0, playerScript.currentTrap.rotatingHead.rotation.eulerAngles.y, 0);

                            y = playerScript.currentTrap.rotatingHead.transform.rotation.eulerAngles.x;
                            x = playerScript.currentTrap.rotatingHead.transform.rotation.eulerAngles.y;
                        }
                        break;
                    }
                case Player.CameraState.ZOOMOUT:
                    this.transform.Translate((this.transform.forward * -4 + this.transform.up * 2) * Time.deltaTime, Space.World);
                    break;
                case Player.CameraState.ZOOMIN:
                    {
                        Quaternion rotation = this.transform.rotation;
                        float noCollisionDistance = distance;
                        timeOnTransition += Time.deltaTime;
                        for (float zOffset = distance; zOffset >= 0.5f; zOffset -= 0.025f)
                        {
                            noCollisionDistance = zOffset;
                            Vector3 tempPos = rotation * new Vector3(cameraX, cameraY, -noCollisionDistance) + player.position;

                            if (DoubleViewingPosCheck(tempPos, zOffset))
                            {
                                break;
                            }
                        }
                        this.transform.position = Vector3.Lerp(this.transform.position, rotation * new Vector3(cameraX, cameraY, -noCollisionDistance) + player.position, 4.0f);
                        this.transform.LookAt(player.transform.position + player.transform.up * focusY + player.transform.right * focusX + player.transform.forward * focusDistance);
                        break;
                    }
                case Player.CameraState.TRANSITION:
                    {
                        y = ClampAngle(y, yMinLimit, yMaxLimit);
                        Quaternion rotation = Quaternion.Euler(y, x, 0);
                        float noCollisionDistance = distance;
                        timeOnTransition += Time.deltaTime;
                        this.transform.position = Vector3.Lerp(this.transform.position, rotation * new Vector3(cameraX, cameraY, -noCollisionDistance) + player.position, timeOnTransition / 2f);
                        this.transform.LookAt(player.transform.position + rotation * Vector3.up * focusY + rotation * Vector3.right * focusX + rotation * Vector3.forward * focusDistance);
                        break;
                    }
                default:
                    break;
            }
            lastState = playerScript.cameraState;
        }
    }
    //set player lerp camera, get the higher

    private void SetPlayerDirection(float rotation, float lerp = 0.8f)
    {
        player.rotation = Quaternion.LerpUnclamped(player.rotation ,Quaternion.Euler(player.rotation.x, rotation, player.rotation.z), lerp);
    }

    private float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360)
            angle += 360;
        while (angle > 360)
            angle -= 360;

        return Mathf.Clamp(angle, min, max);
    }

    private bool DoubleViewingPosCheck(Vector3 checkPos, float offset) 
    {
        float playerFocusHeight = playerCapsuleCollider.height * 0.5f;
        return ViewingPosCheck(checkPos, playerFocusHeight) && ReverseViewingPosCheck(checkPos, playerFocusHeight, offset);
    }

    private bool ViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight) 
    {
        RaycastHit hit;

        if (Physics.Raycast(checkPos, player.position + (Vector3.up * deltaPlayerHeight) - checkPos, out hit)) 
        {
            if (hit.transform.gameObject.layer == 9) 
            {
                return false;
            }
        }
        return true;
    }

    private bool ReverseViewingPosCheck(Vector3 checkPos, float deltaPlayerHeight, float offset) {
        RaycastHit hit;
        if (Physics.Raycast(player.position + (Vector3.up * deltaPlayerHeight), checkPos - player.position, out hit, offset)) 
        {   
            if (hit.transform.gameObject.layer == 9) 
            {
                return false;
            }
        }
        return true;
    }

    public void SetCameraXAngle(float x)
    {
        this.x = x;
    }

    public void SetCameraYAngle(float y)
    {
        this.y = y;
    }

}
