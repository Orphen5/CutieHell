﻿using UnityEngine;

public class IndicatorsController : MonoBehaviour
{
    #region Fields
    public GameObject[] monumentConqueredIcons = new GameObject[3];
    private MonumentIndicator[] monumentIndicators = null;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        monumentIndicators = GetComponentsInChildren<MonumentIndicator>();

        if (monumentIndicators.Length == 0)
            Debug.LogWarning("WARNING: No MonumentIndicator were found in children of IndicatorsController in gameObject '" + gameObject.name + "'!");
    }
    #endregion

    #region Public Methods
    public void OnNewWaveStarted()
    {
        foreach (MonumentIndicator monumentIndicator in monumentIndicators)
        {
            monumentIndicator.OnNewWaveStarted();
        }
    }

    public void MonumentConquered(int index)
    {
        if (index != -1)
            monumentConqueredIcons[index].SetActive(true);
    }

    public void MonumentRepaired(int index)
    {
        if (index != -1)
            monumentConqueredIcons[index].SetActive(false);
    }
    #endregion
}
