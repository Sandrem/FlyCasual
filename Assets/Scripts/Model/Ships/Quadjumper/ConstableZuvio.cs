using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;
using RuleSets;
using SubPhases;

namespace Ship
{
    namespace Quadjumper
    {
        public class ConstableZuvio : Quadjumper, ISecondEditionPilot
        {
            public ConstableZuvio() : base()
            {
                PilotName = "Constable Zuvio";
                PilotSkill = 4;
                Cost = 33;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new Abilities.SecondEdition.ConstableZuvioAbilitySE());

                SEImageNumber = 161;
            }

            public void AdaptPilotToSecondEdition()
            {
                
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ConstableZuvioAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.CanLaunchBombsWithTemplate = 1;
        }

        public override void DeactivateAbility()
        {
            HostShip.CanLaunchBombsWithTemplate = 0;
        }
    }
}