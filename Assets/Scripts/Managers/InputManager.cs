using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    [SerializeField] private bool useHoldClickControls = false;

    // interaction inputs
    private Vector2 mousePos;
    private Vector2 scrollDelta;
    private bool primaryInteractPressed = false;
    private bool secondaryInteractPressed = false;
    
    // dialogue inputs
    private bool continuePressed = false;
    private bool choiceOnePressed = false;
    private bool choiceTwoPressed = false;
    private bool choiceThreePressed = false;
    private bool choiceFourPressed = false;

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

    public bool GetClickControlsStatus()
    {
        return useHoldClickControls;
    }

    public void OnMouseMove(InputAction.CallbackContext context)
    {
        mousePos = context.ReadValue<Vector2>();
    }

    public void OnScroll(InputAction.CallbackContext context)
    {
        scrollDelta = context.ReadValue<Vector2>();
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

    public void ContinuePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            continuePressed = true;
        }
        else if (context.canceled)
        {
            continuePressed = false;
        }
    }

    public void ChoiceOnePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            choiceOnePressed = true;
        }
        else if (context.canceled)
        {
            choiceOnePressed = false;
        }
    }

    public void ChoiceTwoPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            choiceTwoPressed = true;
        }
        else if (context.canceled)
        {
            choiceTwoPressed = false;
        }
    }

    public void ChoiceThreePressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            choiceThreePressed = true;
        }
        else if (context.canceled)
        {
            choiceThreePressed = false;
        }
    }

    public void ChoiceFourPressed(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            choiceFourPressed = true;
        }
        else if (context.canceled)
        {
            choiceFourPressed = false;
        }
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

    public bool GetContinuePressed()
    {
        bool result = continuePressed;
        continuePressed = false;
        return result;
    }

    public bool GetChoiceOnePressed()
    {
        bool result = choiceOnePressed;
        choiceOnePressed = false;
        return result;
    }

    public bool GetChoiceTwoPressed()
    {
        bool result = choiceTwoPressed;
        choiceTwoPressed = false;
        return result;
    }

    public bool GetChoiceThreePressed()
    {
        bool result = choiceThreePressed;
        choiceThreePressed = false;
        return result;
    }

    public bool GetChoiceFourPressed()
    {
        bool result = choiceFourPressed;
        choiceFourPressed = false;
        return result;
    }
}
