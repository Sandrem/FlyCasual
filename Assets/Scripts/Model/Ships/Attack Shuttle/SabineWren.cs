using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using RuleSets;

namespace Ship
{
    namespace AttackShuttle
    {
        public class SabineWren : AttackShuttle, ISecondEditionPilot
        {
            public SabineWren() : base()
            {
                PilotName = "Sabine Wren";
                PilotSkill = 5;
                Cost = 21;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SabineWrenPilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 3;
                Cost = 38;
            }
        }
    }
}

namespace Abilities
{
    public class SabineWrenPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnManeuverIsRevealed += RegisterSabineWrenPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= RegisterSabineWrenPilotAbility;
        }

        private void RegisterSabineWrenPilotAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, PerformFreeReposition);
        }

        private void PerformFreeReposition(object sender, System.EventArgs e)
        {
            List<ActionsList.GenericAction> actions = new List<ActionsList.GenericAction>() { new ActionsList.BoostAction(), new ActionsList.BarrelRollAction() };

            HostShip.AskPerformFreeAction(actions, Triggers.FinishTrigger);
        }
    }
}
