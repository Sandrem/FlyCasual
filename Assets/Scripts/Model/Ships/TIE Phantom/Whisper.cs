using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace TIEPhantom
    {
        public class Whisper : TIEPhantom
        {
            public bool alwaysUseAbility;

            public Whisper() : base()
            {
                PilotName = "\"Whisper\"";
                ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/pilots/Galactic%20Empire/TIE%20Phantom/whisper.png";
                PilotSkill = 7;
                Cost = 32;

                IsUnique = true;
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                OnAttackHitAsAttacker += WhisperAbility;
            }

            public void WhisperAbility()
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Whisper: Assign Focus",
                    TriggerOwner = Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnAttackHit,
                    EventHandler = AskAssignFocus
                });
            }

            private void AskAssignFocus(object sender, System.EventArgs e)
            {
                if (!alwaysUseAbility)
                {
                    Phases.StartTemporarySubPhase(
                        "Whisper's decision",
                        typeof(SubPhases.WhisperDecisionSubPhase),
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

    public class WhisperDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            infoText = "Whisper: Assign Focus token?";

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
            (Selection.ThisShip as Ship.TIEPhantom.Whisper).alwaysUseAbility = true;
            Selection.ThisShip.AssignToken(new Tokens.FocusToken(), ConfirmDecision);
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
