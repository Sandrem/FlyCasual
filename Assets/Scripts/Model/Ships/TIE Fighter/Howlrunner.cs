using ActionsList;
using Ship;

namespace Ship
{
    namespace TIEFighter
    {
        public class Howlrunner : TIEFighter
        {
            public Howlrunner() : base()
            {
                PilotName = "\"Howlrunner\"";
                PilotSkill = 8;
                Cost = 18;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.HowlrunnerAbility());
            }
        }
    }
}

namespace Abilities
{
    public class HowlrunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.AfterGenerateAvailableActionEffectsListGlobal += AddHowlrunnerAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.AfterGenerateAvailableActionEffectsListGlobal -= AddHowlrunnerAbility;
        }

        private void AddHowlrunnerAbility()
        {
            Combat.Attacker.AddAvailableActionEffect(new HowlrunnerAction() { Host = this.HostShip });
        }

        private class HowlrunnerAction : FriendlyRerollAction
        {
            public HowlrunnerAction() : base(1, 1, false, RerollTypeEnum.AttackDice)
            {
                Name = EffectName = "Howlrunner's ability";
                IsReroll = true;
            }

            protected override bool CanReRollWithWeaponClass()
            {
                return Combat.ChosenWeapon.GetType() == typeof(PrimaryWeaponClass);
            }
        }            
    }
}