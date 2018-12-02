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
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.EzraBridgerPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}