using UnityEngine;
using static DocumentField;

public class Document : MonoBehaviour, IInteractable
{
    private string primaryAction = "Pick Up";
    private string secondaryAction = "";

    // Document fields
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

    public void SetFieldValue(FieldType selectedType)
    {
        switch (selectedType)
        {
            case FieldType.financialGain:
                financialGain = true;
                break;

            case FieldType.itemManifestation:
                itemManifestation = true;
                break;

            case FieldType.sensationManipulation:
                sensationManiupulation = true;
                break;

            case FieldType.changeOfHeart:
                changeOfHeart = true;
                break;

            case FieldType.bodilyHarm:
                bodilyHarm = true;
                break;

            case FieldType.death:
                death = true;
                break;

            case FieldType.other:
                other = true;
                break;
        }
    }
}
