using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class inkableSurface : MonoBehaviour
{
    // the current state of the platform
    public bool Inked
    {
        get
        {
            return inked;
        }
        set
        {
            inked = value;
            updateMaterial();
        }
    }

    [SerializeField]
    private bool inked = false;

    [Tooltip("the ink coated version of the material")]
    public Material inkedSurface;
    public Material cleanSurface;

    void OnValidate()
    {
        Inked = inked;
    }

    public void updateMaterial()
    {
        Renderer renderer = gameObject.GetComponent<Renderer>();

        renderer.material = inked ? inkedSurface : cleanSurface;
    }
}

