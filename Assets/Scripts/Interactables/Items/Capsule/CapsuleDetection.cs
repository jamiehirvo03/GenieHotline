using UnityEngine;

public class CapsuleDetection : MonoBehaviour
{
    private Capsule capsule;

    private void Awake()
    {
        capsule = GetComponentInParent<Capsule>();
    }

    public void OnTriggerEnter(Collider other)
    {
        // if document enters trigger range
        if (other.gameObject.CompareTag("Document"))
        {
            if (other.gameObject.GetComponent<IHoldable>() != null)
            {
                // check to make sure the document is being held when entering
                if (other.gameObject.GetComponent<IHoldable>().GetHeldStatus())
                {
                    capsule.DocumentDetection(other.gameObject, true);
                }
            }
            else
            {
                Debug.LogError($"cant access IHoldable of {other.gameObject.name}");
            }
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Document"))
        {
            if (other.GetComponent<IHoldable>().GetHeldStatus())
            {
                capsule.DocumentDetection(other.gameObject, false);
            }
        }
    }
}
