using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class LevelSelectEvents : MonoBehaviour
{

    private UIDocument _document;

    private Button level1Button;
    private Button level2Button;
    private Button level3Button;
    private Button level4Button;
    private Button level5Button;


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        _document = GetComponent<UIDocument>();

        //Setting OnClick events to each button

        level1Button = _document.rootVisualElement.Q("Level1Button") as Button;
        level1Button.RegisterCallback<ClickEvent>(OnLevel1ButtonClick);

        level2Button = _document.rootVisualElement.Q("Level2Button") as Button;
        level2Button.RegisterCallback<ClickEvent>(OnLevel2ButtonClick);

        level3Button = _document.rootVisualElement.Q("Level3Button") as Button;
        level3Button.RegisterCallback<ClickEvent>(OnLevel3ButtonClick);

        level4Button = _document.rootVisualElement.Q("Level4Button") as Button;
        level4Button.RegisterCallback<ClickEvent>(OnLevel4ButtonClick);

        level5Button = _document.rootVisualElement.Q("Level5Button") as Button;
        level5Button.RegisterCallback<ClickEvent>(OnLevel5ButtonClick);
    }


    private void OnDisable()
    {
        level1Button.UnregisterCallback<ClickEvent>(OnLevel1ButtonClick);
        level2Button.UnregisterCallback<ClickEvent>(OnLevel2ButtonClick);
        level3Button.UnregisterCallback<ClickEvent>(OnLevel3ButtonClick);
        level4Button.UnregisterCallback<ClickEvent>(OnLevel4ButtonClick);
        level5Button.UnregisterCallback<ClickEvent>(OnLevel5ButtonClick);
    }

    private void OnLevel1ButtonClick(ClickEvent evt)
    {
        Debug.Log("Pressed Level 1");

        SceneManager.LoadScene("Level 1");
    }

    private void OnLevel2ButtonClick(ClickEvent evt)
    {
        Debug.Log("Pressed Level 2");

        SceneManager.LoadScene("Level 2");
    }

    private void OnLevel3ButtonClick(ClickEvent evt)
    {
        Debug.Log("Pressed Level 3");

        SceneManager.LoadScene("Level 3");
    }

    private void OnLevel4ButtonClick(ClickEvent evt)
    {
        Debug.Log("Pressed Level 4");

        SceneManager.LoadScene("Level 4");
    }

    private void OnLevel5ButtonClick(ClickEvent evt)
    {
        Debug.Log("Pressed Level 5");

        SceneManager.LoadScene("Level 5");
    }

}
