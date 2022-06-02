using System.Collections.Generic;
using Ship;
using ActionsList;
using Abilities.SecondEdition;
using Upgrade;
using Content;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class DelMeeko : TIELnFighter
        {
            public DelMeeko() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Del Meeko",
                    "Inferno Three",
                    Faction.Imperial,
                    4,
                    3,
                    6,
                    isLimited: true,
                    abilityType: typeof(DelMeekoAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    seImageNumber: 85,
                    skinName: "Inferno"
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // When another friendly ship at Range 2 is defending against a damaged ship, it may reroll 1 defense die.
    public class DelMeekoAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddDelMeekoAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddDelMeekoAbility;
        }

        private void AddDelMeekoAbility(GenericShip ship)
        {
            Combat.Defender.AddAvailableDiceModification(
                new DelMeekoAction() {
                    ImageUrl = HostShip.ImageUrl
                },
                HostShip
            );
        }

        private class DelMeekoAction : FriendlyRerollAction
        {
            public DelMeekoAction() : base(1, 2, true, RerollTypeEnum.DefenseDice)
            {
                Name = DiceModificationName = "Del Meeko";
            }

            public override bool IsDiceModificationAvailable()
            {
                if (!Combat.Attacker.Damage.IsDamaged)
                    return false;
                else
                    return base.IsDiceModificationAvailable();
            }
        }
    }
}
