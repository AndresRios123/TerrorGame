using UnityEngine;
using UnityEditor;

public class SetupTetricBlur : MonoBehaviour
{
    [MenuItem("Tools/Setup Tetric Blur")]
    static void CreateMaterialAndSetup()
    {
        // 1. Buscar el shader
        Shader blurShader = Shader.Find("Custom/TetricBlurShader");
        if (blurShader == null)
        {
            EditorUtility.DisplayDialog("Error", "No se encontro el shader. Asegurate de que el archivo TetricBlurShader.shader este en Assets/Shaders/ y que Unity haya terminado de importar (sin errores rojos en la consola).", "OK");
            return;
        }

        if (!blurShader.isSupported)
        {
            EditorUtility.DisplayDialog("Error", "El shader se encontro pero no es compatible con tu pipeline. Revisa la consola por errores de compilacion del shader.", "OK");
            return;
        }

        // 2. Crear la carpeta Materials si no existe
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
        {
            AssetDatabase.CreateFolder("Assets", "Materials");
        }

        // 3. Crear el material
        string materialPath = "Assets/Materials/TetricBlurMaterial.mat";

        // Borrar el anterior si existe
        Material existing = AssetDatabase.LoadAssetAtPath<Material>(materialPath);
        if (existing != null)
        {
            AssetDatabase.DeleteAsset(materialPath);
        }

        Material blurMaterial = new Material(blurShader);
        blurMaterial.SetFloat("_Intensity", 0.3f);
        blurMaterial.hideFlags = HideFlags.None;

        AssetDatabase.CreateAsset(blurMaterial, materialPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        // 4. Seleccionar el material para que lo vea el usuario
        Selection.activeObject = blurMaterial;
        EditorGUIUtility.PingObject(blurMaterial);

        EditorUtility.DisplayDialog("Listo", "Material creado correctamente.\n\nPASOS SIGUIENTES:\n1. Ve a Assets/Settings/PC_Renderer\n2. En Renderer Features, haz click en 'Add Renderer Feature'\n3. Elige 'FullscreenBlurFeature'\n4. Arrastra 'TetricBlurMaterial' al campo 'Blur Material'\n5. Presiona Play para probar.\n\nSi aun no ves el efecto, revisa la consola (Ctrl+Shift+C) por mensajes de TetricBlur.", "OK");
    }
}
