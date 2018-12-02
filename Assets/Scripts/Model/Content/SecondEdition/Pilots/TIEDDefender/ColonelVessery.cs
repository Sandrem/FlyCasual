using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEDDefender
    {
        public class ColonelVessery : TIEDDefender
        {
            public ColonelVessery() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Colonel Vessery",
                    4,
                    88,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.ColonelVesseryAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 123
                );
            }
        }
    }
}