using System.Collections.Generic;
using UnityEngine;
using Ink.Parsed;
using Unity.VisualScripting;
using System.Runtime.CompilerServices;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    private static GameManager instance;

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

        public GameObject callerForm;
    }
    [Header("Add Callers")] public List<Callers> callers = new List<Callers>();

    // caller queue
    private int currentDay;
    [HideInInspector] public bool callersInQueue = false;
    private List<Callers> callerQueue = new List<Callers>();
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

    [Header("Progression")]
    private float chaosLevel;
    private float geneBucks;

    [Header("Camera")]
    [SerializeField] private Transform officeCam;
    [SerializeField] private Transform shopCam;

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

    public void StartDay(int dayNumber)
    {
        currentDay = dayNumber;

        SelectCallers();
        UpdateIndicators();
        SpawnDocuments();
        queuePosition = 0;
        callersInQueue = true;
        callerHistory.Clear();

        submitButton.SetActive(false);

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

            submitButton.SetActive(true);
        }
    }

    public void SubmitDocuments()
    {
        // check submitted documents for the day
        for (int i = 0; i < currentDocuments.Count; i++)
        {
            // find right form GameObject to check
            GameObject formToCheck = currentDocuments[i].GetComponent<Callers>().callerForm;

            // go through 'answer' bools to check if document is correct

        }
    }

    private void CheckFormAnswers()
    {

    }

    private void EndDay()
    {
        Debug.Log($"Day {currentDay} has ended");
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
