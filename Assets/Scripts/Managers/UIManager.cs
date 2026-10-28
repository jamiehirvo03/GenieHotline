using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [Header("General Game UI")]
    [SerializeField] private GameObject gameplayUI;
    [SerializeField] private GameObject barHUD;

    [SerializeField] private GameObject debugMenuToggle;
    [SerializeField] private GameObject debugPanel;

    [SerializeField] private GameObject primaryPanel;
    [SerializeField] private TextMeshProUGUI primaryLabel;
    [SerializeField] private GameObject secondaryPanel;
    [SerializeField] private TextMeshProUGUI secondaryLabel;

    [SerializeField] private TextMeshProUGUI currentItem;
 
    [Header("End-Of-Day Report UI")]
    [SerializeField] private GameObject endOfDayReportPanel;

    [SerializeField] private TextMeshProUGUI dayReportHeader;
    [SerializeField] private TextMeshProUGUI wishesApproved;
    [SerializeField] private TextMeshProUGUI wishesDenied;
    [SerializeField] private TextMeshProUGUI mistakesMade;
    [SerializeField] private Image strikesCounter;
    [SerializeField] private TextMeshProUGUI geneBucksEarned;
    [SerializeField] private TextMeshProUGUI geneBucksBalance;

    [SerializeField] private Image quotaBox;
    [SerializeField] private Sprite quotaMetTick;
    [SerializeField] private Sprite quotaNoTick;

    [SerializeField] private Sprite zeroStrikes;
    [SerializeField] private Sprite oneStrike;
    [SerializeField] private Sprite twoStrikes;
    [SerializeField] private Sprite threeStrikes;


    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogError("Found more than one UIManager in the scene.");
        }

        instance = this;
    }

    private void Start()
    {
        debugMenuToggle.SetActive(false);
        debugPanel.SetActive(false);
        endOfDayReportPanel.SetActive(false);
        gameplayUI.SetActive(true);

        EnableControlLabels(false);

        if (GameManager.GetInstance().debugMode)
        {
            debugMenuToggle.SetActive(true);
        }
    }

    public static UIManager GetInstance()
    {
        return instance;
    }

    public string GetPrimaryLabel()
    {
        return primaryLabel.text;
    }

    public string GetSecondaryLabel()
    {
        return secondaryLabel.text;
    }

    public void SetGameHUD(bool status)
    {
        barHUD.SetActive(status);
    }

    public void SetEndOfDayScreen(bool status, int dayNumber, int approved, int denied, bool quotaMet, int mistakes, int strikes, int newBucks, int totalBucks)
    {
        // if end-of-day screen enabled
        if (status)
        {
            // enable end-of-day report screen
            endOfDayReportPanel.SetActive(true);

            // disable gameplay UI
            gameplayUI.SetActive(false);
            // disable dialogue UI (dialogue UI is kept in DialogueManager)
            DialogueManager.GetInstance().ExitDialogueMode();

            // set new report variables
            dayReportHeader.text = $"Day {dayNumber} Complete!";
            wishesApproved.text = $"{approved}";
            wishesDenied.text = $"{denied}";
            mistakesMade.text = $"{mistakes}";

            if (strikes < 0)
            {
                Debug.LogError("Strikes count is in the negative!");
            }
            if (strikes == 0)
            {
                strikesCounter.sprite = zeroStrikes;
            }
            if (strikes == 1)
            {
                strikesCounter.sprite = oneStrike;
            }
            if (strikes == 2)
            {
                strikesCounter.sprite = twoStrikes;
            }
            if (strikes == 3)
            {
                strikesCounter.sprite = threeStrikes;
            }
            if (quotaMet)
            {
                quotaBox.sprite = quotaMetTick;
            }
            else
            {
                quotaBox.sprite = quotaNoTick;
            }

            geneBucksEarned.text = $"{newBucks}";
            geneBucksBalance.text = $"{totalBucks}";
        }
        // if end-of-day screen disabled
        else
        {
            // disable end-of-day report screen
            endOfDayReportPanel.SetActive(false);
        }
    }

    public void EnableDebugPanel(bool status)
    {
        debugPanel.SetActive(status);
    }

    public void EnableControlLabels(bool status)
    {
        primaryPanel.SetActive(status);
        secondaryPanel.SetActive(status);

        currentItem.gameObject.SetActive(status);
    }

    public void ChangePanelColor(string panel, Color panelColor)
    {
        if (panel == "primary")
        {
            primaryPanel.GetComponent<Image>().color = panelColor;
        }
        else if (panel == "secondary")
        {
            secondaryPanel.GetComponent<Image>().color = panelColor;
        }
    }

    public void ChangeLabelText(GameObject interactable)
    {
        primaryLabel.text = interactable.GetComponent<IInteractable>().GetPrimaryAction();

        secondaryLabel.text = interactable.GetComponent<IInteractable>().GetSecondaryAction();

        currentItem.text = interactable.gameObject.name;
    }

    //public void ToggleDebugWindow()
    //{
    //    DebugWindow.gameObject.SetActive(!DebugWindow.gameObject.activeSelf);
    //}

    public void SelectDay(int dayNumber)
    {
        //EventManager.GetInstance().StartDay();

        GameManager.GetInstance().StartDay(dayNumber);
    }

    public void BackToMenu()
    {
        SceneLoader.ChangeScene(SceneLoader.Scene.MainMenu);
    }
}
