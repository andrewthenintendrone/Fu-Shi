using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Abilityactivator : MonoBehaviour {

    public float inkRadius;
    public float timeRadius;
    private RaycastHit2D[] hits = new RaycastHit2D[100];
    private ContactFilter2D filter;

	// Use this for initialization
	void Start ()
    {
        filter.layerMask = 1 << LayerMask.NameToLayer("Solid");
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            int numHits = Physics2D.CircleCast(transform.position, inkRadius, Vector2.zero, filter, hits, Mathf.Infinity);

            for(int i = 0; i < numHits; i++)
            {

                Debug.Log(hits[i].collider.gameObject.name);
                if( hits[i].collider.gameObject.GetComponent<inkableSurface>() != null)
                {
                    hits[i].collider.gameObject.GetComponent<inkableSurface>().Inked = true;
                }
            }
        }
	}

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.red;
        UnityEditor.Handles.DrawWireDisc(gameObject.transform.position, Vector3.forward, inkRadius);
    }
#endif
}
