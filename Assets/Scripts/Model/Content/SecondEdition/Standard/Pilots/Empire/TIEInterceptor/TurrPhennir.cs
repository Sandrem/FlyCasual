using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class TurrPhennir : TIEInterceptor
        {
            public TurrPhennir() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Turr Phennir",
                    "Ambitious Ace",
                    Faction.Imperial,
                    4,
                    4,
                    9,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TurrPhennirAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 104,
                    skinName: "Red Stripes"
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an attack, you may perform a roll or boost action, even if you are stressed.

    public class TurrPhennirAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += RegisterTurrPhennirPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= RegisterTurrPhennirPilotAbility;
        }

        private void RegisterTurrPhennirPilotAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, TurrPhennirPilotAbility);
        }

        private void TurrPhennirPilotAbility(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new List<ActionsList.GenericAction>()
                {
                new ActionsList.BoostAction() { CanBePerformedWhileStressed = true },
                new ActionsList.BarrelRollAction() { CanBePerformedWhileStressed = true }
                },
                Triggers.FinishTrigger,
                HostShip.PilotInfo.PilotName,
                "After you perform an attack, you may perform a Barrel Roll or Boost action, even if you are stressed",
                HostShip
            );
        }
    }
}