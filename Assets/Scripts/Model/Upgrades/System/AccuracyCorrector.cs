using Ship;
using Upgrade;
using Abilities;
using ActionsList;
using System;

namespace UpgradesList
{
    public class AccuracyCorrector : GenericUpgrade
    {
        public AccuracyCorrector() : base()
        {
            Types.Add(UpgradeType.System);
            Name = "Accuracy Corrector";
            Cost = 3;

            UpgradeAbilities.Add(new AccuracyCorrectorAbility());
        }        
    }
}

namespace Abilities
{
    public class AccuracyCorrectorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += AddAccuracyCorrectorAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= AddAccuracyCorrectorAbility;
        }

        private void AddAccuracyCorrectorAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new AccuracyCorrectorAction());
        }
    }
}

namespace ActionsList
{
    public class AccuracyCorrectorAction : GenericAction
    {
        public AccuracyCorrectorAction()
        {
            Name = DiceModificationName = "Accuracy corrector";
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;
            if (Combat.DiceRollAttack.CriticalSuccesses > 0 && Combat.DiceRollAttack.Successes >= 2)
            {
                // Don't cancel crits when we don't need to
                result = 0;
            }
            else
            {
                if (Combat.DiceRollAttack.Successes < 2) result = 100;
            }
            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack;
        }

        public override void ActionEffect(Action callBack)
        {
            Combat.CurrentDiceRoll.RemoveAll();            
            Combat.CurrentDiceRoll.AddDice(DieSide.Success).ShowWithoutRoll();
            Combat.CurrentDiceRoll.AddDice(DieSide.Success).ShowWithoutRoll();            
            Combat.CurrentDiceRoll.OrganizeDicePositions();
            Combat.Attacker.OnTryAddAvailableDiceModification += UseDiceModificationRestriction;
            Combat.Attacker.OnTryAddDiceModificationOpposite += UseDiceModificationRestriction;
            Combat.Defender.OnDefenceStartAsDefender += RemoveDiceModificationRestriction;
            callBack();
        }

        private void RemoveDiceModificationRestriction()
        {
            Combat.Attacker.OnTryAddAvailableDiceModification -= UseDiceModificationRestriction;
            Combat.Attacker.OnTryAddDiceModificationOpposite -= UseDiceModificationRestriction;
            Combat.Defender.OnDefenceStartAsDefender -= RemoveDiceModificationRestriction;
        }

        private void UseDiceModificationRestriction(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            Messages.ShowInfoToHuman("Accuracy corrector: All dice modifications are disabled");
            canBeUsed = false;
        }       
    }
}