using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class MeshTreePainter : EditorWindow
{
    public GameObject[] treePrefabs;
    public int treeCount = 100;

    public float minScale = 0.8f;
    public float maxScale = 1.2f;

    public float brushSize = 5f;

    private Transform terrain;

    [MenuItem("Tools/Mesh Tree Painter")]
    public static void ShowWindow()
    {
        GetWindow<MeshTreePainter>("Tree Painter");
    }

    private void OnGUI()
    {
        GUILayout.Label("Mesh Tree Painter", EditorStyles.boldLabel);

        terrain = (Transform)EditorGUILayout.ObjectField(
            "Mesh Terrain",
            terrain,
            typeof(Transform),
            true
        );

        SerializedObject so = new SerializedObject(this);
        SerializedProperty prefabs =
            so.FindProperty("treePrefabs");

        EditorGUILayout.PropertyField(
            prefabs,
            new GUIContent("Tree Prefabs"),
            true
        );

        so.ApplyModifiedProperties();

        treeCount = EditorGUILayout.IntField(
            "Tree Count",
            treeCount
        );

        brushSize = EditorGUILayout.FloatField(
            "Brush Size",
            brushSize
        );

        minScale = EditorGUILayout.FloatField(
            "Min Scale",
            minScale
        );

        maxScale = EditorGUILayout.FloatField(
            "Max Scale",
            maxScale
        );

        if (GUILayout.Button("Generate Random Trees"))
        {
            GenerateTrees();
        }
    }

    void GenerateTrees()
    {
        if (terrain == null)
        {
            Debug.LogWarning("Chưa chọn Mesh Terrain!");
            return;
        }

        MeshCollider collider =
            terrain.GetComponent<MeshCollider>();

        if (collider == null)
        {
            collider = terrain.gameObject.AddComponent<MeshCollider>();
        }

        GameObject parent =
            new GameObject("Generated Trees");

        for (int i = 0; i < treeCount; i++)
        {
            Bounds bounds =
                collider.bounds;

            Vector3 origin = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                bounds.max.y + 10f,
                Random.Range(bounds.min.z, bounds.max.z)
            );

            Ray ray = new Ray(
                origin,
                Vector3.down
            );

            RaycastHit hit;

            if (collider.Raycast(
                ray,
                out hit,
                1000f))
            {
                GameObject prefab =
                    treePrefabs[
                        Random.Range(
                            0,
                            treePrefabs.Length
                        )
                    ];

                GameObject tree =
                    (GameObject)PrefabUtility.InstantiatePrefab(
                        prefab
                    );

                tree.transform.position =
                    hit.point;

                tree.transform.rotation =
                    Quaternion.Euler(
                        0,
                        Random.Range(0f, 360f),
                        0
                    );

                float scale =
                    Random.Range(
                        minScale,
                        maxScale
                    );

                tree.transform.localScale =
                    Vector3.one * scale;

                tree.transform.SetParent(
                    parent.transform
                );
            }
        }

        Undo.RegisterCreatedObjectUndo(
            parent,
            "Generate Trees"
        );
    }
}