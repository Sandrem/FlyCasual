using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Ship;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class BarrageRockets : GenericSecondaryWeapon
    {   
        public BarrageRockets() : base()
        {
            Name = "Barrage Rockets";

            Types.Add( UpgradeType.Missile);
            Types.Add( UpgradeType.Missile);

            Cost        = 6;

            MinRange    = 2;
            MaxRange    = 3;
            AttackValue = 3;

            RequiresFocusToShoot = true;

            UsesCharges = true;
            MaxCharges = 5;

            UpgradeRuleType = typeof(RuleSets.SecondEdition);

            UpgradeAbilities.Add(new Abilities.SecondEdition.BarrageRockets());

            SEImageNumber = 36;
        }
    }   
}

namespace Abilities.SecondEdition
{
    public class BarrageRockets : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.Name,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                GetRerollCount,
                payAbilityPostCost: PayAbilityCost
            );
        }

        private bool IsAvailable()
        {
            return Combat.ChosenWeapon == this
                && Combat.ShotInfo.InArcByType(Arcs.ArcTypes.Bullseye)
                && HostUpgrade.Charges > 0
                && Combat.AttackStep == CombatStep.Attack;
        }

        private int GetAiPriority()
        {
            return 81; // Just a bit higher than TL
        }

        private int GetRerollCount()
        {
            return HostUpgrade.Charges;
        }

        private void PayAbilityCost()
        {
            for (int i = 0; i < DiceRoll.CurrentDiceRoll.WasSelectedCount; i++)
            {
                HostUpgrade.SpendCharge();
            }
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}
