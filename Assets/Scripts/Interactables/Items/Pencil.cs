using System.Collections;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class Pencil : MonoBehaviour, IInteractable
{
    private string primaryAction = "";
    private string secondaryAction = "Pick Up";

    [SerializeField] private GameObject tipOrigin;
    [SerializeField] private float tipOffset;
    
    public bool isDrawing = false;
    private float holdHeight;

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
                    StartDrawing();
                    GetComponent<Holdable>().holdHeight = 0.5f;
                }
                else
                {
                    StopDrawing();
                    GetComponent<Holdable>().holdHeight = 1;
                }
            }
        }
    }

    public void SecondaryInteract()
    {
        //Debug.Log($"{primaryAction} was pressed");

        if (GetComponent<IHoldable>().GetHeldStatus())
        {
            StopDrawing();
            GetComponent<Holdable>().holdHeight = 1;

            GetComponent<IHoldable>().Release();

            primaryAction = "";
            secondaryAction = "Pick Up";
        }
        else
        {
            GetComponent<IHoldable>().Grab();

            
            if (isStamp)
            {
                primaryAction = "Stamp";
            }
            else
            {
                primaryAction = "Draw (Hold)";
            }
            
            secondaryAction = "Put Down";

            holdHeight = transform.position.y;
        }
    }

    private void StartDrawing()
    {
        Ray ray = new Ray(tipOrigin.transform.position, tipOrigin.transform.forward);
        Debug.DrawRay(tipOrigin.transform.position, tipOrigin.transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 1))
        {
            Vector3 adjustedPos = new Vector3(transform.position.x, hit.point.y + tipOffset, transform.position.z);

            StartCoroutine(GetComponent<IHoldable>().LerpObjectTransform(adjustedPos, 0.1f, false));

            isDrawing = true;
        }
    }

    private void StopDrawing()
    {
        isDrawing = false;

        Vector3 adjustedPos = new Vector3(transform.position.x, holdHeight, transform.position.z);

        StartCoroutine(GetComponent<IHoldable>().LerpObjectTransform(adjustedPos, 0.1f, false));
    }

    private IEnumerator StampMotion()
    {
        stampInMotion = true;

        Vector3 startPosition = transform.position;
        Vector3 lowerPosition = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

        yield return StartCoroutine(LerpPosition(lowerPosition, 0.25f));

        yield return new WaitForSeconds(0.01f);

        yield return StartCoroutine(LerpPosition(startPosition, 0.25f));

        yield return new WaitForSeconds(0.01f);

        stampInMotion = false;
    }

    private IEnumerator LerpPosition(Vector3 targetPosition, float time)
    {
        GetComponent<IHoldable>().SetFollowStatus(false);

        float elapsedTime = 0;

        Vector3 startPosition = transform.position;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / time);

            yield return null;
        }

        transform.position = targetPosition;

        GetComponent<IHoldable>().SetFollowStatus(true);
    }
}
