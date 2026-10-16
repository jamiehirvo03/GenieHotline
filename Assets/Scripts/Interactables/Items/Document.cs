using UnityEngine;
using TMPro;
using static Checkbox;
using System.Runtime.CompilerServices;

public class Document : MonoBehaviour, IInteractable
{
    private string primaryAction = "Pick Up";
    private string secondaryAction = "";

    // Caller details
    [SerializeField] private string callerName;
    [SerializeField] private int callerAge;
    [SerializeField] private string callerOccupation;

    [SerializeField] private TextMeshProUGUI callerNameText;
    [SerializeField] private TextMeshProUGUI callerAgeText;
    [SerializeField] private TextMeshProUGUI callerOccupationText;

    // Wishes
    [SerializeField] private bool material;
    [SerializeField] private bool knowledge;
    [SerializeField] private bool harm;
    [SerializeField] private bool heart;
    [SerializeField] private bool enhancement;
    [SerializeField] private bool fate;

    // Consequences
    [SerializeField] private bool inconvenience;
    [SerializeField] private bool dayRuiner;
    [SerializeField] private bool familyConflict;
    [SerializeField] private bool legalRepercussions;
    [SerializeField] private bool hexed;
    [SerializeField] private bool death;
 
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

        callerNameText.text = callerName;
        callerAgeText.text = callerAge.ToString();
        callerOccupationText.text = callerOccupation;
    }

    public void CheckboxStatus(CheckboxType checkboxType, bool status)
    {
        switch (checkboxType)
        {
            // wishes 
            case CheckboxType.material:
                material = status;
                break;

            case CheckboxType.knowledge:
                knowledge = status;
                break;

            case CheckboxType.heart:
                heart = status;
                break;

            case CheckboxType.harm:
                harm = status;
                break;

            case CheckboxType.enhancement:
                enhancement = status;
                break;

            case CheckboxType.fate:
                fate = status;
                break;

            // consequences
            case CheckboxType.inconvenience:
                inconvenience = status;
                break;

            case CheckboxType.dayRuiner:
                dayRuiner = status;
                break;

            case CheckboxType.familyConflict:
                familyConflict = status;
                break;

            case CheckboxType.legalRepercussions:
                legalRepercussions = status;
                break;

            case CheckboxType.hexed:
                hexed = status;
                break;

            case CheckboxType.death:
                death = status;
                break;

        }
    }
}
