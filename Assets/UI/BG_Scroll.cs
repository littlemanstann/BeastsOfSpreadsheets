using UnityEngine;

public class BG_Scroll : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    [Range(-1f,1f)]
    public float scrollSpeed = 0.5f;

    private Material mat;
    private float offset;

    void Start()
    {
        mat = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        offset += (Time.deltaTime * scrollSpeed) / 10f;
        mat.SetTextureOffset("_MainTex", new Vector2(offset, 0));
    }
}
