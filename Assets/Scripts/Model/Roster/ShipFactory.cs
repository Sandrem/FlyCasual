using SquadBuilderNS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class ShipFactory {

    //private static GameManagerScript Game;

    public static int lastId;

	public static readonly Vector3 ROTATION_FORWARD = new Vector3 (0, 0, 0);
    public static readonly Vector3 ROTATION_BACKWARD = new Vector3 (0, 180, 0);

    public static void Initialize()
    {
        lastId = 1;
    }

	//TODO: REWRITE ASAP
	public static Ship.GenericShip SpawnShip(SquadBuilderShip shipConfig) {

        //temporary
        int id = 1;
        Vector3 position = Vector3.zero;

        Ship.GenericShip newShipContainer = shipConfig.Instance;
        newShipContainer.InitializeGenericShip(shipConfig.List.PlayerNo, id, position);

        Roster.SubscribeSelectionByInfoPanel(newShipContainer.InfoPanel.transform.Find("ShipInfo").gameObject);
        Roster.SubscribeUpgradesPanel(newShipContainer, newShipContainer.InfoPanel);

        //TODO: Rework this
        newShipContainer.AfterGotNumberOfPrimaryWeaponAttackDice += Rules.DistanceBonus.CheckAttackDistanceBonus;
        newShipContainer.AfterGotNumberOfPrimaryWeaponDefenceDice += Rules.DistanceBonus.CheckDefenceDistanceBonus;
        newShipContainer.AfterGotNumberOfDefenceDice += Rules.AsteroidObstruction.CheckDefenceObstructionBonus;
        newShipContainer.OnTryAddAvailableAction += Rules.Stress.CanPerformActions;
        newShipContainer.OnTryAddAvailableAction += Rules.DuplicatedActions.CanPerformActions;
        newShipContainer.OnMovementStart += Rules.Collision.ClearBumps;
        newShipContainer.OnMovementStart += MovementTemplates.ApplyMovementRuler;
        newShipContainer.OnMovementStart += MovementTemplates.CallReturnRangeRuler;
        newShipContainer.OnPositionFinish += Rules.OffTheBoard.CheckOffTheBoard;
        newShipContainer.OnMovementExecuted += Rules.Stress.PlanCheckStress;
        newShipContainer.AfterGetManeuverAvailablity += Rules.Stress.CannotPerformRedManeuversWhileStressed;
        newShipContainer.OnDestroyed += Rules.TargetLocks.RemoveTargetLocksOnDestruction;

        newShipContainer.OnTokenIsAssigned += Roster.UpdateTokensIndicator;
        newShipContainer.AfterTokenIsRemoved += Roster.UpdateTokensIndicator;
        newShipContainer.AfterAssignedDamageIsChanged += Roster.UpdateRosterHullDamageIndicators;
        newShipContainer.AfterAssignedDamageIsChanged += Roster.UpdateRosterShieldsDamageIndicators;
        newShipContainer.AfterStatsAreChanged += Roster.UpdateShipStats;

        return newShipContainer;
	}

}
