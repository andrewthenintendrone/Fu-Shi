using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkBlot : MonoBehaviour
{
    public GameObject player;
    public float launchForce;

    private void Update()
    {
        if((int)Input.GetAxisRaw("Jump") == 1)
        {
            launch();
        }
    }

    public void launch()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        Vector3 direction = (Vector3.right * xAxis + Vector3.up * yAxis).normalized;

        player.SetActive(true);

        player.GetComponent<Player>().velocity = direction * launchForce;

        // destroy this gameobject
        Destroy(gameObject);
    }
}
