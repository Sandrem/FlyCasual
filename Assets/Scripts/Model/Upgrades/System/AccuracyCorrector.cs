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
            Type = UpgradeType.System;
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
            HostShip.AfterGenerateAvailableActionEffectsList += AddAccuracyCorrectorAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= AddAccuracyCorrectorAbility;
        }

        private void AddAccuracyCorrectorAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new AccuracyCorrectorAction());
        }
    }
}

namespace ActionsList
{
    public class AccuracyCorrectorAction : GenericAction
    {
        private System.Action actionCallback;
        public AccuracyCorrectorAction()
        {
            Name = EffectName = "Accuracy corrector";
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;
            result = 100;
            return result;
        }

        public override bool IsActionEffectAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.RemoveAll();            
            Combat.CurrentDiceRoll.AddDice(DieSide.Success).ShowWithoutRoll();
            Combat.CurrentDiceRoll.AddDice(DieSide.Success).ShowWithoutRoll();            
            Combat.CurrentDiceRoll.OrganizeDicePositions();
            Combat.Attacker.OnTryAddAvailableActionEffect += UseDiceModificationRestriction;
            Combat.Attacker.OnTryAddAvailableOppositeActionEffect += UseDiceModificationRestriction;
            Combat.Defender.OnDefence += RemoveDiceModificationRestriction;
            callBack();
        }

        private void RemoveDiceModificationRestriction()
        {
            Combat.Attacker.OnTryAddAvailableActionEffect -= UseDiceModificationRestriction;
            Combat.Attacker.OnTryAddAvailableOppositeActionEffect -= UseDiceModificationRestriction;
            Combat.Defender.OnDefence -= RemoveDiceModificationRestriction;
        }

        private void UseDiceModificationRestriction(ActionsList.GenericAction action, ref bool canBeUsed)
        {
            Messages.ShowInfoToHuman("Accuracy corrector: All dice modifications are disabled");
            canBeUsed = false;
        }       
    }
}