using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI : MonoBehaviour
{
    public static UI instance;

    [SerializeField] private GameUI gameUI;
    [SerializeField] private MainMenuUI mainMenuUI;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        OpenMainMenu();
    }

    public void SetUICamera(Camera cam)
    {
        GetComponent<Canvas>().worldCamera = cam;
    }

    public void ResetAllButtons()
    {
        foreach (MainMenuButton button in FindObjectsByType<MainMenuButton>(FindObjectsSortMode.None))
        {
            button.ResetButton();
        }
    }

    public void OpenMainMenu()
    {
        MenuCamera.instance.gameObject.SetActive(true);
        mainMenuUI.gameObject.SetActive(true);
        mainMenuUI.OpenTab(MainMenuUI.Tab.main);
        gameUI.gameObject.SetActive(false);
    }


    public void CloseMainMenu()
    {
        MenuCamera.instance.gameObject.SetActive(false);
        mainMenuUI.gameObject.SetActive(false);
        gameUI.gameObject.SetActive(true);
    }
}
