using BoardTools;
using Content;
using Ship;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class MajorStridan : UpsilonClassCommandShuttle
        {
            public MajorStridan() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Major Stridan",
                    "Stentorian Commander",
                    Faction.FirstOrder,
                    4,
                    7,
                    13,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.MajorStridanAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Sensor,
                        UpgradeType.Tech,
                        UpgradeType.Tech,
                        UpgradeType.Cannon,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Crew,
                        UpgradeType.Modification
                    },
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MajorStridanAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckRange += CheckRangeModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckRange -= CheckRangeModification;
        }

        private void CheckRangeModification(GenericShip anotherShip, int minRange, int maxRange, RangeCheckReason reason, ref bool isInRange)
        {
            if ((anotherShip.Owner.PlayerNo == HostShip.Owner.PlayerNo)
                && (reason == RangeCheckReason.CoordinateAction || reason == RangeCheckReason.UpgradeCard)
                && (minRange >= 0 || maxRange <= 1)
            )
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, anotherShip);
                if (distInfo.Range >= 2 && distInfo.Range <= 3)
                {
                    isInRange = true;
                }
            }
        }
    }
}
