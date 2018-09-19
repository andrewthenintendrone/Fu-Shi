using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeAbilityObject : MonoBehaviour
{

    [SerializeField]
    [Tooltip("how the object grows and shrinks over its lifetime")]
    private AnimationCurve lifeTimeCurve;

    [SerializeField]
    [Tooltip("how long the objects lifetime is")]
    private float lifeTime;

    // time that the object was created
    private float startTime;

    void Start()
    {
        startTime = Time.time;
    }

    private void Update()
    {
        doReverseCheck();

        // time since the object was created
        float currentTime = Time.time - startTime;

        // move into the range 0-1
        float normalizedTime = currentTime / lifeTime;

        float currentScale = lifeTimeCurve.Evaluate(normalizedTime);

        transform.localScale = new Vector3(currentScale, currentScale, currentScale);

        if (normalizedTime == lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void doReverseCheck()
    {
        
    }
}
