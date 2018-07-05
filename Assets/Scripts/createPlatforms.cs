using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class createPlatforms : MonoBehaviour
{
    [Tooltip("Platform to use")]
    public GameObject platformPrefab;

    [Tooltip("offset of each platform")]
    public Vector2 offset;

    [Tooltip("number of platforms to create")]
    public int platformCount;

	void Start ()
    {
		for(int i = 0; i < platformCount; i++)
        {
            // create new platform and parent it to this
            GameObject currentPlatform = Instantiate(platformPrefab, this.gameObject.transform);

            // move by offset
            currentPlatform.transform.localPosition = offset * i;

            // give it a name
            currentPlatform.name = "platform_" + i;
        }
	}
}
