﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Faction
{
    Rebels,
    Empire,
    Scum
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
        Ai
    }

    public partial class GenericPlayer
    {
        public PlayerType Type;
        public string Name;
        public PlayerNo PlayerNo;
        public int SquadCost;
        public Dictionary<string, Ship.GenericShip> Ships = new Dictionary<string, Ship.GenericShip>();

        private int id;
        public int Id { get { return (PlayerNo == PlayerNo.Player1) ? 1 : 2; } }

        public GenericPlayer()
        {
            SetPlayerNo();
        }

        private void SetPlayerNo()
        {
            PlayerNo = (Roster.Players.Count == 0) ? PlayerNo.Player1 : PlayerNo.Player2;
            Roster.Players.Add(this);
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
    }

}
