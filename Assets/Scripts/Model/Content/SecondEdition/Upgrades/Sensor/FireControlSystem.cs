﻿using Ship;
using Upgrade;
using System.Collections.Generic;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class FireControlSystem : GenericUpgrade
    {
        public FireControlSystem() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Fire-Control System",
                UpgradeType.Sensor,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.FireControlSystemAbility),
                seImageNumber: 25
            );
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
                HostShip = host
            };
            host.AddAvailableDiceModificationOwn(newAction);
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

            if (Combat.AttackStep == CombatStep.Attack)
            {
                int attackFocuses = Combat.CurrentDiceRoll.FocusesNotRerolled;
                int attackBlanks = Combat.CurrentDiceRoll.BlanksNotRerolled;
                int numFocusTokens = Selection.ActiveShip.Tokens.CountTokensByType(typeof(FocusToken));
                // Only use Fire Control if the number of dice that need re-rolled is 1.
                if (numFocusTokens > 0)
                {
                    // Slightly above Target Lock.
                    if (attackBlanks == 1) result = 81;
                }
                else
                {
                    // Slightly above Target Lock.
                    if (attackBlanks + attackFocuses == 1) result = 81;
                }
            }

            return result;
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack && ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                result = true;
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            if (ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender))
            {
                targetLockLetters = ActionsHolder.GetTargetLocksLetterPairs(Combat.Attacker, Combat.Defender);
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