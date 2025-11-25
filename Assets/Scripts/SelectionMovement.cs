using System;
using System.Collections.Generic;
using UnityEngine;


using Vector3 = UnityEngine.Vector3;

public class SelectionMovement : MonoBehaviour
{
    private GridBuilder grid;
    private RectTransform selectionBox;
    private List<CellData> selectedCells = new List<CellData>();
    private CellData currentCell;
    private CellData anchorCell; // For shift selection
    private bool shiftHeld = false;

    private List<Vector3> inputBuffer = new List<Vector3>();
    private Vector3 currentPosition;

    private EnemyBombSpawns spawner;

    private int row = 0;
    private int col = 0;

    void Start()
    {
        selectionBox = this.GetComponent<RectTransform>();
        grid = FindAnyObjectByType<GridBuilder>();
        spawner = FindAnyObjectByType<EnemyBombSpawns>();
        UpdateSelectionBoxPosition();
    }

    void Update()
    {
        // Check if shift is held
        shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKeyDown(KeyCode.UpArrow))    Move(-1, 0);
        if (Input.GetKeyDown(KeyCode.DownArrow))  Move(1, 0);
        if (Input.GetKeyDown(KeyCode.LeftArrow))  Move(0, -1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move(0, 1);

        // Smoothly interpolate the selection box position

        //store a list of all the places the box should be and move the box through the list until its empty

        if (inputBuffer.Count > 0)//if there's still a destination
        {
            currentPosition = Vector3.Lerp(currentPosition, inputBuffer[0], 100f * Time.deltaTime);
            selectionBox.position = currentPosition;

            //if its close to the location
            if (Vector3.Distance(currentPosition, inputBuffer[0]) < 0.05f)
            { 
                //remove that point
                inputBuffer.RemoveAt(0);
            }
        }

        // Handle cell clearing
        if (Input.GetKeyDown(KeyCode.Delete)) ClearCells();
        if (Input.GetKeyDown(KeyCode.Backspace)) ClearCells();

        // Add timestamp to cell 
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Semicolon)){
            AddTimeStampToSelectedCells();
        }
    }
    void AddTimeStampToSelectedCells()
    {
        foreach (var item in selectedCells)
        {
            string timestamp = System.DateTime.Now.ToString("h:mm:ss tt");
            item.update_text(timestamp);
        }
    }

    void Move(int dr, int dc)
    {
        // Check if new position is within bounds
        if (row + dc < 0 || row + dc >= grid.rows || col + dr < 0 || col + dr >= grid.cols)
            return;

        col = Mathf.Clamp(col + dr, 0, grid.cols - 1);
        row = Mathf.Clamp(row + dc, 0, grid.rows - 1);
        var dest = grid.cell_datas[col, row];
        Debug.Log("moved to cell with contents " + dest.get_text());
        
        UpdateSelectionBoxPosition();
    }
    
    void UpdateSelectionBoxPosition()
    {
        currentCell = grid.cell_datas[col, row];

        // Smooth movement
        if (inputBuffer.Count < 4)
            inputBuffer.Add(new Vector3(currentCell.cell_transform.position.x,
                                        currentCell.cell_transform.position.y,
                                        selectionBox.position.z));

        if (!shiftHeld)
        {
            anchorCell = currentCell;
            ClearHighlights();
            selectedCells.Clear();
            selectedCells.Add(currentCell);
            Highlight(currentCell);
        }
        else
        {
            HandleShiftSelection();
        }
    }

    void HandleShiftSelection()
    {
        ClearHighlights();
        selectedCells.Clear();

        int minRow = Mathf.Min(anchorCell.Row(), currentCell.Row());
        int maxRow = Mathf.Max(anchorCell.Row(), currentCell.Row());
        int minCol = Mathf.Min(anchorCell.Col(), currentCell.Col());
        int maxCol = Mathf.Max(anchorCell.Col(), currentCell.Col());

        for (int r = minRow; r <= maxRow; r++)
        {
            for (int c = minCol; c <= maxCol; c++)
            {
                var target = grid.cell_datas[c, r];
                selectedCells.Add(target);
                Highlight(target);
            }
        }
    }

    void ClearCells()
    {
        foreach (var cell in selectedCells)
        {
            cell.update_text("");
            // Also remove bomb/enemy if present
            cell.SetBomb(false);
            cell.SetEnemy(false);
        }
    }

    void Highlight(CellData cell)
    {
        cell.cell_transform.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    void ClearHighlights()
    {
        foreach (var c in selectedCells)
            c.cell_transform.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
