using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    private Sprite healthImage;
    public Vector2 healthSize;

    private void Start()
    {
        setHealth(3);
    }

    public void setHealth(int health)
    {
        GetComponent<RectTransform>().localScale = new Vector3(healthSize.x, healthSize.y, 1);
        GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, GetComponent<RectTransform>().sizeDelta.x * health);
    }
}
