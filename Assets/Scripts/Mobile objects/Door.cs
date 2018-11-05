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
    private bool m_nearPlayer = false;

    [SerializeField]
    private Texture2D sprite1;

    [SerializeField]
    private Texture2D sprite2;

    private float fade = 0.0f;

    [SerializeField]
    [Tooltip("how many seconds it takes to fade in")]
    private float fadeInSpeed;

    [SerializeField]
    [Tooltip("how many seconds it takes to fade out")]
    private float fadeOutSpeed;

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

        GetComponent<SpriteRenderer>().material.SetTexture("_MainTex", sprite1);
        GetComponent<SpriteRenderer>().material.SetTexture("_MainTex2", sprite2);

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
            if (m_nearPlayer)
            {
                fade = Mathf.Min(1.0f, fade + Time.deltaTime / fadeInSpeed);
            }
            else
            {
                fade = Mathf.Max(0.0f, fade - Time.deltaTime / fadeOutSpeed);
            }

            GetComponent<SpriteRenderer>().material.SetFloat("_Fade", fade);
        }
	}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject == Utils.getPlayer())
        {
            m_nearPlayer = true;
        }
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject == Utils.getPlayer())
        {
            m_nearPlayer = false;
        }

    }
}
