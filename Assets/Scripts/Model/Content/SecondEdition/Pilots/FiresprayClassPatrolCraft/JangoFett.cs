using Upgrade;

namespace Ship
{
    namespace SecondEdition.FiresprayClassPatrolCraft
    {
        public class JangoFett : FiresprayClassPatrolCraft
        {
            public JangoFett() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Jango Fett",
                    6,
                    90,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.JangoFettAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    factionOverride: Faction.Separatists
                );

                ModelInfo.SkinName = "Jango Fett";

                ImageUrl = "https://i.imgur.com/EckqXet.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class JangoFettAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            
        }

        public override void DeactivateAbility()
        {
            
        }
    }
}