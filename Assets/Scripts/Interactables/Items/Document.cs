using UnityEngine;
using static Checkbox;

public class Document : MonoBehaviour, IInteractable
{
    private string primaryAction = "Pick Up";
    private string secondaryAction = "";

    // Caller details
    [SerializeField] private string callerName;
    [SerializeField] private int callerAge;
    [SerializeField] private string callerOccupation;

    // Choice fields
    [SerializeField] private bool financialGain;
    [SerializeField] private bool itemManifestation;
    [SerializeField] private bool sensationManiupulation;
    [SerializeField] private bool changeOfHeart;
    [SerializeField] private bool bodilyHarm;
    [SerializeField] private bool death;
    [SerializeField] private bool other;
 
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
    }

    public void SetCallerDetails(string name, int age, string occupation)
    {
        callerName = name;
        callerAge = age;
        callerOccupation = occupation;
    }

    public void TickCheckbox(CheckboxType checkboxType)
    {
        switch (checkboxType)
        {
            case CheckboxType.financialGain:
                financialGain = true;
                break;

            case CheckboxType.itemManifestation:
                itemManifestation = true;
                break;

            case CheckboxType.sensationManipulation:
                sensationManiupulation = true;
                break;

            case CheckboxType.changeOfHeart:
                changeOfHeart = true;
                break;

            case CheckboxType.bodilyHarm:
                bodilyHarm = true;
                break;

            case CheckboxType.death:
                death = true;
                break;

            case CheckboxType.other:
                other = true;
                break;
        }
    }

    public void ClearCheckbox(CheckboxType checkboxType)
    {
        switch (checkboxType)
        {
            case CheckboxType.financialGain:
                financialGain = false;
                break;

            case CheckboxType.itemManifestation:
                itemManifestation = false;
                break;

            case CheckboxType.sensationManipulation:
                sensationManiupulation = false;
                break;

            case CheckboxType.changeOfHeart:
                changeOfHeart = false;
                break;

            case CheckboxType.bodilyHarm:
                bodilyHarm = false;
                break;

            case CheckboxType.death:
                death = false;
                break;

            case CheckboxType.other:
                other = false;
                break;
        }
    }
}
