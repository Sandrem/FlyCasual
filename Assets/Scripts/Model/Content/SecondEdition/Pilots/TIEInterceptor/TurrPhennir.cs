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
                PilotInfo = new PilotCardInfo(
                    "Turr Phennir",
                    4,
                    44,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.TurrPhennirAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 104
                );

                ModelInfo.SkinName = "Red Stripes";
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