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