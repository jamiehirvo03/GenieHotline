using UnityEngine;
using TMPro;
using static Checkbox;
using System.Collections;
using System.Security;
using System.Runtime.CompilerServices;

public class Document : MonoBehaviour, IInteractable
{
    public string primaryAction = "Pick Up";
    public string secondaryAction = "";

    // Caller details
    [SerializeField] private string callerName;
    [SerializeField] private int callerAge;
    [SerializeField] private string callerOccupation;

    [SerializeField] private TextMeshProUGUI callerNameText;
    [SerializeField] private TextMeshProUGUI callerAgeText;
    [SerializeField] private TextMeshProUGUI callerOccupationText;

    // Wishes
    public bool material;
    public bool knowledge;
    public bool heart;
    public bool harm;
    public bool enhancement;
    public bool fate;

    // Consequences
    public bool inconvenience;
    public bool dayRuiner;
    public bool familyConflict;
    public bool legalRepercussions;
    public bool hexed;
    public bool death;

    // Verdict
    public bool isApproved;
    public bool isDenied;
    [SerializeField] private bool isLocked; // no longer using document locking but will leave in code just in case

    // Capsule
    private GameObject capsule;
    private bool isCapsulePlacementWaiting = false;

    public string GetPrimaryAction()
    {
        return primaryAction;
    }

    public string GetSecondaryAction()
    {
        return secondaryAction;
    }

    public bool GetLockedStatus()
    {
        return isLocked;
    }

    public void PrimaryInteract()
    {
        if (gameObject.GetComponent<IHoldable>().GetHeldStatus())
        {
            if (isCapsulePlacementWaiting)
            {
                capsule.GetComponent<Capsule>().InsertDocument(gameObject);
                
                InteractionManager.GetInstance().PutDownItem();

                isCapsulePlacementWaiting = false;
            }
            else
            {
                gameObject.GetComponent<IHoldable>().Release();

                primaryAction = "Pick Up";
            }
        }
        else
        {
            gameObject.GetComponent<IHoldable>().Grab();

            primaryAction = "Put Down";
        }
    }

    public void SecondaryInteract()
    {     
        // add code that brings document to 'inspection' position and change movement for more fine control
    }

    private void Awake()
    {
        capsule = InteractionManager.GetInstance().pneumaticCapsule;
    }

    public void CapsuleDetection(string labelText, bool isHovered)
    {
        primaryAction = labelText;

        isCapsulePlacementWaiting = isHovered;
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

    public void ApproveDocument()
    {
        isDenied = false;
        isApproved = true;
        //isLocked = true;

        Debug.Log($"{gameObject.name} locked");
    }

    public void DenyDocument()
    {
        isApproved = false;
        isDenied = true;
        //isLocked = true;

        Debug.Log($"{gameObject.name} locked");
    }

    

    private IEnumerator LerpObjectTransform(Vector3 targetPosition, Vector3 targetRotation, float transformTime)
    {
        Debug.Log($"Lerping object to position = {targetPosition}");

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
    }
}
