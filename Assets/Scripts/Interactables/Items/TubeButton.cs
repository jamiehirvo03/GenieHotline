using UnityEngine;
using System.Collections;
using System.Linq;

public class TubeButton : MonoBehaviour, IInteractable
{
    private string primaryAction = "Submit Capsule (End Day)";
    private string secondaryAction = "";

    private bool buttonInMotion = false;

    [SerializeField] private float pressAmount = 0.5f;

    [SerializeField] private GameObject pneumaticTube;

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
        if (!buttonInMotion)
        {
            Debug.Log($"Submission Button was pressed");

            StartCoroutine(KeyPressMovement());

            pneumaticTube.GetComponent<PneumaticTube>().SendCapsule();
        }
    }

    public void SecondaryInteract()
    {
        // no secondary interaction
    }

    private IEnumerator KeyPressMovement()
    {
        buttonInMotion = true;

        Vector3 startPosition = transform.position;
        Vector3 lowerPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z + pressAmount);

        yield return StartCoroutine(LerpPosition(lowerPosition, 0.1f));

        yield return new WaitForSeconds(0.05f);

        yield return StartCoroutine(LerpPosition(startPosition, 0.1f));

        yield return new WaitForSeconds(0.05f);

        buttonInMotion = false;
    }

    private IEnumerator LerpPosition(Vector3 targetPosition, float time)
    {
        float elapsedTime = 0;

        Vector3 startPosition = transform.position;

        while (elapsedTime < time)
        {
            elapsedTime += Time.deltaTime;

            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / time);

            yield return null;
        }

        transform.position = targetPosition;
    }
}
