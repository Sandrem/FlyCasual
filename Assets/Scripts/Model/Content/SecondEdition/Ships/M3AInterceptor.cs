using Movement;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class M3AInterceptor : FirstEdition.M3AInterceptor.M3AInterceptor
        {
            private readonly List<UpgradeType> HardpointSlotTypes = new List<UpgradeType>
            {
                UpgradeType.Cannon,
                UpgradeType.Torpedo,
                UpgradeType.Missile
            };

            public M3AInterceptor() : base()
            {
                ShipInfo.Hull = 3;

                IconicPilots = new Dictionary<Faction, System.Type> {
                    { Faction.Scum, typeof(Inaldra) }
                };

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank), MovementComplexity.Normal);
                DialInfo.AddManeuver(new ManeuverHolder(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight), MovementComplexity.Normal);

                CreateHardpointSlots();

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/2/2a/Maneuver_m3a.png";
            }

            private void CreateHardpointSlots()
            {
                foreach (UpgradeType upgradeType in HardpointSlotTypes)
                {
                    ShipInfo.UpgradeIcons.Upgrades.Add(upgradeType);
                };
            }

            public override void OnPreInstallUpgrade(GenericUpgrade upgrade)
            {
                HardpointSlotTypes
                    .Where(slot => slot != upgrade.UpgradeInfo.UpgradeTypes.First())
                    .ToList()
                    .ForEach(slot => UpgradeBar.RemoveSlot(slot));
            }

            public override void OnRemovePreInstallUpgrade(GenericUpgrade upgrade)
            {
                HardpointSlotTypes
                    .Where(slot => slot != upgrade.UpgradeInfo.UpgradeTypes.First())
                    .ToList()
                    .ForEach(slot => UpgradeBar.AddSlot(slot));
            }
        }
    }
}
