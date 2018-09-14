﻿using System.Linq;
using Upgrade;
using Abilities;
using Ship;
using ActionsList;
using SubPhases;
using Tokens;
using RuleSets;
using System.Collections.Generic;

namespace UpgradesList
{

    public class ExpertHandling : GenericUpgrade, ISecondEditionUpgrade, IVariableCost
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
            isSecondEdition = true;

            UpgradeAbilities.RemoveAll(a => a is ExpertHandlingAbility);
            UpgradeAbilities.Add(new GenericActionBarAbility<BarrelRollAction>());

            SEImageNumber = 5;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            if (isSecondEdition) return ship.ActionBar.HasAction(typeof(BarrelRollAction), isRed:true);
            else return true;
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<BaseSize, int> sizeToCost = new Dictionary<BaseSize, int>()
            {
                {BaseSize.Small, 2},
                {BaseSize.Medium, 4},
                {BaseSize.Large, 6},
            };

            Cost = sizeToCost[ship.ShipBaseSize];
        }
    }
}

namespace Abilities
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
