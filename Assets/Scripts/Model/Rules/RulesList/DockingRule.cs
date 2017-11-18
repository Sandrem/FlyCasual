using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;
using Players;
using GameModes;
using Ship;

namespace RulesList
{
    public class DockingRule
    {
        Dictionary<Func<GenericShip>, Func<GenericShip>> dockedShipsPairs = new Dictionary<Func<GenericShip>, Func<GenericShip>>();

        public DockingRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            Phases.OnSetupPhaseStart += DockShips;
        }

        public void Dock(Func<GenericShip> host, Func<GenericShip> docked)
        {
            if (host != null && docked != null)
            {
                dockedShipsPairs.Add(docked, host);
            }
        }

        private void DockShips()
        {
            foreach (var dockedShipsPair in dockedShipsPairs)
            {
                GenericShip docked = dockedShipsPair.Key();
                GenericShip host = dockedShipsPair.Value();
                if (host != null && docked != null)
                {
                    Roster.HideShip("ShipId:" + docked.ShipId);
                    host.DockedShips.Add(docked);
                    docked.Model.SetActive(false);

                    host.OnMovementExecuted += AskUndock;
                }
            }
        }

        private void AskUndock(GenericShip ship)
        {
            Debug.Log("Ask Undock");
            Undock(ship, ship.DockedShips[0]);
        }

        private void Undock(GenericShip host, GenericShip docked)
        {
            SetUndockPosition(host, docked);

            Roster.ShowShip(docked);
            host.DockedShips.Remove(docked);
            docked.Model.SetActive(true);

            docked.SetAssignedManeuver(new Movement.StraightMovement(2, Movement.ManeuverDirection.Forward, Movement.ManeuverBearing.Straight, Movement.ManeuverColor.White));

            host.OnMovementExecuted -= AskUndock;
        }

        private void SetUndockPosition(GenericShip host, GenericShip docked)
        {
            docked.SetPosition(host.GetBack());
            docked.SetAngles(host.GetAngles() + new Vector3(0, 180, 9));
        }

    }

}