using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UIElements;

public interface IInteractable
{
    public string GetPrimaryAction();
    public string GetSecondaryAction();
    public void PrimaryInteract();
    public void SecondaryInteract();
}

public interface IHoldable
{
    public bool GetHeldStatus();
    public void Grab();
    public void Release();
    public void ReturnHeldObject();
}

public class InteractionManager : MonoBehaviour
{
    private static InteractionManager instance;

    private GameObject hoveredObj;
    private RaycastHit raycastHit;

    private bool isItemHeld = false;
    [SerializeField] private GameObject heldItem;

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one InteractionManager in the scene.");
        }

        instance = this;
    }

    public static InteractionManager GetInstance()
    {
        return instance;
    }

    public void PickUpItem(GameObject item)
    {
        // keep track of held item
        heldItem = item;

        isItemHeld = true;
    }

    public void PutDownItem()
    {
        isItemHeld = false;

        // clear held item
        heldItem = null;
    }

    private void Update()
    {
        if (!isItemHeld)
        {
            if (heldItem == null)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//Mouse.current.position.ReadValue());

                // if mouse raycast hitting any object without being obstructed by UI objects
                if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
                {
                    // if raycast hits any object
                    if (raycastHit.transform.gameObject != null)
                    {
                        // if hit object 'interactable'
                        if (raycastHit.transform.gameObject.layer == LayerMask.NameToLayer("Interactable")) //raycastHit.transform.gameObject.CompareTag("Interactable"))
                        {
                            // if there is a stored object
                            if (hoveredObj != null)
                            {
                                // if stored object isn't the newly hit one
                                if (hoveredObj != raycastHit.transform.gameObject)
                                {
                                    //  disable stored object outline
                                    hoveredObj.GetComponent<Outline>().enabled = false;
                                }
                            }

                            // store object that is being hovered
                            hoveredObj = raycastHit.transform.gameObject;

                            //Debug.Log($"{hoveredObj.name} is hovered");

                            //enable hovered objects highlight (first check that highlight component has been added in editor)
                            if (hoveredObj.GetComponent<Outline>() != null)
                            {
                                // enable new object outline
                                hoveredObj.GetComponent<Outline>().enabled = true;

                                Debug.Log("Outline enabled");

                                // show control labels
                                UIManager.GetInstance().EnableControlPanel(true);

                                // change control labels
                                UIManager.GetInstance().ChangeLabelText(hoveredObj);

                                if (Input.GetMouseButtonDown(0))//InputManager.GetInstance().GetPrimaryInteractPressed())
                                {
                                    hoveredObj.GetComponent<IInteractable>().PrimaryInteract();
                                }

                                if (Input.GetMouseButtonUp(1))//InputManager.GetInstance().GetSecondaryInteractPressed())
                                {
                                    hoveredObj.GetComponent<IInteractable>().SecondaryInteract();
                                }
                            }
                        }
                        // if hit object not 'interactable'
                        else
                        {
                            Debug.Log("Hovered object is not interactable");

                            // if previously hovered object is still stored
                            if (hoveredObj != null)
                            {
                                // disable stored object outline
                                hoveredObj.GetComponent<Outline>().enabled = false;

                                //clear control labels
                                UIManager.GetInstance().EnableControlPanel(false);

                                // clear stored object
                                hoveredObj = null;
                            }
                        }
                    }
                }
            }  
        }
        // if an item is already held
        else
        {
            if (heldItem != null)
            {
                Debug.Log("Regardless of mouse hover, current held object is ready for inputs");

                if (Input.GetMouseButtonDown(0))//InputManager.GetInstance().GetPrimaryInteractPressed())
                {
                    heldItem.GetComponent<IInteractable>().PrimaryInteract();
                }

                if (Input.GetMouseButtonUp(1))//InputManager.GetInstance().GetSecondaryInteractPressed())
                {
                    heldItem.GetComponent<IInteractable>().SecondaryInteract();
                }

                if (UIManager.GetInstance().GetPrimaryLabel() != heldItem.GetComponent<IInteractable>().GetPrimaryAction() | UIManager.GetInstance().GetSecondaryLabel() != heldItem.GetComponent<IInteractable>().GetSecondaryAction())
                {
                    UIManager.GetInstance().ChangeLabelText(heldItem);
                }
            } 
        }
    }
}
