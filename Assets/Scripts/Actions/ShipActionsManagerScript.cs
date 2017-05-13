using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//focus, target lock
//barrel roll
//crit undo

public delegate void ShipActionExecution();

public class ShipActionsManagerScript: MonoBehaviour {

    private GameManagerScript Game;

	private const float DISTANCE_1 = 3.28f / 3f;

    //EVENTS

    public delegate void EventHandler2Ships(ref bool result, Ship.GenericShip attacker, Ship.GenericShip defender);

    public event EventHandler2Ships OnCheckCanPerformAttack;

    void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

	public bool InArcCheck(Ship.GenericShip thisShip, Ship.GenericShip anotherShip) {
		bool result = thisShip.IsTargetInArc (anotherShip);
		return result;
	}

    public int GetRange(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        float distance = Vector3.Distance (thisShip.Model.GetClosestEdgesTo (anotherShip) ["this"], thisShip.Model.GetClosestEdgesTo (anotherShip) ["another"]);
		int range = Mathf.CeilToInt(distance / DISTANCE_1);
		return range;
	}

	public void OnlyCheckShot() {
		CheckShot ();
	}

	private bool CheckShot() {
        bool result = true;
        OnCheckCanPerformAttack(ref result, Game.Selection.ThisShip, Game.Selection.AnotherShip);
		return result;
	}

	public void PerformAttack() {
        Game.UI.HideContextMenu();
        if (CheckShot()) Game.Combat.PerformAttack(Game.Selection.ThisShip, Game.Selection.AnotherShip);
	}

}
