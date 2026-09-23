using System.Collections.Generic;
using UnityEngine;
using Ink.Parsed;


public class GameManager : MonoBehaviour
{
    private int dayNumber;

    [SerializeField] private List<TextAsset> dayOneCallers = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dayTwoCallers = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dayThreeCallers = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dayFourCallers = new List<TextAsset>();
    [SerializeField] private List<TextAsset> dayFiveCallers = new List<TextAsset>();

    private List<TextAsset> currentCallerQueue;
    private int queuePosition;
    private TextAsset currentCaller; // ink JSON file

    private void SelectDay()
    {
        switch (dayNumber)
        {
            case 1:
                currentCallerQueue = dayOneCallers;

                break;
            case 2:
                currentCallerQueue = dayTwoCallers;

                break;
            case 3:
                currentCallerQueue = dayThreeCallers;

                break;
            case 4:
                currentCallerQueue = dayFourCallers;

                break;
            case 5:
                currentCallerQueue = dayFiveCallers;

                break;
        }
    }

    private void NextCaller()
    {

    }
}
