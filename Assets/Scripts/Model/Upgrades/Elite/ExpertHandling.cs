using System.Linq;
using Upgrade;
using Abilities;
using Ship;
using ActionsList;
using SubPhases;
using Tokens;
using RuleSets;

namespace UpgradesList
{

    public class ExpertHandling : GenericUpgrade, ISecondEditionUpgrade
    {
        private bool isSecondEdition = false;

        public ExpertHandling() : base()
        {
            Types.Add(UpgradeType.Elite);
            Name = "Expert Handling";
            Cost = 2;

            UpgradeAbilities.Add(new ExpertHandlingAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            ImageUrl = "https://i.imgur.com/z0hYUNj.png";
            isSecondEdition = true;

            UpgradeAbilities.RemoveAll(a => a is ExpertHandlingAbility);
            UpgradeAbilities.Add(new GenericActionBarAbility<BarrelRollAction>());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            if (isSecondEdition) return ship.PrintedActions.Any(a => a is BarrelRollAction && (a as BarrelRollAction).IsRed);
            else return true;
        }
    }
}

namespace Abilities
{
    public class ExpertHandlingAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList += AddExpertHandlingAction;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionsList -= AddExpertHandlingAction;
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
            Name = EffectName = "Expert Handling";
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

            bool hasBarrelRollAction = (Host.PrintedActions.Count(n => n.GetType() == typeof(BarrelRollAction)) != 0);

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
