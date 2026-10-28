using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ink.Parsed;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    public bool debugMode;

    // caller info
    [System.Serializable]
    public class Callers
    {
        public int dayNumber;
        public TextAsset callerJSON;
        public string callerName;
        public int callerAge;
        public string callerOccupation;

        // answers
        public bool material;
        public bool knowledge;
        public bool heart;
        public bool harm;
        public bool enhancement;
        public bool fate;

        // verditc
        public bool canBeGranted;

        public GameObject callerForm;

        public Document GetCallerDocument()
        {
            return callerForm.GetComponent<Document>();
        }

        public bool hasHungUp = false;
    }
    [Header("Add Callers")] public List<Callers> callers = new List<Callers>();

    // caller queue
    private int currentDay;
    [HideInInspector] public bool callersInQueue = false;
    private List<Callers> callerQueue = new List<Callers>();
    private List<Callers> currentDayCallers = new List<Callers>();
    private int queuePosition;

    private List<Callers> callerHistory = new List<Callers>();

    [Header("Phone")]
    [SerializeField] private List<GameObject> phoneIndicators = new List<GameObject>();
    [SerializeField] private Material indicatorOff;
    [SerializeField] private Material indicatorOn;

    [Header("Documents")]
    [SerializeField] private GameObject documentPrefab;
    
    [SerializeField] private Transform pileLocation;
    [SerializeField] private List<GameObject> currentDocuments = new List<GameObject>();
    [SerializeField] private List<Callers> currentCallers = new List<Callers>();

    [Header("Progression/Scoring")]
    // persistive 
    private int strikesCount;
    private int chaosIndex;
    private int geneBucksBalance;

    // quotas
    [SerializeField] private int dayOneQuota = 1;
    [SerializeField] private int dayTwoQuota = 2;
    [SerializeField] private int dayThreeQuota = 2;
    [SerializeField] private int dayFourQuota = 3;
    [SerializeField] private int dayFiveQuota = 4;

    // point awards
    [SerializeField] private int correctDetail = 5;
    [SerializeField] private int quotaBonus = 50;
    [SerializeField] private float flawlessBonus = 2f;
    

    // daily resets
    private int currentDayQuota;
    private int wishesApproved;
    private int wishesDenied;
    private int mistakesMade;
    private bool wasQuotaMet;
    private bool strikeGivenToday;
    private int score;
    private int geneBucksEarnt;

    [Header("Camera")]
    [SerializeField] private Transform defaultCamTransform;
    [SerializeField] private Transform endOfDayCamTransform;

    [SerializeField] private GameObject submitButton;

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

    private void Start()
    {
        // before starting day, do a check on the persistent data script for last saved day number.

        // start day
        StartDay(1);
    }

    public void StartDay(int dayNumber)
    {
        currentDay = dayNumber;

        // reset day-dependant variables
        queuePosition = 0;
        callersInQueue = true;
        currentDayCallers.Clear();
        
        submitButton.SetActive(false);

        wishesApproved = 0;
        wishesDenied = 0;
        mistakesMade = 0;
        wasQuotaMet = false;
        strikeGivenToday = false;
        score = 0;
        geneBucksEarnt = 0;

        if (currentDay == 1)
        {
            currentDayQuota = dayOneQuota;
        }
        if (currentDay == 2)
        {
            currentDayQuota = dayTwoQuota;
        }
        if (currentDay == 3)
        {
            currentDayQuota = dayThreeQuota;
        }
        if (currentDay == 4)
        {
            currentDayQuota = dayFourQuota;
        }
        if (currentDay == 5)
        {
            currentDayQuota = dayFiveQuota;
        }


        // setup new day
        SelectCallers();
        UpdateIndicators();
        SpawnDocuments();

        Debug.Log($"Day {dayNumber} started");
    }

    public void SelectCallers()
    {

        for (int i = 0; i < callers.Count; i++)
        {
            if (callers[i].dayNumber == currentDay)
            {
                Debug.Log($"{callers[i].callerName}");
                callerQueue.Add(callers[i]);

                currentDayCallers.Add(callers[i]);
            }
        }
        RandomizeQueue();
    }

    private void RandomizeQueue()
    {
        //// clear previous queue first
        //if (callerQueue != null && !callersInQueue)
        //{
        //    callerQueue.Clear();
        //}

        for (int i = 0; i < callerQueue.Count; i++)
        {
            int r = (int)(Random.value * (callerQueue.Count - i));
            Callers tempVar = callerQueue[r];
            callerQueue[r] = callerQueue[i];
            callerQueue[i] = tempVar;
        }
    }

    private void UpdateIndicators()
    {
        for (int i = 0; i < callerQueue.Count; i++)
        {
            if (callerQueue[i].hasHungUp)
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

    private void SpawnDocuments()
    {
        for (int i = 0; i < callerQueue.Count; i++)
        {
            // add a slight vertical offset based on the current loop index so that subsequent objects create a pile
            Vector3 spawnPosition = new Vector3(pileLocation.position.x, (pileLocation.position.y + (0.05f * i)), pileLocation.position.z);

            GameObject newInstance = Instantiate(documentPrefab, spawnPosition, Quaternion.identity);
            newInstance.name = $"Document({callerQueue[i].callerName})";

            currentDocuments.Add(newInstance);

            currentDocuments[i].GetComponent<Document>().SetCallerDetails(callerQueue[i].callerName, callerQueue[i].callerAge, callerQueue[i].callerOccupation);

            currentCallers.Add(callerQueue[i]);

            // attach the newly created document object to the recently added caller in queue
            callerQueue[i].callerForm = newInstance;
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
            TextAsset currentCallerJSON = callerQueue[queuePosition].callerJSON;

            DialogueManager.GetInstance().EnterDialogueMode(currentCallerJSON, "introduction");
        }   
    }

    public void HangUpPhone()
    {
        DialogueManager.GetInstance().HangUp();

        callerHistory.Add(callerQueue[queuePosition]);

        callerQueue[queuePosition].hasHungUp = true;

        UpdateIndicators();

        if (queuePosition < callerQueue.Count - 1)
        {
            queuePosition++;

            Debug.Log($"Moving on to next caller: {currentDayCallers[queuePosition]}");
        }
        else
        {
            Debug.Log("Caller queue is cleared for the day!");

            callersInQueue = false;

            submitButton.SetActive(true);
        }
    }

    public void SubmitDocuments()
    {
        CheckFormAnswers();

        CalculateScore();
        CalculateBucks();

        EndDay();
    }

    private void GiveStrike()
    {
        if (!strikeGivenToday)
        {
            strikesCount++;
            strikeGivenToday = true;
        }
    }

    private void CheckFormAnswers()
    {
        // check submitted documents for the day
        for (int i = 0; i < currentDayCallers.Count; i++)
        {
            // find right form GameObject to check
            Document formToCheck = currentDayCallers[i].callerForm.GetComponent<Document>();

            // go through 'answer' bools to check if checkboxes are correct (only need to check and award points for the correct answers)
            if (currentDayCallers[i].material)
            {
                if (formToCheck.material == currentDayCallers[i].material)
                {
                    score =  + correctDetail;
                }
                else
                {
                    GiveStrike();
                    mistakesMade++;
                }
            }
            if (currentDayCallers[i].knowledge)
            {
                if (formToCheck.knowledge == currentDayCallers[i].knowledge)
                {
                    score = score + correctDetail;
                }
                else
                {
                    GiveStrike();
                    mistakesMade++;
                }
            }
            if (currentDayCallers[i].heart)
            {
                if (formToCheck.heart == currentDayCallers[i].heart)
                {
                    score = score + correctDetail;
                }
                else
                {
                    GiveStrike();
                    mistakesMade++;
                }
            }
            if (currentDayCallers[i].harm)
            {
                if (formToCheck.harm == currentDayCallers[i].harm)
                {
                    score = score + correctDetail;
                }
                else
                {
                    GiveStrike();
                    mistakesMade++;
                }
            }
            if (currentDayCallers[i].enhancement)
            {
                if (formToCheck.enhancement == currentDayCallers[i].enhancement)
                {
                    score = score + correctDetail;
                }
                else
                {
                    GiveStrike();
                    mistakesMade++;
                }
            }
            if (currentDayCallers[i].fate)
            {
                if (formToCheck.fate == currentDayCallers[i].fate)
                {
                    score = score + correctDetail;
                }
                else
                {
                    GiveStrike();
                    mistakesMade++;
                }
            }

            // check form approval status
            if (currentDayCallers[i].canBeGranted)
            {
                if (formToCheck.isApproved)
                {
                    wishesApproved++;
                }
                else if (formToCheck.isDenied)
                {
                    wishesDenied++;

                    GiveStrike();
                    mistakesMade++;
                }
            }
            else
            {
                if (formToCheck.isApproved)
                {
                    wishesApproved++;

                    GiveStrike();
                    mistakesMade++;
                }
                else if (formToCheck.isDenied)
                {
                    wishesDenied++;
                }
            }
        }
    }

    private void CalculateScore()
    {
        if (wishesApproved >= currentDayQuota)
        {
            wasQuotaMet = true;
            score = score + quotaBonus;
        }
    }

    private void CalculateBucks()
    {
        // if day was flawless
        if (!strikeGivenToday)
        {
            float adjustedScore = score;
            adjustedScore *= flawlessBonus;

            geneBucksEarnt = (int)adjustedScore;
            geneBucksBalance += geneBucksEarnt;
        }
    }

    private void EndDay()
    {
        Debug.Log($"Day {currentDay} has ended");

        // show end-of-day screen
        CameraManager.GetInstance().MoveCamera(endOfDayCamTransform);

        UIManager.GetInstance().SetEndOfDayScreen(true, currentDay, wishesApproved, wishesDenied, wasQuotaMet, mistakesMade, strikesCount, geneBucksEarnt, geneBucksBalance);
    }

    private void BuyItem()
    {

    }

    private void PlayCinematic(string cinematicName)
    {
        // code for playing cinematic when triggered (when memory trinket acquired or ending reached)
    }
}
