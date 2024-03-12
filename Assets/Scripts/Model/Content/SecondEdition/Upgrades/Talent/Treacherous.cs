﻿using ActionsList;
using BoardTools;
using Ship;
using System;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Treacherous : GenericUpgrade
    {
        public Treacherous() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Treacherous",
                UpgradeType.Talent,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.TreacherousAbility),
                restriction: new FactionRestriction(Faction.Separatists),
                charges: 1
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class TreacherousAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite += AddDiceModification;
            GenericShip.OnShipIsDestroyedGlobal += TryRestoreCharge;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModificationsOpposite -= AddDiceModification;
            GenericShip.OnShipIsDestroyedGlobal -= TryRestoreCharge;
        }

        private void AddDiceModification(GenericShip ship)
        {
            TreacherousDiceModification newAction = new TreacherousDiceModification()
            {
                ImageUrl = HostUpgrade.ImageUrl,
                HostShip = HostShip,
                Source = HostUpgrade,
                DoDiceModification = DoTreacherousDiceModification
            };
            HostShip.AddAvailableDiceModificationOwn(newAction);
        }

        private void DoTreacherousDiceModification(Action action)
        {
            Phases.CurrentSubPhase.Pause();

            SelectTargetForAbility(
                ShipIsSelected,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                "Treacherous",
                "Choose a ship obstructing the attack and to cancel 1 damage and assign 1 strain token to it",
                imageSource: HostUpgrade,
                showSkipButton: false
            );
        }

        private void ShipIsSelected()
        {
            SubPhases.SelectShipSubPhase.FinishSelectionNoCallback();

            HostUpgrade.State.SpendCharge();

            TargetShip.Tokens.AssignToken(
                typeof(StrainToken),
                SelectDieToRemove
            );
        }

        private void SelectDieToRemove()
        {
            if (Combat.DiceRollAttack.Successes > 0)
            {
                if (Combat.DiceRollAttack.RegularSuccesses > 0 && Combat.DiceRollAttack.CriticalSuccesses == 0)
                {
                    CancelResultAndFinish(DieSide.Success);
                }
                else if (Combat.DiceRollAttack.CriticalSuccesses > 0 && Combat.DiceRollAttack.RegularSuccesses == 0)
                {
                    CancelResultAndFinish(DieSide.Crit);
                }
                else
                {
                    AskToUseAbility(
                        "Treacherous",
                        AlwaysUseByDefault,
                        CancelCritDamage,
                        CancelHitDamage,
                        descriptionLong: "Do you want to cancel Crit result instead of regular Hit?",
                        imageHolder: HostUpgrade,
                        requiredPlayer: HostShip.Owner.PlayerNo
                    );
                }
            }
            else
            {
                Phases.CurrentSubPhase.CallBack();
                Phases.CurrentSubPhase.Resume();
            }
        }

        private void CancelHitDamage(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();
            CancelResultAndFinish(DieSide.Success);
        }

        private void CancelCritDamage(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();
            CancelResultAndFinish(DieSide.Crit);
            Phases.CurrentSubPhase.Resume();
        }

        private void CancelResultAndFinish(DieSide dieSide)
        {
            Combat.DiceRollAttack.RemoveType(dieSide);
        }

        private bool FilterTargets(GenericShip ship)
        {
            return Combat.ShotInfo.ObstructedByShips.Contains(ship);
        }

        private int GetAiPriority(GenericShip ship)
        {
            int modifier = (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo) ? 1 : 5;
            return ship.PilotInfo.Cost * modifier;
        }

        private void TryRestoreCharge(GenericShip ship, bool flag)
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            if (distInfo.Range < 4 && HostUpgrade.State.Charges < HostUpgrade.State.MaxCharges)
            {
                Messages.ShowInfo("Treacherous: Charge is restored");
                HostUpgrade.State.RestoreCharge();
            }
        }
    }
}

namespace ActionsList
{
    public class TreacherousDiceModification : GenericAction
    {
        public TreacherousDiceModification()
        {
            Name = DiceModificationName = "Treacherous";
            DiceModificationTiming = DiceModificationTimingType.Opposite;
        }

        public override int GetDiceModificationPriority()
        {
            return 0;
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.ShotInfo.ObstructedByShips.Count > 0
                && Source.State.Charges > 0;
        }
    }

}
