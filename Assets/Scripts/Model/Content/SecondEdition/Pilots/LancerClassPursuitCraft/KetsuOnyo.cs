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
                    74,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.KetsuOnyoPilotAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 218
                );
            }
        }
    }
}