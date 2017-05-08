using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class RulerManagement : MonoBehaviour {

    private GameManagerScript Game;

    public GameObject Rulers;
	public GameObject movementRuler;
	private Vector3 savedRulerPosition;
	private Vector3 savedRulerRotation;
    private List<Vector3> rulerCenterPoints = new List<Vector3>();

    void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        //Fix this
        if (Game.Movement == null)
        {
            Game.Movement = this.GetComponent<ShipMovementScript>();
        }
        //Fix this
        if (Game.Actions == null)
        {
            Game.Actions = this.GetComponent<ShipActionsManagerScript>();
        }
        Game.Actions.OnCheckCanPerformAttack += ShowRange;
    }

    void Update () {

	}

    public void AddRulerCenterPoint(Vector3 point)
    {
        rulerCenterPoints.Add(point);
    }

    public void ResetRuler(Ship.GenericShip ship)
    {
        rulerCenterPoints = new List<Vector3>();
        HideLastMovementRuler();
    }

    public Vector3 FindNearestRulerCenterPoint(Vector3 pointShipStand)
    {
        Vector3 result = Vector3.zero;
        float minDistance = float.MaxValue;
        foreach (Vector3 rulerCenterPoint in rulerCenterPoints)
        {
            float currentDistance = Vector3.Distance(rulerCenterPoint, pointShipStand);
            if (currentDistance < minDistance)
            {
                minDistance = currentDistance;
                result = rulerCenterPoint;
            }
        }

        return result;
    }

    public void ApplyMovementRuler(Ship.GenericShip thisShip) {

        if (Game.Movement.CurrentMovementData.Speed != 0)
        {
            movementRuler = GetMovementRuler();
            savedRulerPosition = movementRuler.transform.position;
            savedRulerRotation = movementRuler.transform.eulerAngles;

            movementRuler.transform.position = thisShip.Model.GetPosition();
            movementRuler.transform.eulerAngles = thisShip.Model.GetAngles() + new Vector3(0f, 90f, 0f);
            if (Game.Movement.CurrentMovementData.MovementDirection == ManeuverDirection.Left)
            {
                movementRuler.transform.eulerAngles = movementRuler.transform.eulerAngles + new Vector3(180f, 0f, 0f);
            }
        }
        
	}

    private GameObject GetMovementRuler()
    {
        GameObject result = null;
        switch (Game.Movement.CurrentMovementData.MovementBearing)
        {
            case ManeuverBearing.Straight:
                return Rulers.transform.Find("straight" + Game.Movement.CurrentMovementData.Speed).gameObject;
            case ManeuverBearing.Bank:
                return Rulers.transform.Find("bank" + Game.Movement.CurrentMovementData.Speed).gameObject;
            case ManeuverBearing.Turn:
                return Rulers.transform.Find("turn" + Game.Movement.CurrentMovementData.Speed).gameObject;
            case ManeuverBearing.KoiogranTurn:
                return Rulers.transform.Find("straight" + Game.Movement.CurrentMovementData.Speed).gameObject;
        }
        return result;
    }

    private void HideLastMovementRuler(){
		movementRuler.transform.position = savedRulerPosition;
		movementRuler.transform.eulerAngles = savedRulerRotation;
	}

    public void ShowRange(ref bool result, Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        Vector3 vectorToTarget = thisShip.Model.GetClosestEdgesTo(anotherShip)["another"] - thisShip.Model.GetClosestEdgesTo(anotherShip)["this"];
        Rulers.transform.Find("RangeRuler").position = thisShip.Model.GetClosestEdgesTo(anotherShip)["this"];
        Rulers.transform.Find("RangeRuler").rotation = Quaternion.LookRotation(vectorToTarget);
    }

}
