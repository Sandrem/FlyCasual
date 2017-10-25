using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEFO
    {
        public class OmegaAce : TIEFO
        {
            private char targetLock = ' ';

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
                //setupDecisionPilotAbility (TriggerTypes.OnAttackPerformed);
                //OnCombatPhaseStart += this.RegisterPilotDecisionAbility;
                //this.OnAttackPerformed += RegisterEventHandlerPilotDecisionAbility;
            }

            public void AddOmegaAcePilotAbility(GenericShip ship)
            {
                ship.AddAvailableActionEffect(new PilotAbilities.OmegaAceAction());
            }

            /*
            // ==== Pilot Ability ==== //
            protected override bool ShouldShowDecision(object sender)
            {
                UI.AddTestLogEntry ("OmegaAce::ShuldShowDecision");
                bool hasTargetLock = false;
                if (Combat.Defender != null) {
                    targetLock = this.GetTargetLockLetterPair (Combat.Defender);
                    if (targetLock != ' ') {
                        hasTargetLock = true;
                    }
                }

                // TODO check if it has a target lock & 1 focus token
                if (this.HasToken(typeof(Tokens.FocusToken) ) && hasTargetLock) {
                    return true;
                }
                // OR if it has 1 focus and can use a target lock from an ally --- target synchronizer
                // OR if it has DeadEye and has 2 Focus Tokens
                return false;
            }

            public override void UsePilotAbility(SubPhases.PilotDecisionSubPhase subPhase)
            {
                UI.AddTestLogEntry ("OmegaAce::UsePilotAbility");
                base.UsePilotAbility (subPhase);

                // will need to change this for
                // * dead eye
                // * target synchronizer
                this.RemoveToken(typeof(Tokens.FocusToken));
                this.RemoveToken (typeof(Tokens.BlueTargetLockToken),targetLock);

                // cancel the results
                Combat.DiceRollAttack.ChangeAll(DieSide.Blank,DieSide.Crit);
                Combat.DiceRollAttack.ChangeAll(DieSide.Focus,DieSide.Crit);
                Combat.DiceRollAttack.ChangeAll(DieSide.Success,DieSide.Crit);
            }
            */
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
            UI.AddTestLogEntry ("[OmegaAce::PilotAbilities::IsActionEffectAvailable]");
            if (Combat.AttackStep == CombatStep.Defence) {
            UI.AddTestLogEntry ("[OmegaAce::PilotAbilities::IsActionEffectAvailable] in step");
                bool hasTargetLock = false;
                if (Combat.Defender != null) {
            UI.AddTestLogEntry ("[OmegaAce::PilotAbilities::IsActionEffectAvailable] defender != null");
                    targetLock = Combat.Attacker.GetTargetLockLetterPair (Combat.Defender);
            UI.AddTestLogEntry ("[OmegaAce::PilotAbilities::IsActionEffectAvailable] targetLock = "+targetLock);
                    if (targetLock != ' ') {
                        hasTargetLock = true;
                    }
                }

                // TODO check if it has a target lock & 1 focus token
                if (Combat.Attacker.HasToken(typeof(Tokens.FocusToken) ) && hasTargetLock) {
            UI.AddTestLogEntry ("[OmegaAce::PilotAbilities::IsActionEffectAvailable] has both");
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
