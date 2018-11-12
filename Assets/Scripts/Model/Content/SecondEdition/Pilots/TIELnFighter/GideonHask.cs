using System.Collections;
using System.Collections.Generic;
using Abilities.SecondEdition;
using BoardTools;
using Arcs;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class GideonHask : TIELnFighter
        {
            public GideonHask() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Gideon Hask",
                    4,
                    30,
                    limited: 1,
                    abilityType: typeof(GideonHaskAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 84;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GideonHaskAbility : Abilities.FirstEdition.ScourgeAbility
    {
        protected override void SendExtraDiceMessage()
        {
            Messages.ShowInfo("Defender is in your bullseye arc. Roll an additional attack die.");
        }

        protected override void CheckConditions()
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, Combat.Defender, HostShip.PrimaryWeapon);
            if (shotInfo.InArcByType(ArcTypes.Bullseye))
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            }
        }
    }
}