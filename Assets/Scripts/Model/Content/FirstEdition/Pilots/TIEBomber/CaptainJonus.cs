﻿using ActionsList;
using Ship;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.TIEBomber
    {
        public class CaptainJonus : TIEBomber
        {
            public CaptainJonus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Jonus",
                    6,
                    22,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.CaptainJonusAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class CaptainJonusAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal += AddCaptainJonusAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnGenerateDiceModificationsGlobal -= AddCaptainJonusAbility;
        }

        private void AddCaptainJonusAbility(GenericShip ship)
        {
            Combat.Attacker.AddAvailableDiceModification(new CaptainJonusAction() { HostShip = this.HostShip });
        }

        private class CaptainJonusAction : FriendlyRerollAction
        {
            public CaptainJonusAction() : base(2, 1, false, RerollTypeEnum.AttackDice)
            {
                Name = DiceModificationName = "Captain Jonus's ability";
                IsReroll = true;
            }

            protected override bool CanReRollWithWeaponClass()
            {
                return Combat.ChosenWeapon is GenericSpecialWeapon;
            }
        }
    }
}
