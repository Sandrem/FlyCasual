﻿using ActionsList;
using Ship;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M3AInterceptor
    {
        public class Serissu : M3AInterceptor
        {
            public Serissu() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Serissu",
                    5,
                    40,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.SerissuAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 183
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // When another friendly ship at Range 0-1 is defending, it may reroll 1 defense die.
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

        protected class SerissuAction : FriendlyRerollAction
        {
            public SerissuAction() : base(1, 1, true, RerollTypeEnum.DefenseDice)
            {
                Name = DiceModificationName = "Serissu's ability";
            }
        }
    }
}