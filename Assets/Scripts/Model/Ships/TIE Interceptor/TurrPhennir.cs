using System.Collections.Generic;
using Ship;
using System;
using RuleSets;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class TurrPhennir : TIEInterceptor, ISecondEditionPilot
        {
            public TurrPhennir() : base()
            {
                PilotName = "Turr Phennir";
                PilotSkill = 7;
                Cost = 25;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Red Stripes";

                PilotAbilities.Add(new Abilities.TurrPhennirAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 44;

                PilotAbilities.RemoveAll(ability => ability is Abilities.TurrPhennirAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.TurrPhennirAbility());
            }
        }        
    }
}

namespace Abilities
{
    //After you perform an attack, you may perform a free boost or barrel roll action.
    public class TurrPhennirAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinish += RegisterTurrPhennirPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinish -= RegisterTurrPhennirPilotAbility;
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
    public class TurrPhennirAbility : Abilities.TurrPhennirAbility
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
