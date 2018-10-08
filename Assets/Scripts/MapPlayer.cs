using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapPlayer : MonoBehaviour
{
    // actual position of the player gameobject
    private Transform actualPlayerPosition;

    // where does the player start
    public Vector3 playerStartPosition;

    public float scale;

    private void Awake()
    {
        actualPlayerPosition = GameObject.FindGameObjectWithTag("Player").transform;
    }

    private void OnEnable()
    {
        Vector3 actualPlayerOffset = actualPlayerPosition.position - playerStartPosition;

        GetComponent<RectTransform>().localPosition = Vector3.right * 10;
    }
}
