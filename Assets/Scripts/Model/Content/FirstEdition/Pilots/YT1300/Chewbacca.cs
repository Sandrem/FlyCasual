using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.YT1300
    {
        public class Chewbacca : YT1300
        {
            public Chewbacca() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Chewbacca",
                    5,
                    42,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.ChewbaccaRebelPilotAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Elite, UpgradeType.Missile }
                );

                ShipInfo.ArcInfo.Firepower = 3;
                ShipInfo.Hull = 8;
                ShipInfo.Shields = 5;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class ChewbaccaRebelPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckFaceupCrit += FlipCrits;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckFaceupCrit -= FlipCrits;
        }

        private void FlipCrits(ref bool result)
        {
            if (result == true)
            {
                Messages.ShowInfo("Chewbacca: Crit is flipped facedown");
                Sounds.PlayShipSound("Chewbacca");
                result = false;
            }
        }
    }
}