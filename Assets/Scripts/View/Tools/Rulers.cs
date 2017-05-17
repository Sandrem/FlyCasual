using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementTemplates {

    private GameManagerScript Game;

	private Vector3 savedRulerPosition;
	private Vector3 savedRulerRotation;
    private List<Vector3> rulerCenterPoints = new List<Vector3>();

    public GameObject CurrentMovementTemplate;

    public void Start()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Game.Actions.OnCheckCanPerformAttack += CallShowRange;
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
            CurrentMovementTemplate = GetMovementRuler();
            savedRulerPosition = CurrentMovementTemplate.transform.position;
            savedRulerRotation = CurrentMovementTemplate.transform.eulerAngles;

            CurrentMovementTemplate.transform.position = thisShip.Model.GetPosition();
            CurrentMovementTemplate.transform.eulerAngles = thisShip.Model.GetAngles() + new Vector3(0f, 90f, 0f);
            if (Game.Movement.CurrentMovementData.MovementDirection == ManeuverDirection.Left)
            {
                CurrentMovementTemplate.transform.eulerAngles = CurrentMovementTemplate.transform.eulerAngles + new Vector3(180f, 0f, 0f);
            }
        }
        
	}

    private GameObject GetMovementRuler()
    {
        GameObject result = null;
        switch (Game.Movement.CurrentMovementData.MovementBearing)
        {
            case ManeuverBearing.Straight:
                return Game.PrefabList.RulersHolder.transform.Find("straight" + Game.Movement.CurrentMovementData.Speed).gameObject;
            case ManeuverBearing.Bank:
                return Game.PrefabList.RulersHolder.transform.Find("bank" + Game.Movement.CurrentMovementData.Speed).gameObject;
            case ManeuverBearing.Turn:
                return Game.PrefabList.RulersHolder.transform.Find("turn" + Game.Movement.CurrentMovementData.Speed).gameObject;
            case ManeuverBearing.KoiogranTurn:
                return Game.PrefabList.RulersHolder.transform.Find("straight" + Game.Movement.CurrentMovementData.Speed).gameObject;
        }
        return result;
    }

    private void HideLastMovementRuler(){
        CurrentMovementTemplate.transform.position = savedRulerPosition;
		CurrentMovementTemplate.transform.eulerAngles = savedRulerRotation;
	}

    public void CallShowRange(ref bool result, Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        ShowRange(thisShip, anotherShip);
    }

    public void ShowRange(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        Vector3 vectorToTarget = thisShip.Model.GetClosestEdgesTo(anotherShip)["another"] - thisShip.Model.GetClosestEdgesTo(anotherShip)["this"];
        Game.PrefabList.RulersHolder.transform.Find("RangeRuler").position = thisShip.Model.GetClosestEdgesTo(anotherShip)["this"];
        Game.PrefabList.RulersHolder.transform.Find("RangeRuler").rotation = Quaternion.LookRotation(vectorToTarget);
    }

    public void CallReturnRangeRuler(Ship.GenericShip thisShip)
    {
        ReturnRangeRuler();
    }

    public void ReturnRangeRuler()
    {
        Game.PrefabList.RulersHolder.transform.Find("RangeRuler").transform.position = new Vector3(9.5f, 0f, 2.2f);
        Game.PrefabList.RulersHolder.transform.Find("RangeRuler").transform.eulerAngles = new Vector3(0, -90, 0);
    }

}
