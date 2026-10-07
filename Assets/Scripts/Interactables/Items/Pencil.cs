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

    public void SecondaryInteract()
    {
        //Debug.Log($"{primaryAction} was pressed");

        if (GetComponent<IHoldable>().GetHeldStatus())
        {
            GetComponent<IHoldable>().Release();

            primaryAction = "";
            secondaryAction = "Pick Up";
        }
        else
        {
            GetComponent<IHoldable>().Grab();

            primaryAction = "Draw (Hold)";
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

            StartCoroutine(GetComponent<IHoldable>().LerpObjectTransform(adjustedPos, transform.eulerAngles, 0.1f, false));

            isDrawing = true;
        }
    }

    private void StopDrawing()
    {
        isDrawing = false;

        Vector3 adjustedPos = new Vector3(transform.position.x, holdHeight, transform.position.z);

        StartCoroutine(GetComponent<IHoldable>().LerpObjectTransform(adjustedPos, transform.eulerAngles, 0.1f, false));
    }
}
