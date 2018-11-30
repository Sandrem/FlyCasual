using Ship;
using Upgrade;
using System.Collections.Generic;
using SubPhases;
using System.Linq;
using Tokens;
using ActionsList;

namespace UpgradesList.FirstEdition
{
    public class OperationsSpecialist : GenericUpgrade
    {
        public OperationsSpecialist() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Operations Specialist",
                UpgradeType.Crew,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.OperationsSpecialistAbility)
            );
        }        
    }
}

namespace Abilities.FirstEdition
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
            if (Combat.Attacker.Owner.PlayerNo == HostShip.Owner.PlayerNo && BoardTools.Board.GetRangeOfShips(HostShip, Combat.Attacker) <= 2)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackMissed, (s, e) => OperationsSpecialistEffect(Combat.Attacker));
            }
        }

        protected void OperationsSpecialistEffect(GenericShip attacker)
        {
            SelectTargetForAbility(
                GrantFreeFocusToken,
                (ship) => FilterByTargetType(ship, new[] { TargetTypes.OtherFriendly, TargetTypes.This }.ToList()) && BoardTools.Board.GetRangeOfShips(attacker, ship) <= 2,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                "You may assign focus token to a ship at Range 1-3 of the attacker.",
                HostUpgrade
            );
        }


        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            result += NeedTokenPriority(ship);
            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        private int NeedTokenPriority(GenericShip ship)
        {
            if (!ship.Tokens.HasToken(typeof(FocusToken))) return 100;
            if (ship.ActionBar.HasAction(typeof(EvadeAction)) && !ship.Tokens.HasToken(typeof(EvadeToken))) return 50;
            if (ship.ActionBar.HasAction(typeof(TargetLockAction)) && !ship.Tokens.HasToken(typeof(BlueTargetLockToken), '*')) return 50;
            return 0;
        }

        private void GrantFreeFocusToken()
        {
            TargetShip.Tokens.AssignToken(typeof(FocusToken), SelectShipSubPhase.FinishSelection);
        }
    }
}