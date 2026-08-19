using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Transactions;
using UnityEngine;
using UnityEngine.EventSystems;

public class Holdable : MonoBehaviour, IHoldable
{
    [SerializeField] private bool useHoldClickControls = false; // will need to set this up later as project wide setting (in input manager)

    // HELD OBJECT MOVEMENT
    private bool isHeld = false;

    private Vector3 grabOriginPos;
    private Vector3 hoverPos;
    private Vector3 placementPos;
    private float timeToMove = 0.1f; // FOR VERTICAL HEIGHT LERP ONLY
    [SerializeField] private float holdHeight; // height above the surface you're hovering over

    [SerializeField] private LayerMask objectFollowingLayers; // layers to be detected for moving the held object
    [SerializeField] private float followTime;
    private Vector3 velocity = new Vector3(0, 0, 0);

    // PLACEMENT VALIDITY CHECKING
    [SerializeField] private List<Transform> objectBoundaries = new List<Transform>();
    [SerializeField] private LayerMask placementValidityMask;
    private GameObject lowestValidObject;
    private GameObject highestDocument;

    // INVALID PLACEMENT FLASHING
    private Renderer objectRenderer;
    public GameObject meshObject;
    private Color originalColor;
    [SerializeField] private Color flashColor = Color.red;
    [SerializeField] private float flashDuration = 0.1f;
    private bool isFlashingDone = true;

    public bool GetHeldStatus()
    {
        return isHeld;
    }

    private void Awake()
    {
        objectRenderer = meshObject.GetComponent<MeshRenderer>();
    }

    private void Start()
    {
        originalColor = objectRenderer.material.color;
    }

    private void Update()
    {
        if (isHeld)
        {
            DragObject();
        }
    }

    public void Grab()
    {
        if (isFlashingDone)
        {            
            isHeld = true;

            // store position of the object before the grab incase the player tries to place it somewhere they can't (only used for holdclick controls)
            grabOriginPos = transform.position;

            // raise object to hold height
            LerpObjectHeight(new Vector3(transform.position.x, holdHeight, transform.position.z));
        } 
    }

    private IEnumerator LerpObjectHeight(Vector3 target)
    {
        Debug.Log($"Lerping object height to y = {target.y}");

        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < timeToMove)
        {
            elapsedTime += Time.deltaTime;

            float lerpTime = elapsedTime / timeToMove;

            transform.position = Vector3.Lerp(startPosition, target, lerpTime);

            yield return null;
        }
        // everything below this will happen once lerp is complete

        transform.position = target; // snap position to target to fix weird numbers
        
        // clear current placementPos as it is no longer needed
        placementPos = Vector3.zero;
    }

    public void Release()
    {
        if (isFlashingDone)
        {
            CheckPlacementValidity();

            if (placementPos != Vector3.zero)
            {
                Debug.Log($"PlacementPos is {placementPos}");

                StartCoroutine(LerpObjectHeight(placementPos));

                isHeld = false;
            }
        }
        else
        {
            Debug.Log("Blinking not done, try again shortly");
        }
    }

    private void CheckPlacementValidity()
    {
        if (objectBoundaries.Count > 0)
        {
            int validPoints = 0; // number of boundary points that are above a valid location
            int documentPoints = 0; // number of boundary points that are above a document

            int highestHitHeight = 0;



            foreach (Transform currentBoundary in objectBoundaries)
            {
                // raycast down towards the desk from each boundary point
                if (Physics.Raycast(currentBoundary.position, Vector3.down, out RaycastHit hit, Mathf.Infinity, placementValidityMask))
                {
                    Debug.DrawLine(currentBoundary.position, hit.point, Color.green, 2);

                    // if a ray hits the desk
                    if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Desk"))
                    {
                        validPoints++;

                        if (lowestValidObject != null)
                        {
                            if (hit.collider.transform.position.y < lowestValidObject.transform.position.y)
                            {
                                lowestValidObject = hit.collider.gameObject;
                            }
                        }
                        else
                        {
                            lowestValidObject = hit.collider.gameObject;
                        }
                        
                    }
                    else if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Interactable"))
                    {
                        // if a ray hits a document
                        if (hit.collider.gameObject.CompareTag("Document"))
                        {
                            validPoints++;
                            documentPoints++;

                            if (lowestValidObject != null)
                            {
                                if (hit.collider.transform.position.y < lowestValidObject.transform.position.y)
                                {
                                    lowestValidObject = hit.collider.gameObject;
                                }
                            }
                            else
                            {
                                lowestValidObject = hit.collider.gameObject;
                            }

                            // if this hit document is on a higher y level than the current highest, overwrite it
                            if (highestDocument != null)
                            {
                                if (hit.collider.transform.position.y > highestDocument.transform.position.y)
                                {
                                    highestDocument = hit.collider.gameObject;
                                }
                            }
                            else
                            {
                                highestDocument = hit.collider.gameObject;
                            }                            
                        }

                        if (hit.collider.gameObject.CompareTag("Stationery"))
                        {
                            validPoints++;

                            if (lowestValidObject != null)
                            {
                                if (hit.collider.transform.position.y < lowestValidObject.transform.position.y)
                                {
                                    lowestValidObject = hit.collider.gameObject;
                                }
                            }
                            else
                            {
                                lowestValidObject = hit.collider.gameObject;
                            }  
                        }
                    }
                }
            }

            // if all of this objects boundary points are above a valid spot
            if (validPoints == objectBoundaries.Count)
            {
                if (lowestValidObject != null)
                {
                    Debug.Log($"Lowest Valid Point: {lowestValidObject.transform.position.y}");
                }

                if (highestDocument != null)
                {
                    Debug.Log($"Highest Document Level: {highestDocument.transform.position.y}");
                }
                
                Debug.Log("All points are valid, object being placed");

                placementPos = new Vector3(this.transform.position.x, 5, this.transform.position.z);

                Debug.Log($"PlacementPos: {placementPos}");

                // if atleast one boundary point's ray hit a document
                if (documentPoints > 0)
                {
                    // raise the y level of this placement
                    placementPos = new Vector3(this.transform.position.x, highestHitHeight, this.transform.position.z);
                }
            }
            else
            {
                Debug.Log($"{this.gameObject.name} only has {validPoints}/{objectBoundaries.Count} valid boundary checks, it cannot be placed here");

                if (useHoldClickControls)
                {
                    ReturnHeldObject();
                }
                else
                {
                    Flash();
                }
            }
        }
        else
        {
            Debug.Log($"{this.gameObject.name} has no assigned boundaries");
        }
    }

    public void ReturnHeldObject() // KEEP PUBLIC SO IT CAN BE CALLED FROM 'OUT OF BOUNDS' COLLIDERS
    {
        isHeld = false;

        gameObject.transform.position = grabOriginPos;

        Flash();
    }


    private void Flash()
    {
        StopAllCoroutines();
        StartCoroutine(DoFlash());
    }

    private IEnumerator DoFlash()
    {
        objectRenderer.material.color = flashColor;
        
        yield return new WaitForSeconds(flashDuration);

        objectRenderer.material.color = originalColor;
    }

    private void DragObject()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        //raycast to a position on the desk to find the location the held object should hover over
        if (!EventSystem.current.IsPointerOverGameObject() && Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, objectFollowingLayers))
        {
            Debug.DrawLine(Camera.main.transform.position, hit.point, Color.green);
            Debug.DrawRay(hit.point, hit.normal * 2f, Color.red);

            hoverPos = hit.point;

            Vector3 hoverAdjusted = new Vector3(hoverPos.x, hoverPos.y + holdHeight, hoverPos.z);

            transform.position = Vector3.SmoothDamp(transform.position, hoverAdjusted, ref velocity, followTime);
        }
    }
}
