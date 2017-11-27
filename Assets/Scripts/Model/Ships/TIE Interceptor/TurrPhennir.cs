using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;

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

                PilotAbilities.Add(new PilotAbilitiesNamespace.TurrPhennirAbility());
            }
        }
    }
}

namespace PilotAbilitiesNamespace
{
    public class TurrPhennirAbility : GenericPilotAbility
    {
        public override void Initialize(Ship.GenericShip host)
        {
            base.Initialize(host);

            Host.OnAttackFinish += RegisterTurrPhennirPilotAbility;
        }

        private void RegisterTurrPhennirPilotAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, TurrPhennirPilotAbility);
        }

        private void TurrPhennirPilotAbility(object sender, System.EventArgs e)
        {
            Host.AskPerformFreeAction(
                new List<ActionsList.GenericAction>()
                {
                    new ActionsList.BoostAction(),
                    new ActionsList.BarrelRollAction()
                },
                Triggers.FinishTrigger);
        }
    }
}
