using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class SoontirFell : TIEInterceptor
        {
            public bool alwaysUseAbility;

            public SoontirFell() : base()
            {
                PilotName = "Soontir Fell";
                ImageUrl = "https://vignette4.wikia.nocookie.net/xwing-miniatures/images/c/c2/Alpha_Squadron_Pilot.png";
                PilotSkill = 9;
                Cost = 27;

                IsUnique = true;
                AddUpgradeSlot(Upgrade.UpgradeSlot.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                AfterTokenIsAssigned += SoontirFellAbility;
            }

            public void SoontirFellAbility(GenericShip ship, System.Type tokenType)
            {
                if (tokenType == typeof(Tokens.StressToken))
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Soontir Fell: Assign Focus",
                        TriggerOwner = ship.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnPilotAbility,
                        EventHandler = AskAssignFocus
                    });
                }
            }

            private void AskAssignFocus(object sender, System.EventArgs e)
            {
                if (!alwaysUseAbility)
                {
                    Phases.StartTemporarySubPhase(
                        "Soontir Fell Decision",
                        typeof(SubPhases.SoontirFellDecisionSubPhase),
                        delegate () { Triggers.FinishTrigger(); }
                    );
                }
                else
                {
                    Selection.ThisShip.AssignToken(new Tokens.FocusToken(), Triggers.FinishTrigger);
                }
            }
        }
    }
}

namespace SubPhases
{

    public class SoontirFellDecisionSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Soontir Fell: Assign Focus token?";

            AddDecision("Yes", AssignToken);
            AddDecision("No", NotAssignToken);
            AddDecision("Always", AlwaysAssignToken);

            defaultDecision = "Always";
        }

        private void AssignToken(object sender, System.EventArgs e)
        {
            Selection.ThisShip.AssignToken(new Tokens.FocusToken(), ConfirmDecision);
        }

        private void NotAssignToken(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        private void AlwaysAssignToken(object sender, System.EventArgs e)
        {
            (Selection.ThisShip as Ship.TIEInterceptor.SoontirFell).alwaysUseAbility = true;
            Selection.ThisShip.AssignToken(new Tokens.FocusToken(), ConfirmDecision);
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
