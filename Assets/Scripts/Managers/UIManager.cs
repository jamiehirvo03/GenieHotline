using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    // general game UI
    [SerializeField] private GameObject gameHUD;

    [SerializeField] private GameObject debugPanel;
    [SerializeField] private GameObject primaryPanel;
    [SerializeField] private TextMeshProUGUI primaryLabel;
    [SerializeField] private GameObject secondaryPanel;
    [SerializeField] private TextMeshProUGUI secondaryLabel;

    // end of day report UI
    [SerializeField] private GameObject endOfDayReportPanel;

    [SerializeField] private TextMeshProUGUI dayReportHeader;
    [SerializeField] private TextMeshProUGUI wishesApproved;
    [SerializeField] private TextMeshProUGUI wishesDenied;
    [SerializeField] private TextMeshProUGUI mistakesMade;
    [SerializeField] private TextMeshProUGUI strikesCount;
    [SerializeField] private TextMeshProUGUI pointsEarnt;
    [SerializeField] private TextMeshProUGUI pointsBalance;
    

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
        debugPanel.SetActive(false);
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

    public void SetGameHUDStatus(bool status)
    {
        gameHUD.SetActive(status);
    }

    public void SetEndOfDayScreenStatus(bool status)
    {
        // if end-of-day screen enabled
        if (status)
        {
            // set new report variables

        }
        // if end-of-day screen disabled
        else
        {
            // clear old report variables

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
    }

}
