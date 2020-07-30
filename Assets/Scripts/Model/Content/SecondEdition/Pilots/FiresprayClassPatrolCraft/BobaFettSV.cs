using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class BobaFettSV : FiresprayClassPatrolCraft
        {
            public BobaFettSV() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Boba Fett",
                    5,
                    86,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.BobaFettScumAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 149
                );

                ModelInfo.SkinName = "Boba Fett";
            }
        }
    }
}