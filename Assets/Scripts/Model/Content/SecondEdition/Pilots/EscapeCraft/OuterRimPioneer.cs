using BoardTools;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EscapeCraft
    {
        public class OuterRimPioneer : EscapeCraft
        {
            public OuterRimPioneer() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Outer Rim Pioneer",
                    3,
                    24,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.OuterRimPioneerAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 227;
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