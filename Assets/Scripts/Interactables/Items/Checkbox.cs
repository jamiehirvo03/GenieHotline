using Unity.VisualScripting;
using UnityEngine;

public class Checkbox : MonoBehaviour
{
    private Document document;

    public enum CheckboxType
    {
        // wish details                                             //wishes are broken down into common categories (all but maybe one or two will fit into the 7 main categories,
                                                                    //other will require the genie to do his own deliberation, this will affect the point balancing)
        financialGain,
        itemManifestation,
        sensationManipulation,
        changeOfHeart,
        bodilyHarm,
        death,
        fateReversal,
        other,

        // consequences                                            //each consequence also has its own point value, these work as a subtraction from the points gained above
                                                                   //players should bare minimum be aiming for a balance of '0' but entering the negative allows for bonus pay or a quota
        minorInconvenience,
        dayRuiner,
        injury,
        familyDrama,
        legalScandal,
        badLuckCharm,
        hexed,
        walkingCalamity,
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
