using Ship;
using System;
using System.Collections.Generic;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class TurrPhennir : TIEInterceptor
        {
            public TurrPhennir() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Turr Phennir",
                    4,
                    44,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.TurrPhennirAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                ModelInfo.SkinName = "Red Stripes";

                SEImageNumber = 104;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    //After you perform an attack, you may perform a free boost or barrel roll action.
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

        protected virtual void TurrPhennirPilotAbility(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new List<ActionsList.GenericAction>()
                {
                    new ActionsList.BoostAction(),
                    new ActionsList.BarrelRollAction()
                },
                Triggers.FinishTrigger);
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an attack, you may perform a roll or boost action, even if you are stressed.
    public class TurrPhennirAbility : Abilities.FirstEdition.TurrPhennirAbility
    {
        protected override void TurrPhennirPilotAbility(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction(
                new List<ActionsList.GenericAction>()
                {
                    new ActionsList.BoostAction() { CanBePerformedWhileStressed = true },
                    new ActionsList.BarrelRollAction() { CanBePerformedWhileStressed = true }
                },
                Triggers.FinishTrigger);
        }
    }
}