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
                    extraUpgradeIcon: UpgradeType.Talent,
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
            Messages.ShowInfo("The defender is in your bullseye arc, you roll an additional attack die");
        }

        protected override void CheckConditions()
        {
            if (HostShip.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye))
            {
                HostShip.AfterGotNumberOfAttackDice += RollExtraDice;
            }
        }
    }
}