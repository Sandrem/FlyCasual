using Abilities.FirstEdition;

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
                    abilityType: typeof(MaulerMithelAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(Upgrade.UpgradeType.Elite);

                SEImageNumber = 80;
            }
        }
    }
}