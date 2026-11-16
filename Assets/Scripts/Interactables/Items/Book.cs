using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Book : MonoBehaviour, IInteractable
{
    private string primaryAction = "Pick Up";
    private string secondaryAction = "";

    [SerializeField] private GameObject coverPage;
    [SerializeField] private GameObject leftPage;
    [SerializeField] private GameObject rightPage;

    [SerializeField] private Material coverPageMat;
    [SerializeField] private Material leftPageMat;
    [SerializeField] private Material rightPageMat;

    [SerializeField] private GameObject bookClosed;
    [SerializeField] private GameObject bookOpen;

    private BoxCollider boxCollider;
    [SerializeField] private Vector3 bookClosedColliderSize;
    [SerializeField] private Vector3 bookOpenColliderSize;

    [SerializeField] private bool isActive = false;
    [SerializeField] private Transform restingTransform;
    [SerializeField] private Transform activeTransform;
    private bool isLerping = false;

    [SerializeField] private List<Texture> pages = new List<Texture>();
    private int currentPage;
    
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    private void Start()
    {
        leftPageMat.mainTexture = pages[1];
        rightPageMat.mainTexture = pages[2];
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
        // if book is currently out of resting place
        if (isActive)
        {
            if (isLerping)
            {
                StopAllCoroutines();
            }

            CloseBook();

            StartCoroutine(LerpObjectTransform(restingTransform.position, restingTransform.rotation.eulerAngles, 0.2f));

            isActive = false;

            primaryAction = "Pick Up";
        }
        else
        {
            if (isLerping)
            {
                StopAllCoroutines();
            }

            StartCoroutine(LerpObjectTransform(activeTransform.position, activeTransform.rotation.eulerAngles, 0.2f));

            isActive = true;

            primaryAction = "Put Back";
        }
    }

    public void SecondaryInteract()
    {
        // pick up book
        //if (gameObject.GetComponent<IHoldable>().GetHeldStatus())
        //{
        //    gameObject.GetComponent<IHoldable>().Release();

        //    secondaryAction = "Pick Up";
        //}
        //else
        //{
        //    gameObject.GetComponent<IHoldable>().Grab();

        //    secondaryAction = "PutDown";
        //}
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
        }
        else if (0 < currentPage && currentPage < (pages.Count - 2))
        {
            currentPage = currentPage + 2;

            leftPageMat.mainTexture = pages[currentPage];
            rightPageMat.mainTexture = pages[currentPage + 1];
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
            currentPage = currentPage - 2;

            leftPageMat.mainTexture = pages[currentPage];
            rightPageMat.mainTexture = pages[currentPage + 1];
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
}