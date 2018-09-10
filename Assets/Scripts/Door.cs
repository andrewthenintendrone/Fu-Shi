using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    [HideInInspector]
    public bool hasBeenOpened = false;
    private bool m_isOpen = false;
    [HideInInspector]
    public bool stuckOpen = false;
    private float lastChangeTime = 0;

    public bool isOpen
    {
        get { return m_isOpen; }
        set { m_isOpen = value; hasBeenOpened = true; lastChangeTime = Time.time; }
    }

    private Vector3 closedPosition;
    public Transform openPosition;

    private void Start()
    {
        closedPosition = transform.position;
    }

    void FixedUpdate ()
    {
        if(!(stuckOpen && transform.position == openPosition.position))
        {
            if (m_isOpen || stuckOpen)
            {
                transform.position = Vector3.Lerp(transform.position, openPosition.position, Time.time - lastChangeTime);
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, closedPosition, Time.time - lastChangeTime);
            }
        }
	}
}
