using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Board;

public static class MovementTemplates {

	private static Vector3 savedRulerPosition;
	private static Vector3 savedRulerRotation;
    private static List<Vector3> rulerCenterPoints = new List<Vector3>();

    private static Transform Templates;
    public static Transform CurrentTemplate;

    public static void PrepareMovementTemplates()
    {
        Templates = GameObject.Find("SceneHolder/Board/RulersHolder").transform;
    }

    public static void AddRulerCenterPoint(Vector3 point)
    {
        rulerCenterPoints.Add(point);
    }

    public static void ResetRuler()
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

        if (Selection.ThisShip.AssignedManeuver.Speed != 0)
        {
            ApplyMovementRuler(thisShip, thisShip.AssignedManeuver);
        }        
	}

    public static void ApplyMovementRuler(Ship.GenericShip thisShip, Movement.GenericMovement movement)
    {
        CurrentTemplate = GetMovementRuler(movement);

        if (CurrentTemplate != null)
        {
            SaveCurrentMovementRulerPosition();

            CurrentTemplate.position = thisShip.GetPosition();
            CurrentTemplate.eulerAngles = thisShip.GetAngles() + new Vector3(0f, 90f, 0f);
            if (movement.Direction == Movement.ManeuverDirection.Left)
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

    public static Transform GetMovementRuler(Movement.GenericMovement movement)
    {
        ResetRuler();

        Transform result = null;
        if (movement != null)
        {
            switch (movement.Bearing)
            {
                case Movement.ManeuverBearing.Straight:
                    return Templates.Find("straight" + movement.Speed);
                case Movement.ManeuverBearing.Bank:
                    return Templates.Find("bank" + movement.Speed);
                case Movement.ManeuverBearing.SegnorsLoop:
                    return Templates.Find("bank" + movement.Speed);
                case Movement.ManeuverBearing.Turn:
                    return Templates.Find("turn" + movement.Speed);
                case Movement.ManeuverBearing.TallonRoll:
                    return Templates.Find("turn" + movement.Speed);
                case Movement.ManeuverBearing.KoiogranTurn:
                    return Templates.Find("straight" + movement.Speed);
                case Movement.ManeuverBearing.Stationary:
                    return null;
            }
        }
        return result;
    }

    public static void HideLastMovementRuler(){
        if (CurrentTemplate != null)
        {
            CurrentTemplate.position = savedRulerPosition;
            CurrentTemplate.eulerAngles = savedRulerRotation;
        }
	}

    public static void ShowRange(Ship.GenericShip thisShip, Ship.GenericShip anotherShip)
    {
        ShowRangeRuler(new ShipDistanceInformation(thisShip, anotherShip));
    }

    public static void ShowFiringArcRange(ShipShotDistanceInformation shotInfo)
    {
        if (shotInfo.InShotAngle)
        {
            ShowRangeRuler(shotInfo);
        }
        else
        {
            Messages.ShowErrorToHuman("Out of arc!");
            ShowRangeRuler(new ShipShotOutOfArcDistanceInformation(shotInfo.ThisShip, shotInfo.AnotherShip));
        }
    }

    public static void ShowRangeRuler(GeneralShipDistanceInformation shipDistanceInfo)
    {
        Templates.Find("RangeRuler").position = shipDistanceInfo.ThisShipNearestPoint;
        Templates.Find("RangeRuler").rotation = Quaternion.LookRotation(shipDistanceInfo.Vector);
    }

    public static void CallReturnRangeRuler(Ship.GenericShip thisShip)
    {
        ReturnRangeRuler();
    }

    public static void ReturnRangeRuler()
    {
        Templates.Find("RangeRuler").transform.position = new Vector3(7f, 0f, -1.5f);
        Templates.Find("RangeRuler").transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public static Transform GetMovement1Ruler()
    {
        CurrentTemplate = Templates.Find("straight1");
        SaveCurrentMovementRulerPosition();
        return CurrentTemplate;
    }

    public static Transform GetMovement2Ruler()
    {
        CurrentTemplate = Templates.Find("straight2");
        SaveCurrentMovementRulerPosition();
        return CurrentTemplate;
    }

}
