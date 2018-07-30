using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessing : MonoBehaviour
{
    public Material effectMaterial;

    [Range(0, 10)]
    public int Iterations;
    [Range(0, 4)]
    public int DownRes;

    // gradient testing
    public Gradient shaderGradient;
    public Texture2D gradTex;

    private void OnEnable()
    {
        // create gradient texture
        gradTex = new Texture2D(256, 1);

        List<Color> gradColors = new List<Color>();

        for (float i = 0.0f; i < 1.0f; i += 1.0f / 256.0f)
        {
            gradColors.Add(shaderGradient.Evaluate(i));
        }

        gradTex.SetPixels(gradColors.ToArray());

        effectMaterial.SetTexture("_GradientTexture", gradTex);
    }

    void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        effectMaterial.SetFloat("_camX", transform.position.x);
        effectMaterial.SetFloat("_camY", transform.position.y);

        int width = source.width >> DownRes;
        int height = source.height >> DownRes;

        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, rt);

        for(int i = 0; i < Iterations; i++)
        {
            RenderTexture rt2 = RenderTexture.GetTemporary(width, height);
            Graphics.Blit(rt, rt2, effectMaterial);
            RenderTexture.ReleaseTemporary(rt);
            rt = rt2;
        }

        Graphics.Blit(rt, destination);
        RenderTexture.ReleaseTemporary(rt);
    }
}
