using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inkmeleeslash : MonoBehaviour {

    [SerializeField]
    private GameObject inkbulletprefab;

    [HideInInspector]
    public Vector2 direction;

    [Tooltip("how long the ink will persist")]
    [SerializeField]
    private float lifetime;

    //spawn the smaller inkprojectiles here
    void Start ()
    {
        float rotation = Vector2.SignedAngle(Vector2.left, direction);
        transform.eulerAngles = Vector3.forward * rotation;


        GameObject inkbullet1 = Instantiate(inkbulletprefab, transform.position, transform.rotation);
        inkbullet1.GetComponent<inkBullet>().direction = direction;


        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // check if object has inkable surface
        if (collision.gameObject.GetComponentInChildren<inkableSurface>() != null)
        {
            collision.gameObject.GetComponentInChildren<inkableSurface>().Inked = true;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<Enemy>() != null)
        {
            collision.gameObject.GetComponent<Enemy>().health--;
        }
    }
}
