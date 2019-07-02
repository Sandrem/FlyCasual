using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ActionsList;
using Ship;
using SubPhases;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Aggressor
    {
        public class IG88C : Aggressor
        {
            public IG88C() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "IG-88C",
                    6,
                    36,
                    isLimited: true,
                    abilityType: typeof(Abilities.FirstEdition.IG88CAbility),
                    extraUpgradeIcon: UpgradeType.Talent
                );
            }
        }
    }
}

namespace Abilities.FirstEdition
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
                    AskToUseAbility(
                        HostShip.PilotInfo.PilotName,
                        AlwaysUseByDefault,
                        PerformFreeEvadeActionDecision,
                        descriptionLong: "Do you want to perform an Evade Action?",
                        imageHolder: HostShip,
                        showAlwaysUseOption: true
                    );
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
            //Action is forced now
            Selection.ThisShip.AskPerformFreeAction(
                new EvadeAction(),
                DecisionSubPhase.ConfirmDecision,
                HostShip.PilotInfo.PilotName,
                "You must perform a free evade action",
                HostShip,
                isForced: true
            );
        }
    }
}