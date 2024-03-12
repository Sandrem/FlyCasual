using ActionsList;
using Ship;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class ProtonTorpedoes : GenericSpecialWeapon
    {
        public ProtonTorpedoes() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Proton Torpedoes",
                UpgradeType.Torpedo,
                cost: 12,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 4,
                    minRange: 2,
                    maxRange: 3,
                    charges: 2,
                    requiresToken: typeof(BlueTargetLockToken)
                ),
                abilityType: typeof(Abilities.SecondEdition.ProtonTorpedoesAbility),
                seImageNumber: 35
            );
        }        
    }
}

namespace Abilities.SecondEdition
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
            ProtonTorpedoesDiceModificationSE action = new ProtonTorpedoesDiceModificationSE()
            {
                HostShip = host,
                ImageUrl = HostUpgrade.ImageUrl,
                Source = HostUpgrade
            };

            host.AddAvailableDiceModificationOwn(action);
        }
    }
}



namespace ActionsList
{
    public class ProtonTorpedoesDiceModificationSE : ProtonTorpedoesDiceModificationFE
    {
        public ProtonTorpedoesDiceModificationSE()
        {
            IsTurnsOneFocusIntoSuccess = false;
        }

        public override bool IsDiceModificationAvailable()
        {
            return base.IsDiceModificationAvailable() && Combat.CurrentDiceRoll.HasResult(DieSide.Success);
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack)
            {
                if (Combat.DiceRollAttack.RegularSuccesses > 0) result = 100;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Success, DieSide.Crit);
            callBack();
        }
    }
}

namespace ActionsList
{
    public class ProtonTorpedoesDiceModificationFE : GenericAction
    {

        public ProtonTorpedoesDiceModificationFE()
        {
            Name = DiceModificationName = "Proton Torpedoes";

            IsTurnsOneFocusIntoSuccess = true;
        }

        private void ProtonTorpedoesAddDiceModification(GenericShip ship)
        {
            ship.AddAvailableDiceModificationOwn(this);
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