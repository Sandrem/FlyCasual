using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList
{

    public class ExpertHandling : GenericUpgrade
    {

        public ExpertHandling() : base()
        {
            Type = UpgradeType.Elite;
            Name = "Expert Handling";
            Cost = 2;
        }

        public override void AttachToShip(Ship.GenericShip host)
        {
            base.AttachToShip(host);

            host.AfterGenerateAvailableActionsList += AddExpertHandlingAction;
        }

        private void AddExpertHandlingAction(Ship.GenericShip host)
        {
            ActionsList.GenericAction newAction = new ActionsList.ExpertHandlingAction()
            {
                ImageUrl = ImageUrl,
                Host = Host
            };
            host.AddAvailableAction(newAction);
        }

    }
}

namespace ActionsList
{

    public class ExpertHandlingAction : GenericAction
    {
        public ExpertHandlingAction()
        {
            Name = EffectName = "Expert Handling";
        }

        public override void ActionTake()
        {
            Phases.CurrentSubPhase.Pause();
            if (!Selection.ThisShip.IsAlreadyExecutedAction(typeof(BarrelRollAction)))
            {
                Phases.StartTemporarySubPhase(
                    "Expert Handling: Barrel Roll",
                    typeof(SubPhases.BarrelRollPlanningSubPhase),
                    CheckStress
                );
            }
            else
            {
                Messages.ShowError("Cannot use Expert Handling: Barrel Roll is already executed");
                Phases.CurrentSubPhase.Resume();
            }
        }

        private void CheckStress()
        {
            Selection.ThisShip.AddAlreadyExecutedAction(new BarrelRollAction());

            bool hasBarrelRollAction = (Host.BuiltInActions.Count(n => n.GetType() == typeof(BarrelRollAction)) != 0);

            if (hasBarrelRollAction)
            {
                RemoveTargetLock();
            }
            else
            {
                Host.AssignToken(new Tokens.StressToken(), RemoveTargetLock);
            }
            
        }

        private void RemoveTargetLock()
        {
            if (Host.HasToken(typeof(Tokens.RedTargetLockToken), '*'))
            {
                Phases.StartTemporarySubPhase(
                    "Expert Handling: Select target lock to remove",
                    typeof(SubPhases.ExpertHandlingTargetLockDecisionSubPhase),
                    Finish
                );
            }
            else
            {
                Finish();
            }
        }

        private void Finish()
        {
            Phases.CurrentSubPhase.CallBack();
        }

    }

}

namespace SubPhases
{

    public class ExpertHandlingTargetLockDecisionSubPhase : DecisionSubPhase
    {

        public override void Prepare()
        {
            infoText = "Select target lock to remove";

            foreach (var token in Selection.ThisShip.GetAssignedTokens())
            {
                if (token.GetType() == typeof(Tokens.RedTargetLockToken))
                {
                    AddDecision(
                        "Remove token \"" + (token as Tokens.RedTargetLockToken).Letter + "\"",
                        delegate { RemoveRedTargetLockToken((token as Tokens.RedTargetLockToken).Letter); }
                    );
                }
            }

            AddDecision("Don't remove", delegate { ConfirmDecision(); });

            defaultDecision = GetDecisions().First().Key;
        }

        private void RemoveRedTargetLockToken(char letter)
        {
            Selection.ThisShip.RemoveToken(typeof(Tokens.RedTargetLockToken), letter);
            ConfirmDecision();
        }


        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }

    }

}
