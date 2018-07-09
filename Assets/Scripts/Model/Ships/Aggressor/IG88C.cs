using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using Ship;
using Upgrade;
using System.Linq;
using ActionsList;
using SubPhases;

namespace Ship
{
    namespace Aggressor
    {
        public class IG88C : Aggressor
        {
            public IG88C() : base()
            {
                PilotName = "IG-88C";
                PilotSkill = 6;
                Cost = 36;

                IsUnique = true;

                PilotAbilities.Add(new IG88CAbility());
            }
        }
    }
}

namespace Abilities
{
    public class IG88CAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckBoostBonus;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckBoostBonus;
        }

        private void CheckBoostBonus(GenericAction action)
        {
            if (action is BoostAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionDecisionSubPhaseEnd, AskAssignEvade);
            }
        }

        private void AskAssignEvade(object sender, System.EventArgs e)
        {
            if (Selection.ThisShip.CanPerformAction(new EvadeAction()))
            {
                if (!alwaysUseAbility)
                {
                    AskToUseAbility(AlwaysUseByDefault, PerformFreeEvadeActionDecision, null, null, true);
                }
                else
                {
                    PerformFreeEvadeActionDecision(null, null);
                }
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void PerformFreeEvadeActionDecision(object sender, System.EventArgs e)
        {
            Phases.CurrentSubPhase.CallBack = delegate {
                Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                Triggers.FinishTrigger();
            };
            (new EvadeAction()).ActionTake();
        }
    }
}
