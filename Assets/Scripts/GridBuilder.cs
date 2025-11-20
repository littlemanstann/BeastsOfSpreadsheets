using UnityEngine;

public class GridBuilder : MonoBehaviour
{
    public RectTransform gridParent;
    public GameObject cellPrefab;

    public int rows = 2;
    public int cols = 1;
    public float cellWidth = 100f;
    public float cellHeight = 30f;

    public RectTransform[,] cellRects;

    void Start()
    {
        cellRects = new RectTransform[cols, rows];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                var cell = Instantiate(cellPrefab, gridParent).GetComponent<RectTransform>();
                cell.anchoredPosition = new Vector2(r * cellWidth, -c * cellHeight);
                cell.sizeDelta = new Vector2(cellWidth, cellHeight);

                cellRects[c, r] = cell;
            }
        }
    }
}