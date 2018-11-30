using Abilities.SecondEdition;
using Arcs;
using BoardTools;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class ScourgeSkutu : TIELnFighter
        {
            public ScourgeSkutu()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Scourge\" Skutu",
                    5,
                    32,
                    isLimited: true,
                    abilityType: typeof(ScourgeSkutuAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 82
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ScourgeSkutuAbility : Abilities.FirstEdition.ScourgeAbility
    {
        protected override void SendExtraDiceMessage()
        {
            Messages.ShowInfo("Defender is in your bullseye arc. Roll an additional attack die.");
        }

        protected override void CheckConditions()
        {
            ShotInfo shotInfo = new ShotInfo(HostShip, Combat.Defender, HostShip.PrimaryWeapon);
            if (shotInfo.InArcByType(ArcType.Bullseye))
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            }
        }
    }
}