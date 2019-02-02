using Upgrade;

namespace Ship
{
    namespace SecondEdition.LancerClassPursuitCraft
    {
        public class KetsuOnyo : LancerClassPursuitCraft
        {
            public KetsuOnyo() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ketsu Onyo",
                    5,
                    70,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.KetsuOnyoPilotAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 218
                );
            }
        }
    }
}