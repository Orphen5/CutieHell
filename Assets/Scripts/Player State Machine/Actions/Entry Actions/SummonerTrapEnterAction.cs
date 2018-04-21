﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Player State Machine/Actions/SummonerTrapEnterAction")]
public class SummonerTrapEnterAction : StateAction
{
    public override void Act(Player player)
    {
        player.currentTrap = player.nearbyTrap;
        player.currentTrap.Activate(player);
        player.SetRenderersVisibility(false);
        player.SetEvilLevel(player.currentTrap.usageCost);

        SummonerTrap summonerTrap = player.currentTrap.GetComponent<SummonerTrap>();
        player.mainCamera.transform.position = summonerTrap.trapBasicSummonerEyes.transform.position;
        summonerTrap.trapBasicSummonerBeam.gameObject.SetActive(true);
        summonerTrap.seductiveTrapActiveArea.gameObject.SetActive(true);
        summonerTrap.firstProjection = true;
        summonerTrap.InstantiateSeductiveEnemyProjection();
    }
}
