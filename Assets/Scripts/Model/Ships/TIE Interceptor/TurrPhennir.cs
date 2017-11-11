using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class TurrPhennir : TIEInterceptor
        {
            public TurrPhennir() : base()
            {
                PilotName = "Turr Phennir";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Interceptor/turr-phennir.png";
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

            Host.OnAttackPerformed += RegisterTurrPhennirPilotAbility;
        }

        private void RegisterTurrPhennirPilotAbility()
        {
            RegisterAbilityTrigger(TriggerTypes.OnAttackPerformed, TurrPhennirPilotAbility);
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
