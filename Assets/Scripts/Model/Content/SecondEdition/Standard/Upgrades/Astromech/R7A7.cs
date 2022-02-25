using Upgrade;
using System.Linq;
using System.Collections.Generic;
using System;
using Ship;
using BoardTools;

namespace UpgradesList.SecondEdition
{
    public class R7A7 : GenericUpgrade
    {
        public R7A7() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R7-A7",
                UpgradeType.Astromech,
                cost: 3,
                isLimited: true,
                charges: 3,
                restriction: new FactionRestriction(Faction.Republic),
                abilityType: typeof(Abilities.SecondEdition.R7A7Ability)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f5/84/f58409a7-8000-4201-a912-014b011521cb/swz80_upgrade_r7-a7.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class R7A7Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "R7-A7",
                CanBeUsed,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Success },
                sideCanBeChangedTo: DieSide.Crit,
                payAbilityCost: SpendCharge
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool CanBeUsed()
        {
            return (Combat.AttackStep == CombatStep.Attack && HostUpgrade.State.Charges > 0);
        }

        private int GetAiPriority()
        {
            return 20;
        }

        private void SpendCharge(Action<bool> callback)
        {
            if (HostUpgrade.State.Charges > 0)
            {
                HostUpgrade.State.SpendCharge();
                callback(true);
            }
            else
            {
                Messages.ShowError("No charges to spend");
                callback(false);
            }
            
        }
    }
}