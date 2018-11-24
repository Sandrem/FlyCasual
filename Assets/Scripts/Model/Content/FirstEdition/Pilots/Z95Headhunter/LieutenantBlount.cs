using Upgrade;

namespace Ship
{
    namespace FirstEdition.Z95Headhunter
    {
        public class LieutenantBlount : Z95Headhunter
        {
            public LieutenantBlount() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Lieutenant Blount",
                    6,
                    17,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.LieutenantBlountAbiliity),
                    extraUpgradeIcon: UpgradeType.Elite
                );

                ModelInfo.SkinName = "Red";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class LieutenantBlountAbiliity : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AttackIsAlwaysConsideredHit = true;
        }

        public override void DeactivateAbility()
        {
            HostShip.AttackIsAlwaysConsideredHit = false;
        }
    }
}
