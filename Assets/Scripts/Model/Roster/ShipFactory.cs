﻿using System.Collections;
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

        Ship.GenericShip newShipContainer = shipConfig.Ship;
        newShipContainer.InitializeGenericShip(shipConfig.Player, id, position);

        Roster.SubscribeSelectionByInfoPanel(newShipContainer.InfoPanel.transform.Find("ShipInfo").gameObject);
        Roster.SubscribeUpgradesPanel(newShipContainer, newShipContainer.InfoPanel);

        //TODO: Rework this
        newShipContainer.AfterGotNumberOfPrimaryWeaponAttackDice += Rules.DistanceBonus.CheckAttackDistanceBonus;
        newShipContainer.AfterGotNumberOfPrimaryWeaponDefenceDice += Rules.DistanceBonus.CheckDefenceDistanceBonus;
        newShipContainer.AfterGotNumberOfPrimaryWeaponDefenceDice += Rules.AsteroidObstruction.CheckDefenceDistanceBonus;
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

        newShipContainer.Owner.SquadCost += shipConfig.ShipCost;

        return newShipContainer;
	}

}
