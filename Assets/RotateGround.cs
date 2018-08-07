using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateGround : MonoBehaviour
{
    public float rotationSpeed = 5.0f;

	// Update is called once per frame
	void Update ()
    {
        transform.Rotate(Vector3.forward * rotationSpeed * Input.GetAxis("Vertical"));
	}
}
