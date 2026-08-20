using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhoneKey : MonoBehaviour, IInteractable
{
    private enum KeyID { One, Two, Three, Four, Five, Six, Seven, Eight, Nine, Zero, Star, Hash }

    [SerializeField] private KeyID thisKey;

    private string primaryAction = "Press";
    private string secondaryAction = "";

    private bool keyInMotion = false;
    [SerializeField] private float lowerAmount = 0.1f;

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
        if (!keyInMotion)
        {
            Debug.Log($"{thisKey} was pressed");

            StartCoroutine(KeyPressMovement());
        }
    }

    public void SecondaryInteract()
    {
        if (secondaryAction == "")
        {
            //Debug.Log("SecondaryInteract was pressed but there is no action assigned");
        }
        else
        {
            //Debug.Log($"{secondaryAction} pressed");
        }
    }

    private IEnumerator KeyPressMovement()
    {
        keyInMotion = true;
        
        Vector3 startPosition = transform.position;
        Vector3 lowerPosition = new Vector3(transform.position.x, transform.position.y - lowerAmount, transform.position.z);

        yield return StartCoroutine(LerpPosition(lowerPosition, 0.1f));

        yield return new WaitForSeconds(0.05f);

        yield return StartCoroutine(LerpPosition(startPosition, 0.1f));

        yield return new WaitForSeconds(0.05f);

        keyInMotion = false;
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

    void Start()
    {
        switch (thisKey)
        {
            case KeyID.One:

                break;

            case KeyID.Two:

                break;

            case KeyID.Three:

                break;

            case KeyID.Four:

                break;

            case KeyID.Five:

                break;

            case KeyID.Six:

                break;

            case KeyID.Seven:

                break;

            case KeyID.Eight:

                break;

            case KeyID.Nine:

                break;

            case KeyID.Zero:

                break;

            case KeyID.Star:

                break;

            case KeyID.Hash:

                break;
        }
    }
}
