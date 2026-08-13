using UnityEditor.SpeedTree.Importer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    private GameObject hoveredObj;
    private RaycastHit raycastHit;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        // if mouse is hovering over a gameObject (checking every frame)
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            if (raycastHit.transform.gameObject != null)
            {
                if (raycastHit.transform.gameObject.CompareTag("Selectable"))
                {
                    if (hoveredObj != null)
                    {
                        if (hoveredObj != raycastHit.transform.gameObject)
                        {
                            hoveredObj.GetComponent<Outline>().enabled = false;
                        }
                    }

                    // store object that is being hovered
                    hoveredObj = raycastHit.transform.gameObject;

                    Debug.Log($"{hoveredObj.name} is hovered");

                    //enable hovered objects highlight (first check that highlight component has been added in editor)
                    if (hoveredObj.GetComponent<Outline>() != null)
                    {
                        hoveredObj.GetComponent<Outline>().enabled = true;
                    }
                }
                else
                {
                    if (hoveredObj != null)
                    {
                        hoveredObj.GetComponent<Outline>().enabled = false;

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
