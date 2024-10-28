using UnityEngine;

[ExecuteAlways]
public class ApplyMaterial : MonoBehaviour
{
    public Material material;
    private static readonly int ObjectIdShaderID = Shader.PropertyToID("_MyObjectId");
    private static readonly int ColorShaderID = Shader.PropertyToID("_Color");
    private static readonly int MyObjectCountShaderID = Shader.PropertyToID("_MyObjectCount");

    private void OnEnable()
    {
        var renderers = FindObjectsOfType<Renderer>();
        for (var i = 0; i < renderers.Length; i++)
        {
            var currRenderer = renderers[i];
            currRenderer.material = material;
            var block = new MaterialPropertyBlock();
            block.SetInt(ObjectIdShaderID, i + 1);

            var color = Color.HSVToRGB(i * 1.0f / renderers.Length, 1.0f, 1.0f);
            if (renderers[i].gameObject.name == "Plane") color = Color.white;
            block.SetColor(ColorShaderID, color);
            currRenderer.SetPropertyBlock(block);
        }
        Shader.SetGlobalInt(MyObjectCountShaderID, renderers.Length);
    }
}