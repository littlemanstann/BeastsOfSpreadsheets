using System.Collections.Generic;
using UnityEngine;

using Vector3 = UnityEngine.Vector3;

public class SelectionMovement : MonoBehaviour
{
    public GridBuilder grid;
    private RectTransform selectionBox;

    private List<Vector3> inputBuffer = new List<Vector3>();
    private Vector3 currentPosition;

    private int row = 0;
    private int col = 0;

    void Start()
    {
        selectionBox = this.GetComponent<RectTransform>();
        UpdateSelectionBoxPosition();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))    Move(-1, 0);
        if (Input.GetKeyDown(KeyCode.DownArrow))  Move(1, 0);
        if (Input.GetKeyDown(KeyCode.LeftArrow))  Move(0, -1);
        if (Input.GetKeyDown(KeyCode.RightArrow)) Move(0, 1);

        // Smoothly interpolate the selection box position

        if (inputBuffer.Count > 0)
        {
            currentPosition = Vector3.Lerp(currentPosition, inputBuffer[0], 100f * Time.deltaTime);
            selectionBox.position = currentPosition;

            if (Vector3.Distance(currentPosition, inputBuffer[0]) < 0.3f && inputBuffer.Count > 1)
            {
                inputBuffer.RemoveAt(0);
            }

        }

    }

    void Move(int dr, int dc)
    {
        col = Mathf.Clamp(col + dr, 0, grid.cols - 1);
        row = Mathf.Clamp(row + dc, 0, grid.rows - 1);
        var dest = grid.cell_datas[col, row];
        Debug.Log("moved to cell with contents " + dest.contents);
        UpdateSelectionBoxPosition();
    }

    void UpdateSelectionBoxPosition()
    {
        var cell = grid.cell_datas[col, row];
        if (inputBuffer.Count < 6)
            inputBuffer.Add(new Vector3(cell.cell_transform.position.x, cell.cell_transform.position.y, selectionBox.position.z));
    }
}
