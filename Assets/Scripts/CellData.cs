using TMPro;
using UnityEngine;

public class CellData : MonoBehaviour
{

    string contents = "---";
    public RectTransform cell_transform;
    public TextMeshPro text_plane;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //update_text(contents);
    }

    // Update is called once per frame
    void Update()
    {
        
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


}
