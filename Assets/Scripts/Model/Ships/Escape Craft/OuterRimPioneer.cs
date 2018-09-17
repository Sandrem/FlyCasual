using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using RuleSets;
using BoardTools;

namespace Ship
{
    namespace EscapeCraft
    {
        public class OuterRimPioneer : EscapeCraft, ISecondEditionPilot
        {
            public OuterRimPioneer() : base()
            {
                PilotName = "Outer Rim Pioneer";
                PilotSkill = 3;
                Cost = 24;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.OuterRimPioneerAbility());

                SEImageNumber = 227;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class OuterRimPioneerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnCanAttackWhileLandedOnObstacleGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnCanAttackWhileLandedOnObstacleGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, ref bool canAttack)
        {
            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo)
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
                if (distInfo.Range < 2) canAttack = true;
            }
        }
    }
}