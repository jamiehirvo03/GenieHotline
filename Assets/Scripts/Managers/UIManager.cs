using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    [SerializeField] private GameObject controlPanel;
    [SerializeField] private GameObject primaryPanel;
    [SerializeField] private TextMeshProUGUI primaryLabel;
    [SerializeField] private GameObject secondaryPanel;
    [SerializeField] private TextMeshProUGUI secondaryLabel;

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
        controlPanel.SetActive(false);
    }

    public static UIManager GetInstance()
    {
        return instance;
    }

    public void EnableControlPanel(bool status)
    {
        controlPanel.SetActive(status);
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
