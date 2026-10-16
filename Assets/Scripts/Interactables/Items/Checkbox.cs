using Unity.VisualScripting;
using UnityEngine;

public class Checkbox : MonoBehaviour
{
    private Document document;

    public enum CheckboxType
    {
        // wish details                                             //wishes are broken down into common categories (all but maybe one or two will fit into the 7 main categories,
                                                                    //other will require the genie to do his own deliberation, this will affect the point balancing)
        material,
        knowledge,
        heart,
        harm,
        enhancement,
        fate,

        // consequences                                            //each consequence also has its own point value, these work as a subtraction from the points gained above
                                                                   //players should bare minimum be aiming for a balance of '0' but entering the negative allows for bonus pay or a quota
        inconvenience,
        dayRuiner,
        familyConflict,
        legalRepercussions,
        hexed,
        death,
    }

    public CheckboxType selectedType;

    private void OnEnable()
    {
        document = GetComponentInParent<Document>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("PencilTip"))
        {
            // set appropriate checkbox to true
            document.CheckboxStatus(selectedType, true);

            Debug.Log($"{selectedType} set to TRUE");
        }

        if (other.gameObject.CompareTag("EraserTip"))
        {
            // set appropriate checkbox to false
            document.CheckboxStatus(selectedType, false);

            // if any part of the checkbox is erased, clear the whole area


            Debug.Log($"{selectedType} set to FALSE");
        }
    }
}
