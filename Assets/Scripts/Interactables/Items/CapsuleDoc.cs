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

    private void OnDisable()
    {
        // when dummy object is disabled clear its stored document
        storedDocument = null;
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

        primaryAction = $"Remove {storedDocument.name}";
    }
}
