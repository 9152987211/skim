using UnityEngine;
using UnityEngine.UI;


public class MainMenuButton : MonoBehaviour
{
    [SerializeField] private ButtonType buttonType;
    [SerializeField] private bool resetButtonOnClick;
    [SerializeField] private bool playHoverSound = true;

    [Header("SOUNDS")]
    [SerializeField] private AudioSource hoverSound;
    [SerializeField] private AudioSource clickSound;
    [SerializeField] private AudioSource errorSound;

    [Header("SIZE")]
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float hoverScaleSpeed = 20.0f;

    [Header("COLOUR")]
    [SerializeField] private Color hoverColour;
    [SerializeField] private float hoverColourSpeed = 10.0f;


    private RectTransform rectTransform;
    private Image image;

    private bool isHovering = false;

    private Vector3 hoverScaleVector;

    private Color defaultColour;


    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        image = GetComponent<Image>();

        hoverScaleVector = new Vector3(hoverScale, hoverScale, 1.0f);
        defaultColour = new Color(image.color.r, image.color.g, image.color.b, image.color.a);
    }

    private void Update()
    {
        //UPDATE BUTTON SIZE
        rectTransform.localScale = Vector3.Lerp(rectTransform.localScale, isHovering ? hoverScaleVector : Vector3.one, hoverScaleSpeed * Time.deltaTime);

        //UPDATE BUTTON COLOUR
        image.color = Color.Lerp(image.color, isHovering ? hoverColour : defaultColour, hoverColourSpeed * Time.deltaTime);

        //USER CLICKED BUTTON
        if (Input.GetKeyDown(KeyCode.Mouse0) && isHovering) ButtonClicked();
    }

    public void Hovering()
    {
        isHovering = true;
        if (playHoverSound) hoverSound.Play();
    }

    public void StoppedHovering()
    {
        isHovering = false;
    }

    public enum ButtonType
    {
        BackToMain, OpenHost, OpenJoin, OpenOffline, OpenSettings, OpenQuit, Discord, Host, Join, Offline, EscapeMenuDisconnect, EscapeMenuQuit, EscapeMenuSettings
    }

    private void ButtonClicked()
    {
        if (resetButtonOnClick) ResetButton();

        switch(buttonType)
        {
            case ButtonType.BackToMain:
                clickSound.Play();
                MainMenuUI.instance.OpenTab(MainMenuUI.Tab.main);
                break;
            case ButtonType.OpenHost:
                clickSound.Play();
                MainMenuUI.instance.OpenTab(MainMenuUI.Tab.host);
                break;
            case ButtonType.OpenJoin:
                clickSound.Play();
                MainMenuUI.instance.OpenTab(MainMenuUI.Tab.join);
                break;
            case ButtonType.OpenOffline:
                clickSound.Play();
                MainMenuUI.instance.OpenTab(MainMenuUI.Tab.offline);
                break;
            case ButtonType.OpenSettings:
                clickSound.Play();
                MainMenuUI.instance.OpenTab(MainMenuUI.Tab.settings);
                break;
            case ButtonType.OpenQuit:
                clickSound.Play();
                MainMenuUI.instance.OpenTab(MainMenuUI.Tab.quit);
                break;
            case ButtonType.Discord:
                errorSound.Play();
                //clickSound.Play();
                //Application.OpenURL("https://discord.gg/1234");
                break;

            case ButtonType.Host:
                if (PlayerPrefs.GetString("DisplayName", "") == "")
                {
                    errorSound.Play();
                }
                else
                {
                    clickSound.Play();
                    GameHandler.instance.HostGame();
                }
                break;
            case ButtonType.Join:
                if (PlayerPrefs.GetString("DisplayName", "") == "" || MainMenuUI.instance.GetJoinCode() == "")
                {
                    errorSound.Play();
                }
                else
                {
                    clickSound.Play();
                    GameHandler.instance.JoinGame();
                }
                break;
            case ButtonType.Offline:
                clickSound.Play();
                GameHandler.instance.HostOfflineGame();
                break;



            //------------- ESCAPE MENU -------------
            case ButtonType.EscapeMenuDisconnect:
                clickSound.Play();
                GameHandler.instance.Disconnect();
                break;
            case ButtonType.EscapeMenuQuit:
                Application.Quit();
                break;
            case ButtonType.EscapeMenuSettings:
                break;
        }
    }

    public void ResetButton()
    {
        isHovering = false;
        rectTransform.localScale = Vector3.one;
        image.color = defaultColour;
    }
}
