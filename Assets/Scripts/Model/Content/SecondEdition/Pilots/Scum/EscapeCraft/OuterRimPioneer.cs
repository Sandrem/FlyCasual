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
                PilotInfo = new PilotCardInfo25
                (
                    "Outer Rim Pioneer",
                    "Skillful Outlaw",
                    Faction.Scum,
                    3,
                    3,
                    4,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.OuterRimPioneerAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    },
                    seImageNumber: 227
                );

                ShipAbilities.Add(new Abilities.SecondEdition.CoPilotAbility());
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