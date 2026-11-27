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
        public string name;
        [Space]
        public int dayNumber;
        public TextAsset callerJSON;
        public string callerName;
        public int callerAge;
        public string callerOccupation;
        [Space]
        // answers
        public bool material;
        public bool knowledge;
        public bool heart;
        public bool harm;
        public bool enhancement;
        public bool fate;
        [Space]
        // verditc
        public bool canBeGranted;

        [HideInInspector] public GameObject callerForm;

        public Document GetCallerDocument()
        {
            return callerForm.GetComponent<Document>();
        }

        [HideInInspector] public bool hasHungUp = false;
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

    [Header("Documents")]
    [SerializeField] private GameObject documentPrefab;
    
    [SerializeField] private Transform pileLocation;
    [SerializeField] private List<GameObject> currentDocuments = new List<GameObject>();
    [SerializeField] private List<Callers> currentCallers = new List<Callers>();

    [Header("Chaos Values (Parameters)")]
    [SerializeField] private int materialCost = -2;
    [SerializeField] private int knowledgeCost = -3;
    [SerializeField] private int heartCost = -5;
    [SerializeField] private int harmCost = -6;
    [SerializeField] private int enhancementCost = -8;
    [SerializeField] private int fateCost = -10;

    [Header("Chaos Values (Consequences)")]
    [SerializeField] private int inconvenienceValue = 2;
    [SerializeField] private int dayRuinerValue = 3;
    [SerializeField] private int familyConflictValue = 5;
    [SerializeField] private int legalRepercussionsValue = 7;
    [SerializeField] private int hexedValue = 8;
    [SerializeField] private int deathValue = 10;

    [Header("Chaos")]
    [SerializeField] private int minChaos = -3;
    [SerializeField] private int maxChaos = 3;
    [Space]
    public int chaosIndex;
    public string chaosLevel;
    [SerializeField] private int changePerChaosIndex;

    [Header("Employee Performance")]
    [SerializeField] private int strikesCount;
    public int geneBucksBalance;

    [Header("Quotas")]
    [SerializeField] private int dayOneQuota = 1;
    [SerializeField] private int dayTwoQuota = 2;
    [SerializeField] private int dayThreeQuota = 2;
    [SerializeField] private int dayFourQuota = 3;
    [SerializeField] private int dayFiveQuota = 4;

    [Header("Point Awards")]
    [SerializeField] private int correctDetail = 5;
    [SerializeField] private int quotaBonus = 50;
    [SerializeField] private float flawlessMultiplier = 2f;
    [Space]
    [Tooltip("Score per point OVER balance")][SerializeField] private int surplusAward = 5;
    [Tooltip("Score per point UNDER balance")][SerializeField] private int defecitAward = -5;
    [Space]
    [SerializeField] private int pointsPerGeneBuck = 10;

    [Header("Daily Resets")]
    private int currentDayQuota;
    private int wishesApproved;
    private int wishesDenied;
    private int mistakesMade;

    private bool wasQuotaMet;
    private bool strikeGivenToday;

    private int chaosChange;

    private int grossScore; // before awards/multipliers are applied
    private int netScore;   // final score that gets converted into geneBucks
    private int geneBucksEarnt;

    [SerializeField] private int inconveniencesGiven;
    [SerializeField] private int dayRuinersGiven;
    [SerializeField] private int familyConflictsGiven;
    [SerializeField] private int legalRepercussionsGiven;
    [SerializeField] private int hexesGiven;
    [SerializeField] private int deathsGiven;

    [HideInInspector] public int yesterdaysInconveniencesGiven;
    [HideInInspector] public int yesterdaysDayRuinersGiven;
    [HideInInspector] public int yesterdaysFamilyConflictsGiven;
    [HideInInspector] public int yesterdaysLegalRepercussionsGiven;
    [HideInInspector] public int yesterdaysHexesGiven;
    [HideInInspector] public int yesterdaysDeathsGiven;

    [Header("Consequence Stats for each day")]
    // day one
    [HideInInspector] public int dayOneInconveniencesGiven;
    [HideInInspector] public int dayOneDayRuinersGiven;
    [HideInInspector] public int dayOneFamilyConflictsGiven;
    [HideInInspector] public int dayOneLegalRepercussionsGiven;
    [HideInInspector] public int dayOneHexesGiven;
    [HideInInspector] public int dayOneDeathsGiven;
    // day two
    [HideInInspector] public int dayTwoInconveniencesGiven;
    [HideInInspector] public int dayTwoDayRuinersGiven;
    [HideInInspector] public int dayTwoFamilyConflictsGiven;
    [HideInInspector] public int dayTwoLegalRepercussionsGiven;
    [HideInInspector] public int dayTwoHexesGiven;
    [HideInInspector] public int dayTwoDeathsGiven;
    // day three
    [HideInInspector] public int dayThreeInconveniencesGiven;
    [HideInInspector] public int dayThreeDayRuinersGiven;
    [HideInInspector] public int dayThreeFamilyConflictsGiven;
    [HideInInspector] public int dayThreeLegalRepercussionsGiven;
    [HideInInspector] public int dayThreeHexesGiven;
    [HideInInspector] public int dayThreeDeathsGiven;
    // day four
    [HideInInspector] public int dayFourInconveniencesGiven;
    [HideInInspector] public int dayFourDayRuinersGiven;
    [HideInInspector] public int dayFourFamilyConflictsGiven;
    [HideInInspector] public int dayFourLegalRepercussionsGiven;
    [HideInInspector] public int dayFourHexesGiven;
    [HideInInspector] public int dayFourDeathsGiven;
    // day five
    [HideInInspector] public int dayFiveInconveniencesGiven;
    [HideInInspector] public int dayFiveDayRuinersGiven;
    [HideInInspector] public int dayFiveFamilyConflictsGiven;
    [HideInInspector] public int dayFiveLegalRepercussionsGiven;
    [HideInInspector] public int dayFiveHexesGiven;
    [HideInInspector] public int dayFiveDeathsGiven;

    // item purchase
    [SerializeField] private List<Transform> deliveryPoints = new List<Transform>();

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
        UIManager.GetInstance().ToggleEndOfDayScreen(false);

        currentDay = dayNumber;

        // reset day-dependant variables
        queuePosition = 0;
        callersInQueue = true;
        currentDayCallers.Clear();

        wishesApproved = 0;
        wishesDenied = 0;
        mistakesMade = 0;
        wasQuotaMet = false;
        strikeGivenToday = false;
        chaosChange = 0;
        grossScore = 0;
        netScore = 0;
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
        string callersLog = "Today's callers: ";

        for (int i = 0; i < callers.Count; i++)
        {
            if (callers[i].dayNumber == currentDay)
            {
                callersLog += $"{callers[i].callerName}, ";
                callerQueue.Add(callers[i]);

                currentDayCallers.Add(callers[i]);
            }
        }

        if (currentDayCallers.Count == 0)
        {
            Debug.LogError($"Day {currentDay} is missing callers! Add more or double-check total callers list for incorrect setup");
        }

        Debug.Log(callersLog);

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
                phoneIndicators[i].GetComponent<MeshRenderer>().materials[0].DisableKeyword("_EMISSION");
            }
            else
            {
                // if queue position filled set the respective indicator to on
                phoneIndicators[i].GetComponent<MeshRenderer>().materials[0].EnableKeyword("_EMISSION");
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
            newInstance.name = $"Document ({callerQueue[i].callerName})";

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
        }
    }

    private void BuyItem(string itemName)
    {

    }

    private void PlayCinematic(string cinematicName)
    {
        // code for playing cinematic when triggered (when memory trinket acquired or ending reached)
    }


    public void SubmitDocuments()
    {
        CheckFormAnswers();
        CalculateScore();

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

            if (formToCheck.material == currentDayCallers[i].material)
            {
                grossScore += correctDetail;
            }
            else
            {
                GiveStrike();
                mistakesMade++;
            }

            if (formToCheck.knowledge == currentDayCallers[i].knowledge)
            {
                grossScore += correctDetail;
            }
            else
            {
                GiveStrike();
                mistakesMade++;
            }

            if (formToCheck.heart == currentDayCallers[i].heart)
            {
                grossScore += correctDetail;
            }
            else
            {
                GiveStrike();
                mistakesMade++;
            }

            if (formToCheck.harm == currentDayCallers[i].harm)
            {
                grossScore += correctDetail;
            }
            else
            {
                GiveStrike();
                mistakesMade++;
            }

            if (formToCheck.enhancement == currentDayCallers[i].enhancement)
            {
                grossScore += correctDetail;
            }
            else
            {
                GiveStrike();
                mistakesMade++;
            }

            if (formToCheck.fate == currentDayCallers[i].fate)
            {
                grossScore += correctDetail;
            }
            else
            {
                GiveStrike();
                mistakesMade++;
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

            // only calculate the chaos of wishes that will actually be granted (can be granted and approved by player)
            if (currentDayCallers[i].canBeGranted && formToCheck.isApproved)
            {
                CalculateChaos(formToCheck);
            }
        }

        // once all submitted documents are checked, take the final consequences (highest one selected for each caller) and add them to the daily chaos change 
        chaosChange += (inconveniencesGiven * inconvenienceValue) + (dayRuinersGiven * dayRuinerValue) + (familyConflictsGiven * familyConflictValue) + (legalRepercussionsGiven * legalRepercussionsValue) + (hexesGiven * hexedValue) + (deathsGiven * deathValue);

        // calculate chaos index change based on chaos change
        if (chaosChange > changePerChaosIndex)
        {
            int indexChange;

            indexChange = chaosChange / changePerChaosIndex;

            chaosIndex += indexChange;

            if (chaosIndex > maxChaos)
            {
                Debug.Log($"Chaos Index has gone over the allowed range! ({chaosIndex}), it has been lowered to the maximum ({maxChaos}");

                chaosIndex = maxChaos;
            }
        }
        else if (chaosChange < (-changePerChaosIndex))
        {
            int indexChange;

            indexChange = chaosChange / changePerChaosIndex;

            chaosIndex -= indexChange;

            if (chaosIndex < minChaos)
            {
                Debug.Log($"Chaos Index has gone under the allowed range! ({chaosIndex}), it has been raised to the minimum ({minChaos}");

                chaosIndex = minChaos;
            }
        }
        else
        {
            Debug.Log("Today's chaosChange was within balance threshold, no change to chaosIndex!");
        }
    }

    private void CalculateChaos(Document formToCheck)
    {
        SelectLowestParameter(formToCheck);
        SelectHighestConsequence(formToCheck);
    }   

    private void SelectLowestParameter(Document formToCheck)
    {
        // PARAMETERS      // for the sake of balance (to prevent all/multiple parameters being chosen at once) only accept the LOWEST COST
        
        // go through the parameters in ASCENDING order of COST, the first one that is selected will be the LOWEST COST
        if (formToCheck.material)
        {
            chaosChange += materialCost;
            return;
        }
        if (formToCheck.knowledge)
        {
            chaosChange += knowledgeCost;
            return;
        }
        if (formToCheck.heart)
        {
            chaosChange += heartCost;
            return;
        }
        if (formToCheck.harm)
        {
            chaosChange += harmCost;
            return;
        }
        if (formToCheck.enhancement)
        {
            chaosChange += enhancementCost;
            return;
        }
        if (formToCheck.fate)
        {
            chaosChange += fateCost;
            return;
        }
    }

    private void SelectHighestConsequence(Document formToCheck)
    {
        // CONSEQUENCES     // only the HIGHEST VALUE consequence will be given, when checking a selected consequence make previous ones invalid

        // go through the consequences in DESCENDING order of VALUE, the first one that is selected will be the HIGHEST VALUE
        if (formToCheck.death)
        {
            deathsGiven++;
            return;
        }
        if (formToCheck.hexed)
        {
            hexesGiven++;
            return;
        }
        if (formToCheck.legalRepercussions)
        {
            legalRepercussionsGiven++;
            return;
        }
        if (formToCheck.familyConflict)
        {
            familyConflictsGiven++;
            return;
        }
        if (formToCheck.dayRuiner)
        {
            dayRuinersGiven++;
            return;
        }
        if (formToCheck.inconvenience)
        {
            inconveniencesGiven++;
            return;
        }
    }

    private void CalculateScore()
    {
        // make temporary float which will later be turned back into an int once calculated
        float modifiedScore = grossScore;

        // check if quota bonus should be awarded
        if (wishesApproved >= currentDayQuota)
        {
            wasQuotaMet = true;
            modifiedScore += quotaBonus;
        }

        // add chaos defecit/surplus based on chaosChange
        if (chaosChange > 0)
        {
            Debug.Log("Player is in a Chaos Surplus!");

            modifiedScore += chaosChange * surplusAward;
        }
        else if (chaosChange < 0)
        {
            Debug.Log("Player is in a Chaos Defecit!");

            modifiedScore -= chaosChange * defecitAward;
        }

        // apply a bonus multiplier to final score if day was flawless
        if (!strikeGivenToday)
        {
            modifiedScore *= flawlessMultiplier;
        }

        // turn modified float score back into int (save to netScore to preserve grossScore for reference of before awards/multipliers were applied)
        netScore = (int)modifiedScore;
        CalculateBucks();
    }

    private void CalculateBucks()
    {
        geneBucksEarnt = netScore / pointsPerGeneBuck;

        geneBucksBalance += geneBucksEarnt;
    }

    public void DeliverPurchasedItem(GameObject purchasedItem)
    {

    }

    private void EndDay()
    {
        Debug.Log($"Day {currentDay} has ended");

        UIManager.GetInstance().ToggleEndOfDayScreen(true);
        UIManager.GetInstance().SetEndOfDayDetails(currentDay, wishesApproved, wishesDenied, wasQuotaMet, mistakesMade, strikesCount, chaosChange, chaosIndex, geneBucksEarnt, geneBucksBalance);

        // at the end of each day, store the consequences given on that day in each appropriate variable
        if (currentDay == 1)
        {
            dayOneInconveniencesGiven = inconveniencesGiven;
            dayOneDayRuinersGiven = dayRuinersGiven;
            dayOneFamilyConflictsGiven = familyConflictsGiven;
            dayOneLegalRepercussionsGiven = legalRepercussionsGiven;
            dayOneHexesGiven = hexesGiven;
            dayOneDeathsGiven = deathsGiven;
        }
        if (currentDay == 2)
        {
            dayTwoInconveniencesGiven = inconveniencesGiven;
            dayTwoDayRuinersGiven = dayRuinersGiven;
            dayTwoFamilyConflictsGiven = familyConflictsGiven;
            dayTwoLegalRepercussionsGiven = legalRepercussionsGiven;
            dayTwoHexesGiven = hexesGiven;
            dayTwoDeathsGiven = deathsGiven;

                // store these for newspaper
                yesterdaysInconveniencesGiven = dayOneInconveniencesGiven;
                yesterdaysDayRuinersGiven = dayOneDayRuinersGiven;
                yesterdaysFamilyConflictsGiven = dayOneFamilyConflictsGiven;
                yesterdaysLegalRepercussionsGiven = dayOneLegalRepercussionsGiven;
                yesterdaysHexesGiven = dayOneHexesGiven;
                yesterdaysDeathsGiven = dayOneDeathsGiven;
        }
        if (currentDay == 3)
        {
            dayThreeInconveniencesGiven = inconveniencesGiven;
            dayThreeDayRuinersGiven = dayRuinersGiven;
            dayThreeFamilyConflictsGiven = familyConflictsGiven;
            dayThreeLegalRepercussionsGiven = legalRepercussionsGiven;
            dayThreeHexesGiven = hexesGiven;
            dayThreeDeathsGiven = deathsGiven;

                // store these for newspaper
                yesterdaysInconveniencesGiven = dayTwoInconveniencesGiven;
                yesterdaysDayRuinersGiven = dayTwoDayRuinersGiven;
                yesterdaysFamilyConflictsGiven = dayTwoFamilyConflictsGiven;
                yesterdaysLegalRepercussionsGiven = dayTwoLegalRepercussionsGiven;
                yesterdaysHexesGiven = dayTwoHexesGiven;
                yesterdaysDeathsGiven = dayTwoDeathsGiven;
        }
        if (currentDay == 4)
        {
            dayFourInconveniencesGiven = inconveniencesGiven;
            dayFourDayRuinersGiven = dayRuinersGiven;
            dayFourFamilyConflictsGiven = familyConflictsGiven;
            dayFourLegalRepercussionsGiven = legalRepercussionsGiven;
            dayFourHexesGiven = hexesGiven;
            dayFourDeathsGiven = deathsGiven;

                // store these for newspaper
                yesterdaysInconveniencesGiven = dayThreeInconveniencesGiven;
                yesterdaysDayRuinersGiven = dayThreeDayRuinersGiven;
                yesterdaysFamilyConflictsGiven = dayThreeFamilyConflictsGiven;
                yesterdaysLegalRepercussionsGiven = dayThreeLegalRepercussionsGiven;
                yesterdaysHexesGiven = dayThreeHexesGiven;
                yesterdaysDeathsGiven = dayThreeDeathsGiven;
        }
        if (currentDay == 5)
        {
            dayFiveInconveniencesGiven = inconveniencesGiven;
            dayFiveDayRuinersGiven = dayRuinersGiven;
            dayFiveFamilyConflictsGiven = familyConflictsGiven;
            dayFiveLegalRepercussionsGiven = legalRepercussionsGiven;
            dayFiveHexesGiven = hexesGiven;
            dayFiveDeathsGiven = deathsGiven;

                // store these for newspaper
                yesterdaysInconveniencesGiven = dayFourInconveniencesGiven;
                yesterdaysDayRuinersGiven = dayFourDayRuinersGiven;
                yesterdaysFamilyConflictsGiven = dayFourFamilyConflictsGiven;
                yesterdaysLegalRepercussionsGiven = dayFourLegalRepercussionsGiven;
                yesterdaysHexesGiven = dayFourHexesGiven;
                yesterdaysDeathsGiven = dayFourDeathsGiven;
        }
    }

    public void ContinueToNextDay()
    {
        // do a check before starting new day to see if the player has 3 strikes, if so end their run (fire their ass)
        if (strikesCount > 3)
        {
            Debug.LogError("Something is wrong! player somehow has more than 3 strikes");
        }
        if (strikesCount == 3)
        {
            Fired();
        }
        else if (strikesCount <= 3)
        {
            if (currentDay < 5)
            {
                StartDay(currentDay + 1);
            }
            else if (currentDay == 5)
            {
                EndOfWeek();
            }
        }
    }

    private void Fired()
    {
        // stuff that happens when player is fired
    }

    private void EndOfWeek()
    {

    }
}
