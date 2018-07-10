using ActionsList;
using Ship;
using Upgrade;

namespace Ship
{
    namespace TIEBomber
    {
        public class CaptainJonus : TIEBomber
        {
            public CaptainJonus() : base()
            {
                PilotName = "Captain Jonus";
                PilotSkill = 6;
                Cost = 22;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.CaptainJonusAbility());
            }
        }
    }
}

namespace Abilities
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

        private void AddCaptainJonusAbility()
        {
            Combat.Attacker.AddAvailableDiceModification(new CaptainJonusAction() { Host = this.HostShip });
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
                return Combat.ChosenWeapon is GenericSecondaryWeapon;
            }
        }
    }
}
