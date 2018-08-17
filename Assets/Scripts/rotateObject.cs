using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rotateObject : MonoBehaviour {

    // Use this for initialization
    [Tooltip("Rotation rate of an object per physics step,negative numbers will inverse direction")]
    public float rotationRate = 1;

	
	// Update is called once per frame
	void FixedUpdate ()
    {
        transform.Rotate(0, 0, rotationRate);
	}
}
