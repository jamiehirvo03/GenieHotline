using Unity.VisualScripting;
using UnityEngine;

public class Checkbox : MonoBehaviour
{
    private Document document;

    public enum CheckboxType
    {
        // wish details
        financialGain,
        itemManifestation,
        sensationManipulation,
        changeOfHeart,
        bodilyHarm,
        death,
        fateReversal,
        other,

        // consequences

    }

    public CheckboxType selectedType;

    private void OnEnable()
    {
        document = GetComponentInParent<Document>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("PencilTip"))
        {
            document.TickCheckbox(selectedType);
        }
    }
}
