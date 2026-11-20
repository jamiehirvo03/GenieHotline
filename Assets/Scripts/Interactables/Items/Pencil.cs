using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Pencil : MonoBehaviour, IInteractable
{
    private string primaryAction = "Pick Up";
    private string secondaryAction = "";

    [SerializeField] private GameObject tipOrigin;
    [SerializeField] private float tipOffset;
    
    public bool isDrawing = false;
    private float holdHeight;
    [SerializeField] private float drawingHeight = 0.5f;

    [SerializeField] private bool isStamp = false;
    private bool stampInMotion = false;


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
        // HOLDING CONTROLS

        if (GetComponent<IHoldable>().GetHeldStatus())
        {
            StopDrawing();
            GetComponent<Holdable>().holdHeight = 1;

            GetComponent<IHoldable>().Release();

            primaryAction = "Pick Up";
            secondaryAction = "";
        }
        else
        {
            GetComponent<IHoldable>().Grab();

            primaryAction = "Put Down";

            if (isStamp)
            {
                secondaryAction = "Stamp";
            }
            else
            {
                secondaryAction = "Start Drawing";
            }

            holdHeight = transform.position.y;
        }
    }

    public void SecondaryInteract()
    {
        // DRAWING CONTROLS 

        if (GetComponent<IHoldable>().GetHeldStatus())
        {
            // check if pencil is a stamp
            if (isStamp && !stampInMotion)
            {
                StartCoroutine(StampMotion());
            }
            else
            {
                // add code for primary action when held (draw)

                if (!isDrawing)
                {
                    GetComponent<Holdable>().holdHeight = drawingHeight;
                    StartDrawing();

                    secondaryAction = "Stop Drawing";
                }
                else
                {
                    GetComponent<Holdable>().holdHeight = 1;
                    StopDrawing();

                    secondaryAction = "Start Drawing";
                }
            }
        }
    }

    private void StartDrawing()
    {
        Ray ray = new Ray(tipOrigin.transform.position, tipOrigin.transform.forward);
        Debug.DrawRay(tipOrigin.transform.position, tipOrigin.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 1))
        {
            Vector3 adjustedPos = new Vector3(transform.position.x, hit.point.y + tipOffset, transform.position.z);

            //Debug.Log($"Current Drawing Height: {adjustedPos.y}");

            StartCoroutine(LerpPosition(adjustedPos, 0.1f, true));

            isDrawing = true;
        }
    }

    private void StopDrawing()
    {
        Vector3 adjustedPos = new Vector3(transform.position.x, 6, transform.position.z);

        //StartCoroutine(GetComponent<IHoldable>().LerpObjectTransform(adjustedPos, transform.rotation.eulerAngles, 0.1f, false));

        StartCoroutine(LerpPosition(adjustedPos, 0.1f, true));

        isDrawing = false;
    }

    private IEnumerator StampMotion()
    {
        stampInMotion = true;

        Vector3 startPosition = transform.position;
        Vector3 lowerPosition = new Vector3(transform.position.x, transform.position.y - drawingHeight, transform.position.z);

        yield return StartCoroutine(LerpPosition(lowerPosition, 0.1f, false));

        yield return new WaitForSeconds(0.01f);

        yield return StartCoroutine(LerpPosition(startPosition, 0.1f, true));

        yield return new WaitForSeconds(0.01f);

        stampInMotion = false;
    }

    private IEnumerator LerpPosition(Vector3 targetPosition, float time, bool canFollow)
    {
        GetComponent<IHoldable>().SetFollowStatus(canFollow);

        float elapsedTime = 0;

        Vector3 startPosition = transform.position;

        //Debug.Log($"Lerp start pos: {startPosition}, target pos: {targetPosition}");

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / time);

            yield return null;
        }
        //Debug.Log($"Lerp end pos: {transform.position}");

        transform.position = targetPosition;

        GetComponent<IHoldable>().SetFollowStatus(true);
    }
}
