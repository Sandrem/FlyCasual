using Abilities.SecondEdition;
using RuleSets;
using System.Collections;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace M3AScyk
    {
        public class LaetinAshera : M3AScyk, ISecondEditionPilot
        {
            public LaetinAshera() : base()
            {
                PilotName = "Laetin A'shera";
                PilotSkill = 3;
                Cost = 35;

                IsUnique = true;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new LaetinAsheraSE());
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LaetinAsheraSE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += RegisterTrigger;
            HostShip.OnAttackMissedAsDefender += RegisterTrigger;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= RegisterTrigger;
            HostShip.OnAttackMissedAsDefender -= RegisterTrigger;
        }

        private void RegisterTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackMissed, delegate 
            {
                HostShip.Tokens.AssignToken(typeof(EvadeToken), Triggers.FinishTrigger);
            });
        }
    }
}