using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
//using UnityEngine.InputSystem;
using Ink.Runtime;
using Ink.Parsed;
using Story = Ink.Runtime.Story;
using Choice = Ink.Runtime.Choice;



public class DialogueManager : MonoBehaviour
{
    [Header("Params")]
    [SerializeField] private float typingSpeed = 0.02f;

    [Header("Dialogue UI")]
    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private GameObject continueIcon;

    [Header("Choices UI")]
    [SerializeField] private GameObject[] choices;
    private TextMeshProUGUI[] choicesText;

    public Story currentStory;
    public bool isDialoguePlaying { get; private set; }

    private Coroutine displayLineCoroutine;

    private bool canContinueToNextLine = false;
    public GameObject dialogueCharacter { get; private set; }

    // Constants for inky tags (character_emotion)
    private const string PORTRAIT_TAG = "portrait";
    [SerializeField] private Animator portraitAnimator;

    private static DialogueManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("Found more than one Dialogue Manager in the scene");
        }

        instance = this;
    }

    public static DialogueManager GetInstance()
    {
        return instance;
    }

    private void Start()
    {
        //EventManager.current.onExitDialogueMode += ExitDialogueMode;

        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);

        choicesText = new TextMeshProUGUI[choices.Length];
        int index = 0;
        foreach (GameObject choice in choices)
        {
            choicesText[index] = choice.GetComponentInChildren<TextMeshProUGUI>();
            index++;
        }
    }

    private void Update()
    {
        if (!isDialoguePlaying)
        {
            return;
        }

        //if (InputManager.GetInstance().GetSubmitPressed() && canContinueToNextLine)
        //{
        //    ContinueStory();
        //}
    }

    public void EnterDialogueMode(CallerQueue callerQueue, string knotName)
    {
        isDialoguePlaying = true;
        dialoguePanel.SetActive(true);
        
        // Select the characters JSON file
        //currentStory = new Story(callerQueue.inkJSON.text);

        // Set the dialogue character
        //dialogueCharacter = callerQueue.gameObject;

        //EventManager.current.CallerChosen();

        // Choose the appropriate knot
        if (!knotName.Equals(""))
        {
            currentStory.ChoosePathString(knotName);
        }
        else
        {
            Debug.LogWarning("Knot name was the empty string when entering dialogue");
        }

            ContinueStory();
    }

    public void ExitDialogueMode()
    {
        isDialoguePlaying = false;
        dialoguePanel.SetActive(false);
        dialogueText.text = "";
    }

    private IEnumerator WaitToExit()
    {
        yield return new WaitForSeconds(0.2f);

        ExitDialogueMode();
    }

    private void ContinueStory()
    {
        if (currentStory.canContinue)
        {
            if (displayLineCoroutine != null)
            {
                StopCoroutine(displayLineCoroutine);
            }
            displayLineCoroutine = StartCoroutine(DisplayLine(currentStory.Continue()));

            HandleTags(currentStory.currentTags);
        }
        else
        {
            StartCoroutine(WaitToExit());
        }
    }

    private void HandleTags(List<string> currentTags)
    {
        // Loop through each tag and handle it accordingly
        foreach (string tag in currentTags)
        {
            // Parse the tag
            string[] splitTag = tag.Split(':');
            if (splitTag.Length != 2)
            {
                Debug.LogError("Tag could not be parsed: " + tag);
            }
            string tagKey = splitTag[0].Trim();
            string tagValue = splitTag[1].Trim();

            if (tagKey == PORTRAIT_TAG)
            {
                portraitAnimator.Play(tagValue);
            }
        }
    }

    private IEnumerator DisplayLine(string line)
    {
        // Clear dialogue text so previous line is no longer showing
        dialogueText.text = "";
        // Hide items while text is typing
        continueIcon.SetActive(false);
        HideChoices();

        canContinueToNextLine = false;

        // Display each line one letter at a time
        foreach (char letter in line.ToCharArray())
        {
            // If the submit button is pressed, finish up displaying the line right away
            //if (InputManager.GetInstance().GetSubmitPressed())
            //{
            //    dialogueText.text = line;
            //    break;
            //}

            dialogueText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
        // Actions to take after the entire line of dialogue has finished typing
        DisplayChoices();
        
        // Continue icon will be enabled from displaychoices() if no choices are to be shown

        canContinueToNextLine = true;
    }

    private void HideChoices()
    {
        foreach (GameObject choiceButton in choices)
        {
            choiceButton.SetActive(false);
        }
    }

    private void DisplayChoices()
    {
        List<Choice> currentChoices = currentStory.currentChoices;

        if (currentChoices.Count > choices.Length)
        {
            Debug.LogError("More choices were given than the UI can support.");
        }

        int index = 0;

        foreach(Choice choice in currentChoices)
        {
            choices[index].gameObject.SetActive(true);
            choicesText[index].text = choice.text;
            index++;
        }

        for (int i = index; i < choices.Length; i++)
        {
            choices[i].gameObject.SetActive(false);
        }

        if (currentChoices.Count == 0)
        {
            continueIcon.SetActive(true);
        }

        StartCoroutine(SelectFirstChoice());
    }

    private IEnumerator SelectFirstChoice()
    {
        // Unity's event system requires the selection being cleared before the first choice can be selected by default
        EventSystem.current.SetSelectedGameObject(null);
        yield return new WaitForEndOfFrame();
        EventSystem.current.SetSelectedGameObject(choices[0].gameObject);
    }

    public void MakeChoice(int choiceIndex)
    {
        if (canContinueToNextLine)
        {
            currentStory.ChooseChoiceIndex(choiceIndex);
            //InputManager.GetInstance().RegisterSubmitPressed();
            ContinueStory();
        }
    } 
}
