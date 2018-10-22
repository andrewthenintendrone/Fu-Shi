using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerAppear : MonoBehaviour
{
    [SerializeField]
    [Tooltip("distance to be completely invisible")]
    private float transparentDistance;

    [SerializeField]
    [Tooltip("distance to be completely opaque")]
    private float opaqueDistance;

    [SerializeField]
    private Color color;

    private void Start()
    {
        color = GetComponent<Renderer>().material.color;
    }

    private void Update()
    {
        float distToPlayer = Vector3.Magnitude(Utils.getPlayer().transform.position - transform.position);

        color.a = 1 - Mathf.Clamp01((distToPlayer - opaqueDistance) / (transparentDistance - opaqueDistance));

        GetComponent<Renderer>().material.color = color;
    }
}
