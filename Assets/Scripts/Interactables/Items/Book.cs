using Mono.Cecil;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Book : MonoBehaviour, IInteractable
{
    private string primaryAction = "";
    private string secondaryAction = "Pick Up";

    [SerializeField] private GameObject bookCover;
    [SerializeField] private GameObject pageLeft;
    [SerializeField] private GameObject pageRight;

    [SerializeField] private List<Texture> pages = new List<Texture>();

    private int currentPage;

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
    }

    public void NextPage()
    {
        if (currentPage == 0)
        {
            bookCover.SetActive(false);
            pageLeft.SetActive(true);
            pageRight.SetActive(true);

            currentPage++;
        }
        else if (0 < currentPage | currentPage < pages.Count)
        {
            currentPage = currentPage + 2;

            pageLeft.GetComponent<Material>().mainTexture = pages[currentPage];
            pageRight.GetComponent<Material>().mainTexture = pages[currentPage];
        }
    }

    public void PreviousPage()
    {
        if (currentPage == 1)
        {
            pageLeft.SetActive(false);
            pageRight.SetActive(false);
            bookCover.SetActive(true);

            currentPage--;
        }
        else if (1 < currentPage)
        {
            currentPage = currentPage - 2;
        }
    }
}