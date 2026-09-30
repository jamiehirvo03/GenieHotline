using System.Collections.Generic;
using UnityEngine;
using Ink.Parsed;
using UnityEditorInternal.VersionControl;
using Unity.VisualScripting;


public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField] private List<TextAsset> dayOneCallers = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dayTwoCallers = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dayThreeCallers = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dayFourCallers = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dayFiveCallers = new List<TextAsset>();

    private int currentDay;
    private List<TextAsset> currentDayPool;
    private List<TextAsset> callerQueue;
    private int queuePosition;

    private List<TextAsset> callerHistory = new List<TextAsset>();

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one GameManager in the scene.");
        }

        instance = this;
    }

    public static GameManager GetInstance()
    {
        return instance;
    }

    public void StartDay(int dayNumber)
    {
        currentDay = dayNumber;

        SelectCallers();
        queuePosition = 0;

        Debug.Log($"Day {dayNumber} started");
    }

    public void SelectCallers()
    {
        switch (currentDay)
        {
            case 1:
                currentDayPool = new List<TextAsset>(dayOneCallers);

                break;
            case 2:
                currentDayPool = new List<TextAsset>(dayTwoCallers);

                break;
            case 3:
                currentDayPool = new List<TextAsset>(dayThreeCallers);

                break;
            case 4:
                currentDayPool = new List<TextAsset>(dayFourCallers);

                break;
            case 5:
                currentDayPool = new List<TextAsset>(dayFiveCallers);

                break;
        }

        RandomizeQueue();
    }
    
    private void RandomizeQueue()
    {
        // clear previous queue first
        if (callerQueue != null && callerQueue != currentDayPool)
        {
            callerQueue.Clear();
        }

        callerQueue = new List<TextAsset>(currentDayPool);
        
        for (int i = 0; i < callerQueue.Count; i++)
        {
            int r = (int)(Random.value * (callerQueue.Count - i));
            TextAsset tempVar = callerQueue[r];
            callerQueue[r] = callerQueue[i];
            callerQueue[i] = tempVar;
        }
    }

    public void AnswerPhone()
    {
        if (callerQueue == null)
        {
            Debug.Log("No callers currently in queue!");
        }
        else
        {
            DialogueManager.GetInstance().EnterDialogueMode(callerQueue[queuePosition], "Introduction");
        }   
    }

    public void HangUpPhone()
    {
        DialogueManager.GetInstance().HangUp();

        callerHistory.Add(callerQueue[queuePosition]);

        if (queuePosition < callerQueue.Count - 1)
        {
            queuePosition++;

            Debug.Log($"Moving on to next caller: {callerQueue[queuePosition]}");
        }
        else
        {
            Debug.Log("Caller queue is cleared for the day!");
        }
    }
}
