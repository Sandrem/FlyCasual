using Upgrade;

namespace Ship
{
    namespace FirstEdition.SheathipedeClassShuttle
    {
        public class EzraBridger : SheathipedeClassShuttle
        {
            public EzraBridger() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ezra Bridger",
                    5,
                    17,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.EzraBridgerPilotAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
            }
        }
    }
}