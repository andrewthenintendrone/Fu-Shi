using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public Sprite unpressedSprite;
    public Sprite pressedSprite;

    bool pressed = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            pressed = true;
            GetComponent<SpriteRenderer>().sprite = pressedSprite;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            pressed = false;
            GetComponent<SpriteRenderer>().sprite = unpressedSprite;
        }
    }
}
