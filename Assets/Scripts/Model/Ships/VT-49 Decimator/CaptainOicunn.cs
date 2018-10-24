using Abilities.SecondEdition;
using RuleSets;
using Ship;

namespace Ship
{
    namespace VT49Decimator
    {
        public class CaptainOicunn : VT49Decimator, ISecondEditionPilot
        {
            public CaptainOicunn() : base()
            {
                PilotName = "Captain Oicunn";
                PilotSkill = 3;
                Cost = 84;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new CaptainOicunnAbilitySE());

                SEImageNumber = 146;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not needed
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainOicunnAbilitySE : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnCanAttackBumpedTarget += CanAttack;
            HostShip.PrimaryWeapon.MinRange = 0;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCanAttackBumpedTarget -= CanAttack;
            HostShip.PrimaryWeapon.MinRange = 1;
        }

        private void CanAttack(ref bool canAttack, GenericShip attacker, GenericShip defender)
        {
            canAttack = true;
        }

    }
}