using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;

namespace Ship
{
    namespace Quadjumper
    {
        public class UnkarPlutt : Quadjumper
        {
            public UnkarPlutt() : base()
            {
                PilotName = "Unkar Plutt";
                PilotSkill = 3;
                Cost = 17;

                PilotAbilities.Add (new UnkarPluttAbility());
            }
        }
    }
}

namespace Abilities
{
    public class UnkarPluttAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCombatPhaseStart += TryRegisterPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatPhaseStart -= TryRegisterPilotAbility;
        }

        private void TryRegisterPilotAbility(GenericShip ship)
        {
            if (TargetsForAbilityExist(HostShip.ShipsBumped.Contains))
            {
                AssignTokensToBumpedEnemies();
            }
        }

        private void AssignTokensToBumpedEnemies()
        {
            foreach (GenericShip ship in HostShip.ShipsBumped)
            {
                Triggers.RegisterTrigger(new Trigger() {
                    Name = "Assign tractor beam to " + ship.PilotName,
                    TriggerType = TriggerTypes.OnCombatPhaseStart,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = delegate { AssignTractorBeamToken(ship); }
                });
            }
        }

        private void AssignTractorBeamToken(GenericShip bumpedShip)
        {
            Messages.ShowError(HostShip.PilotName + " assigns Tractor Beam Token\nto " + bumpedShip.PilotName);
            bumpedShip.Tokens.AssignToken(new Tokens.TractorBeamToken(bumpedShip, HostShip.Owner), Triggers.FinishTrigger);
        }
    }
}