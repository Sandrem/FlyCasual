using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShipFactory {

    private GameManagerScript Game;

    private int lastId = 1;

	private static Vector3 ROTATION_FORWARD = new Vector3 (0, 0, 0);
	private static Vector3 ROTATION_BACKWARD = new Vector3 (0, 180, 0);

    public ShipFactory()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

	//TODO: REWRITE ASAP
	public Ship.GenericShip SpawnShip(string pilotName, Players.GenericPlayer player) {

        //temporary
        int id = 1;
        Vector3 position = Vector3.zero;

        Ship.GenericShip newShipContainer = (Ship.GenericShip) System.Activator.CreateInstance(System.Type.GetType(pilotName), player, id, position);
        newShipContainer.Model.SetShipInstertImage();

        newShipContainer.InfoPanel = Game.Roster.CreateRosterInfo(newShipContainer);

        newShipContainer.OnDestroyed += Game.Rules.WinConditions.CheckWinConditions;
        newShipContainer.AfterGotNumberOfAttackDices += Game.Rules.DistanceBonus.CheckAttackDistanceBonus;
        newShipContainer.AfterGotNumberOfDefenceDices += Game.Rules.DistanceBonus.CheckDefenceDistanceBonus;
        newShipContainer.OnTryPerformAction += Game.Rules.Stress.CanPerformActions;
        newShipContainer.OnTryPerformAction += Game.Rules.DuplicatedActions.CanPerformActions;
        newShipContainer.OnMovementFinishWithoutColliding += Game.Rules.KoiogranTurn.CheckKoiogranTurn;
        newShipContainer.OnMovementFinishWithColliding += Game.Rules.KoiogranTurn.CheckKoiogranTurnError;
        newShipContainer.OnMovementStart += Game.Rules.Collision.ClearCollision;
        newShipContainer.OnMovementFinishWithoutColliding += Game.Rules.Collision.ClearCollision;
        newShipContainer.OnMovementFinishWithColliding += Game.Rules.Collision.AssignCollision;
        newShipContainer.OnMovementStart += Game.Ruler.ApplyMovementRuler;
        newShipContainer.OnMovementStart += Game.Ruler.CallReturnRangeRuler;
        newShipContainer.OnMovementFinish += Game.Ruler.ResetRuler;
        newShipContainer.OnMovementFinish += Game.Rules.OffTheBoard.CheckOffTheBoard;
        newShipContainer.OnMovementFinish += Game.Rules.Stress.CheckStress;
        newShipContainer.AfterGetManeuverAvailablity += Game.Rules.Stress.CannotPerformRedManeuversWhileStressed;

        newShipContainer.AfterTokenIsAssigned += Game.Roster.UpdateTokensIndicator;
        newShipContainer.AfterTokenIsRemoved += Game.Roster.UpdateTokensIndicator;
        newShipContainer.AfterAssignedDamageIsChanged += Game.Roster.UpdateRosterHullDamageIndicators;
        newShipContainer.AfterAssignedDamageIsChanged += Game.Roster.UpdateRosterShieldsDamageIndicators;
        newShipContainer.AfterStatsAreChanged += Game.Roster.UpdateShipStats;

        return newShipContainer;
	}

    public GameObject CreateShipModel(Ship.GenericShip newShipContainer, Vector3 position) {

        Vector3 facing = (newShipContainer.Owner.PlayerNo == Players.PlayerNo.Player1) ? ROTATION_FORWARD : ROTATION_BACKWARD;

        position = new Vector3(0, 0, (newShipContainer.Owner.PlayerNo == Players.PlayerNo.Player1) ? -4 : 4);

        GameObject newShip = MonoBehaviour.Instantiate(Game.PrefabList.ShipModel, position + new Vector3(0, 0.03f, 0), Quaternion.Euler(facing), Game.PrefabList.Board.transform);
        newShip.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(newShipContainer.Type).gameObject.SetActive(true);

        newShipContainer.ShipId = lastId;
        lastId = lastId + 1;
        SetTagOfChildren(newShip.transform, "ShipId:" + newShipContainer.ShipId.ToString());

        //Check Size of stand
        //Debug.Log(Board.transform.InverseTransformPoint(newShip.transform.TransformPoint(new Vector3(-0.5f, -0.5f, -0.5f))));
        //Debug.Log(Board.transform.InverseTransformPoint(newShip.transform.TransformPoint(new Vector3(0.5f, 0.5f, 0.5f))));

        //Check size of playmat
        //Debug.Log(Board.transform.InverseTransformPoint(Board.transform.Find("Playmat").transform.TransformPoint(new Vector3(-0.5f, -0.5f, -0.5f))));
        //Debug.Log(Board.transform.InverseTransformPoint(Board.transform.Find("Playmat").transform.TransformPoint(new Vector3(0.5f, 0.5f, 0.5f))));


        return newShip;
    }

    private void SetTagOfChildren(Transform parent, string tag)
    {
        parent.gameObject.tag = tag;
        if (parent.childCount > 0)
        {
            foreach (Transform t in parent)
            {
                SetTagOfChildren(t, tag);
            }
        }
    }

}
