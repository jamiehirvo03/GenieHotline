using System.Collections;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Document : MonoBehaviour, IInteractable
{
    private string primaryAction;
    private string secondaryAction = "Pick Up"; // alt: PutDown
 
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

        if (gameObject.GetComponent<IHoldable>().GetHeldStatus())
        {
            gameObject.GetComponent<IHoldable>().Release();

            secondaryAction = "Pick Up";
        }
        else
        {
            gameObject.GetComponent<IHoldable>().Grab();

            secondaryAction = "Put Down";
        }
    }
}
