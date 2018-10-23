using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

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
        float distToPlayer = Vector2.Distance(Utils.getPlayer().transform.position, transform.position);

        color.a = 1 - Mathf.Clamp01((distToPlayer - opaqueDistance) / (transparentDistance - opaqueDistance));

        GetComponent<Renderer>().material.color = color;
    }

#if UNITY_EDITOR

    void OnDrawGizmosSelected()
    {
        Handles.color = Color.red;
        Handles.DrawWireDisc(transform.position, -Vector3.forward, transparentDistance);

        Handles.color = Color.yellow;
        Handles.DrawWireDisc(transform.position, -Vector3.forward, opaqueDistance);
    }

#endif
}
