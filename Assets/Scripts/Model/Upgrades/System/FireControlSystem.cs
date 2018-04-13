using Abilities;
using Ship;
using Upgrade;

namespace UpgradesList
{
    public class FireControlSystem : GenericUpgrade
    {
        public FireControlSystem() : base()
        {
            Types.Add(UpgradeType.System);
            Name = "Fire-Control System";
            Cost = 2;
            UpgradeAbilities.Add(new FireControlSystemAbility());
        }                
    }
}

namespace Abilities
{
    public class FireControlSystemAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList += AddFireControlSystemAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGenerateAvailableActionEffectsList -= AddFireControlSystemAbility;
        }

        private void AddFireControlSystemAbility(GenericShip ship)
        {
            if (Combat.Attacker.ShipId == HostShip.ShipId)
            {
                if (!(Combat.Defender.IsDestroyed || Combat.Defender.IsReadyToBeDestroyed))
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Fire-Control System: Aquire target lock",
                        TriggerOwner = HostShip.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnAttackFinish,
                        EventHandler = AskAquireTargetLock
                    });
                }
            }
        }

        private void AskAquireTargetLock(object sender, System.EventArgs e)
        {
            Phases.StartTemporarySubPhaseOld(
                "Fire-Control System's decision",
                typeof(SubPhases.FireControlSystemDecisionSubPhase),
                Triggers.FinishTrigger
            );
        }
    }
}


namespace SubPhases
{

    public class FireControlSystemDecisionSubPhase : DecisionSubPhase
    {

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Fire-Control System: Aquire target lock?";

            AddDecision("Yes", AcquireTargetLock);
            AddDecision("No", NotAssignToken);

            DefaultDecisionName = "Yes";

            callBack();
        }

        private void AcquireTargetLock(object sender, System.EventArgs e)
        {
            Messages.ShowInfo("Fire-Control System: Free Target Lock");
            Actions.AssignTargetLockToPair(Combat.Attacker, Combat.Defender, ConfirmDecision, ConfirmDecision);            
        }

        private void NotAssignToken(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }
    }
}