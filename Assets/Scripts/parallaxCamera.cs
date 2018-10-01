using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class parallaxCamera : MonoBehaviour
{
    // parallax material
    private Material parallaxMaterial;

    [SerializeField]
    [Tooltip("how fast to scroll layer 1")]
    private Vector2 layerScrollSpeed1;
    [SerializeField]
    [Tooltip("how fast to scroll layer 2")]
    private Vector2 layerScrollSpeed2;
    [SerializeField]
    [Tooltip("how fast to scroll layer 3")]
    private Vector2 layerScrollSpeed3;

    void Start ()
    {
	    if(GetComponentInChildren<Renderer>() != null)
        {
            parallaxMaterial = GetComponentInChildren<Renderer>().material;
        }
	}
	
	void FixedUpdate ()
    {
        if(parallaxMaterial != null)
        {
            // adjust texture offsets in the shader
            parallaxMaterial.SetTextureOffset("_Texture1", (Vector3.right * transform.position.x * layerScrollSpeed1.x + Vector3.up * transform.position.y * layerScrollSpeed1.y));
            parallaxMaterial.SetTextureOffset("_Texture2", (Vector3.right * transform.position.x * layerScrollSpeed2.x + Vector3.up * transform.position.y * layerScrollSpeed2.y));
            parallaxMaterial.SetTextureOffset("_Texture3", (Vector3.right * transform.position.x * layerScrollSpeed3.x + Vector3.up * transform.position.y * layerScrollSpeed3.y));
        }
    }
}
