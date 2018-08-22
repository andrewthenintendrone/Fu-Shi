using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkBlot : MonoBehaviour
{
    public GameObject player;
    public float launchForce;
    private LineRenderer lr;
    private Vector3 direction;


    private void Start()
    {
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");

        direction = (Vector3.right * xAxis + Vector3.up * yAxis).normalized;

        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + direction * 3);

        // draw line in direction
        Debug.DrawLine(transform.position, transform.position + direction, Color.red);

        if ((int)Input.GetAxisRaw("Jump") == 1)
        {
            launch();
        }
    }

    public void launch()
    {
        player.SetActive(true);
        player.GetComponent<Player>().isLaunching = true;

        player.GetComponent<Player>().velocity = direction * launchForce;

        Debug.Log("launched");

        // destroy this gameobject
        Destroy(gameObject);
    }
}
