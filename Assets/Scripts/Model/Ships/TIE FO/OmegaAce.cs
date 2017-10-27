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
                PilotName = "Omega Ace";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/First%20Order/TIE-fo%20Fighter/omega-ace.png";
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
            Combat.CurentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
            Combat.CurentDiceRoll.ChangeAll(DieSide.Blank,DieSide.Crit);
            Combat.CurentDiceRoll.ChangeAll(DieSide.Focus,DieSide.Crit);
            Combat.CurentDiceRoll.ChangeAll(DieSide.Success,DieSide.Crit);
            Combat.Attacker.RemoveToken(typeof(Tokens.FocusToken));
            Combat.Attacker.RemoveToken (typeof(Tokens.BlueTargetLockToken),targetLock);
            callBack();
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
                    if (Combat.DiceRollDefence.Focuses > 0) result = 80;
                }
            }

            return result;
        }

    }
}
