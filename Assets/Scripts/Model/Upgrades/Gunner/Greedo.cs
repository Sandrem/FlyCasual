using Abilities.SecondEdition;
using RuleSets;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{
    public class Greedo : GenericUpgrade, ISecondEditionUpgrade
    {
        public Greedo() : base()
        {
            Types.Add(UpgradeType.Gunner);
            Name = "Greedo";
            Cost = 1;

            UsesCharges = true;
            MaxCharges = 1;

            isUnique = true;
            UpgradeRuleType = typeof(SecondEdition);

            UpgradeAbilities.Add(new GreedoGunnerAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            //Nothing to do, already second edition upgrade
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GreedoGunnerAbility : GenericAbility
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
                DieSide.Crit,
                isGlobal: true,
                payAbilityCost: PayAbilityCost
            );

            Phases.Events.OnEndPhaseStart_NoTriggers += RestoreCharge;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
            Phases.Events.OnEndPhaseStart_NoTriggers -= RestoreCharge;
        }

        private void RestoreCharge()
        {
            HostUpgrade.RestoreCharge();
        }

        private bool IsDiceModificationAvailable()
        {
            return (HostUpgrade.Charges > 0 && Combat.AttackStep == CombatStep.Attack && (Combat.Attacker == HostShip || Combat.Defender == HostShip));
        }

        private int GetDiceModificationAiPriority()
        {
            return 20;
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            HostUpgrade.SpendCharge(() => callback(true));
        }
    }
}
