using UnityEngine;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI instance;

    [Header("TABS")]
    [SerializeField] private GameObject mainTab;
    [SerializeField] private GameObject hostTab;
    [SerializeField] private GameObject joinTab;
    [SerializeField] private GameObject offlineTab;
    [SerializeField] private GameObject settingsTab;
    [SerializeField] private GameObject quitTab;
    [SerializeField] private GameObject connectingTab;

    [Header("SOUNDS")]
    [SerializeField] private AudioSource clickSound;

    [Header("INPUT FIELDS")]
    [SerializeField] private TMP_InputField hostingNameInput;
    [SerializeField] private TMP_InputField joiningNameInput;
    [SerializeField] private TMP_InputField joinCodeInput;

    [Header("CONNECTING TAB")]
    [SerializeField] private TextMeshProUGUI connectingText;
    [SerializeField] private GameObject connectingBackButton;

    public enum Tab { main, host, join, offline, settings, quit, connecting}

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        OpenTab(Tab.main);
    }

    private void Update()
    {
        //PRESS ESCAPE TO RETURN TO MAIN MENU
        if (Input.GetKeyDown(KeyCode.Escape) && !mainTab.activeInHierarchy && !connectingTab.activeInHierarchy)
        {
            clickSound.Play();
            UI.instance.ResetAllButtons();
            OpenTab(Tab.main);
        }
    }

    public void OpenTab(Tab tab)
    {
        mainTab.SetActive(false);
        hostTab.SetActive(false);
        joinTab.SetActive(false);
        offlineTab.SetActive(false);
        settingsTab.SetActive(false);
        quitTab.SetActive(false);
        connectingTab.SetActive(false);

        switch (tab)
        {
            case Tab.main:
                mainTab.SetActive(true);
                break;
            case Tab.host:
                hostTab.SetActive(true);
                hostingNameInput.text = PlayerPrefs.GetString("DisplayName", "");
                break;
            case Tab.join:
                joinTab.SetActive(true);
                joiningNameInput.text = PlayerPrefs.GetString("DisplayName", "");
                break;
            case Tab.offline:
                offlineTab.SetActive(true);
                break;
            case Tab.settings:
                settingsTab.SetActive(true);
                break;
            case Tab.quit:
                quitTab.SetActive(true);
                break;
            case Tab.connecting:
                connectingTab.SetActive(true);
                break;
        }
    }

    public void OnDisable()
    {
        UI.instance.ResetAllButtons();
    }

    public void UpdateDisplayName(bool isHosting)
    {
        PlayerPrefs.SetString("DisplayName", isHosting ? hostingNameInput.text : joiningNameInput.text);
    }

    public string GetJoinCode()
    {
        return joinCodeInput.text;
    }

    public void SetConnectingText(string text, bool isError)
    {
        connectingText.text = text;
        connectingBackButton.SetActive(isError);
    }

    public bool IsConnecting()
    {
        return connectingTab.activeInHierarchy;
    }
}
