﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State Machine/Actions/StrongAttack")]
public class StrongAttack : StateAction
{
    public int damage;
    public ParticleSystem strongAttackVFX;
    public float timeToGoOut, timeToGoIn, delay;

    [SerializeField]
    private AudioClip attackSfx;

    public override void Act(Player player)
    {
        switch (player.teleportState)
        {
            case Player.TeleportStates.OUT:
                if (player.timeSinceLastStrongAttack >= timeToGoOut)
                {
                    player.canMove = true;
                    player.teleportState = Player.TeleportStates.TRAVEL;

                }
                break;
            case Player.TeleportStates.TRAVEL:
                if (InputManager.instance.GetOButtonDown())
                {
                    player.teleportState = Player.TeleportStates.IN;
                    player.timeSinceLastStrongAttack = 0.0f;
                    ParticlesManager.instance.LaunchParticleSystem(strongAttackVFX, player.transform.position, strongAttackVFX.transform.rotation);
                    player.canMove = false;
                    player.animator.Rebind();
                }
                break;
            case Player.TeleportStates.IN:
                if (player.timeSinceLastStrongAttack >= timeToGoIn)
                {
                    player.cameraState = Player.CameraState.MOVE;
                    player.SetRenderersVisibility(true);
                    player.mainCameraController.y = 10.0f;
                    player.timeSinceLastStrongAttack = 0.0f;
                    player.teleported = true;
                    player.teleportState = Player.TeleportStates.DELAY;
                    HurtEnemies(player, damage);
                }
                break;
            case Player.TeleportStates.DELAY:
                if (player.timeSinceLastStrongAttack >= delay)
                {
                    player.comeBackFromStrongAttack = true;
                }
                break;
            default:
                break;
        }
    }
    private void HurtEnemies(Player player, int damage)
    {
        foreach (AIEnemy aiEnemy in player.currentStrongAttackTargets)
        {
            aiEnemy.TakeDamage(damage, AttackType.STRONG);
        }
    }
}
