using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Tooltip("Sprites to use for unpressed / pressed")]
    public Sprite[] sprites = new Sprite[2];

    [Tooltip("Door that this pressure plate is linked to")]
    public Door linkedDoor;

    // is the pressure plate pressed
    public bool isPressed
    {
        get
        {
            return m_isPressed;
        }
        set
        {
            m_isPressed = value;
            UpdateImage();
        }
    }

    [SerializeField]
    private bool m_isPressed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // player touches pressure plate
        if(collision.gameObject.tag == "Player")
        {
            // become pressed
            isPressed = true;
            UpdateImage();
            linkedDoor.isOpen = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // player leaves pressure plate
        if (collision.gameObject.tag == "Player")
        {
            // become unpressed
            isPressed = false;
            UpdateImage();
            linkedDoor.isOpen = false;
        }
    }

    // update image to match isPressed value
    private void UpdateImage()
    {
        GetComponent<SpriteRenderer>().sprite = sprites[m_isPressed ? 1 : 0];
    }

    private void OnValidate()
    {
        isPressed = m_isPressed;
    }
}
