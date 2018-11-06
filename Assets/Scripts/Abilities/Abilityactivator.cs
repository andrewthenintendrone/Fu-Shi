using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Abilityactivator : MonoBehaviour
{
    [Tooltip("ink wave prefab gameobject")]
    [SerializeField]
    private GameObject inkSlashPrefab;

    [Tooltip("time ability prefab gameobject")]
    [SerializeField]
    private GameObject timeAbilityPrefab;

    // is the ink axis held
    private bool inkHeld = false;

    // is the time axis held
    private bool timeHeld = false;

    [Tooltip("this is the switch determining whether the player can use the ink spray ability")]
    public bool hasInkAbility = false;

    [Tooltip("this is the switch determining whether the player can use the time reverse ability")]
    public bool hasTimeAbility = false;

    // can the player use ink (only once in the air)
    [HideInInspector]
    public bool canUseInkAbility = true;

    [SerializeField]
    [Tooltip("extra height gained by using ink ability in the air")]
    private float extraHeightFromInk;

    [SerializeField]
    [Tooltip("how long after using the time ability before it can be used again")]
    private float timeAbilityCooldown;

    // can the player use the time ability (cooldown)
    [HideInInspector]
    private bool canUseTimeAbility = true;

    private AudioClip inkSound;

    void Start()
    {
        inkSound = Resources.Load<AudioClip>("Spray");
    }

	void Update ()
    {
        if(!Utils.gamePaused)
        {
            // read both ability axis
            float inkAxis = Input.GetAxis("Ink");
            float timeAxis = Input.GetAxis("Time");

            // the player can always use ink when on the ground
            if (gameObject.GetComponent<CharacterController2D>().isGrounded)
            {
                canUseInkAbility = true;
            }

            if (inkAxis > 0.5f)
            {
                if (!inkHeld)
                {
                    // ink axis is now held
                    inkHeld = true;

                    // attempt to use the ink ability
                    useInkAbility();
                }
            }
            else
            {
                inkHeld = false;
            }

            if (timeAxis > 0.5f)
            {
                if (!timeHeld)
                {
                    // time axis is now held
                    timeHeld = true;

                    // attempt to use the time ability
                    useTimeAbility();
                    timeHeld = true;
                }
            }
            else
            {
                timeHeld = false;
            }
        }
    }

    // tries to use the ink ability
    public void useInkAbility()
    {
        // to use the ink ability the player must have unlocked it
        if (hasInkAbility && canUseInkAbility)
        {
            // player can no longer ink until they touch the ground
            canUseInkAbility = false;

            //create a gameobject InkWave
            GameObject CurrentInkwave = Instantiate(inkSlashPrefab, transform.position + new Vector3(0, 0.7f), Quaternion.identity);

            //if player has R stick input use it
            //else use player facing
            Vector2 RstickDir = new Vector2(Input.GetAxis("RstickX"), Input.GetAxis("RstickY")).normalized;
            Vector2 LstickDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

            if (RstickDir.sqrMagnitude == 0)
            {
                if (LstickDir.sqrMagnitude == 0)
                {
                    if (gameObject.GetComponent<Player>().facingRight)
                    {
                        CurrentInkwave.GetComponent<Inkmeleeslash>().direction = Vector2.right;
                    }
                    else
                    {
                        CurrentInkwave.GetComponent<Inkmeleeslash>().direction = Vector2.left;
                    }

                    // ternary
                    // CurrentInkwave.GetComponent<Inkmeleeslash>().direction = (gameObject.GetComponent<Player>().facingRight ? Vector2.right : Vector2.left);
                }
                else
                {
                    CurrentInkwave.GetComponent<Inkmeleeslash>().direction = LstickDir;
                }
            }
            else
            {
                CurrentInkwave.GetComponent<Inkmeleeslash>().direction = RstickDir;
            }

            // add extra y velocity for a bit of air time
            if(!GetComponent<CharacterController2D>().isGrounded)
            {
                GetComponent<Player>().velocity.y = extraHeightFromInk;
            }

            // play sound effect
            SoundManager.instance.playSingle(inkSound);

            // play animation
            GetComponent<Animator>().SetTrigger("inkSpray");
        }
    }

    // tries to use the time ability
    public void useTimeAbility()
    {
        if (hasTimeAbility && canUseTimeAbility)
        {
            // spawn time ability object prefab
            GameObject timeAbilityObject = Instantiate(timeAbilityPrefab, transform);
            timeAbilityObject.transform.position = GetComponent<Collider2D>().bounds.center;

            canUseTimeAbility = false;
            Invoke("enableTimeAbility", timeAbilityCooldown);
        }
    }

    private void enableTimeAbility()
    {
        canUseTimeAbility = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name == "inkGiver")
        {
            hasInkAbility = true;
            Destroy(collision.gameObject);
            Utils.showNotification("You got the ink ability!", "Press B to continue");
        }
        else if (collision.gameObject.name == "timeGiver")
        {
            hasTimeAbility = true;
            Destroy(collision.gameObject);
            Utils.showNotification("You got the time ability!", "Press B to continue");
        }
    }
}
