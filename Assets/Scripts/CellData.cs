using TMPro;
using UnityEngine;

public class CellData : MonoBehaviour
{

    string contents = "";
    public RectTransform cell_transform;
    public TextMeshPro text_plane;

    // References to bomb and enemy GameObjects
    public GameObject bomb;
    public GameObject enemy;
    private bool bombEnabled = false;
    private bool enemyEnabled = false;
    public bool IsBombEnabled() { return bombEnabled; }
    public bool IsEnemyEnabled() { return enemyEnabled; }

    // Make getters for row and col
    public int row;
    public int col;
    public int Row() { return row; }
    public int Col() { return col; }

    void Awake()
    {
        cell_transform = this.GetComponent<RectTransform>();
        text_plane = this.GetComponentInChildren<TextMeshPro>();
        contents = text_plane.text;
        //bomb = this.transform.Find("Bomb").gameObject;
        //enemy = this.transform.Find("Enemy").gameObject;
    }

    public void SetCoordinates(int r, int c)
    {
        row = r;
        col = c;
        print("Set cell coordinates to: " + row + ", " + col);
    }


    public void update_text(string text)
    {
        text_plane.text = text;
        contents = text;
    }

    public string get_text()
    {
        return contents;
    }

    public void SetBomb(bool enabled)
    {
        bombEnabled = enabled;
        if (bomb != null)
            bomb.SetActive(enabled);
    }

    public void SetEnemy(bool enabled)
    {
        enemyEnabled = enabled;
        if (enemy != null)
            enemy.SetActive(enabled);
    }
}