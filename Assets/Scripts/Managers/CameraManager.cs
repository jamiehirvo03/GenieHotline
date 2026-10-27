using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour
{
    private static CameraManager instance;

    // camera
    private Camera activeCamera;

    [SerializeField] private float transitionSpeed;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one CameraManager in the scene.");
        }

        instance = this;

        activeCamera = Camera.main;
    }

    public static CameraManager GetInstance()
    {
        return instance;
    }

    public void MoveCamera(Transform targetTransform)
    {
        StartCoroutine(LerpObjectTransform(targetTransform.position, targetTransform.eulerAngles, transitionSpeed));
    }

    private IEnumerator LerpObjectTransform(Vector3 targetPosition, Vector3 targetRotation, float transformTime)
    {
        Debug.Log($"Lerping object to position = {targetPosition}");

        Vector3 startPosition = activeCamera.transform.localPosition;
        Quaternion startRotation = activeCamera.transform.localRotation;
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);

        float elapsedTime = 0f;

        while (elapsedTime < transformTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpTime = elapsedTime / transformTime;

            activeCamera.transform.localPosition = Vector3.Lerp(startPosition, targetPosition, lerpTime);

            activeCamera.transform.localRotation = Quaternion.Lerp(startRotation, targetQuaternion, lerpTime);

            yield return null;
        }
        // everything below this will happen once lerp is complete

        activeCamera.transform.localPosition = targetPosition; // snap position to target to fix weird numbers

    }
}
