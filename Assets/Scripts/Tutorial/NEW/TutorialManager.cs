﻿using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private delegate void VoidCallback();
    [System.Serializable]
    private struct HierarchyInfo
    {
        Transform transform;
        Transform parent;
        int siblingIndex;

        public HierarchyInfo(Transform transform)
        {
            this.transform = transform;
            parent = transform.parent;
            siblingIndex = transform.GetSiblingIndex();
        }

        public void ReturnToOrigin()
        {
            transform.SetParent(parent);
            transform.SetSiblingIndex(siblingIndex);
        }
    }

    #region Fields
    [Header("Setup")]
    [SerializeField]
    private ScreenFadeController screenFadeController;
    [SerializeField]
    private CinematicStripes cinematicStripes;
    [SerializeField]
    private Transform tutorialForegroundParent;
    
    [Header("Scripted messages")]
    [SerializeField]
    private GameObject userWaitPrompts;
    [SerializeField]
    private GameObject firstMessage;
    [SerializeField]
    private Transform[] firstMessageForeground;
    [SerializeField]
    private GameObject secondMessage;

    // General
    private bool tutorialRunning;
    private List<HierarchyInfo> hierarchyInfos = new List<HierarchyInfo>();

    // Player
    Player player;

    // Skiping all further messages
    private bool skipAll = false;

    // WaitForUser related
    private VoidCallback waitEndedCallback = null;
    private bool waitingForUser = false;
    #endregion

    #region MonoBehaviour Methods
    private void Awake()
    {
        UnityEngine.Assertions.Assert.IsNotNull(screenFadeController, "ERROR: Screen Fade Controller (ScreenFadeController) not assigned for TutorialManager script in GameObject " + gameObject.name);
        UnityEngine.Assertions.Assert.IsNotNull(cinematicStripes, "ERROR: Cinematic Stripes (CinematicStripes) not assigned for TutorialManager script in GameObject " + gameObject.name);
        UnityEngine.Assertions.Assert.IsNotNull(tutorialForegroundParent, "ERROR: Tutorial Foreground Parent (Transform) not assigned for TutorialManager script in GameObject " + gameObject.name);
        UnityEngine.Assertions.Assert.IsNotNull(userWaitPrompts, "ERROR: User Wait Prompts (GameObject) not assigned for TutorialManager script in GameObject " + gameObject.name);
        UnityEngine.Assertions.Assert.IsNotNull(firstMessage, "ERROR: First Message (GameObject) not assigned for TutorialManager script in GameObject " + gameObject.name);
    }

    private void Start()
    {
        player = GameManager.instance.GetPlayer1();
    }

    private void Update()
    {
        if (tutorialRunning)
        {
            if (waitingForUser && !GameManager.instance.gameIsPaused)
            {
                if (InputManager.instance.GetXButtonDown())
                    Continue();
                else if (InputManager.instance.GetOButtonDown())
                    SkipAll();
            }
        }
    }
    #endregion

    #region Public Methods
    public void RequestStartTutorial()
    {
        tutorialRunning = true;
        player.OnRoundOver(); // Triggers stopped state
        cinematicStripes.Show();
        screenFadeController.TurnOpaque();
        screenFadeController.FadeToAlpha(0.9f, 1.0f, ShowFirstMessage);
    }
    #endregion

    #region Private Methods

    private void RequestEndTutorial()
    {
        screenFadeController.FadeToTransparent(0.5f, OnTutorialEnded);
        cinematicStripes.HideAnimated();
    }

    private void OnTutorialEnded()
    {
        player.OnRoundStarted();
        tutorialRunning = false;
    }

    #region Scripted messages
    private void ShowFirstMessage()
    {
        userWaitPrompts.SetActive(true);
        firstMessage.SetActive(true);
        BringToForeground(firstMessageForeground);
        WaitForUser(OnFirstMessageClosed);
    }

    private void OnFirstMessageClosed()
    {
        userWaitPrompts.SetActive(false);
        firstMessage.SetActive(false);
        SendToBackground();
        if (!skipAll)
        {
            // Next scripted message
            ShowSecondMessage();
        }
        else
        {
            // Finish
            RequestEndTutorial();
        }
    }

    private void ShowSecondMessage()
    {
        userWaitPrompts.SetActive(true);
        secondMessage.SetActive(true);
        WaitForUser(OnSecondMessageClosed);
    }

    private void OnSecondMessageClosed()
    {
        userWaitPrompts.SetActive(false);
        secondMessage.SetActive(false);
        if (!skipAll)
        {
            // Next scripted message
            RequestEndTutorial();
        }
        else
        {
            // Finish
            RequestEndTutorial();
        }
    }
    #endregion

    #region Helper Methods
    private void WaitForUser(VoidCallback callback)
    {
        waitEndedCallback = callback;
        userWaitPrompts.SetActive(true);
        waitingForUser = true;
    }

    private void Continue()
    {
        waitingForUser = false;
        userWaitPrompts.SetActive(false);
        waitEndedCallback();
    }

    private void SkipAll()
    {
        skipAll = true;
        waitingForUser = false;
        userWaitPrompts.SetActive(false);
        waitEndedCallback();
    }

    private void BringToForeground(Transform[] transforms)
    {
        for (int i = 0; i < transforms.Length; ++i)
        {
            Transform transform = transforms[i];
            hierarchyInfos.Add(new HierarchyInfo(transform));
            transform.SetParent(tutorialForegroundParent);
        }
}

    private void SendToBackground()
    {
        for (int i = hierarchyInfos.Count - 1; i >= 0; --i)
        {
            hierarchyInfos[i].ReturnToOrigin();
        }
        hierarchyInfos.Clear();
    }
    #endregion

    #endregion
}
