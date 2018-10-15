using Upgrade;
using Ship;
using SubPhases;
using Abilities;
using System;
using RuleSets;
using Abilities.SecondEdition;
using ActionsList;
using UnityEngine;

namespace UpgradesList
{
    public class LukeSkywalker : GenericUpgrade, ISecondEditionUpgrade
    {
        public LukeSkywalker() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Luke Skywalker";
            Cost = 7;

            isUnique = true;

            UpgradeAbilities.Add(new LukeSkywalkerCrewAbility());

            //Avatar = new AvatarInfo(Faction.Rebel, new Vector2(0, 0));
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 30;
            Types.Clear();
            Types.Add(UpgradeType.Gunner);
            UpgradeAbilities.Clear();
            UpgradeAbilities.Add(new LukeSkywalkerGunnerAbility());

            SEImageNumber = 98;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebel;
        }

    }
}

namespace Abilities.SecondEdition
{
    public class LukeSkywalkerGunnerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckLukeAbility;
            HostShip.MaxForce += 1;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckLukeAbility;
            HostShip.MaxForce -= 1;
        }

        private void CheckLukeAbility()
        {
            if (HostShip.Force > 0 && HostShip.ActionBar.HasAction(typeof(RotateArcAction)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToRotateArc);
            }
        }

        private void AskToRotateArc(object sender, EventArgs e)
        {
            Messages.ShowInfo("Luke Skywalker: You may spend 1 force to rotate your arc.");
            HostShip.BeforeFreeActionIsPerformed += SpendForce;
            Selection.ChangeActiveShip(HostShip);
            //Card states that you can rotate your arc, not perform a rotate arc action so you can do it while stressed
            HostShip.AskPerformFreeAction(new RotateArcAction() { IsRed = false, CanBePerformedWhileStressed = true }, Triggers.FinishTrigger);
        }

        private void SpendForce(GenericAction action)
        {
            HostShip.Force--;
            HostShip.BeforeFreeActionIsPerformed -= SpendForce;
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
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= CheckLukeAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
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

                HostShip.OnGenerateDiceModifications += AddLukeSkywalkerCrewAbility;
                Phases.Events.OnCombatPhaseEnd_NoTriggers += RemoveLukeSkywalkerCrewAbility;

                Combat.StartAdditionalAttack(
                    HostShip,
                    FinishAdditionalAttack,
                    IsPrimaryWeaponShot,
                    HostUpgrade.Name,
                    "You may perform a primary weapon attack.",
                    HostUpgrade
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

        private bool IsPrimaryWeaponShot(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;

            if (Combat.ChosenWeapon is PrimaryWeaponClass)
            {
                result = true;
            }
            else
            {
                if (!isSilent) Messages.ShowError("Attack must be performed from primary weapon");
            }

            return result;
        }

        public void AddLukeSkywalkerCrewAbility(GenericShip ship)
        {
            ship.AddAvailableDiceModification(new ActionsList.LukeSkywalkerCrewAction());
        }

        public void RemoveLukeSkywalkerCrewAbility()
        {
            Phases.Events.OnCombatPhaseEnd_NoTriggers -= RemoveLukeSkywalkerCrewAbility;
            HostShip.OnGenerateDiceModifications -= AddLukeSkywalkerCrewAbility;
        }
    }
}

namespace ActionsList
{
    public class LukeSkywalkerCrewAction : GenericAction
    {

        public LukeSkywalkerCrewAction()
        {
            Name = DiceModificationName = "Luke Skywalker's ability";

            IsTurnsOneFocusIntoSuccess = true;
        }

        public override void ActionEffect(System.Action callBack)
        {
            Combat.CurrentDiceRoll.ChangeOne(DieSide.Focus, DieSide.Crit);
            callBack();
        }

        public override bool IsDiceModificationAvailable()
        {
            bool result = false;
            if (Combat.AttackStep == CombatStep.Attack) result = true;
            return result;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Attack && Combat.DiceRollAttack.Focuses > 0) result = 100;

            return result;
        }

    }
}
