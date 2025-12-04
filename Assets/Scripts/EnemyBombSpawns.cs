using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class EnemyBombSpawns : MonoBehaviour
{
    private GridBuilder grid;
    private SelectionMovement selectionMovement;
    private List<CellData> bombCells = new List<CellData>();
    private List<CellData> enemyCells = new List<CellData>();

    public GameObject endScreenUI;
    public GameObject endScreenTitle;
    public GameObject endScreenBody;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI scoreText;
    public float bombChance = 0.2f;
    public float enemyChance = 0.7f;
    public float multiSpawnChance = 0.2f;
    public float difficultyScaling = 0.001f;
    private int spawnAmount = 1;
    public float spawnInterval = 1.7f;

    private float scoreTimer = 0f;
    private float spawnTimer = 0f;
    private int score = 0;
    private bool gameOver = false;

    void Awake()
    {
        grid = FindAnyObjectByType<GridBuilder>();
        selectionMovement = FindAnyObjectByType<SelectionMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameOver) return;

        // Add to timers
        scoreTimer += Time.deltaTime;
        timerText.text = "Timer: " + scoreTimer.ToString("F2");
        spawnTimer += Time.deltaTime;
        spawnAmount = 1;

        // Check for game over from selection movement
        if (selectionMovement.Mistake[0])
        {
            LoseGame(0);
            return;
        }
        else if (selectionMovement.Mistake[1])
        {
            LoseGame(1);
            return;
        }

        // Decide to spawn multiple guys or not
        while (Random.Range(0f, 1f) < multiSpawnChance)
        {
            spawnAmount++;
        }

        // Timer for every 0.5 seconds
        if (spawnTimer >= spawnInterval)
        {
            MoveElementsDown();

            for (int i = 0; i < spawnAmount; i++)
            {
                float roll = Random.Range(0f, 1f);

                // Choose a random row without any bomb/enemy in the top row
                int spawnRow = Random.Range(0, grid.rows);
                while (grid.cell_datas[0, spawnRow].IsBombEnabled() || grid.cell_datas[0, spawnRow].IsEnemyEnabled())
                {
                    spawnRow = Random.Range(0, grid.rows);
                }

                // Spawn bomb or enemy based on chance
                if (roll < bombChance)
                {
                    SpawnBombAt(spawnRow, 0);
                    bombCells.Add(grid.cell_datas[0, spawnRow]);
                }
                else if (roll < enemyChance)
                {
                    SpawnEnemyAt(spawnRow, 0);
                    enemyCells.Add(grid.cell_datas[0, spawnRow]);
                }
            }
            
            spawnTimer = 0f;
            // Make spawn interval shorter over time to increase difficulty
            spawnInterval = Mathf.Max(0.2f, spawnInterval - difficultyScaling / 1f);
            bombChance = Mathf.Min(0.4f, bombChance + difficultyScaling / 2f);
            enemyChance = Mathf.Min(1.0f, enemyChance + difficultyScaling / 2f);
            multiSpawnChance = Mathf.Min(0.7f, multiSpawnChance + difficultyScaling / 4f);
        }

        // Update score display
        scoreText.text = "SCORE: " + score.ToString();
    }

    void MoveElementsDown()
    {
        List<CellData> newBombList = new List<CellData>();
        List<CellData> newEnemyList = new List<CellData>();
        int scoreMultiplier = 1;

        // Move bombs and enemies down one row
        foreach (var cell in bombCells)
        {
            int r = cell.Row();
            int c = cell.Col();

            // If there's no bomb in this cell (bomb deleted), lose the game
            if (!cell.IsBombEnabled())
            {
                LoseGame(1);
                continue;
            }
            
            cell.SetBomb(false); // always disable current cell

            if (c + 1 < grid.cols)
            {
                CellData next = grid.cell_datas[c + 1, r];
                next.SetBomb(true);
                newBombList.Add(next);
            }
        }
        foreach (var cell in enemyCells)
        {
            int r = cell.Row();
            int c = cell.Col();

            // If there's no enemy in this cell (enemy deleted), add to score and skip
            if (!cell.IsEnemyEnabled()) {
                score += 1 * scoreMultiplier;
                scoreMultiplier *= 2;
                continue;
            }

            cell.SetEnemy(false); // always disable current cell

            if (c + 1 < grid.cols)
            {
                CellData next = grid.cell_datas[c + 1, r];
                next.SetEnemy(true);
                newEnemyList.Add(next);
            } 
            else
            {
                // Enemy reached the end - lose the game
                LoseGame(0);
            }
        }

        bombCells = newBombList;
        enemyCells = newEnemyList;
    }

    void LoseGame(int type)
    {   
        Debug.Log("Game Over!");
        gameOver = true;
        Time.timeScale = 0f; // Pause game time
        
        //added this just to have a temporary win condition for testing
        //-evan
        endScreenUI.SetActive(true);

        // Lose game based on type
        if (type == 0)
        {
            endScreenTitle.GetComponent<TextMeshProUGUI>().text = "You Lost!";
            endScreenBody.GetComponent<TextMeshProUGUI>().text = "An enemy reached the end of the grid!\nSCORE: " + score.ToString() + " in " + scoreTimer.ToString("F2") + " seconds.";
        }
        else if (type == 1)
        {
            endScreenTitle.GetComponent<TextMeshProUGUI>().text = "You Lost!";
            endScreenBody.GetComponent<TextMeshProUGUI>().text = "You triggered a bomb!\nSCORE: " + score.ToString() + " in " + scoreTimer.ToString("F2") + " seconds.";
        }
    }

    void SpawnBombAt(int row, int col)
    {
        CellData cell = grid.cell_datas[col, row];
        cell.SetBomb(true);
    }

    void SpawnEnemyAt(int row, int col)
    {
        CellData cell = grid.cell_datas[col, row];
        cell.SetEnemy(true);
    }
}
