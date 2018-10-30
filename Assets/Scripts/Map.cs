﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class Map : MonoBehaviour
{
    [SerializeField]
    [Tooltip("map camera position for this room.")]
    private Vector2 cameraPosition;

    [SerializeField]
    [Tooltip("map camera orthographic scale for this room. (zoom)")]
    private float cameraOrthoScale;

    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.tag == "Player")
        {
            foreach (Transform child in GetComponentsInChildren<Transform>())
            {
                if(child != transform)
                {
                    child.gameObject.SetActive(true);
                }
            }
            GameObject.Find("MapCam").transform.position = new Vector3(transform.position.x + cameraPosition.x, transform.position.y + cameraPosition.y, -10);
            GameObject.Find("MapCam").GetComponent<Camera>().orthographicSize = Mathf.Floor(cameraOrthoScale) / 2;
        }
    }

    private void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "Player")
        {
            foreach (Transform child in GetComponentsInChildren<Transform>())
            {
                if (child != transform)
                {
                    child.gameObject.SetActive(false);
                }
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(new Vector3(transform.position.x + cameraPosition.x, transform.position.y + cameraPosition.y, -10), new Vector3(cameraOrthoScale, cameraOrthoScale, 0.1f));
    }

#endif
}
