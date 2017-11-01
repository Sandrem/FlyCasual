using Ship;
using Upgrade;

namespace UpgradesList
{
    public class FireControlSystem : GenericUpgrade
    {
        public FireControlSystem() : base()
        {
            Type = UpgradeType.System;
            Name = "Fire-Control System";
            Cost = 2;
        }


        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnAttackPerformed += FireControlSystemAbility;
        }

        private void FireControlSystemAbility()
        {
            if (!Combat.Defender.IsDestroyed)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Fire-Control System: Aquire target lock",
                    TriggerOwner = Host.Owner.PlayerNo,
                    TriggerType = TriggerTypes.OnAttackPerformed,
                    EventHandler = AskAquireTargetLock
                });
            }
        }

        private void AskAquireTargetLock(object sender, System.EventArgs e)
        {
            Phases.StartTemporarySubPhase(
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

        public override void Prepare()
        {
            infoText = "Fire-Control System: Aquire target lock?";

            AddDecision("Yes", AcquireTargetLock);
            AddDecision("No", NotAssignToken);

            defaultDecision = "Yes";
        }

        private void AcquireTargetLock(object sender, System.EventArgs e)
        {
            Actions.AssignTargetLockToPair(Combat.Attacker, Combat.Defender, ConfirmDecision, ConfirmDecision);            
        }

        private void NotAssignToken(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        private void ConfirmDecision()
        {
            Phases.FinishSubPhase(this.GetType());
            CallBack();
        }
    }
}