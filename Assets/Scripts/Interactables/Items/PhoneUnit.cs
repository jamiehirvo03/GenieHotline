using UnityEngine;

public class PhoneUnit : MonoBehaviour, IInteractable
{
    private string primaryAction = "Answer Phone";
    private string secondaryAction = "";

    private bool isActive = false;

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
        if (!isActive)
        {
            // move phone unit gameobject to active position

            GameManager.GetInstance().AnswerPhone();
            primaryAction = "Hang Up Phone";
            isActive = true;
        }
        else
        {
            // move phone unit gameObject to rest position

            GameManager.GetInstance().HangUpPhone();
            primaryAction = "Answer Phone";
            isActive = false;
        }
    }

    public void SecondaryInteract()
    {
        Debug.Log($"{gameObject} has no Secondary Interaction");
    }
}
