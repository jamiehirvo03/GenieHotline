using System.Runtime.CompilerServices;
using UnityEngine;

public class Page : MonoBehaviour, IInteractable
{
    private Book book;
    private string primaryAction = "Turn Page";
    private string secondaryAction = "";

    [SerializeField] private bool isLeftPage;

    private void Awake()
    {
        book = GetComponentInParent<Book>();
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
        // turn page in set direction
        if (isLeftPage)
        {
            book.PreviousPage();
        }
        else
        {
            book.NextPage();
        }
    }

    public void SecondaryInteract()
    {
        // page requires no secondary action
    }

}
