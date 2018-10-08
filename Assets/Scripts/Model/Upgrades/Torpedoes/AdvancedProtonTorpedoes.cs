using Abilities;
using ActionsList;
using RuleSets;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class AdvancedProtonTorpedoes : GenericSecondaryWeapon, ISecondEditionUpgrade
    {
        public AdvancedProtonTorpedoes() : base()
        {
            Types.Add(UpgradeType.Torpedo);

            Name = "Advanced Proton Torpedoes";
            Cost = 6;

            MinRange = 1;
            MaxRange = 1;
            AttackValue = 5;

            RequiresTargetLockOnTargetToShoot = true;
            SpendsTargetLockOnTargetToShoot = true;
            IsDiscardedForShot = true;

            UpgradeAbilities.Add(new AdvancedProtonTorpedoesAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Name = "Adv. Proton Torpedoes";
            MaxCharges = 1;

            SpendsTargetLockOnTargetToShoot = false;
            IsDiscardedForShot = false;
            UsesCharges = true;

            UpgradeAbilities.RemoveAll(a => a is AdvancedProtonTorpedoesAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.AdvProtonTorpedoesAbilitySE());

            SEImageNumber = 33;
        }
    }
}

namespace Abilities
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
                Host = host,
                ImageUrl = HostUpgrade.ImageUrl,
                Source = HostUpgrade
            };
            host.AddAvailableDiceModification(action);
        }
    }

    namespace SecondEdition
    {
        //Attack(lock) : Spend 1 charge. Change 1 hit result to a crit result.
        public class AdvProtonTorpedoesAbilitySE : GenericAbility
        {
            public override void ActivateAbility()
            {
                AddDiceModification(
                    HostUpgrade.Name,
                    IsDiceModificationAvailable,
                    GetDiceModificationAiPriority,
                    DiceModificationType.Change,
                    1,
                    new List<DieSide>() { DieSide.Success },
                    DieSide.Crit
                );
            }

            public override void DeactivateAbility()
            {
                RemoveDiceModification();
            }

            private bool IsDiceModificationAvailable()
            {
                return HostShip.IsAttacking && Combat.ChosenWeapon == HostUpgrade;
            }

            private int GetDiceModificationAiPriority()
            {
                return 30;
            }
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