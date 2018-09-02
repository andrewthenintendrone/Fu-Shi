using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InkBlot : MonoBehaviour
{
    [HideInInspector]
    // reference to the player gameobject
    public GameObject player;

    [Tooltip("force to launch the player at")]
    public float launchForce;

    [Tooltip("how long it takes for the player to regain control after launch")]
    public float launchTime;

    [Tooltip("how long until the player can turn into an ink blot after launch")]
    public float gracePeriod;

    // debug line renderer
    private LineRenderer lr;

    // debug launch direction
    private Vector3 direction;

    [HideInInspector]
    public bool jumpHeld;

    private void Start()
    {
        // get debug line renderer
        lr = GetComponent<LineRenderer>();
    }

    private void Update()
    {
        player.transform.position = transform.position;

        // get inputs
        float xAxis = Input.GetAxis("Horizontal");
        float yAxis = Input.GetAxis("Vertical");
        int jumpAxis = (int)Input.GetAxisRaw("Jump");

        if(jumpAxis == 0)
        {
            jumpHeld = false;
        }

        // get normalized launch direction
        direction = (Vector3.right * xAxis + Vector3.up * yAxis).normalized;

        // if there is no input find a good direction
        if (yAxis == 0 && xAxis == 0)
        {
            // get direction to the center of the object first
            Vector3 directionToCenter = (transform.parent.position - transform.position).normalized;

            RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, directionToCenter, 1 << LayerMask.NameToLayer("Player"));

            if (hitInfo)
            {
                if (hitInfo.collider.gameObject.transform == transform.parent)
                {
                    direction = hitInfo.normal.normalized;
                }
            }
        }

        // reset line points and draw direction line
        lr.positionCount = 2;
        lr.SetPosition(0, transform.position);
        lr.SetPosition(1, transform.position + direction * 3);
        Debug.DrawLine(transform.position, transform.position + direction, Color.red);

        // the jump button launches
        if ((int)Input.GetAxisRaw("Jump") == 1 && !jumpHeld)
        {
            launch();
        }
    }

    public void launch()
    {
        // reactivate the player gameobject and set isLaunching to true
        player.SetActive(true);
        player.GetComponent<Player>().isLaunching = true;

        // set the players velocity to the launch force
        player.GetComponent<Player>().velocity = direction * launchForce;

        // stop the launch after launchTime
        player.GetComponent<Player>().Invoke("cancelLaunch", launchTime);

        player.GetComponent<Player>().jumpHeld = true;

        // destroy this gameobject
        Destroy(gameObject);
    }
}
