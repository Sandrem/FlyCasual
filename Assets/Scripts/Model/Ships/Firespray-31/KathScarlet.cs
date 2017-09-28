using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace Firespray31
    {
        public class KathScarletEmpire : Firespray31
        {
            public KathScarletEmpire() : base()
            {
                PilotName = "Kath Scarlet";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/Firespray-31/kath-scarlet.png";
                PilotSkill = 7;
                Cost = 38;

                SkinName = "Kath Scarlet";

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                faction = Faction.Empire;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                OnAtLeastOneCritWasCancelledByDefender += RegisterKathScarletPilotAbility;
            }

            private void RegisterKathScarletPilotAbility()
            {
                Triggers.RegisterTrigger(new Trigger
                {
                    Name = "Kath Scarlet's ability",
                    TriggerType = TriggerTypes.OnAtLeastOneCritWasCancelledByDefender,
                    TriggerOwner = this.Owner.PlayerNo,
                    EventHandler = KathScarletPilotAbility
                });
            }

            private void KathScarletPilotAbility(object sender, System.EventArgs e)
            {
                Messages.ShowInfo("Critical hit was cancelled - stress token is assigned to the defender");
                Combat.Defender.AssignToken(new Tokens.StressToken(), Triggers.FinishTrigger);
            }
        }
    }
}
