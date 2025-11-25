using System.Collections.Generic;
using UnityEngine;

public class EnemyBombSpawns : MonoBehaviour
{
    private GridBuilder grid;
    private List<CellData> bombCells = new List<CellData>();
    private List<CellData> enemyCells = new List<CellData>();

    public float bombChance = 0.2f;
    public float enemyChance = 0.8f;
    public float multiSpawnChance = 0.4f;
    public float difficultyScaling = 0.01f;
    private int spawnAmount = 1;
    public float spawnInterval = 1f;

    private float scoreTimer = 0f;
    private float spawnTimer = 0f;
    private int score = 0;

    void Awake()
    {
        grid = FindAnyObjectByType<GridBuilder>();
    }

    // Update is called once per frame
    void Update()
    {
        // Add to timer
        spawnTimer += Time.deltaTime;
        spawnAmount = 1;

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
            spawnInterval = Mathf.Max(0.2f, spawnInterval - difficultyScaling);
            bombChance = Mathf.Min(0.4f, bombChance + difficultyScaling / 2f);
            enemyChance = Mathf.Min(1.0f, enemyChance + difficultyScaling / 2f);
            multiSpawnChance = Mathf.Min(0.7f, multiSpawnChance + difficultyScaling / 4f);
        }
    }

    void MoveElementsDown()
    {
        List<CellData> newBombList = new List<CellData>();
        List<CellData> newEnemyList = new List<CellData>();

        // Move bombs and enemies down one row
        foreach (var cell in bombCells)
        {
            int r = cell.Row();
            int c = cell.Col();

            // If there's no bomb in this cell (bomb deleted), skip
            if (!cell.IsBombEnabled())
                continue;
            
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

            // If there's no enemy in this cell (enemy deleted), skip
            if (!cell.IsEnemyEnabled())
                continue;

            cell.SetEnemy(false); // always disable current cell

            if (c + 1 < grid.cols)
            {
                CellData next = grid.cell_datas[c + 1, r];
                next.SetEnemy(true);
                newEnemyList.Add(next);
            }
        }

        bombCells = newBombList;
        enemyCells = newEnemyList;
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
