using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

namespace Ship
{
    namespace AttackShuttle
    {
        public class SabineWren : AttackShuttle
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
        }
    }
}

namespace Abilities
{
    public class SabineWrenPilotAbility : GenericAbility
    {
        public override void Initialize(GenericShip host)
        {
            base.Initialize(host);

            HostShip.OnManeuverIsRevealed += RegisterSabineWrenPilotAbility;
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
