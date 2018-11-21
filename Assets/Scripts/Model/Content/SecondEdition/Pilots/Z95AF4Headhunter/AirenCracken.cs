using Ship;
using SubPhases;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class AirenCracken : Z95AF4Headhunter
        {
            public AirenCracken() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Airen Cracken",
                    5,
                    36,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.AirenCrackenAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 27;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class AirenCrackenAbility : Abilities.FirstEdition.AirenCrackenAbiliity
    {
        protected override void PerformFreeAction()
        {
            Selection.ThisShip = TargetShip;

            TargetShip.AskPerformFreeAction(
                TargetShip.GetAvailableActionsAsRed(),
                delegate {
                    Selection.ThisShip = HostShip;
                    SelectShipSubPhase.FinishSelection();
                });
        }
    }
}
