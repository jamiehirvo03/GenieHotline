using UnityEditor.SpeedTree.Importer;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class InteractionManager : MonoBehaviour
{
    private Transform hoveredObj;

    private RaycastHit raycastHit;

    [SerializeField] private Shader hovered;
    private Shader storedShader;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());

        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out raycastHit))
        {
            hoveredObj = raycastHit.transform;

            if (hoveredObj.CompareTag("Selectable"))
            {
                Debug.Log($"{hoveredObj.name} is hovered");

                if (storedShader != hoveredObj.GetComponent<Renderer>().material.shader)
                {
                    //store hovered object's default shader temporarily
                    storedShader = hoveredObj.GetComponent<Renderer>().material.shader;
                }

                //change hovered object's shader to highlight
                hoveredObj.gameObject.GetComponent<Renderer>().material.shader = hovered;
            }
            else
            {
                Debug.Log("Hovered object is not selectable");

                if (hoveredObj != null)
                {
                    //when an unselectable object is hovered, reset last hovered object's shader
                    hoveredObj.gameObject.GetComponent<Renderer>().material.shader = storedShader;

                    hoveredObj = null;
                }
            }
        }
        else
        {
            hoveredObj = null;
        }
    }
}
