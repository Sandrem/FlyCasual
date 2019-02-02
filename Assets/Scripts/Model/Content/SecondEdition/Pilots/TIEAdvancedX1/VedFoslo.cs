using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class VedFoslo : TIEAdvancedX1
        {
            public VedFoslo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ved Foslo",
                    4,
                    45,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.JunoEclipseAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 95
                );
            }
        }
    }
}