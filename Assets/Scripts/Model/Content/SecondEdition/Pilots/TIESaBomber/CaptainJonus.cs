using ActionsList;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class CaptainJonus : TIESaBomber, TIE
        {
            public CaptainJonus() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Captain Jonus",
                    4,
                    36,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainJonusAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 108
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
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
            Combat.Attacker.AddAvailableDiceModification(new CaptainJonusAction() { Host = this.HostShip });
        }

        private class CaptainJonusAction : FriendlyRerollAction
        {
            public CaptainJonusAction() : base(2, 1, true, RerollTypeEnum.AttackDice)
            {
                Name = DiceModificationName = "Captain Jonus's ability";
                IsReroll = true;
            }

            protected override bool CanReRollWithWeaponClass()
            {
                if (Combat.ChosenWeapon is GenericSpecialWeapon)
                {
                    GenericSpecialWeapon upgradeWeapon = Combat.ChosenWeapon as GenericSpecialWeapon;
                    return upgradeWeapon.HasType(UpgradeType.Missile) || upgradeWeapon.HasType(UpgradeType.Torpedo);
                }

                return false;
            }
        }
    }
}