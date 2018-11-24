using System.Collections;
using System.Collections.Generic;
using Ship;
using ActionsList;
using Abilities.SecondEdition;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class DelMeeko : TIELnFighter
        {
            public DelMeeko() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Del Meeko",
                    4,
                    30,
                    limited: 1,
                    abilityType: typeof(DelMeekoAbility),
                    extraUpgradeIcon: UpgradeType.Elite,
                    seImageNumber: 85
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
            Combat.Defender.AddAvailableDiceModification(new DelMeekoAction() { Host = this.HostShip });
        }

        private class DelMeekoAction : FriendlyRerollAction
        {
            public DelMeekoAction() : base(1, 2, true, RerollTypeEnum.DefenseDice)
            {
                Name = DiceModificationName = "Del Meeko's ability";
            }

            public override bool IsDiceModificationAvailable()
            {
                if (!Combat.Attacker.Damage.IsDamaged())
                    return false;
                else
                    return base.IsDiceModificationAvailable();
            }
        }
    }
}
