using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class MovementTemplates {

    private static GameManagerScript Game;

	private static Vector3 savedRulerPosition;
	private static Vector3 savedRulerRotation;
    private static List<Vector3> rulerCenterPoints = new List<Vector3>();

    private static Transform Templates;
    public static Transform CurrentTemplate;

    static MovementTemplates()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
        Actions.OnCheckCanPerformAttack += CallShowRange;
        Templates = Game.PrefabsList.RulersHolderTransform;
    }

    public static void AddRulerCenterPoint(Vector3 point)
    {
        rulerCenterPoints.Add(point);
    }

    public static void ResetRuler(Ship.GenericShip ship)
    {
        rulerCenterPoints = new List<Vector3>();
        HideLastMovementRuler();
    }

    public static Vector3 FindNearestRulerCenterPoint(Vector3 pointShipStand)
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

    public static void ApplyMovementRuler(Ship.GenericShip thisShip) {

        if (Game.Movement.CurrentMovementData.Speed != 0)
        {
            CurrentTemplate = GetMovementRuler();

            SaveCurrentMovementRulerPosition();

            CurrentTemplate.position = thisShip.GetPosition();
            CurrentTemplate.eulerAngles = thisShip.GetAngles() + new Vector3(0f, 90f, 0f);
            if (Game.Movement.CurrentMovementData.MovementDirection == ManeuverDirection.Left)
            {
                CurrentTemplate.eulerAngles = CurrentTemplate.eulerAngles + new Vector3(180f, 0f, 0f);
            }
        }
        
	}

    public static void SaveCurrentMovementRulerPosition()
    {
        savedRulerPosition = CurrentTemplate.position;
        savedRulerRotation = CurrentTemplate.eulerAngles;
    }

    private static Transform GetMovementRuler()
    {
        Transform result = null;
        switch (Game.Movement.CurrentMovementData.MovementBearing)
        {
            case ManeuverBearing.Straight:
                return Templates.Find("straight" + Game.Movement.CurrentMovementData.Speed);
            case ManeuverBearing.Bank:
                return Templates.Find("bank" + Game.Movement.CurrentMovementData.Speed);
            case ManeuverBearing.Turn:
                return Templates.Find("turn" + Game.Movement.CurrentMovementData.Speed);
            case ManeuverBearing.KoiogranTurn:
                return Templates.Find("straight" + Game.Movement.CurrentMovementData.Speed);
        }
        return result;
    }

    public static void HideLastMovementRuler(){
        CurrentTemplate.position = savedRulerPosition;
		CurrentTemplate.eulerAngles = savedRulerRotation;
	}

    public static void CallShowRange(ref bool result, Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        ShowRange(thisShip, anotherShip);
    }

    public static void ShowRange(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        Vector3 vectorToTarget = thisShip.GetClosestEdgesTo(anotherShip)["another"] - thisShip.GetClosestEdgesTo(anotherShip)["this"];
        Templates.Find("RangeRuler").position = thisShip.GetClosestEdgesTo(anotherShip)["this"];
        Templates.Find("RangeRuler").rotation = Quaternion.LookRotation(vectorToTarget);
    }

    public static void CallReturnRangeRuler(Ship.GenericShip thisShip)
    {
        ReturnRangeRuler();
    }

    public static void ReturnRangeRuler()
    {
        Templates.Find("RangeRuler").transform.position = new Vector3(9.5f, 0f, 2.2f);
        Templates.Find("RangeRuler").transform.eulerAngles = new Vector3(0, -90, 0);
    }

    public static Transform GetMovement1Ruler()
    {
        return Templates.Find("straight1");
    }

}
