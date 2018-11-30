using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.FirstEdition
{
    public class R5K6 : GenericUpgrade
    {
        public R5K6() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R5-K6",
                UpgradeType.Astromech,
                cost: 2,
                isLimited: true,
                abilityType: typeof(Abilities.FirstEdition.R5K6Ability)
            );
        }
    }
}

namespace Abilities.FirstEdition
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

        private void RegisterR5K6Ability(GenericShip ship, System.Type type)
        {
            if (type == typeof(BlueTargetLockToken))
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
        private List<char> targetLockLetters = new List<char>();

        public override void Prepare()
        {
            DiceKind = DiceKind.Defence;
            DiceCount = 1;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DieSide.Success)
            {
                Sounds.PlayShipSound("R2D2-Proud");
                ActionsHolder.AcquireTargetLock(Combat.Attacker, Combat.Defender, CallBack, CallBack);

                targetLockLetters = ActionsHolder.GetTargetLocksLetterPairs(Combat.Attacker, Combat.Defender);
                foreach (char targetLockLetter in targetLockLetters)
                {
                    GenericToken newTargetLockToken = Combat.Attacker.Tokens.GetToken(typeof(BlueTargetLockToken), targetLockLetter);
                    newTargetLockToken.CanBeUsed = false;
                }

                Combat.Attacker.OnAttackFinish += SetTargetLockCanBeUsed;
            }
            else
            {
                CallBack();
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