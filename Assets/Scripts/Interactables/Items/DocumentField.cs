using Unity.VisualScripting;
using UnityEngine;

public class DocumentField : MonoBehaviour
{
    private Document document;

    public enum FieldType
    {
        financialGain,
        itemManifestation,
        sensationManipulation,
        changeOfHeart,
        bodilyHarm,
        death,
        fateReversal,
        other,
    }

    public FieldType selectedType;

    private void OnEnable()
    {
        document = GetComponent<Document>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PencilTip"))
        {
            document.SetFieldValue(selectedType);
        }
    }
}
