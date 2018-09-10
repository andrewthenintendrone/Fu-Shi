using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessing : MonoBehaviour
{
    [Tooltip("material to use for the effect")]
    public Material effectMaterial;

    private Transform playerTransform;

    private Camera cam;

    // called when this script is enabled
    private void OnEnable()
    {
        cam = GetComponent<Camera>();
    }

    // called on the first frame that this script exists
    void Start()
    {
        cam.depthTextureMode = DepthTextureMode.Depth;
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
    }

    // called at the last moment before blitting each frame to the screen
    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        // set _CameraPosition in the shader
        effectMaterial.SetVector("_CameraPosition", transform.position);

        // set _OrthoScale in the shader
        Vector2 canvasTextureOffset = transform.position / 16.0f;
        canvasTextureOffset.x = canvasTextureOffset.x / 16.0f * 9.0f * effectMaterial.GetTextureScale("_CanvasTex").x;
        canvasTextureOffset.y = canvasTextureOffset.y * effectMaterial.GetTextureScale("_CanvasTex").y;

        effectMaterial.SetTextureOffset("_CanvasTex", canvasTextureOffset);

        // set _PlayerPosition in the shader (screen space)
        effectMaterial.SetVector("_PlayerPosition", Camera.main.WorldToViewportPoint(playerTransform.position));

        // sample time control axis
        float timeAxis = Input.GetAxis("Time");

        // set _TimeWarpRadius in the shader
        effectMaterial.SetFloat("_TimeWarpRadius", timeAxis);

        // blit from render texture to the destination (the screen)
        Graphics.Blit(source, destination, effectMaterial);
    }
}
