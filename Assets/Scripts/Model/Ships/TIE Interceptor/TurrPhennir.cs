using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using System;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class TurrPhennir : TIEInterceptor
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
        }
    }
}

namespace Abilities
{
    public class TurrPhennirAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinish += RegisterTurrPhennirPilotAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinish += RegisterTurrPhennirPilotAbility;
        }

        private void RegisterTurrPhennirPilotAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, TurrPhennirPilotAbility);
        }

        private void TurrPhennirPilotAbility(object sender, System.EventArgs e)
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
