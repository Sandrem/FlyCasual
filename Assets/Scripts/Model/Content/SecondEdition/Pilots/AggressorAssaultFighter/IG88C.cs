using ActionsList;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.AggressorAssaultFighter
    {
        public class IG88C : AggressorAssaultFighter
        {
            public IG88C() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "IG-88C",
                    4,
                    70,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.IG88CAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                SEImageNumber = 199;
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