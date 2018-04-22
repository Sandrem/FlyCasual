using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;
using Abilities;

namespace UpgradesList
{

    public class R5K6 : GenericUpgrade
    {
        public R5K6() : base()
        {
            Types.Add(UpgradeType.Astromech);
            Name = "R5-K6";
            isUnique = true;
            Cost = 2;

            UpgradeAbilities.Add(new R5K6Ability());
        }
    }

}

namespace Abilities
{
    public class R5K6Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTokenIsSpent += RegisterR5K6Ability;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsSpent -= RegisterR5K6Ability;
        }

        private void RegisterR5K6Ability(Ship.GenericShip ship, System.Type type)
        {
            if (type == typeof(Tokens.BlueTargetLockToken))
            {
                RegisterAbilityTrigger(TriggerTypes.OnTokenIsSpent, StartSubphaseForR5K6Ability);
            }
        }

        private void StartSubphaseForR5K6Ability(object sender, System.EventArgs e)
        {
            Phases.CurrentSubPhase.Pause();

            Selection.ActiveShip = Selection.ThisShip;
            Phases.StartTemporarySubPhaseOld(
                "R5-K6: Try to re-aquire Target Lock",
                typeof(SubPhases.R5K6CheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.R5K6CheckSubPhase));
                    Phases.CurrentSubPhase.Resume();
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}

namespace SubPhases
{

    public class R5K6CheckSubPhase : DiceRollCheckSubPhase
    {

        public override void Prepare()
        {
            diceType = DiceKind.Defence;
            diceCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success)
            {
                Sounds.PlayShipSound("R2D2-Proud");
                Actions.AcquireTargetLock(Combat.Attacker, Combat.Defender, CallBack, CallBack);

                //TODO: Avoid code after callback
                char newTargetLockTokenLetter = Combat.Attacker.Tokens.GetTargetLockLetterPair(Combat.Defender);
                Tokens.GenericToken newTargetLockToken = Combat.Attacker.Tokens.GetToken(typeof(Tokens.BlueTargetLockToken), newTargetLockTokenLetter);
                newTargetLockToken.CanBeUsed = false;

                Combat.Attacker.OnAttackFinish += delegate { newTargetLockToken.CanBeUsed = true; };
            }
            else
            {
                CallBack();
            }
            
        }

    }

}
