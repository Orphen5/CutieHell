﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State Machine/Actions/MeteoriteEnterAction")]
public class MeteoriteEnterAction : StateAction
{
    public override void Act(Player player)
    {
        player.initialPos = player.transform.position;
        player.meteoriteDestinationMarker.SetActive(false);
        player.comeBackFromMeteoriteAttack = false;
        player.cameraState = Player.CameraState.METEORITEAIM;
        //player.transform.position = new Vector3(player.transform.position.x, 55.0f, player.transform.position.z);
        player.mainCameraController.x = -2.3f;
        player.mainCameraController.y = 61;
        player.transform.position = player.meteoritesPlayerPosition[player.currentZonePlaying].position;
        player.transform.rotation = Quaternion.LookRotation(Vector3.forward);
        player.SetZoneController(player.meteoriteZones[player.currentZonePlaying]);
    }
}
