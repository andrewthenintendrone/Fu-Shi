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
    private GameObject prefab;

    private void Start()
    {
        if(texture != null)
        {
            for(int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    GameObject currentObject = Instantiate(prefab, new Vector3(x * distance, y * distance, 0), Quaternion.identity, transform);
                    currentObject.name = prefab.name + "_" + x.ToString() + "_" + y.ToString() + ")";

                    // set color
                    currentObject.GetComponentInChildren<SpriteRenderer>().color = texture.GetPixel(x, y);
                }
            }
        }
    }
}
