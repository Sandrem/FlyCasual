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
        private List<GenericShip> shipsToAssignTractorBeamTokens;

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
            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AssignTokens);
        }

        private void AssignTokens(object sender, System.EventArgs e)
        {
            shipsToAssignTractorBeamTokens = new List<GenericShip>(HostShip.ShipsBumped);
            AssignTokensRecursive();
        }

        private void AssignTokensRecursive()
        {
            if (shipsToAssignTractorBeamTokens.Count > 0)
            {
                GenericShip shipToAssign = shipsToAssignTractorBeamTokens[0];
                shipsToAssignTractorBeamTokens.Remove(shipToAssign);
                Messages.ShowErrorToHuman(shipToAssign.PilotName + " is bumped into \"Unkar Plutt\" and gets a Tractor Beam token");
                shipToAssign.Tokens.AssignToken(new Tokens.TractorBeamToken(shipToAssign, HostShip.Owner), delegate {
                    // TODO Figure out how this should work, how do we hook into after the tractor beam effect triggers?
                    Triggers.FinishTrigger();
                });
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}