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
            HostShip.OnCombatPhaseStart += RegisterPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatPhaseStart -= RegisterPilotAbility;
        }

        private void RegisterPilotAbility(GenericShip ship)
        {
            if (TargetsForAbilityExist(HostShip.ShipsBumped.Contains)) {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AssignTokens);
            }
        }

        private void AssignTokens(object sender, System.EventArgs e)
        {
            foreach (GenericShip ship in HostShip.ShipsBumped)
            {
                Triggers.RegisterTrigger(new Trigger() {
                    Name = "Assign tractor beam to " + ship.PilotName,
                    TriggerType = TriggerTypes.OnAbilityDirect,
                    TriggerOwner = HostShip.Owner.PlayerNo,
                    EventHandler = delegate {
                        Messages.ShowError(HostShip.PilotName + " assigns Tractor Beam Token\nto " + TargetShip.PilotName);
                        ship.Tokens.AssignToken(new Tokens.TractorBeamToken(ship, HostShip.Owner), Triggers.FinishTrigger);
                    }
                });
            }

            Triggers.ResolveTriggers (TriggerTypes.OnAbilityDirect, delegate { });
        }
    }
}