using Upgrade;
using Ship;
using SubPhases;
using Abilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;

namespace UpgradesList
{
    public class OperationsSpecialist : GenericUpgrade
    {
        public OperationsSpecialist() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Operations Specialist";
            Cost = 3;
            isLimited = true;

            UpgradeAbilities.Add(new OperationsSpecialistAbility());
        }
    }
}

namespace Abilities
{
    // After a friendly ship at Range 1-2 performs an attack that does not hit, you may assign 1 focus token to a friendly ship at Range 1-3 of the attacker.
    public class OperationsSpecialistAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnAttackMissedAsAttackerGlobal += CheckOperationsSpecialistAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackMissedAsAttackerGlobal -= CheckOperationsSpecialistAbility;
        }

        private void CheckOperationsSpecialistAbility()
        {
            if (Combat.Attacker.Owner.Id == HostShip.Owner.Id && Board.BoardManager.GetRangeOfShips(HostShip, Combat.Attacker) <= 2)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, (s, e) => OperationsSpecialistEffect(Combat.Attacker));
            }
        }

        protected void OperationsSpecialistEffect(GenericShip attacker)
        {
            SelectTargetForAbility(
                GrantFreeFocusToken,
                (ship) => FilterByTargetType(ship, new[] { TargetTypes.OtherFriendly, TargetTypes.This }.ToList()) && Board.BoardManager.GetRangeOfShips(attacker, ship) <= 2,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                true,
                null,
                HostUpgrade.Name,
                "You may assign focus token to a ship at Range 1-3 of the attacker.",
                HostUpgrade.ImageUrl
            );
        }
                

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            result += NeedTokenPriority(ship);
            result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

            return result;
        }

        private int NeedTokenPriority(GenericShip ship)
        {
            if (!ship.Tokens.HasToken(typeof(Tokens.FocusToken))) return 100;
            if (ship.PrintedActions.Any(n => n.GetType() == typeof(ActionsList.EvadeAction)) && !ship.Tokens.HasToken(typeof(Tokens.EvadeToken))) return 50;
            if (ship.PrintedActions.Any(n => n.GetType() == typeof(ActionsList.TargetLockAction)) && !ship.Tokens.HasToken(typeof(Tokens.BlueTargetLockToken), '*')) return 50;
            return 0;
        }

        private void GrantFreeFocusToken()
        {
            Selection.ThisShip = TargetShip;
            RegisterAbilityTrigger(TriggerTypes.OnTokenIsAssigned, AssignFocusToken);
            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsAssigned, SelectShipSubPhase.FinishSelection);
        }

        private void AssignFocusToken(object sender, EventArgs e)
        {
            Selection.ThisShip.Tokens.AssignToken(new FocusToken(HostShip), () =>
            {
                Selection.ThisShip = HostShip;
                Phases.CurrentSubPhase.Resume();
                Triggers.FinishTrigger();
            });
        }
    }
}