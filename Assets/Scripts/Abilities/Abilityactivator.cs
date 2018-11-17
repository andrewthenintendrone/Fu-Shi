using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class Abilityactivator : MonoBehaviour
{
    [SerializeField]
    [Tooltip("ink wave prefab gameobject")]
    private GameObject inkSlashPrefab;

    [SerializeField]
    [Tooltip("time ability prefab gameobject")]
    private GameObject timeAbilityPrefab;

    // are the ink and time axis held
    private bool inkHeld = false;
    private bool timeHeld = false;

    [Tooltip("this is the switch determining whether the player can use the ink spray ability")]
    public bool hasInkAbility = false;

    [Tooltip("this is the switch determining whether the player can use the time reverse ability")]
    public bool hasTimeAbility = false;

    // can the player use ink (only once in the air)
    [HideInInspector]
    public bool canUseInkAbility = true;

    // can the player use the time ability (cooldown)
    [HideInInspector]
    public bool canUseTimeAbility = true;

    [SerializeField]
    [Tooltip("extra height gained by using ink ability in the air")]
    private float extraHeightFromInk;

    [SerializeField]
    [Tooltip("how long after using the time ability before it can be used again")]
    private float timeAbilityCooldown;

    [SerializeField]
    private string InkText;
    [SerializeField]
    private string InkText2;

    [SerializeField]
    private string TimeText;
    [SerializeField]
    private string TimeText2;

    [SerializeField]
    private string HealthText;
    [SerializeField]
    private string HealthText2;

	void Update ()
    {
        if(!Utils.gamePaused)
        {
            // read both ability axis
            float inkAxis = Input.GetAxis("Ink");
            float timeAxis = Input.GetAxis("Time");

            DrawAimDirection();

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
            // remove any old ink abilities
            if(GameObject.Find("inkSlash"))
            {
                Destroy(GameObject.Find("inkSlash"));
            }

            // player can no longer ink until they touch the ground
            canUseInkAbility = false;

            //create a gameobject InkWave
            GameObject CurrentInkwave = Instantiate(inkSlashPrefab, transform.position + new Vector3(0, 0.7f, -5.0f), Quaternion.identity, transform);
            CurrentInkwave.name = "inkSlash";

            //if player has R stick input use it
            //else use player facing
            Vector2 RstickDir = new Vector2(Input.GetAxis("RstickX"), Input.GetAxis("RstickY")).normalized;
            Vector2 LstickDir = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")).normalized;

            if (RstickDir.sqrMagnitude == 0)
            {
                if (LstickDir.sqrMagnitude == 0)
                {
                    // there is no stick input so just face the way the player is
                    CurrentInkwave.GetComponent<Inkmeleeslash>().direction = (GetComponent<Player>().facingRight ? Vector2.right : Vector2.left);
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
            SoundManager.instance.playInkSpray();

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
            SoundManager.instance.playReverseFX();
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
        // ink ability
        if (collision.gameObject.name == "inkGiver")
        {
            hasInkAbility = true;
            Destroy(collision.gameObject);
            Utils.showNotification(InkText, InkText2);
            SoundManager.instance.playAbilityPickup();
        }
        // time ability
        else if (collision.gameObject.name == "timeGiver")
        {
            hasTimeAbility = true;
            Destroy(collision.gameObject);
            Utils.showNotification(TimeText, TimeText2);
            SoundManager.instance.playAbilityPickup();
        }
        // extra health
        if (collision.name == "healthGiver")
        {
            Utils.maxHealth = 6;
            Utils.Health = 6;
            Destroy(collision.gameObject);
            Utils.showNotification(HealthText, HealthText2);
            SoundManager.instance.playAbilityPickup();
        }
    }

    // draw the aim direction
    void DrawAimDirection()
    {
        float RstickX = Input.GetAxis("RstickX");
        float RstickY = Input.GetAxis("RstickY");

        // get normalized launch direction
        Vector3 direction = (Vector3.right * RstickX + Vector3.up * RstickY).normalized;

        // if there is right stick input draw it
        if (hasInkAbility && canUseInkAbility && (RstickX != 0 || RstickY != 0))
        {
            LineRenderer lr = GetComponentInChildren<LineRenderer>();

            lr.enabled = true;

            // reset line points and draw direction line
            lr.positionCount = 2;
            lr.SetPosition(0, GetComponent<BoxCollider2D>().bounds.center);
            lr.SetPosition(1, GetComponent<BoxCollider2D>().bounds.center + direction * 3);
            Debug.DrawLine(transform.position, transform.position + direction, Color.red);
        }
        // otherwise disable the arrow
        else
        {
            GetComponentInChildren<LineRenderer>().enabled = false;
        }
    }
}
