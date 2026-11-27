using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using NUnit.Framework;
using Mono.Cecil;
using UnityEngine.UI;
using UnityEditor.PackageManager.Requests;

public class Book : MonoBehaviour, IInteractable
{
    private string primaryAction;
    private string secondaryAction;

    [SerializeField] private GameObject coverPage;
    [SerializeField] private GameObject leftPage;
    [SerializeField] private GameObject rightPage;
    [SerializeField] private GameObject storedPages;

    [SerializeField] private TextMeshProUGUI leftPageNumber;
    [SerializeField] private TextMeshProUGUI rightPageNumber;
    [SerializeField] private GameObject leftPageCurl;
    [SerializeField] private GameObject rightPageCurl;

    [SerializeField] private GameObject bookClosed;
    [SerializeField] private GameObject bookOpen;

    private BoxCollider boxCollider;
    [SerializeField] private Vector3 bookClosedColliderSize;
    [SerializeField] private Vector3 bookOpenColliderSize;

    [SerializeField] private bool isActive = false;
    [SerializeField] private Transform restingTransform;
    [SerializeField] private Transform activeTransform;
    private bool isLerping = false;

    [SerializeField] private List<GameObject> pages = new List<GameObject>();
    private int currentPage;

    [SerializeField] private bool isMovableBook = false;

    [Header("Newspaper Stuff")]
    private bool isNewspaper = false;
    [Space]
    [SerializeField] private Image headlineImage;
    [SerializeField] private TextMeshProUGUI headlineHeading;
    [SerializeField] private TextMeshProUGUI headlineBody;
    [Space]
    [SerializeField] private Image coverBannerImage;
    [SerializeField] private TextMeshProUGUI coverBannerHeading;
    [SerializeField] private TextMeshProUGUI coverBannerBody;
    [Space]
    [SerializeField] private int totalSectionsInNewspaper = 8;
    [Space]
    // ALL OTHER SECTION ELEMENTS ARE STORED IN 'NEWSPAPERLAYOUT' SCRIPTS ATTACHED TO EACH LAYOUT PREFAB
    [Header("Inconvenience Stories")]
    [SerializeField] private List<string> inconvenienceHeaders= new List<string>();
    [SerializeField] private List<string> inconvenienceBodies = new List<string>();
    [SerializeField] private List<Sprite> inconvenienceImages = new List<Sprite>();
    [Header("DayRuiner Stories")]
    [SerializeField] private List<string> dayRuinerHeaders = new List<string>();
    [SerializeField] private List<string> dayRuinerBodies = new List<string>();
    [SerializeField] private List<Sprite> dayRuinerImages= new List<Sprite>();
    [Header("FamilyConflict Stories")]
    [SerializeField] private List<string> familyConflictHeaders= new List<string>();
    [SerializeField] private List<string> familyConflictBodies = new List<string>();
    [SerializeField] private List<Sprite> familyConflictImages = new List<Sprite>();
    [Header("LegalRepercussions Stories")]
    [SerializeField] private List<string> legalRepercussionsHeaders = new List<string>();
    [SerializeField] private List<string> legalRepercussionsBodies = new List<string>();
    [SerializeField] private List<Sprite> legalRepercussionsImages = new List<Sprite>();
    [Header("Hex Stories")]
    [SerializeField] private List<string> hexHeaders = new List<string>();
    [SerializeField] private List<string> hexBodies = new List<string>();
    [SerializeField] private List<Sprite> hexImages = new List<Sprite>();
    [Header("Death Stories")]
    [SerializeField] private List<string> deathHeaders = new List<string>();
    [SerializeField] private List<string> deathBodies = new List<string>();
    [SerializeField] private List<Sprite> deathImages = new List<Sprite>();
    [Space]
    // FILLER STORIES
    [Header("LowChaos Stories")]
    [SerializeField] private List<string> lowChaosFillerHeaders = new List<string>();
    [SerializeField] private List<string> lowChaosFillerBodies = new List<string>();
    [SerializeField] private List<Sprite> lowChaosFillerImages = new List<Sprite>();
    [Header("NeutralChaos Stories")]
    [SerializeField] private List<string> neutralChaosFillerHeaders = new List<string>();
    [SerializeField] private List<string> neutralChaosFillerBodies = new List<string>();
    [SerializeField] private List<Sprite> neutralChaosFillerImages = new List<Sprite>();
    [Header("HighChaos Stories")]
    [SerializeField] private List<string> highChaosFillerHeaders = new List<string>();
    [SerializeField] private List<string> highChaosFillerBodies = new List<string>();
    [SerializeField] private List<Sprite> highChaosFillerImages = new List<Sprite>();

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            // disable all pages just in case any were left enabled from editing
            pages[i].gameObject.SetActive(false);
        }

        CloseBook();

        coverPage.GetComponent<BoxCollider>().enabled = false;

        if (isMovableBook)
        {
            primaryAction = "Pick Up";
            secondaryAction = "Read";
        }
        else
        {
            primaryAction = "Read";
            secondaryAction = "";
        }

        if (isNewspaper)
        {
            SetNewspaperPages();
        }
    }

    public string GetPrimaryAction()
    {
        return primaryAction;
    }
    public string GetSecondaryAction()
    {
        return secondaryAction;
    }

    public void PrimaryInteract()
    {
        if (isMovableBook)
        {
            // if book isn't already being read
            if (!isActive)
            {
                // if book is currently being held
                if (gameObject.GetComponent<IHoldable>().GetHeldStatus())
                {
                    // put book down
                    gameObject.GetComponent<IHoldable>().Release();

                    primaryAction = "Pick Up";
                    secondaryAction = "Read";
                }
                else
                {
                    // hold book
                    gameObject.GetComponent<IHoldable>().Grab();

                    primaryAction = "Put Down";
                    secondaryAction = "";
                }
            }
        }
        // if the book is setup to sit in dedicated resting place when not being read
        else
        {
            // if book isn't already being read
            if (!isActive)
            {
                ToggleRead(true);
            }
            // if book is already being read
            else
            {
                ToggleRead(false);
            }
        }
    }

    public void SecondaryInteract()
    {
        // secondary action only needed if book can also be picked up and moved around
        if (isMovableBook)
        {
            // if book isn't already being read
            if (!isActive)
            {
                // only need secondary action for movable book if it's not already being held
                if (!gameObject.GetComponent<IHoldable>().GetHeldStatus())
                {
                    ToggleRead(true);
                }
            }
            // if book is already being read
            else
            {
                ToggleRead(false);
            }
        }
    }

    public void ToggleRead(bool status)
    {
        // start reading
        if (status)
        {
            if (isLerping)
            {
                StopAllCoroutines();
            }
            StartCoroutine(LerpObjectTransform(activeTransform.position, activeTransform.rotation.eulerAngles, 0.2f));

            isActive = true;

            coverPage.GetComponent<BoxCollider>().enabled = true;

            if (isMovableBook)
            {
                primaryAction = "";
                secondaryAction = "Stop Reading";
            }
            else
            {
                primaryAction = "Stop Reading";
                secondaryAction = "";
            }
        }
        // stop reading
        else
        {
            CloseBook();

            if (isLerping)
            {
                StopAllCoroutines();
            }
            StartCoroutine(LerpObjectTransform(restingTransform.position, restingTransform.rotation.eulerAngles, 0.2f));

            isActive = false;

            coverPage.GetComponent<BoxCollider>().enabled = false;

            if (isMovableBook)
            {
                primaryAction = "Pick Up";
                secondaryAction = "Read";
            }
            else
            {
                primaryAction = "Read";
                secondaryAction = "";
            }
        }
    }

    public void OpenBook()
    {
        bookClosed.SetActive(false);
        bookOpen.SetActive(true);
        //boxCollider.size = bookOpenColliderSize;
        boxCollider.enabled = false;
    }

    public void CloseBook()
    {
        bookOpen.SetActive(false);
        bookClosed.SetActive(true);
        //boxCollider.size = bookClosedColliderSize;
        boxCollider.enabled = true;

        currentPage = 0;
    }

    public void NextPage()
    {
        if (currentPage == 0)
        {
            OpenBook();

            currentPage++;

            if (!isNewspaper)
            {
                SetPage(true);
                SetPage(false);
            }
        }
        else if (0 < currentPage && currentPage < (pages.Count - 2))
        {
            ClearPage(true);
            ClearPage(false);

            currentPage = currentPage + 2;

            SetPage(true);
            SetPage(false);
        }
        else
        {
            Debug.Log("No further pages");
        }
    }

    public void PreviousPage()
    {
        if (currentPage == 1)
        {
            CloseBook();
        }
        else if (1 < currentPage)
        {
            ClearPage(true);
            ClearPage(false);

            currentPage = currentPage - 2;

            SetPage(true);
            SetPage(false);
        }
    }

    private void SetPage(bool isLeftPage)
    {
        // set new left page
        if (isLeftPage)
        {
            // enable gameObject for page that is to go on left side
            pages[currentPage].SetActive(true);

            // set page's parent to the left side panel
            pages[currentPage].transform.SetParent(leftPage.transform);

            // make sure newly added page is above the pageNumber gameObject in the hierarchy
            pages[currentPage].transform.SetAsFirstSibling();

            // fix the stretch of page's RectTransform to ensure fit
            pages[currentPage].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // set new page number
            leftPageNumber.text = $"{currentPage}";
        }
        // set new right page
        else
        {
            int currentRightPage = currentPage + 1;

            // enable gameObject for page that is to go on right side
            pages[currentRightPage].SetActive(true);

            // set page's parent to the right side panel
            pages[currentRightPage].transform.SetParent(rightPage.transform);

            // make sure newly added page is above the pageNumber gameObject in the hierarchy
            pages[currentRightPage].transform.SetAsFirstSibling();

            // fix the stretch of page's RectTransform to ensure fit
            pages[currentRightPage].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // set new page number
            rightPageNumber.text = $"{currentPage + 1}";

            if (currentRightPage == pages.Count - 1)
            {
                rightPageCurl.SetActive(false);
            }
            else
            {
                rightPageCurl.SetActive(true);
            }
        }
    }

    // essentially doing SetPage() backwards to undo current pages before they change to next pages
    private void ClearPage(bool isLeftPage)
    {
        // clear old left page
        if (isLeftPage)
        {
            // reset unneeded page's parent to storedPages
            pages[currentPage].transform.SetParent(storedPages.transform);

            // fix the stretch of page's RectTransform to ensure fit
            pages[currentPage].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // disable old page gameObject
            pages[currentPage].SetActive(false);
        }
        // clear old right page
        else
        {
            // reset unneeded page's parent to storedPages
            pages[currentPage + 1].transform.SetParent(storedPages.transform);

            // fix the stretch of page's RectTransform to ensure fit
            pages[currentPage + 1].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            // disable old page gameObject
            pages[currentPage + 1].SetActive(false);
        }
    }

    private IEnumerator LerpObjectTransform(Vector3 targetPosition, Vector3 targetRotation, float transformTime)
    {
        Debug.Log($"Lerping object to position = {targetPosition}");

        isLerping = true;

        Vector3 startPosition = transform.localPosition;
        Quaternion startRotation = transform.localRotation;
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);

        float elapsedTime = 0f;

        while (elapsedTime < transformTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpTime = elapsedTime / transformTime;

            transform.localPosition = Vector3.Lerp(startPosition, targetPosition, lerpTime);

            transform.localRotation = Quaternion.Lerp(startRotation, targetQuaternion, lerpTime);

            yield return null;
        }
        // everything below this will happen once lerp is complete

        transform.localPosition = targetPosition; // snap position to target to fix weird numbers

        isLerping = false;
    }

    // NEWSPAPER STUFF (MIGHT BE A GOOD IDEA TO RELOCATE TO DEDICATED NEWSPAPER SCRIPT IF THINGS BECOME TOO MESSY)
    public void SetNewspaperPages()
    {
        // SET PAGE LAYOUTS
            RandomizeLayoutsList();

            //SET LEFT PAGE LAYOUT
            // enable gameObject for randomly selected left page
            pages[0].SetActive(true);

            // set page's parent to the left side panel
            pages[0].transform.SetParent(leftPage.transform);

            // make sure newly added page is above the pageNumber gameObject in the hierarchy
            pages[0].transform.SetAsFirstSibling();

            // fix the stretch of page's rectTransform to ensure fit
            pages[0].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;


            //SET RIGHT PAGE LAYOUT
            // enable gameObject for randomly selected right page
            pages[1].SetActive(true);

            // set page's parent to the right side panel
            pages[1].transform.SetParent(rightPage.transform);

            // make sure newly added page is above the pageNumber gameObject in the hierarchy
            pages[1].transform.SetAsFirstSibling();

            // fix the stretch of page's rectTransform to ensure fit
            pages[1].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        // Create temporary lists for story components (their indexes must stay alligned when the pool is formed)
        List <string> headersPool = new List<string>();
        List <string> bodiesPool = new List<string>();
        List <Sprite> imagesPool = new List<Sprite>();

        SelectDailyStories(headersPool, bodiesPool, imagesPool);

        // get random order to go through each of the lists, this will determine where each story is added in the paper
        List<int> poolOrder = GetJumbledOrderList();

        // APPLY SECTION CONTENT
        // COVER CONTENT
        headlineHeading.text = headersPool[poolOrder[0]];
        headlineBody.text = bodiesPool[poolOrder[0]];   
        headlineImage.sprite = imagesPool[poolOrder[0]];

        coverBannerHeading.text = headersPool[poolOrder[1]];
        coverBannerBody.text = bodiesPool[poolOrder[1]];
        coverBannerImage.sprite = imagesPool[poolOrder[1]];

        // LEFT PAGE CONTENT
            // section one
            pages[0].GetComponent<NewspaperLayout>().sectionOneHeader.text = headersPool[poolOrder[2]];
            pages[0].GetComponent<NewspaperLayout>().sectionOneBody.text = bodiesPool[poolOrder[2]];
            pages[0].GetComponent<NewspaperLayout>().sectionOneImage.sprite = imagesPool[poolOrder[2]];

            // section two
            pages[0].GetComponent<NewspaperLayout>().sectionTwoHeader.text = headersPool[poolOrder[3]];
            pages[0].GetComponent<NewspaperLayout>().sectionTwoBody.text = bodiesPool[poolOrder[3]];
            pages[0].GetComponent<NewspaperLayout>().sectionTwoImage.sprite = imagesPool[poolOrder[3]];

            // section three
            pages[0].GetComponent<NewspaperLayout>().sectionThreeHeader.text = headersPool[poolOrder[4]];
            pages[0].GetComponent<NewspaperLayout>().sectionThreeBody.text = bodiesPool[poolOrder[4]];
            pages[0].GetComponent<NewspaperLayout>().sectionThreeImage.sprite = imagesPool[poolOrder[4]];

        // RIGHT PAGE CONTENT
            // section one
            pages[1].GetComponent<NewspaperLayout>().sectionOneHeader.text = headersPool[poolOrder[5]];
            pages[1].GetComponent<NewspaperLayout>().sectionOneBody.text = bodiesPool[poolOrder[5]];
            pages[1].GetComponent<NewspaperLayout>().sectionOneImage.sprite = imagesPool[poolOrder[5]];

            // section two
            pages[1].GetComponent<NewspaperLayout>().sectionTwoHeader.text = headersPool[poolOrder[6]];
            pages[1].GetComponent<NewspaperLayout>().sectionTwoBody.text = bodiesPool[poolOrder[6]];
            pages[1].GetComponent<NewspaperLayout>().sectionTwoImage.sprite = imagesPool[poolOrder[6]];

            // section three
            pages[1].GetComponent<NewspaperLayout>().sectionThreeHeader.text = headersPool[poolOrder[7]];
            pages[1].GetComponent<NewspaperLayout>().sectionThreeBody.text = bodiesPool[poolOrder[7]];
            pages[1].GetComponent<NewspaperLayout>().sectionThreeImage.sprite = imagesPool[poolOrder[7]];
    }

    public void SelectDailyStories(List<string> headersPool, List<string> bodiesPool, List<Sprite> imagesPool)
    {
        GameManager gameManager = GameManager.GetInstance();

        // use random order to add story to pool so it doesn't get repetitive (also negates chance of a newspaper having doubled up stories)
        

        if (gameManager.yesterdaysInconveniencesGiven > 0)
        {
            List<int> randomOrder = GetJumbledOrderList();

            for (int i = 0; i < gameManager.yesterdaysInconveniencesGiven; i++)
            {
                headersPool.Add(inconvenienceHeaders[randomOrder[i]]);
                bodiesPool.Add(inconvenienceBodies[randomOrder[i]]);
                imagesPool.Add(inconvenienceImages[randomOrder[i]]);
            }
        }
        if (gameManager.yesterdaysDayRuinersGiven > 0)
        {
            List<int> randomOrder = GetJumbledOrderList();

            for (int i = 0; i < gameManager.yesterdaysDayRuinersGiven; i++)
            {
                headersPool.Add(dayRuinerHeaders[randomOrder[i]]);
                bodiesPool.Add(dayRuinerBodies[randomOrder[i]]);
                imagesPool.Add(dayRuinerImages[randomOrder[i]]);
            }
        }
        if (gameManager.yesterdaysFamilyConflictsGiven > 0)
        {
            List<int> randomOrder = GetJumbledOrderList();

            for (int i = 0; i < gameManager.yesterdaysFamilyConflictsGiven; i++)
            {
                headersPool.Add(familyConflictHeaders[randomOrder[i]]);
                bodiesPool.Add(familyConflictBodies[randomOrder[i]]);
                imagesPool.Add(familyConflictImages[randomOrder[i]]);
            }
        }
        if (gameManager.yesterdaysLegalRepercussionsGiven > 0)
        {
            List<int> randomOrder = GetJumbledOrderList();

            for (int i = 0; i < gameManager.yesterdaysLegalRepercussionsGiven; i++)
            {
                headersPool.Add(legalRepercussionsHeaders[randomOrder[i]]);
                bodiesPool.Add(legalRepercussionsBodies[randomOrder[i]]);
                imagesPool.Add(legalRepercussionsImages[randomOrder[i]]);
            }
        }
        if (gameManager.yesterdaysHexesGiven > 0)
        {
            List<int> randomOrder = GetJumbledOrderList();

            for (int i = 0; i < gameManager.yesterdaysHexesGiven; i++)
            {
                headersPool.Add(hexHeaders[randomOrder[i]]);
                bodiesPool.Add(hexBodies[randomOrder[i]]);
                imagesPool.Add(hexImages[randomOrder[i]]);
            }
        }
        if (gameManager.yesterdaysDeathsGiven > 0)
        {
            List<int> randomOrder = GetJumbledOrderList();

            for (int i = 0; i < gameManager.yesterdaysDeathsGiven; i++)
            {
                headersPool.Add(deathHeaders[randomOrder[i]]);
                bodiesPool.Add(deathBodies[randomOrder[i]]);
                imagesPool.Add(deathImages[randomOrder[i]]);
            }
        }

        // calculate number of consequences given that day
        int totalConsequencesGranted = gameManager.yesterdaysInconveniencesGiven + gameManager.yesterdaysDayRuinersGiven + gameManager.yesterdaysFamilyConflictsGiven + gameManager.yesterdaysLegalRepercussionsGiven + gameManager.yesterdaysHexesGiven + gameManager.yesterdaysDeathsGiven;

        // calculate how many slots still need to be filled after consequence stories are added
        int slotsToFill = totalSectionsInNewspaper - totalConsequencesGranted;

        // fill remaining slots with filler stories
        if (slotsToFill > 0)
        {
            List<int> randomOrder = GetJumbledOrderList();

            for (int i = 0; i < slotsToFill; i++)
            {
                if (gameManager.chaosLevel == "LowChaos")
                {
                    headersPool.Add(lowChaosFillerHeaders[randomOrder[i]]);
                    bodiesPool.Add(lowChaosFillerBodies[randomOrder[i]]);
                    imagesPool.Add(lowChaosFillerImages[randomOrder[i]]);
                }
                else if (gameManager.chaosLevel == "NeutralChaos")
                {
                    headersPool.Add(neutralChaosFillerHeaders[randomOrder[i]]);
                    bodiesPool.Add(neutralChaosFillerBodies[randomOrder[i]]);
                    imagesPool.Add(neutralChaosFillerImages[randomOrder[i]]);
                }
                else if (gameManager.chaosLevel == "HighChaos")
                {
                    headersPool.Add(highChaosFillerHeaders[randomOrder[i]]);
                    bodiesPool.Add(highChaosFillerBodies[randomOrder[i]]);
                    imagesPool.Add(highChaosFillerImages[randomOrder[i]]);
                }
                else
                {
                    Debug.LogError("Couldn't find a matching chaos level!");
                }
            }
        }
    }

    // this will be used to navigate through lists randomly (as each story is made up of 3 component lists that must stay consistent with each other) ((randomize the order instead of randomizing the list))
    private List<int> GetJumbledOrderList()
    {
        List<int> standardOrder = new List<int>();
        standardOrder.Add(0);
        standardOrder.Add(1);
        standardOrder.Add(2);
        standardOrder.Add(3);
        standardOrder.Add(4);

        List<int> randomizedOrder = standardOrder;

        for (int i = 0; i < randomizedOrder.Count; i++)
        {
            int r = (int)(Random.value * (randomizedOrder.Count - i));
            int tempVar = randomizedOrder[i];
            randomizedOrder[r] = randomizedOrder[i];
            randomizedOrder[i] = tempVar;
        }

        return randomizedOrder;
    }

    private void RandomizeLayoutsList()
    {
        for (int i = 0; i < pages.Count; i++)
        {
            int r = (int)(Random.value * (pages.Count - i));
            GameObject tempVar = pages[r];
            pages[r] = pages[i];
            pages[i] = tempVar;
        }
    }
}