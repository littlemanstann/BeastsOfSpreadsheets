using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class MainMenuEvents : MonoBehaviour
{

    private UIDocument _document;

    private Button playButton;
    private Button optionsButton;
    private Button quitButton;

    private Button level1Button;
    private Button level2Button;
    private Button level3Button;
    private Button level4Button;
    private Button level5Button;


    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        //Setting OnClick events to each button

        playButton = _document.rootVisualElement.Q("PlayButton") as Button;
        playButton.RegisterCallback<ClickEvent>(OnPlayButtonClick);

        optionsButton = _document.rootVisualElement.Q("OptionsButton") as Button;
        optionsButton.RegisterCallback<ClickEvent>(OnOptionsButtonClick);

        quitButton = _document.rootVisualElement.Q("QuitButton") as Button;
        quitButton.RegisterCallback<ClickEvent>(OnQuitButtonClick);

        //level1Button = _document.rootVisualElement.Q("Level1Button") as Button;
        //level1Button.RegisterCallback<ClickEvent>(OnLevel1ButtonClick);
    }

    private void OnDisable()
    {
        playButton.UnregisterCallback<ClickEvent>(OnPlayButtonClick);
        optionsButton.UnregisterCallback<ClickEvent>(OnOptionsButtonClick);
        quitButton.UnregisterCallback<ClickEvent>(OnQuitButtonClick);
    }

    private void OnPlayButtonClick(ClickEvent evt) 
    {
        Debug.Log("Pressed Play");

        SceneManager.LoadScene("Level Select");
    }


    private void OnOptionsButtonClick(ClickEvent evt)
    {
        Debug.Log("Pressed Options");
    }

    private void OnQuitButtonClick(ClickEvent evt)
    {
        Debug.Log("Pressed Quit");
#if UNITY_STANDALONE
        Application.Quit();
#endif

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
