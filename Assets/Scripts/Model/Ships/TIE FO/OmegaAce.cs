using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFO
    {
        public class OmegaAce : TIEFO
        {
            public OmegaAce () : base ()
            {
                PilotName = "\"Omega Ace\"";
                PilotSkill = 7;
                Cost = 20;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public override void InitializePilot ()
            {
                base.InitializePilot ();
                AfterGenerateAvailableActionEffectsList += AddOmegaAcePilotAbility;
            }

            public void AddOmegaAcePilotAbility(GenericShip ship)
            {
                ship.AddAvailableActionEffect(new PilotAbilities.OmegaAceAction());
            }
        }
    }
}

namespace PilotAbilities
{
    public class OmegaAceAction : ActionsList.GenericAction
    {

        char targetLock = ' ';
        public OmegaAceAction()
        {
            Name = EffectName = "Omega Ace's ability";
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Blank,DieSide.Crit);
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Focus,DieSide.Crit);
            Combat.CurrentDiceRoll.ChangeAll(DieSide.Success,DieSide.Crit);
            IsSpendEvade = true;
            Combat.Attacker.SpendToken(typeof(Tokens.FocusToken),null);
            Combat.Attacker.SpendToken(typeof(Tokens.BlueTargetLockToken),null,targetLock);
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Attack) {
                bool hasTargetLock = false;
                if (Combat.Defender != null) {
                    targetLock = Combat.Attacker.GetTargetLockLetterPair (Combat.Defender);
                    if (targetLock != ' ') {
                        hasTargetLock = true;
                    }
                }

                // TODO check if it has a target lock & 1 focus token
                if (Combat.Attacker.HasToken(typeof(Tokens.FocusToken) ) && hasTargetLock) {
                    return true;
                }
            }
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                if (Combat.DiceRollAttack.Successes > Combat.DiceRollDefence.Successes)
                {
                    if (Combat.DiceRollDefence.Focuses > 0) result = 90;
                }
            }

            return result;
        }

    }
}
