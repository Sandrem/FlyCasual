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
                Phases.StartTemporarySubPhaseOld(
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

            bool hasBarrelRollAction = (Host.PrintedActions.Count(n => n.GetType() == typeof(BarrelRollAction)) != 0);

            if (hasBarrelRollAction)
            {
                RemoveTargetLock();
            }
            else
            {
                Host.Tokens.AssignToken(new Tokens.StressToken(Host), RemoveTargetLock);
            }
            
        }

        private void RemoveTargetLock()
        {
            if (Host.Tokens.HasToken(typeof(Tokens.RedTargetLockToken), '*'))
            {
                Phases.StartTemporarySubPhaseOld(
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

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Select target lock to remove";

            foreach (var token in Selection.ThisShip.Tokens.GetAllTokens())
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

            DefaultDecision = GetDecisions().First().Key;

            callBack();
        }

        private void RemoveRedTargetLockToken(char letter)
        {
            Selection.ThisShip.Tokens.RemoveToken(
                typeof(Tokens.RedTargetLockToken),
                ConfirmDecision,
                letter
            );
        }

    }

}
