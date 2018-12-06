using ActionsList;
using Ship;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.M3AInterceptor
    {
        public class Serissu : M3AInterceptor
        {
            public Serissu() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Serissu",
                    8,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.SerissuAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );

                ModelInfo.SkinName = "Serissu";
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    // When another friendly ship at Range 1 is defending, it may reroll 1 defense die.
    public class SerissuAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddSerissuAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddSerissuAbility;
        }

        private void AddSerissuAbility(GenericShip ship)
        {
            Combat.Defender.AddAvailableDiceModification(new SerissuAction() { HostShip = this.HostShip });
        }

        private class SerissuAction : FriendlyRerollAction
        {
            public SerissuAction() : base(1, 1, false, RerollTypeEnum.DefenseDice)
            {
                Name = DiceModificationName = "Serissu's ability";
            }
        }
    }
}
