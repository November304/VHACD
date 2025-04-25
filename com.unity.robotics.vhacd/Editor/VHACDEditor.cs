using UnityEditor;
using UnityEngine;
using MeshProcess; // Make sure this namespace matches your VHACD script

public class VHACDEditorWindow : EditorWindow
{
    private GameObject targetObject;
    private VHACD vhacdComponent;

    [MenuItem("Tools/VHACD Generator")]
    public static void ShowWindow()
    {
        GetWindow<VHACDEditorWindow>("VHACD Generator");
    }

    void OnGUI()
    {
        GUILayout.Label("VHACD Convex Decomposition", EditorStyles.boldLabel);

        targetObject = (GameObject)EditorGUILayout.ObjectField("Target GameObject", targetObject, typeof(GameObject), true);

        if (targetObject != null)
        {
            if (GUILayout.Button("Generate Convex Meshes"))
            {
                GenerateConvexMeshes();
            }
        }
        else
        {
            EditorGUILayout.HelpBox("Please assign a GameObject with a MeshFilter.", MessageType.Info);
        }
    }

    void GenerateConvexMeshes()
    {
        var meshFilter = targetObject.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("Selected object has no MeshFilter.");
            return;
        }

        vhacdComponent = targetObject.GetComponent<VHACD>();
        if (vhacdComponent == null)
        {
            Debug.LogError("Selected object has no VHACD component.");
            return;
        }

        var resultMeshes = vhacdComponent.GenerateConvexMeshes();

        for (int i = 0; i < resultMeshes.Count; i++)
        {
            var go = new GameObject($"ConvexHull_{i}");
            go.transform.SetParent(targetObject.transform);
            go.transform.localPosition = Vector3.zero;
            go.transform.localRotation = Quaternion.identity;
            go.transform.localScale = Vector3.one;

            var mf = go.AddComponent<MeshFilter>();
            var mr = go.AddComponent<MeshRenderer>();
            var mc = go.AddComponent<MeshCollider>();

            mf.sharedMesh = resultMeshes[i];
            mr.sharedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            mc.sharedMesh = resultMeshes[i];
            mc.convex = true;
        }

        Debug.Log($"VHACD generated {resultMeshes.Count} convex parts.");
    }
}
