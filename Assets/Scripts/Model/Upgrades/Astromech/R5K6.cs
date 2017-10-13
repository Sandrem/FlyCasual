using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class R5K6 : GenericUpgrade
    {

        public R5K6() : base()
        {
            Type = UpgradeType.Astromech;
            Name = "R5-K6";
            isUnique = true;
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.OnTokenIsSpent += R5K6Ability;
        }

        private void R5K6Ability(Ship.GenericShip ship, System.Type type)
        {
            if (type == typeof(Tokens.BlueTargetLockToken))
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "R5-K6' ability",
                    TriggerOwner = ship.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnTokenIsSpent,
                    EventHandler = StartSubphaseForR5K6Ability
                });
            }
        }

        private void StartSubphaseForR5K6Ability(object sender, System.EventArgs e)
        {
            Phases.CurrentSubPhase.Pause();

            Selection.ActiveShip = Selection.ThisShip;
            Phases.StartTemporarySubPhase(
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
                Actions.AssignTargetLockToPair(Combat.Attacker, Combat.Defender, CallBack, CallBack);

                //TODO: Avoid code after callback
                char newTargetLockTokenLetter = Combat.Attacker.GetTargetLockLetterPair(Combat.Defender);
                Tokens.GenericToken newTargetLockToken = Combat.Attacker.GetToken(typeof(Tokens.BlueTargetLockToken), newTargetLockTokenLetter);
                newTargetLockToken.CanBeUsed = false;

                Combat.Attacker.AfterCombatEnd += delegate { newTargetLockToken.CanBeUsed = true; };
            }
            else
            {
                CallBack();
            }
            
        }

    }

}
