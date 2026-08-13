using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;

public class Document : MonoBehaviour, IInteractable
{
    public string primaryAction;
    public string secondaryAction;
    
    private bool isHeld = false;

    [SerializeField] private float raiseAmount;

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
        Debug.Log("Document primary pressed");
    }

    public void SecondaryInteract()
    {
        Debug.Log("Document secondary pressed");

        if (isHeld)
        {
            Release();
        }
        else
        {
            Grab();
        }
    }

    private void Grab()
    {
        //transform.position = new Vector3(transform.position.x, transform.position.y + raiseAmount, transform.position.z);

        isHeld = true;
        secondaryAction = "Put Down";
    }

    private void Release()
    {
        //check if document is in bounds to be placed back down (and not colliding with anything)

        //transform.position = new Vector3(transform.position.x, transform.position.y - raiseAmount, transform.position.z);
        isHeld = false;
        secondaryAction = "Pick Up";
    }
}
