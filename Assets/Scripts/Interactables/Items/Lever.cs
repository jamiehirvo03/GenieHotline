using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class Lever : MonoBehaviour, IInteractable
{
    private string primaryAction = "Grab";
    private string secondaryAction = "";

    private bool isLeverHeld = false;

    private Vector3 velocity = Vector3.zero;
    [SerializeField] private float followTime = 0.01f;
    [SerializeField] private float followSensitivity = 2f;

    private Vector3 defaultHandlePosition;
    [SerializeField] private float leverReturnTime = 0.5f;

    [SerializeField] private float upperLimit;
    [SerializeField] private float lowerLimit;

    private Vector3 upperPosition;
    private Vector3 lowerPosition;

    private float activationLeeway = 0.001f;

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
        if (!isLeverHeld)
        {
            Debug.Log("Lever has been held");

            isLeverHeld = true;
            //InteractionManager.GetInstance().PickUpItem(this.gameObject);
        }
        else
        {
            Debug.Log("Lever has been released");

            ReleaseHandle();
        }
    }
    
    public void SecondaryInteract()
    {
        Debug.Log("Lever has no secondary interact");
    }

    void Start()
    {
        defaultHandlePosition = transform.position;

        upperPosition = new Vector3(defaultHandlePosition.x, defaultHandlePosition.y + upperLimit, defaultHandlePosition.z);
        lowerPosition = new Vector3(defaultHandlePosition.x, defaultHandlePosition.y - lowerLimit, defaultHandlePosition.z);
    }

    void Update()
    {
        if (isLeverHeld)
        {
            FollowCursorHeight();

            // if the handle is moved up and past the upper activation range
            if (transform.position.y > defaultHandlePosition.y && transform.position.y > upperPosition.y - activationLeeway)
            {
                // snap to upper limit
                transform.position = new Vector3(defaultHandlePosition.x, upperPosition.y, defaultHandlePosition.z);

                LeverActivation();
            }
            // if the handle is moved down and past the lower activation range
            if (transform.position.y < defaultHandlePosition.y && transform.position.y < lowerPosition.y + activationLeeway)
            {
                // snap to lower limit
                transform.position = new Vector3(defaultHandlePosition.x, lowerPosition.y, defaultHandlePosition.z);

                LeverActivation();
            }
        }
    }

    private void ReleaseHandle()
    {
        isLeverHeld = false;
        //InteractionManager.GetInstance().PutDownItem();

        StartCoroutine(ReturnLeverPosition());
    }

    private void FollowCursorHeight()
    {
        float mouseDelta = Input.mousePositionDelta.y * followSensitivity;

        Debug.Log($"mouseDelta: {mouseDelta}");

        Vector3 heightTarget = new Vector3(0, 0, 0);

        if (mouseDelta > 0)
        {
            heightTarget = new Vector3(defaultHandlePosition.x, transform.position.y + mouseDelta, defaultHandlePosition.z);
        }
        else if (mouseDelta < 0)
        {
            heightTarget = new Vector3(defaultHandlePosition.x, transform.position.y - mouseDelta, defaultHandlePosition.z);
        }

        transform.position = Vector3.SmoothDamp(transform.position, heightTarget, ref velocity, followTime);
    }

    private void LeverActivation()
    {
        // if it was an upper activation
        if (transform.position.y > defaultHandlePosition.y)
        {
            Debug.Log("Sending capsule to the boss");
            // trigger code for 'sending' capsule to the boss


            ReleaseHandle();
        }
        else if (transform.position.y < defaultHandlePosition.y)
        {
            Debug.Log("Sending capsule to the archives");
            // trigger code for 'sending' capsule to the archives


            ReleaseHandle();
        }
    }
    
    private IEnumerator ReturnLeverPosition()
    {
        Vector3 startPosition = transform.position;

        float elapsedTime = 0f;

        while (elapsedTime < leverReturnTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpTime = elapsedTime / leverReturnTime;

            transform.position = Vector3.Lerp(startPosition, defaultHandlePosition, leverReturnTime);

            yield return null;
        }

        transform.position = defaultHandlePosition;
    }
}
