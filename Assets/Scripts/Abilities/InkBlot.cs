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

    [SerializeField]
    [Tooltip("Ink splatter particles prefab")]
    private GameObject inkParticlePrefab;

    private void Start()
    {
        // get debug line renderer
        lr = GetComponent<LineRenderer>();

        transform.parent.GetComponent<AnimationInkBlot>().PlayAnimation();

        Instantiate(inkParticlePrefab, transform.position, Quaternion.identity);
    }

    private void Update()
    {
        if(!Utils.gamePaused && Utils.Health > 0)
        {
            player.transform.position = transform.position - new Vector3(player.GetComponent<Player>().character.boxCollider.offset.x, player.GetComponent<Player>().character.boxCollider.offset.y, 0);

            // get inputs
            float xAxis = Input.GetAxis("Horizontal");
            float yAxis = Input.GetAxis("Vertical");
            int jumpAxis = (int)Input.GetAxisRaw("Jump");

            if (jumpAxis == 0)
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

                RaycastHit2D hitInfo = Physics2D.Raycast(transform.position, directionToCenter, 10, 1 << LayerMask.NameToLayer("Solid"));

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
    }

    public void launch()
    {
        // reactivate the player gameobject and set isLaunching to true
        player.GetComponent<Animator>().enabled = true;
        player.GetComponent<Player>().enabled = true;
        player.GetComponentInChildren<SkinnedMeshRenderer>().enabled = true;
        player.GetComponent<Abilityactivator>().enabled = true;
        player.GetComponent<Collider2D>().enabled = true;

        player.GetComponent<Player>().isLaunching = true;
        player.GetComponent<Player>().canTurnIntoInkBlot = false;
        player.GetComponent<Abilityactivator>().canUseInkAbility = true;

        // stop the launch after launchTime
        player.GetComponent<Player>().Invoke("cancelLaunch", launchTime);

        // let the player turn back into an ink blot after the grace period
        player.GetComponent<Player>().Invoke("enableCanTurnIntoInkBlot", gracePeriod);

        // set the players velocity to the launch force
        player.GetComponent<Player>().velocity = direction * launchForce;

        player.GetComponent<Player>().jumpHeld = true;

        // play the animation on the leaf (if it is a leaf)
        if(transform.parent.GetComponent<AnimationInkBlot>() != null)
        {
            transform.parent.GetComponent<AnimationInkBlot>().PlayAnimation();
        }

        // double check we are dead
        if(Utils.Health <= 0)
        {
            GameObject.FindObjectOfType<Fade>().triggerFadeOut();
            player.GetComponent<Animator>().SetTrigger("death");
        }

        // play sound effect
        SoundManager.instance.playLaunchFromInkPlatform();

        // particles
        Instantiate(inkParticlePrefab, transform.position, Quaternion.identity);

        // destroy this gameobject
        Destroy(gameObject);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.GetComponent<inkableSurface>() != null && collision.gameObject.GetComponent<enemyProjectile>() == null && collision.gameObject.GetComponent<Inkmeleeslash>() == null && collision.gameObject.GetComponent<inkBullet>() == null)
        {
            launch();
        }
    }

    private void onTriggerEnter2D(Collider2D col)
    {
        if(!Utils.gamePaused && Utils.Health > 0)
        {
            if (col.tag == "reset")
            {
                Utils.KillPlayer();
            }
            else if (col.tag == "enemy")
            {
                Utils.Health = Mathf.Max(Utils.Health - 1, 0);
            }
            // sets the checkpoint
            else if (col.tag == "checkpoint")
            {
                Utils.updateCheckpoint(col.transform.position);
            }
            else if (col.tag == "savepoint")
            {
                Utils.updateCheckpoint(col.transform.position);
                SaveLoad.Save();
            }
        }
    }
}
