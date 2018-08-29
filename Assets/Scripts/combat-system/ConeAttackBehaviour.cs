﻿using System.Collections.Generic;
using UnityEngine;

public class ConeAttackBehaviour : PooledParticleSystem
{
    #region Fields
    public LayerMask layerMask;
    public int damage;
    public ConeAttackDetection enemiesDetector;
    public int enemiesToCombo;
    public float hurtEnemiesDelay;
    public float timeToDisable;

    public bool hitOverTime;
    [Tooltip("The time (in seconds) over which the enemies are hit")]
    public float hitSpreadDuration;

    private List<AIEnemy> targets = new List<AIEnemy>();
    private int comboCount;
    private float timer;
    private float timeToReturnToPool;

    private float hitWaitTime;
    private float timeToNextHit;
    private bool hittingOverTime;

    #endregion

    #region MonoBehaviour Methods
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer > 0.0f)
        {
            timer -= Time.deltaTime;
        }
        else
        {
            AcquireTargets();
            LaunchBulletTime();
            if (hitOverTime)
            {
                SetUpHitOverTime();
            }
            else
            {
                HitAll();
            }
        }

        if (hittingOverTime)
        {
            HitOverTime();
        }

        timeToReturnToPool -= Time.deltaTime;
        if (timeToReturnToPool <= 0.0f)
        {
            HitAll();
            ReturnToPool();
        }
    }

    private void OnValidate()
    {
        if (hitSpreadDuration < 0.0f)
            hitSpreadDuration = 0.0f;

        if (hitSpreadDuration > timeToDisable - hurtEnemiesDelay)
        {
            Debug.LogWarning("WARNING: (ConeAttackBehaviour): Can't set the HitSpreadDuration to a value larger than the 'Time To Disable' minus the 'Hurt Enemies Delay'!");
            hitSpreadDuration = timeToDisable - hurtEnemiesDelay;
        }
    }
    #endregion

    #region Public Methods
    public override void Restart()
    {
        enemiesDetector.attackTargets.Clear();
        targets.Clear();

        timer = hurtEnemiesDelay;
        timeToReturnToPool = timeToDisable;
        comboCount = 0;

        hitWaitTime = 0.0f;
        hittingOverTime = false;
    }
    #endregion

    #region Private Methods
    private void AcquireTargets()
    {
        targets.Clear();
        targets.AddRange(enemiesDetector.attackTargets);
    }

    private void LaunchBulletTime()
    {
        if(targets.Count > 0)
        {
            BulletTime.instance.DoSlowmotion(0.01f,0.1f,0.05f);
        }
    }

    private void SetUpHitOverTime()
    {
        if (targets.Count > 0)
        {
            hitWaitTime = hitSpreadDuration / targets.Count;
            timeToNextHit = 0.0f;
            hittingOverTime = true;
        }
    }

    private void HitOverTime()
    {
        timeToNextHit -= Time.deltaTime;
        if (timeToNextHit <= 0.0f)
        {
            timeToNextHit += hitWaitTime;
            AIEnemy aiEnemy = targets[0];
            targets.RemoveAt(0);
            HitOne(aiEnemy);
        }
        if (targets.Count == 0)
            hittingOverTime = false;
    }

    private void HitAll()
    {
        foreach (AIEnemy aiEnemy in targets)
        {
            HitOne(aiEnemy);
        }
        targets.Clear();
    }

    private void HitOne(AIEnemy aiEnemy)
    {
        aiEnemy.MarkAsTarget(false);
        aiEnemy.SetKnockback(transform.position, 6.0f);
        aiEnemy.TakeDamage(damage, AttackType.CONE);
        comboCount++;
    }
    #endregion
}
