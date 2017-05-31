using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ShipFactory {

    //private static GameManagerScript Game;

    public static int lastId = 1;

	public static readonly Vector3 ROTATION_FORWARD = new Vector3 (0, 0, 0);
    public static readonly Vector3 ROTATION_BACKWARD = new Vector3 (0, 180, 0);

    static ShipFactory()
    {
        //Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
    }

	//TODO: REWRITE ASAP
	public static Ship.GenericShip SpawnShip(ShipConfiguration shipConfig) {

        //temporary
        int id = 1;
        Vector3 position = Vector3.zero;

        Ship.GenericShip newShipContainer = (Ship.GenericShip) System.Activator.CreateInstance(System.Type.GetType(shipConfig.PilotName));
        newShipContainer.InitializeGenericShip(shipConfig.PlayerNo, id, position);
        newShipContainer.InitializeShip();
        newShipContainer.InitializePilot();

        newShipContainer.InfoPanel = Roster.CreateRosterInfo(newShipContainer);

        foreach (var upgrade in shipConfig.Upgrades)
        {
            newShipContainer.InstallUpgrade(upgrade);
        }

        //TODO: Rework this
        newShipContainer.OnDestroyed += Rules.WinConditions.CheckWinConditions;
        newShipContainer.AfterGotNumberOfPrimaryWeaponAttackDices += Rules.DistanceBonus.CheckAttackDistanceBonus;
        newShipContainer.AfterGotNumberOfPrimaryWeaponDefenceDices += Rules.DistanceBonus.CheckDefenceDistanceBonus;
        newShipContainer.AfterGotNumberOfPrimaryWeaponDefenceDices += Rules.AsteroidObstruction.CheckDefenceDistanceBonus;
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
        newShipContainer.OnPositionFinish += Rules.OffTheBoard.CheckOffTheBoard;
        newShipContainer.OnMovementFinish += Rules.Stress.CheckStress;
        newShipContainer.OnMovementFinish += Rules.AsteroidHit.CheckDamage;
        newShipContainer.OnTryPerformAction += Rules.AsteroidHit.CanPerformActions;
        newShipContainer.AfterGetManeuverAvailablity += Rules.Stress.CannotPerformRedManeuversWhileStressed;

        newShipContainer.AfterTokenIsAssigned += Roster.UpdateTokensIndicator;
        newShipContainer.AfterTokenIsRemoved += Roster.UpdateTokensIndicator;
        newShipContainer.AfterAssignedDamageIsChanged += Roster.UpdateRosterHullDamageIndicators;
        newShipContainer.AfterAssignedDamageIsChanged += Roster.UpdateRosterShieldsDamageIndicators;
        newShipContainer.AfterStatsAreChanged += Roster.UpdateShipStats;

        return newShipContainer;
	}

}
