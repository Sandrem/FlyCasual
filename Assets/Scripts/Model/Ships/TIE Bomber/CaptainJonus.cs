using ActionsList;
using RuleSets;
using Ship;
using Upgrade;

namespace Ship
{
    namespace TIEBomber
    {
        public class CaptainJonus : TIEBomber, ISecondEditionPilot
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

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 36;

                PilotAbilities.RemoveAll(ability => ability is Abilities.CaptainJonusAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.CaptainJonusAbilitySE());
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

        private void AddCaptainJonusAbility(GenericShip ship)
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

namespace Abilities.SecondEdition
{
    public class CaptainJonusAbilitySE : GenericAbility
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
                if(Combat.ChosenWeapon is GenericSecondaryWeapon)
                {
                    GenericSecondaryWeapon upgradeWeapon = Combat.ChosenWeapon as GenericSecondaryWeapon;
                    return upgradeWeapon.HasType(UpgradeType.Missile) || upgradeWeapon.HasType(UpgradeType.Torpedo);
                }

                return false;
            }
        }
    }
}