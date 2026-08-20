using UnityEditor;
using UnityEngine;

public class Pencil : MonoBehaviour, IInteractable
{
    private string primaryAction = "Pick Up";
    private string secondaryAction = "";

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
        //Debug.Log($"{primaryAction} was pressed");

        if (gameObject.GetComponent<IHoldable>().GetHeldStatus())
        {
            gameObject.GetComponent<IHoldable>().Release();

            primaryAction = "Pick Up";
        }
        else
        {
            gameObject.GetComponent<IHoldable>().Grab();

            primaryAction = "Put Down";
        }
    }

    public void SecondaryInteract()
    {
        if (secondaryAction == "")
        {
            //Debug.Log("SecondaryInteract was pressed but there is no action assigned");
        }
        else
        {
            //Debug.Log($"{secondaryAction} pressed");
        }

        if (gameObject.GetComponent<IHoldable>().GetHeldStatus())
        {
            // add code for secondary action when held (mark paper)
            
            // check the status of the checkbox you're hovering over 
            //if (gameObject.GetComponent<IInteractable>().GetCheckboxStatus())
            //{
                // if the checkbox is already marked
                //secondaryAction = "Erase";
            //}
            //else
            //{
                // if the checkbox is empty
                secondaryAction = "Sign";
            //}
        }
        {
            secondaryAction = "";
        }
    }
}
