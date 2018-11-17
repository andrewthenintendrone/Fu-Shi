using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class TriggerAppear : MonoBehaviour
{
    #region variables

    [SerializeField]
    [Tooltip("distance to be completely invisible")]
    private float transparentDistance;

    [SerializeField]
    [Tooltip("distance to be completely opaque")]
    private float opaqueDistance;

    [SerializeField]
    [Tooltip("time it takes to switch between images")]
    private float changeTime;

    [SerializeField]
    [Tooltip("sprites to change between")]
    private List<Sprite> images;

    [SerializeField]
    private Color color;

    // used to cycle through images
    private int currentImageIndex = 0;

    #endregion

    #region monobehavior

    private void Start()
    {
        color = GetComponent<Renderer>().material.color;

        if (images.Count > 0)
        {
            InvokeRepeating("changeImage", changeTime, changeTime);
        }
    }

    private void Update()
    {
        float distToPlayer = Vector2.Distance(Utils.getPlayer().transform.position, transform.position);

        color.a = 1 - Mathf.Clamp01((distToPlayer - opaqueDistance) / (transparentDistance - opaqueDistance));

        GetComponent<Renderer>().material.color = color;
    }

    #endregion

    private void changeImage()
    {
        currentImageIndex = (currentImageIndex + 1) % images.Count;
        GetComponent<SpriteRenderer>().sprite = images[currentImageIndex];
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
