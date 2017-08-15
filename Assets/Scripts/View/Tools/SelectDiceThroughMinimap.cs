using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectDiceThroughMinimap : MonoBehaviour {

    private Camera diceCamera;

    // Use this for initialization
    void Start ()
    {
        diceCamera = GameObject.Find("SceneHolder/Board/DiceHolder/DiceCamera").GetComponent<Camera>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (DiceRerollManager.currentDiceRerollManager != null)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector3 mousePosition = transform.InverseTransformPoint(Input.mousePosition);

                if (transform.GetComponent<RectTransform>().rect.Contains(mousePosition))
                {
                    RaycastHit hitInfo = new RaycastHit();

                    mousePosition = new Vector3(mousePosition.x, mousePosition.y+255);
                    if (Physics.Raycast(diceCamera.ScreenPointToRay(mousePosition), out hitInfo))
                    {
                        if (!hitInfo.collider.name.StartsWith("DiceField"))
                        {
                            TrySelectDice(hitInfo.collider.gameObject);
                        }
                    }
                }
            }
        }
    }

    private void TrySelectDice(GameObject Dice)
    {
        Combat.CurentDiceRoll.TrySelectDiceByModel(Dice);
    }
}
