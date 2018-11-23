using RuleSets;
using Ship;
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
	public static GenericShip SpawnShip(SquadBuilderShip shipConfig) {

        //temporary
        int id = 1;
        Vector3 position = Vector3.zero;

        GenericShip newShipContainer = shipConfig.Instance;
        newShipContainer.InitializeGenericShip(shipConfig.List.PlayerNo, id, position);

        Roster.SubscribeSelectionByInfoPanel(newShipContainer.InfoPanel.transform.Find("ShipInfo").gameObject);
        Roster.SubscribeUpgradesPanel(newShipContainer, newShipContainer.InfoPanel);

        //TODO: Rework this
        newShipContainer.AfterGotNumberOfAttackDice += Rules.DistanceBonus.CheckAttackDistanceBonus;
        newShipContainer.AfterGotNumberOfDefenceDice += Rules.DistanceBonus.CheckDefenceDistanceBonus;
        newShipContainer.AfterGotNumberOfDefenceDice += Rules.AsteroidObstruction.CheckDefenceObstructionBonus;
        newShipContainer.OnTryAddAction += Rules.Stress.CanPerformActions;
        newShipContainer.OnTryAddAction += Rules.Actions.CanPerformActions;
        newShipContainer.OnMovementStart += MovementTemplates.ApplyMovementRuler;
        newShipContainer.OnMovementStart += MovementTemplates.CallReturnRangeRuler;
        newShipContainer.OnPositionFinish += Rules.OffTheBoard.CheckOffTheBoard;
        newShipContainer.OnMovementExecuted += Rules.Stress.PlanCheckStress;
        newShipContainer.AfterGetManeuverAvailablity += Rules.Stress.CannotPerformRedManeuversWhileStressed;
        newShipContainer.OnGenerateDiceModifications += Rules.Force.AddForceAction;
        newShipContainer.OnRoundEnd += Rules.Force.RegenerateForce;
        newShipContainer.OnRoundEnd += Rules.Charge.RegenerateCharge;
        newShipContainer.OnRoundEnd += Rules.BonusAttack.ResetCanBonusAttack;
        newShipContainer.OnShipIsDestroyed += Rules.TargetLocks.RegisterRemoveTargetLocksOnDestruction;
        newShipContainer.OnActionIsPerformed += Rules.Actions.RedActionCheck;
        newShipContainer.OnActionIsPerformed += Rules.Actions.CheckLinkedAction;

        newShipContainer.OnTokenIsAssigned += Roster.UpdateTokensIndicator;
        newShipContainer.OnTokenIsRemoved += Roster.UpdateTokensIndicator;
        newShipContainer.OnConditionIsAssigned += Roster.UpdateTokensIndicator;
        newShipContainer.OnConditionIsRemoved += Roster.UpdateTokensIndicator;
        newShipContainer.AfterAssignedDamageIsChanged += Roster.UpdateRosterHullDamageIndicators;
        newShipContainer.AfterAssignedDamageIsChanged += Roster.UpdateRosterShieldsDamageIndicators;
        newShipContainer.AfterStatsAreChanged += Roster.UpdateShipStats;

        Edition.Instance.SubScribeToGenericShipEvents(newShipContainer);

        return newShipContainer;
	}

}
