using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using Tokens;
using UnityEngine;

namespace DamageDeckCardSE
{
    public class WoundedPilot : GenericDamageCard
    {
        public WoundedPilot()
        {
            Name = "Wounded Pilot";
            Type = CriticalCardType.Pilot;
            ImageUrl = "https://i.imgur.com/GtW7OR5.png";
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {            
            Host.OnGenerateActions += CallAddCancelCritAction;
            Host.OnActionIsPerformed += AfterPerformingActionRollForStress;

            Host.Tokens.AssignCondition(typeof(WoundedPilotCritToken));
            Triggers.FinishTrigger();
        }

        private void AfterPerformingActionRollForStress(ActionsList.GenericAction action)
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "#" + Host.ShipId + ": Wounded Pilot Crit",
                TriggerOwner = Host.Owner.PlayerNo,
                TriggerType = TriggerTypes.OnActionIsPerformed,
                EventHandler = RollForStress
            });
        }

        protected void RollForStress(object sender, EventArgs e)
        {
            Selection.ActiveShip = Host;
            Selection.ThisShip = Host;
            var subphase = Phases.StartTemporarySubPhaseNew<WoundedPilotCheckSubPhase>("Wounded Pilot", () =>
            {
                Phases.FinishSubPhase(typeof(WoundedPilotCheckSubPhase));
                Triggers.FinishTrigger();
            });
            subphase.HostShip = Host;
            subphase.Start();
        }

        public override void DiscardEffect()
        {
            base.DiscardEffect();

            Host.Tokens.RemoveCondition(typeof(WoundedPilotCritToken));

            Host.OnGenerateActions -= CallAddCancelCritAction;
            Host.OnActionIsPerformed -= AfterPerformingActionRollForStress;
        }
    }

}

namespace SubPhases
{

    public class WoundedPilotCheckSubPhase : DiceRollCheckSubPhase
    {
        public GenericShip HostShip;

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            RequiredPlayer = Selection.ThisShip.Owner.PlayerNo;

            AfterRoll = FinishAction;

            Name = "#" + Selection.ActiveShip.ShipId + ": " + Name;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            switch(CurrentDiceRoll.DiceList[0].Side)
            {
                case DieSide.Success:
                case DieSide.Crit:
                    SufferStress();
                    break;
                default:
                    NoStress();
                    break;
            }            

        }

        private void SufferStress()
        {
            Messages.ShowInfo("Wounded Pilot: This ship has gained a stress token");
            HostShip.Tokens.AssignToken(typeof(StressToken), CallBack);
        }

        private void NoStress()
        {
            Messages.ShowInfoToHuman("No stress");
            CallBack();
        }
    }

}