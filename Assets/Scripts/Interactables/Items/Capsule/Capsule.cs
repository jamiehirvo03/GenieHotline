using UnityEngine;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class Capsule : MonoBehaviour, IInteractable
{
    private string primaryAction = "Pick Up";
    private string secondaryAction = "";

    [SerializeField] private List<GameObject> capsuleDocs = new List<GameObject>();
    private int docsInserted = 0;

    [SerializeField] private Transform pileLocation;
    [SerializeField] private Transform hiddenPileLocation;

    private CapsuleCollider capsuleCollider;
    [SerializeField] private GameObject documentTrigger;

    public bool isLidOn;

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
        // Put Down
        if (GetComponent<IHoldable>().GetHeldStatus())
        {
            GetComponent<IHoldable>().Release();

            primaryAction = "Pick Up";
        }
        // Pick Up
        else
        {
            GetComponent<IHoldable>().Grab();

            primaryAction = "Put Down";
        }
    }

    public void SecondaryInteract()
    {
        if (docsInserted > 0)
        {
            EmptyCapsule();
        }
    }

    private void Awake()
    {
        capsuleCollider = GetComponent<CapsuleCollider>();
    }

    private void Start()
    {
        for (int i = 0; i < capsuleDocs.Count; i++)
        {
            capsuleDocs[i].SetActive(false);
        }

        isLidOn = true;
        documentTrigger.SetActive(false);
    }

    public void ColliderSwitcher(bool enableTrigger)
    {
        if (!isLidOn)
        {
            if (enableTrigger)
            {
                capsuleCollider.enabled = false;
                documentTrigger.SetActive(true);
            }
            else
            {
                documentTrigger.SetActive(false);
                capsuleCollider.enabled = true;
            }
        } 
    }

   public void DocumentDetection(GameObject detectedDocument, bool isEntry)
   {
        if (isEntry)
        {
            Debug.Log($"{detectedDocument.name} entered the capsule trigger");
            // enable dummy gameObject to show where document would be stored if placed in capsule range
            capsuleDocs[docsInserted].SetActive(true);

            // temporarily hide actual document renderer so there isn't a double up (keep everyting but renderer enabled to maintain detection)
            detectedDocument.transform.Find("VisualElements")?.gameObject.SetActive(false);

            capsuleDocs[docsInserted].GetComponent<Outline>().enabled = true;

            detectedDocument.GetComponent<Document>().CapsuleDetection("Insert Document", true);
        }
        else
        {
            Debug.Log($"{detectedDocument.name} left the capsule trigger");

            // disable temporary dummy gameObject
            capsuleDocs[docsInserted].SetActive(false);

            // unhide actual document renderer
            detectedDocument.transform.Find("VisualElements")?.gameObject.SetActive(true);

            capsuleDocs[docsInserted].GetComponent<Outline>().enabled = false;

            detectedDocument.GetComponent<Document>().CapsuleDetection("Put Down", false);
        }
    }

    public void InsertDocument(GameObject heldDocument)
    {
        if (docsInserted == 0)
        {
            secondaryAction = "Empty Capsule";
        }

        heldDocument.GetComponent<IHoldable>().SetFollowStatus(false);

        // move actual document gameObject to designated area off-screen
        heldDocument.transform.position = hiddenPileLocation.position;
        heldDocument.transform.rotation = hiddenPileLocation.rotation;

        // - CAPSULE DUMMY OBJECT 

        // disable potential dummy outline once document is inserted
        capsuleDocs[docsInserted].GetComponent<Outline>().enabled = false;

        // enable dummy document gameObject
        capsuleDocs[docsInserted].SetActive(true);

        // store document gameObject reference in dummy document
        capsuleDocs[docsInserted].GetComponent<CapsuleDoc>().storedDocument = heldDocument;

        // - RESET VALUES FOR REAL DOCUMENT OBJECT

        // clear held status of hidden document gameObject
        heldDocument.GetComponent<IHoldable>().SetHeldStatus(false);

        // restore canFollow status for later use
        heldDocument.GetComponent<IHoldable>().SetFollowStatus(true);

        // unhide actual document renderer
        heldDocument.transform.Find("VisualElements")?.gameObject.SetActive(true);

        // reset action label for document
        heldDocument.GetComponent<Document>().primaryAction = "Pick Up";

        docsInserted++;
    }

    public void RemoveDocument(GameObject capsuleDocument)
    {
        Ray ray = new Ray(new Vector3(pileLocation.position.x, pileLocation.position.y + 6, pileLocation.position.z), Vector3.down);

        Vector3 placementPos = pileLocation.position;

        // first check if there is already object at placement position
        if (Physics.Raycast(ray, out RaycastHit hit, 6))
        {
            if (hit.collider.gameObject.CompareTag("Document"))
            {
                placementPos.y += 0.1f;
            }
        }

        // move actual document back to middle of desk
        capsuleDocument.GetComponent<CapsuleDoc>().storedDocument.transform.position = placementPos;

        // hide dummy document gameObject (also clears its stored document reference via onDisable())
        capsuleDocument.SetActive(false);

        docsInserted--;
    }

    public void EmptyCapsule()
    {
        // REMOVE ALL DOCUMENTS

        // create temp list of active dummy documents
        List<GameObject> activeDummyDocs = new List<GameObject>();

        // check how many dummy gameObjects are actually currently being used
        for (int i = 0; i < capsuleDocs.Count; i++)
        {
            if (capsuleDocs[i].GetComponent<CapsuleDoc>().storedDocument != null)
            {
                activeDummyDocs.Add(capsuleDocs[i]);
            }
        }

        // pile up documents using similar code to how their spawned at start of day
        for (int i = 0; i < activeDummyDocs.Count; i++)
        {
            // add a slight vertical offset based on the current loop index so that subsequent objects create a pile
            Vector3 spawnPosition = new Vector3(pileLocation.position.x, (pileLocation.position.y + (0.05f * i)), pileLocation.position.z);

            // move documents to pile location
            activeDummyDocs[i].GetComponent<CapsuleDoc>().storedDocument.transform.position = spawnPosition;

            // hide dummy document gameObject (also clears its stored document reference via onDisable())
            activeDummyDocs[i].SetActive(false);

        }

        docsInserted = 0;
    }
}
