using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;
using Ship;
using ActionsList;
using RuleSets;

namespace UpgradesList
{
    public class ProtonTorpedoes : GenericSecondaryWeapon, ISecondEditionUpgrade
    {
        public ProtonTorpedoes() : base()
        {
            Types.Add(UpgradeType.Torpedo);

            Name = "Proton Torpedoes";
            Cost = 4;

            MinRange = 2;
            MaxRange = 3;
            AttackValue = 4;

            RequiresTargetLockOnTargetToShoot = true;
            
            SpendsTargetLockOnTargetToShoot = true;
            IsDiscardedForShot = true;

            UpgradeAbilities.Add(new ProtonTorpedoesAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            MaxCharges = 2;

            SpendsTargetLockOnTargetToShoot = false;

            IsDiscardedForShot = false;
            UsesCharges = true;

            UpgradeAbilities.RemoveAll(a => a is ProtonTorpedoesAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.ProtonTorpedoesAbilitySE());
        }
    }
}

namespace Abilities
{
    namespace SecondEdition
    {
        public class ProtonTorpedoesAbilitySE: ProtonTorpedoesAbility
        {
            protected override void AddProtonTorpedoesDiceMofification(GenericShip host)
            {
                ProtonTorpedoesDiceModificationSE action = new ProtonTorpedoesDiceModificationSE()
                {
                    Host = host,
                    ImageUrl = HostUpgrade.ImageUrl,
                    Source = HostUpgrade
                };

                host.AddAvailableDiceModification(action);
            }
        }
    }
}

namespace ActionsList
{
    public class ProtonTorpedoesDiceModificationSE : ProtonTorpedoesDiceModification
    {
        public ProtonTorpedoesDiceModificationSE()
        {
            IsTurnsOneFocusIntoSuccess = false;
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

namespace Abilities
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
                Host = host,
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
