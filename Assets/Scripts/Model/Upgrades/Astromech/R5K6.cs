using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

/*
 * KNOWN PROBLEMS
 * 1) Ability during combat -> attack dice results are visible
 * 2) Triggered on select action instead of select action callback
 * 3) Pay cost of proton torpedo -> Exception, no subphase callback 
 */


namespace UpgradesList
{

    public class R5K6 : GenericUpgrade
    {

        public R5K6() : base()
        {
            IsHidden = true;

            Type = UpgradeSlot.Astromech;
            Name = ShortName = "R5-K6";
            ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/d/df/R5-K6.png";
            isUnique = true;
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterTokenIsSpent += R5K6Ability;
        }

        private void R5K6Ability(Ship.GenericShip ship, System.Type type)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "R5-K6' ability",
                TriggerOwner = ship.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnTokenIsSpent,
                EventHandler = StartSubphaseForR5K6Ability
            });
        }

        private void StartSubphaseForR5K6Ability(object sender, System.EventArgs e)
        {
            Selection.ActiveShip = Selection.ThisShip;
            Phases.StartTemporarySubPhase(
                "R5-K6: Try to re-aquire Target Lock",
                typeof(SubPhases.R5K6CheckSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SubPhases.R5K6CheckSubPhase));
                    Phases.CurrentSubPhase.CallBack();
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
            dicesType = DiceKind.Defence;
            dicesCount = 1;

            finishAction = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            if (CurrentDiceRoll.DiceList[0].Side == DiceSide.Success)
            {
                Actions.AssignTargetLockToPair(Combat.Attacker, Combat.Defender, CallBack, CallBack);
            }
            else
            {
                CallBack();
            }
            
        }

    }

}
