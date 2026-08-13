using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using UnityEditor.ShaderGraph.Internal;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    private bool primaryInteractPressed = false;
    private bool secondaryInteractPressed = false;
    private Vector2 mousePos;
    private Vector2 scrollDelta;
    

    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one InputManager in the scene.");
        }

        instance = this;
    }

    public static InputManager GetInstance()
    {
        return instance;
    }

    public void PrimaryInteractPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            primaryInteractPressed = true;

            UIManager.GetInstance().ChangePanelColor("primary", Color.gray4);
        }
        else if (context.canceled)
        {
            primaryInteractPressed = false;

            UIManager.GetInstance().ChangePanelColor("primary", Color.gray5);
        }
    }

    public void SecondaryInteractPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            secondaryInteractPressed = true;

            UIManager.GetInstance().ChangePanelColor("secondary", Color.gray4);
        }
        else if (context.canceled)
        {
            secondaryInteractPressed = false;

            UIManager.GetInstance().ChangePanelColor("secondary", Color.gray5);
        }
    }

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector3>();
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        scrollDelta = context.ReadValue<Vector2>();
    }

    //GET FUNCTIONS

    public Vector3 GetMousePos()
    {
        return mousePos;
    }

    public Vector2 GetScrollDelta()
    {
        return scrollDelta;
    }

    // for the below 'get' methods, getting it means also using it.
    // set it to false so that it can't be used again until actually pressed again.
    
    public bool GetPrimaryInteractPressed()
    {
        bool result = primaryInteractPressed;
        primaryInteractPressed = false;
        return result;
    }

    public bool GetSecondaryInteractPressed()
    {
        bool result = secondaryInteractPressed;
        secondaryInteractPressed = false;
        return result;
    }
}
