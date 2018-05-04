﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monument : Building
{

    #region Attributes
    [Header("Monument attributes")]
    [SerializeField]
    private Texture almostConqueredScreenTintTexture;
    private float showAlmostConqueredScreenTintTexture = 0;
    [Range(0, 1)]
    [SerializeField]
    private float lowHealthScreen;
    [Space]
    [SerializeField]
    private MonumentIndicator monumentIndicator;
    #endregion

    #region Public Methods
    // IDamageable
    // If this method is called, it should inform the ZoneController and UIManager
    public override void FullRepair()
    {
        if (!zoneController.isFinalZone || currentHealth != 0)
            base.FullRepair();
    }

    public override void TakeDamage(float damage, AttackType attacktype)
    {
        base.TakeDamage(damage, attacktype);
        monumentIndicator.SetFill((baseHealth - currentHealth) / baseHealth);
    }


    private void OnGUI()
    {
        if (currentHealth <= (baseHealth * lowHealthScreen) && currentHealth > 0)
        {
            if (showAlmostConqueredScreenTintTexture > 1)
            {
                GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), almostConqueredScreenTintTexture);
            }
            if (showAlmostConqueredScreenTintTexture > 2)
            {
                showAlmostConqueredScreenTintTexture = 0;
            }
            showAlmostConqueredScreenTintTexture += Time.deltaTime;
        }
    }
    #endregion

    #region Protected Methods
    protected override void BuildingKilled()
    {
        zoneController.OnMonumentTaken();
    }

    protected override void BuildingRecovered()
    {
        zoneController.OnMonumentRecovered();
    }
    #endregion
}
