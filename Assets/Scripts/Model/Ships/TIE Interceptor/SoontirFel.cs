using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEInterceptor
    {
        public class SoontirFel : TIEInterceptor
        {
            public bool alwaysUseAbility;

            public SoontirFel() : base()
            {
                PilotName = "Soontir Fel";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Interceptor/soontir-fel.png";
                PilotSkill = 9;
                Cost = 27;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "Red Stripes";
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                OnTokenIsAssigned += SoontirFelAbility;
            }

            public void SoontirFelAbility(GenericShip ship, System.Type tokenType)
            {
                if (tokenType == typeof(Tokens.StressToken))
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Soontir Fel: Assign Focus",
                        TriggerOwner = ship.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnTokenIsAssigned,
                        EventHandler = AskAssignFocus
                    });
                }
            }

            private void AskAssignFocus(object sender, System.EventArgs e)
            {
                if (!alwaysUseAbility)
                {
                    Phases.StartTemporarySubPhase(
                        "Soontir Fel's decision",
                        typeof(SubPhases.SoontirFelDecisionSubPhase),
                        Triggers.FinishTrigger
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

    public class SoontirFelDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            infoText = "Soontir Fel: Assign Focus token?";

            AddDecision("Yes", AssignToken);
            AddDecision("No", NotAssignToken);
            AddDecision("Always", AlwaysAssignToken);

            defaultDecision = "Always";

            callBack();
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
            (Selection.ThisShip as Ship.TIEInterceptor.SoontirFel).alwaysUseAbility = true;
            Selection.ThisShip.AssignToken(new Tokens.FocusToken(), ConfirmDecision);
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
