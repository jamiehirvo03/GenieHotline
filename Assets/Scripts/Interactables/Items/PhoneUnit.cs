using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class PhoneUnit : MonoBehaviour, IInteractable
{
    private string primaryAction = "Answer Phone";
    private string secondaryAction = "";

    private bool isActive = false;

    [SerializeField] private Vector3 defaultPosition;
    [SerializeField] private Vector3 defaultOrientation;
    [SerializeField] private Vector3 activePosition;
    [SerializeField] private Vector3 activeOrientation;

    private bool isLerping = false;

    public string GetPrimaryAction()
    {
        return primaryAction;
    }

    public string GetSecondaryAction()
    {
        return secondaryAction;
    }

    void Start()
    {
        defaultPosition = transform.localPosition;
        defaultOrientation = transform.localEulerAngles;
    }

    public void PrimaryInteract()
    {
        if (GameManager.GetInstance().callersInQueue)
        {
            if (!isActive)
            {
                // move phone unit gameobject to active position
                if (isLerping)
                {
                    StopAllCoroutines();
                }

                AnswerPhone();
                primaryAction = "Hang Up Phone";
            }
            else
            {
                // move phone unit gameObject to rest position
                if (isLerping)
                {
                    StopAllCoroutines();
                }

                HangUpPhone();
                primaryAction = "Answer Phone";
            }
        }
    }

    public void SecondaryInteract()
    {
        Debug.Log($"{gameObject} has no Secondary Interaction");
    }

    private void AnswerPhone()
    {
        GameManager.GetInstance().AnswerPhone();
        StartCoroutine(LerpObjectPos(activePosition, activeOrientation, 1f));
        
        isActive = true;
    }

    private void HangUpPhone()
    {
        GameManager.GetInstance().HangUpPhone();
        StartCoroutine(LerpObjectPos(defaultPosition, defaultOrientation, 1f));

        isActive = false;
    }

    private IEnumerator LerpObjectPos(Vector3 targetPosition, Vector3 targetRotation, float transformTime)
    {
        Debug.Log($"Lerping object to position = {targetPosition}");

        isLerping = true;

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

        isLerping = false;
    }
}
