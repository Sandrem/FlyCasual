using ActionsList;
using Ship;
using System;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class FifthBrother : GenericUpgrade
    {
        public FifthBrother() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Fifth Brother",
                UpgradeType.Gunner,
                cost: 9,
                isLimited: true,
                addForce: 1,
                restriction: new FactionRestriction(Faction.Imperial),
                abilityType: typeof(Abilities.SecondEdition.FifthBrotherAbility),
                seImageNumber: 122
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class FifthBrotherAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                HostUpgrade.UpgradeInfo.Name,
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                new System.Collections.Generic.List<DieSide>() { DieSide.Focus },
                DieSide.Crit,
                payAbilityCost: PayForce
            );
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && HostShip.State.Force > 0
                && Combat.CurrentDiceRoll.Focuses > 0;
        }

        private int GetAiPriority()
        {
            return 45;
        }

        private void PayForce(Action<bool> callback)
        {
            if (HostShip.State.Force > 0)
            {
                HostShip.State.Force--;
                callback(true);
            }
            else
            {
                Messages.ShowError(HostUpgrade.UpgradeInfo.Name + ": No Force to spend");
                callback(false);
            }
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}