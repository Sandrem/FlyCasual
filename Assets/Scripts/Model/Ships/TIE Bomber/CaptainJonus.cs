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
            GenericShip.AfterGenerateAvailableActionEffectsListGlobal += AddCaptainJonusAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.AfterGenerateAvailableActionEffectsListGlobal -= AddCaptainJonusAbility;
        }

        private void AddCaptainJonusAbility()
        {
            Combat.Attacker.AddAvailableActionEffect(new CaptainJonusAction() { Host = this.HostShip });
        }

        private class CaptainJonusAction : FriendlyRerollAction
        {
            public CaptainJonusAction() : base(2, 1, false, RerollTypeEnum.AttackDice)
            {
                Name = EffectName = "Captain Jonus's ability";
                IsReroll = true;
            }

            protected override bool CanReRollWithWeaponClass()
            {
                return Combat.ChosenWeapon is GenericSecondaryWeapon;
            }
        }
    }
}
