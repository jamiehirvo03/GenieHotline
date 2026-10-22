using Mono.Cecil;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Book : MonoBehaviour, IInteractable
{
    private string primaryAction = "";
    private string secondaryAction = "Pick Up";

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
        // if held, bring book closer to camera
    }

    public void SecondaryInteract()
    {
        // pick up book
        if (gameObject.GetComponent<IHoldable>().GetHeldStatus())
        {
            gameObject.GetComponent<IHoldable>().Release();

            secondaryAction = "Pick Up";
        }
        else
        {
            gameObject.GetComponent<IHoldable>().Grab();

            secondaryAction = "PutDown";
        }
    }

    public void NextPage()
    {
        if (currentPage == 0)
        {
            bookClosed.SetActive(false);
            bookOpen.SetActive(true);
            boxCollider.size = bookOpenColliderSize;

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
            bookOpen.SetActive(false);
            bookClosed.SetActive(true);
            boxCollider.size = bookClosedColliderSize;

            currentPage--;
        }
        else if (1 < currentPage)
        {
            currentPage = currentPage - 2;

            leftPageMat.mainTexture = pages[currentPage];
            rightPageMat.mainTexture = pages[currentPage + 1];
        }
    }
}