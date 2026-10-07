using UnityEditor;
using UnityEngine;

public class Pencil : MonoBehaviour, IInteractable
{
    private string primaryAction = "";
    private string secondaryAction = "Pick Up";

    private float holdHeight;
    [SerializeField] private GameObject tipOrigin;
    [SerializeField] private float tipOffset = 0.5f;
    private bool isDrawing = false;

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
        if (gameObject.GetComponent<IHoldable>().GetHeldStatus())
        {
            // add code for primary action when held (draw)
            isDrawing = !isDrawing;
        }
    }

    public void SecondaryInteract()
    {
        //Debug.Log($"{primaryAction} was pressed");

        if (gameObject.GetComponent<IHoldable>().GetHeldStatus())
        {
            gameObject.GetComponent<IHoldable>().Release();

            primaryAction = "";
            secondaryAction = "Pick Up";
        }
        else
        {
            gameObject.GetComponent<IHoldable>().Grab();

            primaryAction = "Draw (Hold)";
            secondaryAction = "Put Down";

            holdHeight = transform.position.y;
        }
    }

    private void Update()
    {
        if (isDrawing)
        {
            Ray ray = new Ray(tipOrigin.transform.position, tipOrigin.transform.forward);
            Debug.DrawRay(tipOrigin.transform.position, tipOrigin.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, 1))
            {
                if (hit.collider.gameObject)
                {
                    Vector3 adjustedPos = new Vector3(transform.position.x, hit.point.y + tipOffset, transform.position.z);

                    if (transform.position != adjustedPos)
                    {
                        transform.position = Vector3.Lerp(transform.position, adjustedPos, 0.5f);
                    }
                }
            }
        }
        if (gameObject.GetComponent<IHoldable>().GetHeldStatus() && !isDrawing)
        {
            if (transform.position.y != holdHeight)
            {
                transform.position = Vector3.Lerp(transform.position, new Vector3(transform.position.x, holdHeight, transform.position.z), 0.5f);
            }
        }
    }
}
