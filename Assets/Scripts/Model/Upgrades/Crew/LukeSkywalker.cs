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
            Types.Add(UpgradeType.Crew);
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
            HostShip.OnAttackMissedAsAttacker += CheckLukeAbility;
            Phases.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= CheckLukeAbility;
            Phases.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckLukeAbility()
        {
            if (!IsAbilityUsed && !HostShip.IsCannotAttackSecondTime)
            {
                IsAbilityUsed = true;

                // Trigger must be registered just before it's resolution
                HostShip.OnCombatCheckExtraAttack += RegisterSecondAttackTrigger;
            }
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSecondAttackTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, DoSecondAttack);
        }

        private void DoSecondAttack(object sender, System.EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                HostShip.AfterGenerateAvailableActionEffectsList += AddLukeSkywalkerCrewAbility;
                Phases.OnCombatPhaseEnd_NoTriggers += RemoveLukeSkywalkerCrewAbility;

                Combat.StartAdditionalAttack(
                    HostShip,
                    FinishAdditionalAttack,
                    IsPrimaryWeaponShot,
                    HostUpgrade.Name,
                    "You may perform a primary weapon attack.",
                    HostUpgrade.ImageUrl
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack one more time", HostShip.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishAdditionalAttack()
        {
            // If attack is skipped, set this flag, otherwise regular attack can be performed second time
            HostShip.IsAttackPerformed = true;

            Triggers.FinishTrigger();
        }

        private bool IsPrimaryWeaponShot(GenericShip defender, IShipWeapon weapon)
        {
            bool result = false;

            if (Combat.ChosenWeapon is PrimaryWeaponClass)
            {
                result = true;
            }
            else
            {
                Messages.ShowError("Attack must be performed from primary weapon");
            }

            return result;
        }

        public void AddLukeSkywalkerCrewAbility(GenericShip ship)
        {
            ship.AddAvailableActionEffect(new ActionsList.LukeSkywalkerCrewAction());
        }

        public void RemoveLukeSkywalkerCrewAbility()
        {
            Phases.OnCombatPhaseEnd_NoTriggers -= RemoveLukeSkywalkerCrewAbility;
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
