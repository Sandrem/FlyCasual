using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ShipFactory {

    private static GameManagerScript Game;

    private static int lastId = 1;

	private static readonly Vector3 ROTATION_FORWARD = new Vector3 (0, 0, 0);
	private static readonly Vector3 ROTATION_BACKWARD = new Vector3 (0, 180, 0);

    static ShipFactory()
    {
        Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

	//TODO: REWRITE ASAP
	public static Ship.GenericShip SpawnShip(ShipConfiguration shipConfig) {

        //temporary
        int id = 1;
        Vector3 position = Vector3.zero;

        Ship.GenericShip newShipContainer = (Ship.GenericShip) System.Activator.CreateInstance(System.Type.GetType(shipConfig.PilotName), shipConfig.PlayerNo, id, position);
        newShipContainer.InfoPanel = Roster.CreateRosterInfo(newShipContainer);
        foreach (var upgrade in shipConfig.Upgrades)
        {
            newShipContainer.InstallUpgrade(upgrade);
        }
        newShipContainer.Model.SetShipInstertImage();

        newShipContainer.OnDestroyed += Rules.WinConditions.CheckWinConditions;
        newShipContainer.AfterGotNumberOfAttackDices += Rules.DistanceBonus.CheckAttackDistanceBonus;
        newShipContainer.AfterGotNumberOfDefenceDices += Rules.DistanceBonus.CheckDefenceDistanceBonus;
        newShipContainer.OnTryPerformAction += Rules.Stress.CanPerformActions;
        newShipContainer.OnTryPerformAction += Rules.DuplicatedActions.CanPerformActions;
        newShipContainer.OnMovementFinishWithoutColliding += Rules.KoiogranTurn.CheckKoiogranTurn;
        newShipContainer.OnMovementFinishWithColliding += Rules.KoiogranTurn.CheckKoiogranTurnError;
        newShipContainer.OnMovementStart += Rules.Collision.ClearCollision;
        newShipContainer.OnMovementFinishWithoutColliding += Rules.Collision.ClearCollision;
        newShipContainer.OnMovementFinishWithColliding += Rules.Collision.AssignCollision;
        newShipContainer.OnMovementStart += MovementTemplates.ApplyMovementRuler;
        newShipContainer.OnMovementStart += MovementTemplates.CallReturnRangeRuler;
        newShipContainer.OnMovementFinish += MovementTemplates.ResetRuler;
        newShipContainer.OnMovementFinish += Rules.OffTheBoard.CheckOffTheBoard;
        newShipContainer.OnMovementFinish += Rules.Stress.CheckStress;
        newShipContainer.AfterGetManeuverAvailablity += Rules.Stress.CannotPerformRedManeuversWhileStressed;

        newShipContainer.AfterTokenIsAssigned += Roster.UpdateTokensIndicator;
        newShipContainer.AfterTokenIsRemoved += Roster.UpdateTokensIndicator;
        newShipContainer.AfterAssignedDamageIsChanged += Roster.UpdateRosterHullDamageIndicators;
        newShipContainer.AfterAssignedDamageIsChanged += Roster.UpdateRosterShieldsDamageIndicators;
        newShipContainer.AfterStatsAreChanged += Roster.UpdateShipStats;

        return newShipContainer;
	}

    public static GameObject CreateShipModel(Ship.GenericShip newShipContainer, Vector3 position) {

        Vector3 facing = (newShipContainer.Owner.PlayerNo == Players.PlayerNo.Player1) ? ROTATION_FORWARD : ROTATION_BACKWARD;

        position = new Vector3(0, 0, (newShipContainer.Owner.PlayerNo == Players.PlayerNo.Player1) ? -4 : 4);

        GameObject newShip = MonoBehaviour.Instantiate(Game.PrefabList.ShipModel, position + new Vector3(0, 0.03f, 0), Quaternion.Euler(facing), Board.GetBoard());
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

    private static void SetTagOfChildren(Transform parent, string tag)
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
