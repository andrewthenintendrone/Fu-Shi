using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class PostProcessing : MonoBehaviour
{
    [Tooltip("material to use for the effect")]
    public Material effectMaterial;

    private Transform playerTransform;

    // called when this script is enabled
    private void OnEnable()
    {
        
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
        // set _CameraPosition the shader
        effectMaterial.SetVector("_CameraPosition", transform.position);

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
