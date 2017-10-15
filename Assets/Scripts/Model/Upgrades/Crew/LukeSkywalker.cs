using Upgrade;
using Ship;
using SubPhases;

namespace UpgradesList
{
    public class LukeSkywalker : GenericUpgrade
    {
        private bool IsAblilityActive;

        public LukeSkywalker() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Luke Skywalker";
            Cost = 7;
            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebels;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnAttackMissedAsAttacker += LukeSkywalkerAbility;
        }

        private void LukeSkywalkerAbility()
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
                Name = "Luke Skywalker's ability",
                TriggerType = TriggerTypes.OnCheckSecondAttack,
                TriggerOwner = Host.Owner.PlayerNo,
                EventHandler = DoSecondAttack
            });
        }

        private void DoSecondAttack(object sender, System.EventArgs e)
        {
            Selection.ThisShip.IsCannotAttackSecondTime = true;

            Selection.ThisShip.AfterGenerateAvailableActionEffectsList += AddLukeSkywalkerCrewAbility;
            Selection.ThisShip.AfterAttackWindow += RemoveLukeSkywalkerCrewAbility;

            Phases.StartTemporarySubPhase(
                "Second attack",
                typeof(SelectTargetForSecondAttackSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SelectTargetForSecondAttackSubPhase));
                    Combat.DeclareTarget(Selection.ThisShip.ShipId, Selection.AnotherShip.ShipId);
                });
        }

        public void AddLukeSkywalkerCrewAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new ActionsList.LukeSkywalkerCrewAction());
        }

        public void RemoveLukeSkywalkerCrewAbility(GenericShip ship)
        {
            ship.AfterAttackWindow -= RemoveLukeSkywalkerCrewAbility;
            ship.AfterGenerateAvailableActionEffectsList -= AddLukeSkywalkerCrewAbility;
        }

    }
}

namespace ActionsList
{
    public class LukeSkywalkerCrewAction : GenericAction
    {

        public LukeSkywalkerCrewAction()
        {
            Name = EffectName = "Luke Skywalker's ability";

            IsTurnsOneFocusIntoSuccess = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Crit);
            callBack();
        }

        public override bool IsActionEffectAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack) result = true;
            return result;
        }

        public override int GetActionEffectPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack && Combat.DiceRollAttack.Focuses > 0) result = 100;

            return result;
        }

    }
}
