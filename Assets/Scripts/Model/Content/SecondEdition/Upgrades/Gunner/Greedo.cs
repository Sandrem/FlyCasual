using Upgrade;
using System.Collections.Generic;
using System;

namespace UpgradesList.SecondEdition
{
    public class Greedo : GenericUpgrade
    {
        public Greedo() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Greedo",
                UpgradeType.Gunner,
                cost: 1,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.GreedoGunnerAbility),
                restriction: new FactionRestriction(Faction.Scum),
                charges: 1,
                seImageNumber: 142
            );
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
                HostUpgrade.UpgradeInfo.Name,
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
            HostUpgrade.State.RestoreCharge();
        }

        private bool IsDiceModificationAvailable()
        {
            return (HostUpgrade.State.Charges > 0 && Combat.AttackStep == CombatStep.Attack && (Combat.Attacker == HostShip || Combat.Defender == HostShip));
        }

        private int GetDiceModificationAiPriority()
        {
            return 20;
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            HostUpgrade.State.SpendCharge();
            callback(true);
        }
    }
}