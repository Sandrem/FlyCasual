using Upgrade;

namespace Ship
{
    namespace SecondEdition.VCX100LightFreighter
    {
        public class HeraSyndulla : VCX100LightFreighter
        {
            public HeraSyndulla() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Hera Syndulla",
                    5,
                    73,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.HeraSyndullaAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 73
                );
            }
        }
    }
}