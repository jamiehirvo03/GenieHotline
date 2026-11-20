using Unity.VisualScripting;
using UnityEngine;

public class CapsuleDoc : MonoBehaviour, IInteractable
{
    private string primaryAction = "Remove Document (name)";
    private string secondaryAction = "";

    private Capsule capsule;
    public GameObject storedDocument;

    public string GetPrimaryAction()
    {
        return primaryAction;
    }

    public string GetSecondaryAction()
    {
        return secondaryAction;
    }

    private void Awake()
    {
        capsule = GetComponentInParent<Capsule>();
    }

    public void PrimaryInteract()
    {
        capsule.RemoveDocument(gameObject);
    }

    public void SecondaryInteract()
    {
        // no secondary action
    }

    public void AssignStoredDocument(GameObject assignedDocument)
    {
        storedDocument = assignedDocument;

        primaryAction = $"Remove Document";

        gameObject.name = assignedDocument.name;
    }

    public void ClearStoredDocument()
    {
        storedDocument = null;
    }
}
