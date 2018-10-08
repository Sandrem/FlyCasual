using Abilities;
using RuleSets;
using Ship;
using SubPhases;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList
{
    public class FireControlSystem : GenericUpgrade, ISecondEditionUpgrade
    {
        public FireControlSystem() : base()
        {
            Types.Add(UpgradeType.System);
            Name = "Fire-Control System";
            Cost = 2;
            UpgradeAbilities.Add(new FireControlSystemAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 3;

            UpgradeAbilities.RemoveAll(a => a is FireControlSystemAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.FireControlSystemAbility());

            SEImageNumber = 25;
        }
    }
}

namespace Abilities
{
    public class FireControlSystemAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += AddFireControlSystemAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= AddFireControlSystemAbility;
        }

        private void AddFireControlSystemAbility(GenericShip ship)
        {
            if (Combat.Attacker.ShipId == HostShip.ShipId)
            {
                if (!(Combat.Defender.IsDestroyed || Combat.Defender.IsReadyToBeDestroyed))
                {
                    RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskAcquireTargetLock);
                }
            }
        }

        private void AskAcquireTargetLock(object sender, System.EventArgs e)
        {
            AskToUseAbility(AlwaysUseByDefault, AcquireTargetLock, null, null, true);
        }

        private void AcquireTargetLock(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Fire-Control System: Free Target Lock");
            Actions.AcquireTargetLock(Combat.Attacker, Combat.Defender, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }
    }
}


namespace Abilities.SecondEdition
{
    //While you perform an attack, if you have a lock on the defender, you may reroll 1 attack die. If you do, you cannot spend your lock during this attack.
    public class FireControlSystemAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += FireControlSystemAbilityDiceModification;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= FireControlSystemAbilityDiceModification;
        }

        private void FireControlSystemAbilityDiceModification(GenericShip host)
        {
            var newAction = new ActionsList.SecondEdition.FireControlSystemAbilityActionEffect()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList.SecondEdition
{
    public class FireControlSystemAbilityActionEffect : GenericAction
    {
        private List<char> targetLockLetters = new List<char>();

        public FireControlSystemAbilityActionEffect()
        {
            Name = DiceModificationName = "Fire Control System";
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            result = 110;

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack && Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (Actions.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                targetLockLetters = Actions.GetTargetLocksLetterPairs(Combat.Attacker, Combat.Defender);
                foreach (char targetLockLetter in targetLockLetters)
                {
                    Combat.Attacker.Tokens.GetToken(typeof(BlueTargetLockToken), targetLockLetter).CanBeUsed = false;
                }
                Combat.Attacker.OnAttackFinish += SetTargetLockCanBeUsed;

                DiceRerollManager diceRerollManager = new DiceRerollManager
                {
                    NumberOfDiceCanBeRerolled = 1,
                    CallBack = callBack
                };
                diceRerollManager.Start();                
            }
            else
            {
                Messages.ShowInfoToHuman("Cannot use ability: no Target Lock on defender");
                callBack();
            }
        }

        private void SetTargetLockCanBeUsed(GenericShip ship)
        {
            foreach (char targetLockLetter in targetLockLetters)
            {
                BlueTargetLockToken ownTargetLockToken = (BlueTargetLockToken)Combat.Attacker.Tokens.GetToken(typeof(BlueTargetLockToken), targetLockLetter);
                if (ownTargetLockToken != null) ownTargetLockToken.CanBeUsed = true;
            }

            Combat.Attacker.OnAttackFinish -= SetTargetLockCanBeUsed;
        }

    }

}

