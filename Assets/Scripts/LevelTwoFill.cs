using UnityEditor;
using UnityEngine;
using TMPro;
using Unity.VisualScripting;
using System.ComponentModel.Design;
using UnityEngine.UI;

public class LevelTwoFill : MonoBehaviour
{
    private GridBuilder grid;
    private SelectionMovement selectionMovement;

    public GameObject endScreen;
    public TextMeshProUGUI endTitle;
    public TextMeshProUGUI endBody;

    public TextMeshProUGUI timerText;
    private float timer = 0f;

    void Start()
    {
        grid = FindAnyObjectByType<GridBuilder>();
        selectionMovement = FindAnyObjectByType<SelectionMovement>();

        // Fill the cells randomly with "FILLED" or leave them empty
        for (int r = 0; r < grid.rows; r++)
        {
            for (int c = 0; c < grid.cols; c++)
            {
                if (r == 0 && c == 0)
                {
                    // Skip the first cell
                    continue;
                }

                var cell = grid.cell_datas[c, r];
                // 30% chance to fill the cell
                if (Random.value < 0.3f)
                {
                    cell.update_text("FILLED");
                }
                else
                {
                    cell.update_text("");
                }
            }
        }
    }

    // If you step back into a filled cell, lose the game
    void Update()
    {
        timer += Time.deltaTime;
        timerText.text = "Timer: " + timer.ToString("F2");

       if (selectionMovement.Mistake[0])
       {
            Time.timeScale = 0f; // Pause the game
            Debug.Log("Moved back into an already-visited cell! \n TIP: Hold SHIFT to select cells without visiting.");
            endScreen.SetActive(true);
            endTitle.text = "You Lost!";
            endBody.text = "You moved back into an already-visited cell!\nTIP: Hold SHIFT to select cells without visiting.";
       }
       else if (selectionMovement.Mistake[1])
       {
            Time.timeScale = 0f; // Pause the game
            Debug.Log("Accidentially filled over a filled cell! \n TIP: Plan out your moves carefully.");
            endScreen.SetActive(true);
            endTitle.text = "You Lost!";
            endBody.text = "You accidentially filled over a filled cell!\nTIP: Plan out your moves carefully.";            
       }

       if (selectionMovement.CheckFilled)
        {
            selectionMovement.CheckFilled = false;
            if (IsBoardFilled())
            {
                // Win screen
                Time.timeScale = 0f; // Pause the game
                Debug.Log("All cells filled! You win!");
                endTitle.text = "You Win!";
                endBody.text = "Congratulations! You have successfully filled all the cells. Your time is " + timer.ToString("F2") + " seconds.";
                endScreen.SetActive(true);
            }
        }
    }

    public bool IsBoardFilled()
    {
        for (int r = 0; r < grid.rows; r++)
        {
            for (int c = 0; c < grid.cols; c++)
            {
                var cell = grid.cell_datas[c, r];
                if (cell.get_text() != "FILLED")
                {
                    return false;
                }
            }
        }
        return true;
    }
}
