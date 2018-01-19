using Upgrade;
using Ship;
using SubPhases;
using Abilities;
using System;

namespace UpgradesList
{
    public class LukeSkywalker : GenericUpgrade
    {
        public LukeSkywalker() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Luke Skywalker";
            Cost = 7;

            isUnique = true;

            UpgradeAbilities.Add(new LukeSkywalkerCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

    }
}

namespace Abilities
{
    public class LukeSkywalkerCrewAbility : GenericAbility
    {

        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += SubscribeToCheckSecondAttack;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= SubscribeToCheckSecondAttack;
        }

        private void SubscribeToCheckSecondAttack()
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.OnCheckSecondAttack += RegisterSecondAttackTrigger;
            }
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.OnCheckSecondAttack -= RegisterSecondAttackTrigger;

            Triggers.RegisterTrigger(new Trigger
            {
                Name = "Luke Skywalker's ability",
                TriggerType = TriggerTypes.OnCheckSecondAttack,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = DoSecondAttack
            });
        }

        private void DoSecondAttack(object sender, System.EventArgs e)
        {
            Selection.ThisShip.IsCannotAttackSecondTime = true;
            Combat.IsAttackAlreadyCalled = false;

            Selection.ThisShip.AfterGenerateAvailableActionEffectsList += AddLukeSkywalkerCrewAbility;
            Phases.OnCombatPhaseEnd += RemoveLukeSkywalkerCrewAbility;

            Phases.StartTemporarySubPhaseOld(
                "Second attack",
                typeof(SelectTargetForSecondAttackSubPhase),
                delegate {
                    Phases.FinishSubPhase(typeof(SelectTargetForSecondAttackSubPhase));
                    Selection.ThisShip.IsAttackPerformed = false;
                    Combat.DeclareIntentToAttack(Selection.ThisShip.ShipId, Selection.AnotherShip.ShipId);
                });
        }

        public void AddLukeSkywalkerCrewAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new ActionsList.LukeSkywalkerCrewAction());
        }

        public void RemoveLukeSkywalkerCrewAbility()
        {
            Phases.OnCombatPhaseEnd -= RemoveLukeSkywalkerCrewAbility;
            HostShip.AfterGenerateAvailableActionEffectsList -= AddLukeSkywalkerCrewAbility;
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
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Crit);
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
