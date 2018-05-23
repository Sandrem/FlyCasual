using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DamageDeckCardSE
{
    public class WoundedPilot : GenericDamageCard
    {
        public WoundedPilot()
        {
            Name = "Wounded Pilot";
            Type = CriticalCardType.Pilot;
        }

        public override void ApplyEffect(object sender, EventArgs e)
        {            
            Host.AfterGenerateAvailableActionsList += CallAddCancelCritAction;
            Host.OnActionIsPerformed += AfterPerformingActionRollForStress;

            Host.Tokens.AssignCondition(new Tokens.WoundedPilotCritToken(Host));
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
            var subphase = Phases.StartTemporarySubPhaseNew<SubPhases.WoundedPilotCheckSubPhase>("Wounded Pilot", () =>
            {
                Phases.FinishSubPhase(typeof(SubPhases.WoundedPilotCheckSubPhase));
                Triggers.FinishTrigger();
            });
            subphase.HostShip = Host;
            subphase.Start();
        }

        public override void DiscardEffect()
        {
            Host.Tokens.RemoveCondition(typeof(Tokens.WoundedPilotCritToken));

            Host.AfterGenerateAvailableActionsList -= CallAddCancelCritAction;
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
            diceType = DiceKind.Attack;
            diceCount = 1;

            finishAction = FinishAction;

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
            Messages.ShowError("Wounded Pilot: ship is assigned stress");
            HostShip.Tokens.AssignToken(new Tokens.StressToken(HostShip), CallBack);
        }

        private void NoStress()
        {
            Messages.ShowInfoToHuman("No stress");
            CallBack();
        }
    }

}