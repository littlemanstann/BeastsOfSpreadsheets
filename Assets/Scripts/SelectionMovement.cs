using System;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.UI;

using Vector3 = UnityEngine.Vector3;
using Unity.VisualScripting;
using Unity.Mathematics;

public class SelectionMovement : MonoBehaviour
{
    private GridBuilder grid;
    private RectTransform selectionBox;
    public List<CellData> selectedCells = new List<CellData>();
    private CellData currentCell;
    public CellData CurrentCell { get { return currentCell; } }

    private CellData anchorCell; // For shift selection
    public GameObject anchorImage; // Assign in inspector
    private bool shiftHeld = false;
    public bool ShiftHeld { get { return shiftHeld; } }
    private Image selectionImage;

    private List<Vector3> inputBuffer = new List<Vector3>();
    private Vector3 currentPosition;

    private int row = 0;
    private int col = 0;
    public bool cellEnterCheck = false;
    public bool disableMovement = false;
    // Bools to communicate to Level 1 and Level 2 scripts
    private bool[] mistake = new bool[2] { false, false };
    public bool[] Mistake { get { return mistake; } }
    public bool checkFilled = false;
    public bool CheckFilled { get { return checkFilled; } set { checkFilled = value; } }

    void Awake()
    {
        selectionBox = GetComponent<RectTransform>();
        selectionImage = GetComponent<Image>();
    }

    void Start()
    {
        grid = FindAnyObjectByType<GridBuilder>();
        UpdateSelectionBoxPosition();
        anchorCell = currentCell;
        selectionBox.position = currentPosition;
    }

    void Update()
    {
        if (disableMovement) return;

        // Check if shift is held
        shiftHeld = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        if (Input.GetKeyDown(KeyCode.UpArrow)) {
            Move(-1, 0);
        }
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

        // If anchor was moved, update its position to be stationary on the anchor cell
        if (anchorImage.activeSelf)
        {
            anchorImage.transform.position = new Vector3(anchorCell.cell_transform.position.x,
                                                         anchorCell.cell_transform.position.y,
                                                         anchorImage.transform.position.z);
        }

        // Handle cell clearing
        if (Input.GetKeyDown(KeyCode.Delete)) ClearCells();
        if (Input.GetKeyDown(KeyCode.Backspace)) ClearCells();

        // Add timestamp to cell 
        if(Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Semicolon)){
            AddTimeStampToSelectedCells();
        }

        // More selection
         if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Space))
            SelectWholeCol();

        if ((Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.Space)) || (Input.GetKey(KeyCode.Z) && Input.GetKeyDown(KeyCode.Space)))
            SelectWholeRow();

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.A))
            SelectAllCells();
        // Format Selection
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.GetKeyDown(KeyCode.Alpha1)) FormatAsDecimal();
            if (Input.GetKeyDown(KeyCode.Alpha2)) FormatAsTime();
            if (Input.GetKeyDown(KeyCode.Alpha3)) FormatAsDate();
            if (Input.GetKeyDown(KeyCode.Alpha4)) FormatAsCurrency();
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Alpha6)) FormatAsPercentage();
        }
        // Fill Shortcuts
        if ((Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.D)) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.D)))
        {
            FillDown();
        }
        if ((Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R)) || (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.R)))
        {
            FillRight();
        }
    }
        
    /// Selection Shortcuts ///
    void SelectAllCells()
    {
        if (selectedCells.Count == grid.rows * grid.cols)
        {
            ClearHighlights();
            selectedCells.Clear();
            selectedCells.Add(currentCell);
            Highlight(currentCell);
            return;
        }

        ClearHighlights();
        selectedCells.Clear();
        for (int r = 0; r < grid.rows; r++)
            for (int c = 0; c < grid.cols; c++)
            {
                var cell = grid.cell_datas[c, r];
                selectedCells.Add(cell);
                Highlight(cell);
            }
    }

    void SelectWholeRow()
    {
        bool isSameRow = selectedCells.Count == grid.cols;
        if (isSameRow)
        {
            for (int c = 0; c < grid.cols; c++)
            {
                if (!selectedCells.Contains(grid.cell_datas[c, row]))
                {
                    isSameRow = false;
                    break;
                }
            }
        }

        if (isSameRow)
        {
            ClearHighlights();
            selectedCells.Clear();
            selectedCells.Add(currentCell);
            Highlight(currentCell);
            return;
        }

        ClearHighlights();
        selectedCells.Clear();
        for (int c = 0; c < grid.cols; c++)
        {
            var cell = grid.cell_datas[c, row];
            selectedCells.Add(cell);
            Highlight(cell);
        }
    }

    void SelectWholeCol()
    {
        bool isSameCol = selectedCells.Count == grid.rows;
        if (isSameCol)
        {
            for (int r = 0; r < grid.rows; r++)
            {
                if (!selectedCells.Contains(grid.cell_datas[col, r]))
                {
                    isSameCol = false;
                    break;
                }
            }
        }

        if (isSameCol)
        {
            ClearHighlights();
            selectedCells.Clear();
            selectedCells.Add(currentCell);
            Highlight(currentCell);
            return;
        }

        ClearHighlights();
        selectedCells.Clear();
        for (int r = 0; r < grid.rows; r++)
        {
            var cell = grid.cell_datas[col, r];
            selectedCells.Add(cell);
            Highlight(cell);
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

    // Movement Handling
    void Move(int dr, int dc)
    {
        // If shift is released, reset to anchor cell before updating position
        if (!shiftHeld && anchorCell != currentCell)
        {
            currentCell = anchorCell;
            col = currentCell.Col();
            row = currentCell.Row();
        }

        // Check if new position is within bounds
        if (row + dc < 0 || row + dc >= grid.rows || col + dr < 0 || col + dr >= grid.cols)
            return;

        col = Mathf.Clamp(col + dr, 0, grid.cols - 1);
        row = Mathf.Clamp(row + dc, 0, grid.rows - 1);
        var dest = grid.cell_datas[col, row];
        Debug.Log("Moved to cell with contents " + dest.get_text());

        // If the cell has been visited and shift is not held, mark mistake
        if (dest.IsBeenToEnabled() && !shiftHeld)
            mistake[0] = true;

        UpdateSelectionBoxPosition();
    }
    
    void UpdateSelectionBoxPosition()
    {
        currentCell = grid.cell_datas[col, row];

        // Show "Been To" image if enabled
        if (cellEnterCheck && !shiftHeld)
        {
            currentCell.SetEntered(true);
            Debug.Log("Marked cell at " + currentCell.Row() + ", " + currentCell.Col() + " as visited.");
        }

        // Smooth movement
        if (inputBuffer.Count < 4)
            inputBuffer.Add(new Vector3(currentCell.cell_transform.position.x,
                                        currentCell.cell_transform.position.y,
                                        selectionBox.position.z));

        // When shift is not held
        if (!shiftHeld)
        {
            anchorCell = currentCell;
            anchorImage.SetActive(false);
            selectionImage.color = Color.white;
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

        // Enable anchor image
        anchorImage.SetActive(true);
        selectionImage.color = Color.red;
        
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
            if (cell.IsBombEnabled()) {
                HighlightRed(cell);
                mistake[1] = true;
            }
            cell.SetEnemy(false);
        }
    }

    void Highlight(CellData cell)
    {
        cell.cell_transform.GetComponent<SpriteRenderer>().color = Color.yellow;
    }

    void HighlightRed(CellData cell)
    {
        cell.cell_transform.GetComponent<SpriteRenderer>().color = Color.red;
    }

    void ClearHighlights()
    {
        foreach (var c in selectedCells)
            c.cell_transform.GetComponent<SpriteRenderer>().color = Color.white;
    }


    public void FormatAsDecimal()
    {
        foreach (var cell in selectedCells)
        {
            if (float.TryParse(cell.get_text(), out float num))
            {
                cell.update_text(num.ToString("F2")); // 2 decimal places
            }
        }
    }
    public void FormatAsTime()
    {
        foreach (var cell in selectedCells)
        {
            if (TimeSpan.TryParse(cell.get_text(), out TimeSpan time))
            {
                cell.update_text(time.ToString(@"hh\:mm\:ss"));
            }
            else if (float.TryParse(cell.get_text(), out float hours))
            {
                TimeSpan t = TimeSpan.FromHours(hours);
                cell.update_text(t.ToString(@"hh\:mm\:ss"));
            }
        }
    }

    public void FormatAsDate()
    {
        foreach (var cell in selectedCells)
        {
            string original = cell.get_text();

            if (DateTime.TryParseExact(original, new[] {
                    "M/d/yy", "M/d/yyyy", "MM/dd/yy", "MM/dd/yyyy",
                    "M/d", "MM/dd"
                },
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, out DateTime date))
            {
                if (date.Year < 100)
                {
                    date = date.Year < 50 ? date.AddYears(2000 - date.Year) : date.AddYears(1900 - date.Year);
                }

                cell.update_text(date.ToString("M/d/yyyy"));
            }
            else if (double.TryParse(original, out double oaDate))
            {
                try
                {
                    DateTime fromOADate = DateTime.FromOADate(oaDate);
                    cell.update_text(fromOADate.ToString("M/d/yyyy"));
                }
                catch { }
            }
        }
    }
    public void FormatAsCurrency()
    {
        foreach (var cell in selectedCells)
        {
            if (decimal.TryParse(cell.get_text(), out decimal val))
            {
                cell.update_text(val.ToString("C2"));
            }
        }
    }
    public void FormatAsPercentage()
    {
        foreach (var cell in selectedCells)
        {
            if (double.TryParse(cell.get_text(), out double val))
            {
                val *= 100;
                cell.update_text(val.ToString("F2") + "%");
            }
        }
    }

    public void FillDown()
    {
        foreach (var cell in selectedCells)
        {
            int r = cell.Row();
            int c = cell.Col();

            if (c + 1 < grid.cols)
            {
                var downCell = grid.cell_datas[c + 1, r];
                if (!selectedCells.Contains(downCell)) continue;
                if (downCell.get_text() == "FILLED") {
                    downCell.update_text("MISTAKE!");
                    mistake[1] = true;
                }
                else{
                    downCell.update_text(cell.get_text());
                    checkFilled = true;
                }
            }
        }
    }
    
    public void FillRight()
    {
        foreach (var cell in selectedCells)
        {
            int r = cell.Row();
            int c = cell.Col();
            if (r + 1 < grid.rows)
            {
                var rightCell = grid.cell_datas[c, r + 1];
                if (!selectedCells.Contains(rightCell)) continue;
                if (rightCell.get_text() == "FILLED") {
                    rightCell.update_text("MISTAKE!");
                    mistake[1] = true;
                }
                else {
                    rightCell.update_text(cell.get_text());
                    checkFilled = true;
                }
            }
        }
    }
}
