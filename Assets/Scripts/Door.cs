using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    // has this door ever been opened
    [HideInInspector]
    public bool hasBeenOpened = false;

    // is this door currently open
    private bool m_isOpen = false;

    // is this door permenantly open
    [HideInInspector]
    public bool stuckOpen = false;

    // what time did this door last change states
    private float lastChangeTime = 0;

    // when isOpen is set it also sets hasBeenOpened and stores the time
    public bool isOpen
    {
        get { return m_isOpen; }
        set { m_isOpen = value; hasBeenOpened = true; lastChangeTime = Time.time; }
    }

    private Vector3 closedPosition;
    public Transform openPosition;

    private void Start()
    {
        // the position the door should be while closed is its current position
        closedPosition = transform.position;
    }

    void FixedUpdate ()
    {
        if(!Utils.gamePaused)
        {
            // if the door is not permenantly opened and has finished opening
            if (!(stuckOpen && transform.position == openPosition.position))
            {
                // opening
                if (m_isOpen || stuckOpen)
                {
                    transform.position = Vector3.Lerp(transform.position, openPosition.position, Time.time - lastChangeTime);
                }
                // closing
                else
                {
                    transform.position = Vector3.Lerp(transform.position, closedPosition, Time.time - lastChangeTime);
                }
            }
        }
	}
}
