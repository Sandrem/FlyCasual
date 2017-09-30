using System;
using UnityEngine;
using UnityEngine.UI;
using Ship;

public class DamageNumbersPanel : MonoBehaviour
{
    private GenericShip Host;
    private float delaySeconds = 2f;

    private float movementProgress;

    private const float MOVE_SPEED = 30;
    private static Vector3 ABOVE_SHIP = new Vector3(0, 1, 0);
    private static Vector3 CENTER_PANEL = new Vector3(-85f, 0, 0);

    public void Initialize(GenericShip host, int hullChange, int shieldsChange)
    {
        Host = host;

        if (hullChange != 0)
        {
            transform.Find("Hull/HullNumber").GetComponent<Text>().text = Math.Abs(hullChange).ToString();
        }
        else
        {
            transform.Find("Hull").gameObject.SetActive(false);
            transform.Find("Shields").localPosition = new Vector3(5, 0);
        }
        
        if (shieldsChange != 0)
        {
            transform.Find("Shields/ShieldsNumber").GetComponent<Text>().text = Math.Abs(shieldsChange).ToString();
        }
        else
        {
            transform.Find("Shields").gameObject.SetActive(false);
        }

        if (shieldsChange == 0 || hullChange == 0)
        {
            transform.GetComponent<RectTransform>().sizeDelta = new Vector2(85, 52);
            CENTER_PANEL = new Vector3(-42.5f, 0, 0);
        }

        transform.position = Camera.main.WorldToScreenPoint(Host.GetCenter() + ABOVE_SHIP) + CENTER_PANEL;

        PlanSelfDestruction();
    }

    void Update()
    {
        movementProgress += Time.deltaTime * MOVE_SPEED;
        transform.position = Camera.main.WorldToScreenPoint(Host.GetCenter() + ABOVE_SHIP) + CENTER_PANEL + new Vector3(0, movementProgress, 0);
    }

    private void PlanSelfDestruction()
    {
        MonoBehaviour.Destroy(this.gameObject, delaySeconds);
    }

}
