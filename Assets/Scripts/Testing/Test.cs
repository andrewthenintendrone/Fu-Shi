using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    [SerializeField]
    private Texture2D texture;

    [SerializeField]
    private float distance;

    [SerializeField]
    private float detail;

    [SerializeField]
    private GameObject prefab;

    private void Start()
    {
        if(texture != null)
        {
            for(int x = 0; x < texture.width / detail; x++)
            {
                for (int y = 0; y < texture.height / detail; y++)
                {
                    if(texture.GetPixel(x * (int)detail, y * (int)detail).a > 0.5f)
                    {
                        GameObject currentObject = Instantiate(prefab, new Vector3(x * distance, y * distance, 0), Quaternion.identity, transform);
                        currentObject.name = prefab.name + "_" + x.ToString() + "_" + y.ToString() + ")";

                        // set color
                        currentObject.GetComponentInChildren<SpriteRenderer>().color = texture.GetPixel(x * (int)detail, y * (int)detail);
                    }
                }
            }
        }
    }
}
