using Unity.Loading;
using UnityEngine;


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

    void Awake()
    {
        if (gridParent.childCount == 0)
            GenerateGrid();
        else {
            Debug.Log("Initializing existing grid cells.");
            InitializeCellData();
        }
    }

    public void GenerateGrid()
    {
        cell_datas = new CellData[cols, rows];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                //make the cell data


                //make the cell prefab object
                var new_cell = Instantiate(cellPrefab, gridParent);
                var new_cell_data = new_cell.GetComponent<CellData>();

                // rename the cell
                new_cell.name = $"Cell_{r}_{c}";

                //set the text content
                string[] test = { "gonner eli", "test", "junk" }; //testing junk
                new_cell_data.update_text(test[Random.Range(0, test.Length)]);

                // set the coordinates
                new_cell_data.SetCoordinates(r, c);

                //set the transformations
                var cell_transform = new_cell.GetComponent<RectTransform>();
                cell_transform.anchoredPosition = new Vector2(r * cellWidth, -c * cellHeight);
                new_cell_data.cell_transform = cell_transform;

                //push it to da array
                cell_datas[c, r] = new_cell_data;
            }
        }
    }

    private void InitializeCellData()
    {
        // Load existing cells into the array
        cell_datas = new CellData[cols, rows];

        for (int r = 0; r < rows; r++)
        {
            for (int c = 0; c < cols; c++)
            {
                // Assign the cell data using the already generated grid by name
                Debug.Log($"Assigned cell at position {r},{c} to cell_datas array.");
                cell_datas[c, r] = transform.Find($"Cell_{r}_{c}").GetComponent<CellData>();
            }
        }
    }

#if UNITY_EDITOR
    public void GenerateGridInEditor()
    {
        // Clear existing children
        for (int i = gridParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(gridParent.GetChild(i).gameObject);
        }

        // Generate new grid
        GenerateGrid();
    }
#endif
}