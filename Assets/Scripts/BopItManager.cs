using UnityEngine;
using TMPro;
public class BopItManager : MonoBehaviour
{
    public GridBuilder grid;
    public SpriteRenderer spriteRenderer;
    public TextMeshProUGUI promptText;
    public TextMeshProUGUI toolTip;
    public SelectionMovement selector;

    private float timer = 0f;
    private float promptDuration = 5f;
    private float interval = 5f;

    private bool promptActive = false;
    private float promptTimer = 0f;
    private string currentCommand = "";
    private int targetIndex = -1;
    private int successCount = 0;
    private int successGoal = 25;

    void Start()
    {
        promptText.text = "";
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (!promptActive && timer >= interval)
        {
            StartPrompt();
        }

        if (promptActive)
        {
            promptTimer += Time.deltaTime;

            if (CheckIfCorrect())
            {
                successCount++;
                toolTip.text = $"{successCount} / {successGoal}";

                EndPrompt();

                if (successCount >= successGoal)
                {
                    ClearImage(true);
                    enabled = false;
                }

                return;
            }

            if (promptTimer >= promptDuration)
            {
                ClearImage(false);
                enabled = false;
            }
        }
    }

    void StartPrompt()
    {
        promptActive = true;
        promptTimer = 0f;
        timer = 0f;

        float choice = Random.value;
        if (choice < 0.4f)
        {
            targetIndex = Random.Range(1, 15);
            currentCommand = $"SELECT ROW {targetIndex}";
        }
        else if (choice < 0.8f)
        {
            targetIndex = Random.Range(1, 7);
            currentCommand = $"SELECT COL {targetIndex}";
        }
        else
        {
            currentCommand = "SELECT ALL CELLS";
        }

        promptText.text = currentCommand;
    }

    void EndPrompt()
    {
        promptActive = false;
        promptText.text = "";
        targetIndex = -1;
        currentCommand = "";
    }

    bool CheckIfCorrect()
    {
        if (currentCommand.StartsWith("SELECT ROW"))
        {
            return selector.selectedCells.Count == grid.rows &&
                selector.selectedCells.Contains(grid.cell_datas[targetIndex - 1, 0]);
        }
        if (currentCommand.StartsWith("SELECT COL"))
        {
            return selector.selectedCells.Count == grid.cols &&
                selector.selectedCells.Contains(grid.cell_datas[0, targetIndex - 1]);
        }
        if (currentCommand == "SELECT ALL CELLS")
        {
            return selector.selectedCells.Count == grid.rows * grid.cols;
        }  
        return false;
    }
    void ClearImage(bool success)
    {
        if (success)
        {
            Sprite loaded = Resources.Load<Sprite>("checkmark");
            spriteRenderer.sprite = loaded;
        }
        else
        {
            Sprite loaded = Resources.Load<Sprite>("fail");
            spriteRenderer.sprite = loaded;
        }
    }
}
