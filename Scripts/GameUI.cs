using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameUI : MonoBehaviour
{
    public static GameUI instance;

    [SerializeField] private bool debugEnabled;

    [Header("POWER BAR")]
    [SerializeField] private GameObject powerBarBackground;
    [SerializeField] private Image powerBar;

    [Header("TOP LEFT")]
    [SerializeField] private TextMeshProUGUI lastValue;
    [SerializeField] private TextMeshProUGUI bestValue;

    [Header("TOP RIGHT")]
    [SerializeField] private TextMeshProUGUI debugText;

    [Header("ESCAPE MENU")]
    [SerializeField] private GameObject escapeMenuContainer; 

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        HidePowerBar();
        escapeMenuContainer.SetActive(false);
    }

    private void OnEnable()
    {
        HidePowerBar();
        escapeMenuContainer.SetActive(false);
    }

    private void Update()
    {
        UpdateDebug();

        if (Input.GetKeyDown(KeyCode.Escape)) {
            escapeMenuContainer.SetActive(!escapeMenuContainer.activeInHierarchy);
            Player.instance.SetEnabled(!escapeMenuContainer.activeInHierarchy);
            if (escapeMenuContainer.activeInHierarchy) UI.instance.ResetAllButtons();
        }
    }

    public void ShowPowerBar()
    {
        powerBarBackground.SetActive(true);
    }

    public void HidePowerBar()
    {
        powerBarBackground.SetActive(false);
    }

    public void UpdatePowerBar(float value)
    {
        powerBar.fillAmount = value;
    }

    public void SetLastValue(int value)
    {
        lastValue.text = value.ToString();
    }

    public void SetBestValue(int value)
    {
        bestValue.text = value.ToString();
    }

    private void UpdateDebug()
    {
        if (!debugEnabled) return;

        string text = "";
        GameObject[] stones = Player.instance.GetListOfAllStones();
        foreach (GameObject obj in stones)
        {
            text += $"STONE | POS: ({obj.transform.position}) | OWNER: {obj.GetComponent<Stone>().GetOwner().GetOwnerID()} | HELD: {obj.GetComponent<Stone>().GetIsBeingHeld()}\n";
        }
        debugText.text = text;
    }
}
