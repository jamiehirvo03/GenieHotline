using System;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    private static EventManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }

        instance = this;
    }

    public static EventManager GetInstance()
    {
        return instance;
    }

    public event Action onEnterDialogueMode;
    public void EnterDialogueMode()
    {
        if (onEnterDialogueMode != null)
        {
            onEnterDialogueMode();
        }
    }

    public event Action onExitDialogueMode;
    public void ExitDialogueMode()
    {
        if (onExitDialogueMode != null)
        {
            onExitDialogueMode();
        }
    }


}
