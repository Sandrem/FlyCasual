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

            host.OnAttackFinish += FireControlSystemAbility;
        }

        private void FireControlSystemAbility(GenericShip ship)
        {
            if (Combat.Attacker.ShipId == Host.ShipId)
            {
                if (!Combat.Defender.IsDestroyed)
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Fire-Control System: Aquire target lock",
                        TriggerOwner = Host.Owner.PlayerNo,
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

            DefaultDecision = "Yes";

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