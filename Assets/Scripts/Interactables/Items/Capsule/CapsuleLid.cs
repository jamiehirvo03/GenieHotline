using UnityEngine;
using System.Collections;

public class CapsuleLid : MonoBehaviour, IInteractable
{
    private string primaryAction = "Open Lid";
    private string secondaryAction = "";

    private Capsule capsule;
    private bool isLidOn = true;

    [SerializeField] private Transform lidOnTransform;
    [SerializeField] private Transform lidOffTransform;

    private bool isLerping = false;

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
        if (isLidOn)
        {
            OpenLid();
            primaryAction = "Close Lid";
        }
        else
        {
            CloseLid();
            primaryAction = "Open Lid";
        }
    }

    public void SecondaryInteract()
    {
        // no secondary interaction
    }

    private void Awake()
    {
        capsule = GetComponentInParent<Capsule>();
    }

    private void OpenLid()
    {
        if (isLerping)
        {
            StopAllCoroutines();
        }
        StartCoroutine(LerpObjectTransform(lidOffTransform.localPosition, lidOffTransform.localEulerAngles, 0.5f));

        capsule.isLidOn = false;
        isLidOn = false;
    }

    private void CloseLid()
    {
        if (isLerping)
        {
            StopAllCoroutines();
        }
        StartCoroutine(LerpObjectTransform(lidOnTransform.localPosition, lidOnTransform.localEulerAngles, 0.5f));

        capsule.isLidOn = true;
        isLidOn = true;
    }

    private IEnumerator LerpObjectTransform(Vector3 targetPosition, Vector3 targetRotation, float transformTime)
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
