using UnityEditor.SpeedTree.Importer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private Transform highlight;
    private Transform selection;
    private RaycastHit raycastHit;

    [SerializeField] private Material highlightedMat;

    // Update is called once per frame
    void Update()
    {
        if (highlight != null)
        {
            highlight.GetComponent<MeshRenderer>().materials[1] = null;
            highlight = null;
        }

        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
        
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            highlight = raycastHit.transform;

            if (highlight.CompareTag("Selectable") && highlight != selection)
            {
                if (highlight.gameObject.GetComponent<MeshRenderer>().materials[1] != highlightedMat)
                {
                    Debug.Log($"{this.gameObject} was highlighted");
                    //outlineMat.SetBool("OutlineEnabled", true);
                }
                else
                {
                    highlight.gameObject.GetComponent<MeshRenderer>().materials[1] = highlightedMat;
                }
            }
            else
            {
                highlight = null;
            }
        }

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            if (highlight)
            {
                if (selection != null)
                {
                    selection.gameObject.GetComponent<MeshRenderer>().materials[1] = null;
                }
                selection = raycastHit.transform;
                selection.gameObject.GetComponent<MeshRenderer>().materials[1] = highlightedMat;
                highlight = null;
            }
            else
            {
                if (selection)
                {
                    selection.gameObject.GetComponent<MeshRenderer>().materials[1] = null;
                    selection = null;
                }
            }
        }
    }

    private void ToggleHoverHighlight()
    {

    }
}
