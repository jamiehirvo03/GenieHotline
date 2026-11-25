using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using NUnit.Framework;
using Mono.Cecil;

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
    [SerializeField] private List<string> inconvenienceStories = new List<string>();
    [SerializeField] private List<string> dayRuinerStories = new List<string>();
    [SerializeField] private List<string> familyConflictStories = new List<string>();
    [SerializeField] private List<string> legalRepercussionStories = new List<string>();
    [SerializeField] private List<string> hexStories = new List<string>();
    [SerializeField] private List<string> deathStories = new List<string>();
    [Space]
    [SerializeField] private List<string> fillerStories = new List<string>();

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

            SetPage(true);
            SetPage(false);
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

    public void SetNewspaperPages()
    {
        int randomIndexOne = Random.Range(0, pages.Count - 1);  // random index for left page
        int randomIndexTwo = Random.Range(0, pages.Count - 1);  // random index for right page

        //SET LEFT PAGE
        // enable gameObject for randomly selected left page
        pages[randomIndexOne].SetActive(true);

        // set page's parent to the left side panel
        pages[randomIndexOne].transform.SetParent(leftPage.transform);

        // make sure newly added page is above the pageNumber gameObject in the hierarchy
        pages[randomIndexOne].transform.SetAsFirstSibling();

        // fix the stretch of page's rectTransform to ensure fit
        pages[randomIndexOne].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;


        //SET RIGHT PAGE
        // enable gameObject for randomly selected right page
        pages[randomIndexTwo].SetActive(true);

        // set page's parent to the right side panel
        pages[randomIndexTwo].transform.SetParent(rightPage.transform);

        // make sure newly added page is above the pageNumber gameObject in the hierarchy
        pages[randomIndexTwo].transform.SetAsFirstSibling();

        // fix the stretch of page's rectTransform to ensure fit
        pages[randomIndexTwo].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
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
}