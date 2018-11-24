using Abilities.FirstEdition;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class MaulerMithel : TIELnFighter
        {
            public MaulerMithel() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "\"Mauler\" Mithel",
                    5,
                    32,
                    limited: 1,
                    abilityType: typeof(MaulerMithelAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 80
                );
            }
        }
    }
}