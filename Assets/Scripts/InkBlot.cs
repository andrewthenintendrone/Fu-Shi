using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkBlot : MonoBehaviour
{
    public float gripTime;
    public GameObject player;

    public void Start()
    {
        Invoke("fallOff", gripTime);
    }

    private void Update()
    {
        player.transform.position = transform.position;
    }

    public void fallOff()
    {
        // reenable the player
        player.SetActive(true);

        // destroy this gameobject
        Destroy(gameObject);
    }
}
