using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEAdvPrototype
    {
        public class BaronOfTheEmpire : TIEAdvPrototype
        {
            public BaronOfTheEmpire() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Baron of the Empire",
                    4,
                    19
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}