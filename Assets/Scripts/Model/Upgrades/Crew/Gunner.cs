using Upgrade;
using Ship;
using SubPhases;

namespace UpgradesList
{
    public class Gunner : GenericUpgrade
    {
        private bool IsAblilityActive;

        public Gunner() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Gunner";
            Cost = 5;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnAttackMissedAsAttacker += GunnerAbility;
        }

        private void GunnerAbility()
        {
            SubscribeToCheckSecondAttack();
        }

        private void SubscribeToCheckSecondAttack()
        {
            if (!Host.IsCannotAttackSecondTime)
            {
                Host.OnCheckSecondAttack += RegisterSecondAttackTrigger;
            }
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            Host.OnCheckSecondAttack -= RegisterSecondAttackTrigger;
            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Gunner's ability",
                TriggerType = TriggerTypes.OnCheckSecondAttack,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = DoSecondAttack
            });
        }

        private void DoSecondAttack(object sender, System.EventArgs e)
        {
            Selection.ThisShip.IsCannotAttackSecondTime = true;

            Phases.StartTemporarySubPhaseOld(
                "Second attack",
                typeof(SelectTargetForSecondAttackSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SelectTargetForSecondAttackSubPhase));
                    Combat.DeclareTarget(Selection.ThisShip.ShipId, Selection.AnotherShip.ShipId);
                });
        }
    }
}