using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    [Tooltip("Sprites to use for unpressed / pressed")]
    public Sprite[] sprites = new Sprite[2];

    [SerializeField]
    [Tooltip("is this is checked the pressure plate is pressed")]
    private bool isPressed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // player touches pressure plate
        if(collision.gameObject.tag == "Player")
        {
            // become pressed
            isPressed = true;
            GetComponent<SpriteRenderer>().sprite = sprites[1];
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // player leaves pressure plate
        if (collision.gameObject.tag == "Player")
        {
            // become depressed (like me)
            isPressed = false;
            GetComponent<SpriteRenderer>().sprite = sprites[0];
        }
    }
}
