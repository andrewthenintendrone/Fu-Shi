using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class replaceLeaves : MonoBehaviour
{
    public GameObject prefabLeaf;

    private List<GameObject> leaves = new List<GameObject>();

    private Vector3 offset;

	void Start ()
    {
        offset = new Vector3(-0.91f, 1.99f, 0);

        // find all the leaves
		foreach(inkableSurface i in FindObjectsOfType<inkableSurface>())
        {
            leaves.Add(i.gameObject.transform.parent.gameObject);
        }

        foreach(GameObject currentLeaf in leaves)
        {
            GameObject newLeaf = Instantiate(prefabLeaf, currentLeaf.transform.parent);
            newLeaf.name = currentLeaf.name;

            // copy transform
            newLeaf.transform.eulerAngles = currentLeaf.transform.eulerAngles;
            newLeaf.transform.position = currentLeaf.transform.position + new Vector3(offset.x * currentLeaf.transform.localScale.x, offset.y * currentLeaf.transform.localScale.y, 0);
            newLeaf.transform.localScale = currentLeaf.transform.localScale;

            // copy inked status
            newLeaf.GetComponentInChildren<inkableSurface>().Inked = currentLeaf.GetComponentInChildren<inkableSurface>().Inked;

            // delete old leaf
            Destroy(currentLeaf);
        }
	}
}
