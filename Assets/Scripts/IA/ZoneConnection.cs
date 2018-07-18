﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class ZoneConnection : MonoBehaviour {

    #region Fields
    private List<AIEnemy> aiEnemiesInConnection = new List<AIEnemy>();
    private List<AIEnemy> toRemove = new List<AIEnemy>();
    private Player playerInConnection = null;
    #endregion

    #region MonoBehavior Methods
    private void Awake()
    {
        GetComponent<BoxCollider>().isTrigger = true;
    }

    private void Update()
    {
        foreach (AIEnemy enemy in aiEnemiesInConnection)
        {
            if (enemy.IsDead())
                toRemove.Add(enemy);
        }
        foreach (AIEnemy enemy in toRemove)
        {
            aiEnemiesInConnection.Remove(enemy);
        }
        toRemove.Clear();
    }

    private void OnTriggerEnter(Collider other)
    {
        AIEnemy aiEnemy = other.GetComponent<AIEnemy>();
        if (aiEnemy)
        {
            aiEnemiesInConnection.Add(aiEnemy);
        }
        else
        {
            Player player = other.GetComponentInParent<Player>();
            if (player)
            {
                playerInConnection = player;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        AIEnemy aiEnemy = other.GetComponent<AIEnemy>();
        if (aiEnemy)
        {
            aiEnemiesInConnection.Remove(aiEnemy);
        }
        else
        {
            Player player = other.GetComponentInParent<Player>();
            if (player)
            {
                playerInConnection = null;
            }
        }
    }
    #endregion

    #region Public Methods
    public bool ContainsEnemy(AIEnemy enemy)
    {
        return aiEnemiesInConnection.Contains(enemy);
    }

    public bool ContainsPlayer()
    {
        return playerInConnection != null;
    }
    #endregion
}
