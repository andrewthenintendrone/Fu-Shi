using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeAbilityObject : MonoBehaviour
{
    [Tooltip("range of the time reverse ability (also max radius)")]
    [SerializeField]
    private float timeRadius;

    // list of the objects that have currently been hit by the time reverse ability
    private RaycastHit2D[] hits = new RaycastHit2D[100];

    private ContactFilter2D filter;

    [SerializeField]
    [Tooltip("how the object grows and shrinks over its lifetime")]
    private AnimationCurve lifeTimeCurve;

    [SerializeField]
    [Tooltip("how long the objects lifetime is")]
    private float lifeTime;

    // time that the object was created
    private float startTime;

    // current scale of the object
    private float currentScale;

    void Start()
    {
        startTime = Time.time;

        // only hit solid objects with the time reverse ability
        filter.layerMask = 1 << LayerMask.NameToLayer("Solid");
    }

    private void Update()
    {
        doReverseCheck();

        // time since the object was created
        float currentTime = Time.time - startTime;

        // move into the range 0-1
        float normalizedTime = currentTime / lifeTime;

        currentScale = lifeTimeCurve.Evaluate(normalizedTime) * timeRadius;

        transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        if (normalizedTime >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    // circle cast and reverse objects
    private void doReverseCheck()
    {
        int numHits = Physics2D.CircleCast(transform.position, currentScale, Vector2.zero, new ContactFilter2D(), hits, Mathf.Infinity);
        bool hasReversed = false;
        for (int i = 0; i < numHits && !hasReversed; i++)
        {
            if (hits[i].collider.gameObject.GetComponentInParent<patrolmove>() != null)
            {
                hits[i].collider.gameObject.GetComponentInParent<patrolmove>().reverse();
                hasReversed = true;
            }
            if (hits[i].collider.gameObject.GetComponentInParent<enemyProjectile>() != null)
            {
                hits[i].collider.gameObject.GetComponentInParent<enemyProjectile>().Reverse();
            }
            if (hits[i].collider.gameObject.GetComponent<Door>() != null)
            {
                if (hits[i].collider.gameObject.GetComponent<Door>().hasBeenOpened)
                {
                    hits[i].collider.gameObject.GetComponent<Door>().isOpen = true;
                    hits[i].collider.gameObject.GetComponent<Door>().stuckOpen = true;
                    hasReversed = true;
                }
            }
        }
    }

#if UNITY_EDITOR

    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.cyan;
        UnityEditor.Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, timeRadius);
    }

#endif
}
