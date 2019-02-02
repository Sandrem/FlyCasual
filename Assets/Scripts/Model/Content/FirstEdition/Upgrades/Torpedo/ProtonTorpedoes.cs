using ActionsList;
using Ship;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class ProtonTorpedoes : GenericSpecialWeapon
    {
        public ProtonTorpedoes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Proton Torpedoes",
                UpgradeType.Torpedo,
                cost: 4,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    spendsToken: typeof(BlueTargetLockToken),
                    discard: true
                ),
                abilityType: typeof(Abilities.FirstEdition.ProtonTorpedoesAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ProtonTorpedoesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddProtonTorpedoesDiceMofification;
        }

        public override void DeactivateAbility()
        {
            // Ability is turned off only after full attack is finished
            HostShip.OnCombatDeactivation += DeactivateAbilityPlanned;
        }

        private void DeactivateAbilityPlanned(GenericShip ship)
        {
            HostShip.OnCombatDeactivation -= DeactivateAbilityPlanned;
            HostShip.OnGenerateDiceModifications -= AddProtonTorpedoesDiceMofification;
        }

        protected virtual void AddProtonTorpedoesDiceMofification(GenericShip host)
        {
            ProtonTorpedoesDiceModification action = new ProtonTorpedoesDiceModification()
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
    public class ProtonTorpedoesDiceModification : GenericAction
    {

        public ProtonTorpedoesDiceModification()
        {
            Name = DiceModificationName = "Proton Torpedoes";

            IsTurnsOneFocusIntoSuccess = true;
        }

        private void ProtonTorpedoesAddDiceModification(GenericShip ship)
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
                int attackFocuses = Combat.DiceRollAttack.Focuses;
                if (attackFocuses > 0) result = 70;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Crit);
            callBack();
        }

    }

}