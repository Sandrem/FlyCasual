﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

public enum Faction
{
    None,
    Rebel,
    Imperial,
    Scum
}

public enum SubFaction
{
    None,
    RebelAlliance,
    Resistance,
    GalacticEmpire,
    FirstOrder,
    ScumAndVillainy
}

namespace Players
{
    public enum PlayerNo
    {
        Player1,
        Player2
    }

    public enum PlayerType
    {
        Human,
        Ai,
        Network
    }

    public partial class GenericPlayer
    {
        public PlayerType Type;
        public string Name;
        public PlayerNo PlayerNo;
        public int SquadCost;

        public Dictionary<string, GenericShip> Ships = new Dictionary<string, GenericShip>();

        public Dictionary<string, GenericShip> EnemyShips
        {
            get
            {
                return AnotherPlayer.Ships;
            }
        }

        public GenericPlayer AnotherPlayer
        {
            get
            {
                return Roster.GetPlayer(Roster.AnotherPlayer(PlayerNo));
            }
        }

        public int Id { get { return (PlayerNo == PlayerNo.Player1) ? 1 : 2; } }

        public void SetPlayerNo(PlayerNo playerNo)
        {
            PlayerNo = playerNo;
        }

        public virtual void SetupShip() { }

        public virtual void AssignManeuver() { }

        public virtual void PerformManeuver() { }

        public virtual void PerformAction() { }

        public virtual void PerformFreeAction() { }

        public virtual void PerformAttack() { }

        public virtual void UseOwnDiceModifications() { }

        public virtual void UseOppositeDiceModifications() { }

        public virtual void TakeDecision() { }

        public virtual void AfterShipMovementPrediction() { }

        public virtual void ConfirmDiceCheck() { }

        public virtual void ToggleCombatDiceResults(bool isActive) { }

        public virtual bool IsNeedToShowManeuver(GenericShip ship) { return false; }

        public virtual void OnTargetNotLegalForAttack() { }

        public virtual void ChangeManeuver(Action<string> callback, Func<string, bool> filter = null) { }

        public virtual void SelectManeuver(Action<string> callback, Func<string, bool> filter = null) { }

        public virtual void StartExtraAttack() { }
    }

}
