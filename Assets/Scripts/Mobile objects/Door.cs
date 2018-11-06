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

    [SerializeField]
    private float speed = 4;
    [SerializeField]
    private float shakeAmt = 1;
    [SerializeField]
    private float shakeSpd = 1;

   
    // is this door permenantly open
    [HideInInspector]
    public bool stuckOpen = false;


    // when isOpen is set it also sets hasBeenOpened and stores the time
    public bool isOpen
    {
        get { return m_isOpen; }
        set { m_isOpen = value; hasBeenOpened = true; }
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
            if (!(stuckOpen && Mathf.Abs(transform.position.y - openPosition.position.y) < 0.1))
            {
                

                // opening
                if (m_isOpen || stuckOpen)
                {
                    
                    transform.position = Vector3.MoveTowards(transform.position, openPosition.position, speed * Time.fixedDeltaTime);

                    if(isMoving())
                    {
                        transform.position += Vector3.right * Mathf.Sin(Time.time * shakeSpd) * shakeAmt;
                    }
                }
                // closing
                else
                {
                    
                    transform.position = Vector3.MoveTowards(transform.position, closedPosition, speed * Time.fixedDeltaTime);
                    if (isMoving())
                    {
                        transform.position += Vector3.right * Mathf.Sin(Time.time * shakeSpd) * shakeAmt;
                    }
                }

            }
           


        }
	}
    //determine if the door is moving
    bool isMoving()
    {
        return !(Mathf.Abs(transform.position.y - openPosition.position.y) < 0.1f || Mathf.Abs(transform.position.y - closedPosition.y) < 0.1f);
    }

}
