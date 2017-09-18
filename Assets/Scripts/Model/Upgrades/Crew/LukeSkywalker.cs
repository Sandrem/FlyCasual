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
            Name = ShortName = "Luke Skywalker";
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
                    Combat.DeclareTarget();
                });
        }

        public void AddLukeSkywalkerCrewAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new LukeSkywalkerCrewAction());
        }

        public void RemoveLukeSkywalkerCrewAbility(GenericShip ship)
        {
            ship.AfterAttackWindow -= RemoveLukeSkywalkerCrewAbility;
            ship.AfterGenerateAvailableActionEffectsList -= AddLukeSkywalkerCrewAbility;
        }

        private class LukeSkywalkerCrewAction : ActionsList.GenericAction
        {

            public LukeSkywalkerCrewAction()
            {
                Name = EffectName = "Luke Skywalker's ability";

                IsTurnsOneFocusIntoSuccess = true;
            }

            public override void ActionEffect(System.Action callBack)
            {
                Combat.CurentDiceRoll.ChangeOne(DiceSide.Focus, DiceSide.Crit);
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
}

namespace SubPhases
{
    public class SelectTargetForSecondAttackSubPhase : SelectShipSubPhase
    {

        public override void Prepare()
        {
            isEnemyAllowed = true;
            finishAction = FinishActon;

            UI.ShowSkipButton();
        }

        private void FinishActon()
        {
            Selection.AnotherShip = TargetShip;
            Combat.ChosenWeapon = Selection.ThisShip.PrimaryWeapon;
            Combat.ShotInfo = new Board.ShipShotDistanceInformation(Selection.ThisShip, TargetShip, Combat.ChosenWeapon);
            MovementTemplates.ShowFiringArcRange(Combat.ShotInfo);
            CallBack();
        }

        protected override void RevertSubPhase() { }

        public override void SkipButton()
        {
            Phases.FinishSubPhase(typeof(SelectTargetForSecondAttackSubPhase));
            Triggers.FinishTrigger();
        }

    }
}
