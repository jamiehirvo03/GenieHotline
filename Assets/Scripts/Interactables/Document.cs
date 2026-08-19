using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Document : MonoBehaviour, IInteractable, IHoldable
{
    public string primaryAction;
    public string secondaryAction;
    
    private bool isHeld = false;
    [SerializeField] private float raiseAmount;

    private Vector3 hoverLocation;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private float maxFollowSpeed;
    [SerializeField] private float followTime;
    private Vector3 velocity = new Vector3(0, 0, 0);
 
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
        Debug.Log("Document primary pressed");
    }

    public void SecondaryInteract()
    {
        Debug.Log("Document secondary pressed");

        if (isHeld)
        {
            Release();
        }
        else
        {
            Grab();
        }
    }

    public void Grab()
    {
        isHeld = true;
        secondaryAction = "Put Down";

        //raise the object a certain amount above the desk
        transform.position = new Vector3(transform.position.x, transform.position.y + raiseAmount, transform.position.z);
    }

    public void Release()
    {
        //check if document is in bounds to be placed back down (and not colliding with anything)

        //lower the object the same amount that it was raised
        transform.position = new Vector3(transform.position.x, transform.position.y - raiseAmount, transform.position.z);

        isHeld = false;
        secondaryAction = "Pick Up";
    }

    public void FollowCursor()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //raycast to a position on the desk to find the location the held object should hover over
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out hit, layerMask))
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.green);
            Debug.DrawRay(hit.point, hit.normal * 2f, Color.red);
            
            hoverLocation = hit.point;

            Vector3 hoverAdjusted = new Vector3(hoverLocation.x, hoverLocation.y + raiseAmount, hoverLocation.z);

            transform.position = Vector3.SmoothDamp(transform.position, hoverAdjusted, ref velocity, maxFollowSpeed, followTime);

            Debug.Log(transform.position);
        }
    }

    void FixedUpdate()
    {
        if (isHeld)
        {
            FollowCursor();
        }
    }
}
