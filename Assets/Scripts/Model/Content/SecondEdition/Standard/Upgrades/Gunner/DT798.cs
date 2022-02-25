using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class DT798 : GenericUpgrade
    {
        public DT798() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "DT-798",
                UpgradeType.Gunner,
                cost: 3,
                restriction: new FactionRestriction(Faction.FirstOrder),
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.DT798Ability)
            );

            ImageUrl = "https://i.imgur.com/jcFAIMm.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DT798Ability : GenericAbility
    {
        List<GenericShip> FriendlyShipsInArc;

        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += CheckAssignStrainAbility;

            AddDiceModification
            (
                HostUpgrade.UpgradeInfo.Name,
                IsAvailable,
                GetDiceModificationPriority,
                DiceModificationType.Reroll,
                GetRerollCount
            );
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= CheckAssignStrainAbility;

            RemoveDiceModification();
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && GetRerollCount() > 0;
        }

        private int GetDiceModificationPriority()
        {
            return 90; // Free rerolls
        }

        private int GetRerollCount()
        {
            int shipsInArcWithRedOrangeTokens = 0;

            foreach (GenericShip ship in Roster.AllShips.Values)
            {
                if (HasOrangeOrRedNonLockTokens(ship))
                {
                    ShotInfo shotInfo = new ShotInfo(HostShip, ship, HostShip.PrimaryWeapons);
                    if (shotInfo.InArc) shipsInArcWithRedOrangeTokens++;
                }
            }

            return shipsInArcWithRedOrangeTokens;
        }

        private bool HasOrangeOrRedNonLockTokens(GenericShip ship)
        {
            int orangeTokensCount = ship.Tokens.GetTokensByColor(TokenColors.Orange).Count;
            if (orangeTokensCount > 0) return true;

            int redNonLockTokensCount = ship.Tokens.GetTokensByColor(TokenColors.Red).Count(n => !(n is RedTargetLockToken));
            if (redNonLockTokensCount > 0) return true;

            return false;
        }

        private void CheckAssignStrainAbility()
        {
            if (GetFriendlyShipsInFiringArc().Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToAssignStrain);
            }
        }

        private List<GenericShip> GetFriendlyShipsInFiringArc()
        {
            FriendlyShipsInArc = new List<GenericShip>();

            foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
            {
                ShotInfo shotInfo = new ShotInfo(HostShip, friendlyShip, HostShip.PrimaryWeapons);
                if (shotInfo.InArc) FriendlyShipsInArc.Add(friendlyShip);
            }

            return FriendlyShipsInArc;
        }

        private void AskToAssignStrain(object sender, EventArgs e)
        {
            SelectTargetForAbility
            (
                AssignStrainToTarget,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                "You may assign Strain token to a friendly ship in your firing arc",
                HostUpgrade
            );
        }

        private void AssignStrainToTarget()
        {
            SubPhases.SelectShipSubPhase.FinishSelectionNoCallback();

            Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: {TargetShip.PilotInfo.PilotName} gains Strain token");
            TargetShip.Tokens.AssignToken(typeof(Tokens.StrainToken), Triggers.FinishTrigger);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FriendlyShipsInArc.Contains(ship);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }
    }
}