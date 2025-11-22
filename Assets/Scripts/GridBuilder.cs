using Unity.Loading;
using UnityEngine;


public class CellData
{
    public string contents = "---";
    public GameObject cell_prefab;
    public RectTransform cell_transform;
}

public class GridBuilder : MonoBehaviour
{
    public RectTransform gridParent;
    public GameObject cellPrefab;
    

    public int rows = 2;
    public int cols = 1;
    public float cellWidth = 100f;
    public float cellHeight = 30f;

   // public RectTransform[,] cellRects;

    public CellData[,] cell_datas;

    void Start()
    {
        //cellRects = new RectTransform[cols, rows];
        cell_datas = new CellData[cols, rows];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                //make the cell data
                var new_cell_data = new CellData();

                //set the text content
                string[] test = { "goon", "test", "p" }; //testing junk
                new_cell_data.contents = test[c % 3];

                //make the cell sprite object
                var cell = Instantiate(cellPrefab, gridParent);
                new_cell_data.cell_prefab = cell;

                //set the transformations
                var cell_transform = cell.GetComponent<RectTransform>();
                cell_transform.anchoredPosition = new Vector2(r * cellWidth, -c * cellHeight);
                cell_transform.sizeDelta = new Vector2(cellWidth, cellHeight);
                new_cell_data.cell_transform = cell_transform;

                //push it to da array
                cell_datas[c, r] = new_cell_data;
            }
        }
    }
}