using ActionsList;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace T70XWing
    {
        public class SnapWexley : T70XWing
        {
            public SnapWexley() : base()
            {
                PilotName = "\"Snap\" Wexley";
                PilotSkill = 6;
                Cost = 28;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
                PilotAbilities.Add(new Abilities.SnapWexleyAbility());
            }
        }
    }
}

namespace Abilities
{
    //After you execute a 2-, 3-, or 4-speed maneuver, if you are not touching a ship, you may perform a free boost action.
    public class SnapWexleyAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterSnapAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterSnapAbility;
        }

        private void RegisterSnapAbility(GenericShip hostShip)
        {
            if (Selection.ThisShip.IsBumped) return;

            int assignedSpeed = HostShip.AssignedManeuver.Speed;
            if (assignedSpeed < 2 || assignedSpeed > 4) return;

            if (Board.BoardManager.IsOffTheBoard(hostShip)) return;

            RegisterAbilityTrigger(TriggerTypes.OnShipMovementFinish, PerformFreeBoost);
        }

        private void PerformFreeBoost(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(new List<GenericAction> { new BoostAction() }, Triggers.FinishTrigger);
        }
    }
}
