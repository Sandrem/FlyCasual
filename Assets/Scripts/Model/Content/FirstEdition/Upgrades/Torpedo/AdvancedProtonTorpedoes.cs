using ActionsList;
using Ship;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class AdvancedProtonTorpedoes : GenericSpecialWeapon
    {
        public AdvancedProtonTorpedoes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Advanced Proton Torpedoes",
                UpgradeType.Torpedo,
                cost: 6,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 5,
                    minRange: 1,
                    maxRange: 1,
                    requiresToken: typeof(BlueTargetLockToken),
                    spendsToken: typeof(BlueTargetLockToken),
                    discard: true
                ),
                abilityType: typeof(Abilities.FirstEdition.AdvancedProtonTorpedoesAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class AdvancedProtonTorpedoesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddDiceModification;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnGenerateDiceModifications -= AddDiceModification;
        }

        private void AddDiceModification(GenericShip host)
        {
            AdvancedProtonTorpedoesAction action = new AdvancedProtonTorpedoesAction()
            {
                HostShip = host,
                ImageUrl = HostUpgrade.ImageUrl,
                Source = HostUpgrade
            };
            host.AddAvailableDiceModification(action);
        }
    }
}

namespace ActionsList
{
    public class AdvancedProtonTorpedoesAction : GenericAction
    {
        public AdvancedProtonTorpedoesAction()
        {
            Name = DiceModificationName = "Advanced Proton Torpedoes";

            IsTurnsOneFocusIntoSuccess = true;
        }

        private void AdvancedProtonTorpedoesAddDiceModification(Ship.GenericShip ship)
        {
            ship.AddAvailableDiceModification(this);
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = true;

            if (Combat.AttackStep != CombatStep.Attack) result = false;

            if (Combat.ChosenWeapon != Source) result = false;

            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int blanks = Combat.DiceRollAttack.Blanks;
                if (blanks > 0) result = 100;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.Change(DieSide.Blank, DieSide.Focus, 3);
            callBack();
        }
    }
}