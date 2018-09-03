using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessing : MonoBehaviour
{
    [Tooltip("material to use for the effect")]
    public Material effectMaterial;

    [Tooltip("how many times to apply the effect")]
    [Range(0, 10)]
    public int Iterations;

    [Tooltip("how many times the image should be downscaled before applying the effect")]
    [Range(0, 4)]
    public int DownRes;

    [Tooltip("gradient used for adjusting contrast between black and white")]
    public Gradient shaderGradient;

    // look up texture created from shaderGradient
    public Texture2D gradTex;

    private Transform playerTransform;

    // called when this script is enabled
    private void OnEnable()
    {
        // create gradient texture at 256x1
        gradTex = new Texture2D(256, 1, TextureFormat.RGB24, false);
        gradTex.filterMode = FilterMode.Point;
        gradTex.wrapMode = TextureWrapMode.Clamp;
        gradTex.anisoLevel = 0;

        // create a new list of colors for the gradient texture
        List<Color> gradColors = new List<Color>();

        // iterate through the gradient and add evaluated colors to the list
        for (float i = 0.0f; i < 1.0f; i += 1.0f / 256.0f)
        {
            gradColors.Add(shaderGradient.Evaluate(i));
        }

        // set the pixels of the gradient texture
        gradTex.SetPixels(gradColors.ToArray());

        gradTex.Apply();

        // set the shader uniform for the material
        effectMaterial.SetTexture("_GradientTexture", gradTex);
    }

    // called on the first frame that this script exists
    void Start()
    {
        GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // called at the last moment before blitting each frame to the screen
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // set _camX and _camY in the shader
        effectMaterial.SetVector("_CameraPosition", transform.position);

        effectMaterial.SetVector("_PlayerPosition", Camera.main.WorldToViewportPoint(playerTransform.position));

        float timeAxis = Input.GetAxis("Time");

        effectMaterial.SetFloat("_EffectDistance", timeAxis);

        // downscale the image (if required)
        int width = source.width >> DownRes;
        int height = source.height >> DownRes;

        // create a temporary RenderTexture and blit the source to it
        RenderTexture rt = RenderTexture.GetTemporary(width, height);
        Graphics.Blit(source, rt);

        // repeat for number of iterations
        for(int i = 0; i < Iterations; i++)
        {
            // create a temporary RenderTexture at the right size
            RenderTexture rt2 = RenderTexture.GetTemporary(width, height);

            // blit from rt to rt2 using the effect material
            Graphics.Blit(rt, rt2, effectMaterial);

            // release rt
            RenderTexture.ReleaseTemporary(rt);

            // set rt to rt2 for next iteration
            rt = rt2;
        }

        // finally blit from rt to the destination (the screen)
        Graphics.Blit(rt, destination);

        // release rt
        RenderTexture.ReleaseTemporary(rt);
    }
}
