using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;

public class PneumaticTube : MonoBehaviour
{
    private GameObject capsuleInRange;
    [SerializeField] private GameObject insertedCapsule;
    [SerializeField] private Transform capsuleInsertedTransform;
    [SerializeField] private Transform capsuleDestinationTransform;

    [SerializeField] private GameObject dummyCapsule;
    [SerializeField] private List<GameObject> dummyCapsuleDocs = new List<GameObject>();

    [SerializeField] private GameObject tubeButton;

    //[SerializeField] private Transform hiddenArea;

    private void Start()
    {
        dummyCapsule.SetActive(false);

        for (int i = 0; i < dummyCapsuleDocs.Count; i++)
        {
            dummyCapsuleDocs[i].SetActive(false);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Capsule"))
        {
            if (other.gameObject.GetComponent<IHoldable>().GetHeldStatus() && other.gameObject.GetComponent<Capsule>().isLidOn)
            {
                InCapsuleRange(true, other.gameObject);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Capsule"))
        {
            if (other.gameObject.GetComponent<IHoldable>().GetHeldStatus())
            {
                InCapsuleRange(false, other.gameObject);
            }
        }
    }

    public void InCapsuleRange(bool status, GameObject capsule)
    {
        if (status)
        {
            capsuleInRange = capsule;

            // hide real capsule visuals
            capsuleInRange.GetComponent<Holdable>().meshObject.SetActive(false);

            // show dummy capsule with correct amount of dummy docs inside
            ShowDummyObjects(true);

            capsuleInRange.GetComponent<Capsule>().InTubeRange(true);
        }
        else
        {
            // hide dummy capsule and all dummy docs inside
            ShowDummyObjects(false);

            // show real capsule visuals
            capsule.GetComponent<Holdable>().meshObject.SetActive(true);

            capsule.GetComponent<Capsule>().InTubeRange(false);

            capsuleInRange = null;
        }
    }

    public void InsertCapsule()
    {
        // hide dummy capsule as its no longer needed
        ShowDummyObjects(false);

        // unhide real capsule visuals
        capsuleInRange.GetComponent<Holdable>().meshObject.SetActive(true);

        // disable interacton for capsule lid while in tube
        //capsuleInRange.GetComponent<Capsule>().capsuleLid.GetComponent<BoxCollider>().enabled = false;

        // move real capsule to tube spot once confirmed
        capsuleInRange.transform.position = capsuleInsertedTransform.position;

        // move objectInRange to insertedCapsule and clear the former
        insertedCapsule = capsuleInRange;
        capsuleInRange = null;

        // indicate that capsule can be sent by making the send button glow
        tubeButton.GetComponent<MeshRenderer>().material.EnableKeyword("_EMISSION");
    }

    public void RemoveCapsule()
    {
        // clear inserted capsule
        insertedCapsule = null;

        // turn off button glow once capsule is no longer in position to be sent
        tubeButton.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");
    }

    public void SendCapsule()
    {
        if (insertedCapsule != null)
        {
            // turn off button glow as capsule has already been sent
            tubeButton.GetComponent<MeshRenderer>().material.DisableKeyword("_EMISSION");

            StartCoroutine(LerpObjectTransform(capsuleDestinationTransform.position, capsuleDestinationTransform.eulerAngles, 0.3f));
        }
        else
        {
            Debug.Log("Send Failed! No Capsule currently in the tube.");
        }
    }

    private void ShowDummyObjects(bool status)
    {
        if (status)
        {
            dummyCapsule.SetActive(true);

            for (int i = 0; i < capsuleInRange.GetComponent<Capsule>().docsInserted; i++)
            {
                dummyCapsuleDocs[i].SetActive(true);
            }

            dummyCapsule.GetComponent<Outline>().enabled = true;
        }
        else
        {
            for (int i = 0; i < dummyCapsuleDocs.Count; i++)
            {
                dummyCapsuleDocs[i].SetActive(false);
            }

            dummyCapsule.GetComponent<Outline>().enabled = false;

            dummyCapsule.SetActive(false);
        }
    }

    private IEnumerator LerpObjectTransform(Vector3 targetPosition, Vector3 targetRotation, float transformTime)
    {
        Debug.Log($"Lerping object to position = {targetPosition}");

        Vector3 startPosition = transform.position;
        Quaternion startRotation = transform.rotation;
        Quaternion targetQuaternion = Quaternion.Euler(targetRotation);

        float elapsedTime = 0f;

        while (elapsedTime < transformTime)
        {
            elapsedTime += Time.deltaTime;
            float lerpTime = elapsedTime / transformTime;

            transform.position = Vector3.Lerp(startPosition, targetPosition, lerpTime);

            transform.rotation = Quaternion.Lerp(startRotation, targetQuaternion, lerpTime);

            yield return null;
        }
        // everything below this will happen once lerp is complete

        insertedCapsule.transform.localPosition = targetPosition; // snap position to target to fix weird numbers

        GameManager.GetInstance().SubmitDocuments();
    }
}
