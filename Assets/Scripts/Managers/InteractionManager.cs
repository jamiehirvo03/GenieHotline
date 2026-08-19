using UnityEngine;
using UnityEngine.EventSystems;

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
    private GameObject hoveredObj;
    private RaycastHit raycastHit;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);//Mouse.current.position.ReadValue());

        // if mouse is hovering over a gameObject (checking every frame)
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

                    Debug.Log($"{hoveredObj.name} is hovered");

                    //enable hovered objects highlight (first check that highlight component has been added in editor)
                    if (hoveredObj.GetComponent<Outline>() != null)
                    {
                        // enable new object outline
                        hoveredObj.GetComponent<Outline>().enabled = true;

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

       
        // if hovered object is not tagged 'selectable'
        else
        {
            Debug.Log("Hovered object is not selectable");
        }
    }
}
