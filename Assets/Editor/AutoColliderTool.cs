using UnityEngine;
using UnityEditor;

public class AutoColliderTool : EditorWindow
{
    private bool useConvex = true;
    private bool removeExisting = true;

    [MenuItem("Tools/Auto Add Colliders")]
    public static void ShowWindow()
    {
        GetWindow<AutoColliderTool>("Auto Colliders");
    }

    private void OnGUI()
    {
        GUILayout.Label("Añadir Colliders a selección", EditorStyles.boldLabel);
        useConvex = EditorGUILayout.Toggle("Convex (recomendado)", useConvex);
        removeExisting = EditorGUILayout.Toggle("Eliminar colliders previos", removeExisting);

        GUILayout.Space(10);

        if (GUILayout.Button("Añadir Mesh Colliders", GUILayout.Height(30)))
        {
            AddMeshColliders();
        }

        if (GUILayout.Button("Añadir Box Colliders (más rápido)", GUILayout.Height(30)))
        {
            AddBoxColliders();
        }

        if (GUILayout.Button("Eliminar TODOS los Colliders", GUILayout.Height(30)))
        {
            RemoveAllColliders();
        }

        GUILayout.Space(10);
        EditorGUILayout.HelpBox(
            "Mesh Collider: Adapta la forma exacta del modelo. Usa Convex para que funcione con CharacterControllers.\n\n" +
            "Box Collider: Un cubo simple. Mucho más eficiente para andenes, paredes, pilares.",
            MessageType.Info);
    }

    private void AddMeshColliders()
    {
        int count = 0;
        foreach (GameObject obj in Selection.gameObjects)
        {
            count += ProcessObject(obj, ColliderType.Mesh);
        }
        Debug.Log($"[AutoCollider] Se añadieron {count} Mesh Colliders.");
    }

    private void AddBoxColliders()
    {
        int count = 0;
        foreach (GameObject obj in Selection.gameObjects)
        {
            count += ProcessObject(obj, ColliderType.Box);
        }
        Debug.Log($"[AutoCollider] Se añadieron {count} Box Colliders.");
    }

    private int ProcessObject(GameObject obj, ColliderType type)
    {
        int count = 0;

        // Procesar el objeto actual y todos sus hijos
        foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
        {
            GameObject go = t.gameObject;

            // Solo objetos que tengan un MeshRenderer (son visibles)
            if (go.GetComponent<MeshRenderer>() == null)
                continue;

            // Saltar objetos con nombres de vías/riel si no quieres caminar ahí
            string lowerName = go.name.ToLower();
            if (lowerName.Contains("rail") || lowerName.Contains("track") || lowerName.Contains("rieles") || lowerName.Contains("via"))
            {
                Debug.Log($"[AutoCollider] Saltado (vías): {go.name}");
                continue;
            }

            if (removeExisting)
            {
                Collider[] existing = go.GetComponents<Collider>();
                foreach (var col in existing)
                    DestroyImmediate(col);
            }

            if (type == ColliderType.Mesh)
            {
                MeshFilter mf = go.GetComponent<MeshFilter>();
                if (mf != null && mf.sharedMesh != null)
                {
                    MeshCollider mc = go.AddComponent<MeshCollider>();
                    mc.sharedMesh = mf.sharedMesh;
                    mc.convex = useConvex;
                    count++;
                }
            }
            else if (type == ColliderType.Box)
            {
                go.AddComponent<BoxCollider>();
                count++;
            }
        }

        return count;
    }

    private void RemoveAllColliders()
    {
        int count = 0;
        foreach (GameObject obj in Selection.gameObjects)
        {
            foreach (Transform t in obj.GetComponentsInChildren<Transform>(true))
            {
                Collider[] cols = t.GetComponents<Collider>();
                foreach (var col in cols)
                {
                    DestroyImmediate(col);
                    count++;
                }
            }
        }
        Debug.Log($"[AutoCollider] Se eliminaron {count} colliders.");
    }

    private enum ColliderType { Mesh, Box }
}
