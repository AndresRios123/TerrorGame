using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering.Universal;
using System.Reflection;

public class TetricBlurAutoSetup : MonoBehaviour
{
    [MenuItem("Tools/Setup Tetric Blur")]
    static void Setup()
    {
        // 1. Verificar shader
        Shader shader = Shader.Find("Custom/TetricBlurShader");
        if (shader == null)
        {
            EditorUtility.DisplayDialog("Error", "No se encontro el shader 'Custom/TetricBlurShader'. Asegurate de que Assets/Shaders/TetricBlurShader.shader existe y que no hay errores rojos en la consola.", "OK");
            return;
        }

        // 2. Crear carpeta y material
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        string materialPath = "Assets/Materials/TetricBlurMaterial.mat";
        Material existingMat = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (existingMat != null)
        {
            AssetDatabase.DeleteAsset(materialPath);
        }

        Material blurMaterial = new Material(shader);
        blurMaterial.SetFloat("_Intensity", 0.3f);
        AssetDatabase.CreateAsset(blurMaterial, materialPath);
        AssetDatabase.SaveAssets();

        // 3. Configurar PC_Renderer
        string[] rendererGuids = AssetDatabase.FindAssets("t:ScriptableRendererData", new[] { "Assets" });
        ScriptableRendererData targetRenderer = null;
        string targetRendererPath = "";

        foreach (string guid in rendererGuids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (path.Contains("PC_Renderer") || path.Contains("Renderer"))
            {
                ScriptableRendererData rd = AssetDatabase.LoadAssetAtPath<ScriptableRendererData>(path);
                if (rd != null)
                {
                    targetRenderer = rd;
                    targetRendererPath = path;
                    break;
                }
            }
        }

        if (targetRenderer == null)
        {
            EditorUtility.DisplayDialog("Error", "No se encontro ningun Renderer Data (PC_Renderer.asset). Busca manualmente en Assets/Settings/.", "OK");
            return;
        }

        // Verificar si ya existe el feature
        bool alreadyExists = false;
        foreach (var feature in targetRenderer.rendererFeatures)
        {
            if (feature != null && feature.GetType().Name == "FullscreenBlurFeature")
            {
                alreadyExists = true;
                // Actualizar settings
                var so = new SerializedObject(feature);
                so.FindProperty("settings.blurMaterial").objectReferenceValue = blurMaterial;
                so.FindProperty("settings.intensity").floatValue = 0.3f;
                so.FindProperty("settings.enable").boolValue = true;
                so.ApplyModifiedProperties();
                break;
            }
        }

        if (!alreadyExists)
        {
            FullscreenBlurFeature newFeature = ScriptableObject.CreateInstance<FullscreenBlurFeature>();
            newFeature.name = "FullscreenBlurFeature";
            newFeature.settings = new FullscreenBlurFeature.BlurSettings
            {
                enable = true,
                intensity = 0.3f,
                blurMaterial = blurMaterial
            };

            AssetDatabase.AddObjectToAsset(newFeature, targetRenderer);
            targetRenderer.rendererFeatures.Add(newFeature);
        }

        EditorUtility.SetDirty(targetRenderer);
        AssetDatabase.SaveAssets();

        // 4. Seleccionar el material para que lo vea el usuario
        Selection.activeObject = blurMaterial;
        EditorGUIUtility.PingObject(blurMaterial);

        EditorUtility.DisplayDialog("Listo!", "Todo configurado automaticamente.\n\nRenderer: " + targetRendererPath + "\nMaterial: Assets/Materials/TetricBlurMaterial.mat\n\nAhora solo presiona PLAY para probar el blur.\n\nSi aun no se ve:\n1. Revisa que no haya errores ROJOS en la consola (Ctrl+Shift+C)\n2. Sube el valor de Intensity a 0.8 para probar si es muy sutil.", "OK");
    }
}
