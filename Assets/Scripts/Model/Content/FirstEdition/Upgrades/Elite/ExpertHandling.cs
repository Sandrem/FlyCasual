using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using System.Linq;
using Tokens;
using SubPhases;

namespace UpgradesList.FirstEdition
{
    public class ExpertHandling : GenericUpgrade
    {
        public ExpertHandling() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Expert Handling",
                UpgradeType.Elite,
                cost: 2,
                abilityType: typeof(Abilities.FirstEdition.ExpertHandlingAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class ExpertHandlingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += AddExpertHandlingAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= AddExpertHandlingAction;
        }

        private void AddExpertHandlingAction(GenericShip host)
        {
            GenericAction newAction = new ExpertHandlingAction()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                Host = host
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
            Name = DiceModificationName = "Expert Handling";
        }

        public override void ActionTake()
        {
            Phases.CurrentSubPhase.Pause();
            if (!Selection.ThisShip.IsAlreadyExecutedAction(typeof(BarrelRollAction)))
            {
                Phases.StartTemporarySubPhaseOld(
                    "Expert Handling: Barrel Roll",
                    typeof(BarrelRollPlanningSubPhase),
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

            bool hasBarrelRollAction = Host.ActionBar.HasAction(typeof(BarrelRollAction));

            if (hasBarrelRollAction)
            {
                RemoveTargetLock();
            }
            else
            {
                Host.Tokens.AssignToken(typeof(StressToken), RemoveTargetLock);
            }

        }

        private void RemoveTargetLock()
        {
            if (Host.Tokens.HasToken(typeof(RedTargetLockToken), '*'))
            {
                Phases.StartTemporarySubPhaseOld(
                    "Expert Handling: Select target lock to remove",
                    typeof(ExpertHandlingTargetLockDecisionSubPhase),
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
                if (token.GetType() == typeof(RedTargetLockToken))
                {
                    AddDecision(
                        "Remove token \"" + (token as RedTargetLockToken).Letter + "\"",
                        delegate { RemoveRedTargetLockToken((token as RedTargetLockToken).Letter); }
                    );
                }
            }

            AddDecision("Don't remove", delegate { ConfirmDecision(); });

            DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

        private void RemoveRedTargetLockToken(char letter)
        {
            Selection.ThisShip.Tokens.RemoveToken(
                typeof(RedTargetLockToken),
                ConfirmDecision,
                letter
            );
        }

    }

}