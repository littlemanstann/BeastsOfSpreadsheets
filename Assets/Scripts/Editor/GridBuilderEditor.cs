using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GridBuilder))]
public class GridBuilderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        GridBuilder gridBuilder = (GridBuilder)target;

        if (GUILayout.Button("Generate Grid"))
        {
            gridBuilder.GenerateGridInEditor();
        }
    }
}
