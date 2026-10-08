using System.Collections.Generic;
using UnityEngine;
using Ink.Parsed;


public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    // caller queue
    private int currentDay;
    private List<TextAsset> currentDayPool;

    [SerializeField] private List<TextAsset> dayOneCallers = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dayTwoCallers = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dayThreeCallers = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dayFourCallers = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dayFiveCallers = new List<TextAsset>();

    [HideInInspector] public bool callersInQueue = false;
    private List<TextAsset> callerQueue;
    private int queuePosition;

    [SerializeField] private List<GameObject> phoneIndicators = new List<GameObject>();
    [SerializeField] private Material indicatorOff;
    [SerializeField] private Material indicatorOn;

    private List<TextAsset> callerHistory = new List<TextAsset>();

    // progression
    private float chaosLevel;
    private float geneBucks;

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
        UpdateIndicators();
        queuePosition = 0;
        callersInQueue = true;

        Debug.Log($"Day {dayNumber} started");
    }

    private void UpdateIndicators()
    {        
        for (int i = 0; i < callerQueue.Count; i++)
        {
            if (callerQueue[i] == null)
            {
                // if queue position empty set the respective indicator to off
                phoneIndicators[i].GetComponent<Renderer>().material = indicatorOff;
            }
            else
            {
                // if queue position filled set the respective indicator to on
                phoneIndicators[i].GetComponent<Renderer>().material = indicatorOn;
            }
        }
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
        if (!callersInQueue)
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

        callerQueue[queuePosition] = null;

        UpdateIndicators();

        if (queuePosition < callerQueue.Count - 1)
        {
            queuePosition++;

            Debug.Log($"Moving on to next caller: {callerQueue[queuePosition]}");
        }
        else
        {
            Debug.Log("Caller queue is cleared for the day!");

            callersInQueue = false;
        }
    }

    private void GoToShop()
    {
        // load available shop items in their positions with matching name/price tags

        // switch to shop camera/move main camera to shop position

    }

    private void BuyItem()
    {

    }

    private void ReturnToOffice()
    {
        // switch back to office camera/move main camera back to office position

        // unload shop items/fx when not needed
    }

    private void PlayCinematic(string cinematicName)
    {
        // code for playing cinematic when triggered (when memory trinket acquired or ending reached)
    }
}
