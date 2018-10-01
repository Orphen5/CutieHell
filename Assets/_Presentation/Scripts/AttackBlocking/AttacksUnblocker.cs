﻿using UnityEngine;

public class AttacksUnblocker : MonoBehaviour, IZoneTakenListener
{
    #region Fields
    [SerializeField]
    [Tooltip("The AIZoneController to listen to")]
    private AIZoneController referenceZone;
    [Header("Attack to enable")]
    [SerializeField]
    private bool unblockMines;
    [SerializeField]
    private bool unblockBlackHole;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        UnityEngine.Assertions.Assert.IsNotNull(referenceZone, "ERROR: Reference Zone (AIZoneController) not assigned for AttacksUnblocker script in GameObject " + gameObject.name);
    }

    private void Start()
    {
        referenceZone.AddIZoneTakenListener(this);
    }

    private void OnDestroy()
    {
        referenceZone.RemoveIZoneTakenListener(this);
    }
    #endregion

    #region Public Methods
    // IZoneTakenListener
    public void OnZoneTaken()
    {
        if (unblockMines)
        {
            AttacksBlocker.instance.UnblockMines();
        }
        if (unblockBlackHole)
        {
            AttacksBlocker.instance.UnblockBlackHole(); ;
        }
    }
    #endregion
}
