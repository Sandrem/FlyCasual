using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;
using UpgradesList.SecondEdition;

namespace UpgradesList.SecondEdition
{
    public class Cutthroat : GenericUpgrade
    {
        public Cutthroat() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Cutthroat",
                UpgradeType.Talent,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.CutthroatAbility),
                restriction: new FactionRestriction(Faction.Scum)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/fd/7b/fd7b2ccc-d500-4a02-bb2a-9e0538406d65/swz85_upgrade_cutthroat.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class CutthroatAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, bool flag)
        {
            if (!Tools.IsSameTeam(HostShip, ship) || Tools.IsSameShip(HostShip, ship)) return;
            if (!ship.PilotInfo.IsLimited && !ship.UpgradeBar.HasUpgradeInstalled(typeof(Cutthroat))) return;
            
            DistanceInfo distanceInfo = new DistanceInfo(HostShip, ship);
            if (distanceInfo.Range > 3) return;

            if (HasWhatToDo())
            {
                RegisterAbilityTrigger(
                    TriggerTypes.OnShipIsDestroyed,
                    AskWhatToDo,
                    customTriggerName: $"{HostUpgrade.UpgradeInfo.Name} (ID: {HostShip.ShipId})"
                );
            }
        }

        private bool HasWhatToDo()
        {
            return HostShip.Tokens.HasTokenByColor(TokenColors.Red)
                || HostShip.Tokens.HasTokenByColor(TokenColors.Orange)
                || (HostShip.State.MaxCharges > 0
                    && HostShip.State.RegensCharges == 0
                    && HostShip.State.Charges < HostShip.State.MaxCharges)
                || HasUpgradeToRecharge();
        }

        private bool HasUpgradeToRecharge()
        {
            foreach (GenericUpgrade upgrade in HostShip.UpgradeBar.GetUpgradesAll())
            {
                if (upgrade.State.MaxCharges > 0
                    && upgrade.UpgradeInfo.RegensChargesCount == 0
                    && upgrade.State.Charges < upgrade.State.MaxCharges
                    && !upgrade.UpgradeInfo.CannotBeRecharged
                )
                {
                    return true;
                }
            }

            return false;
        }

        private void AskWhatToDo(object sender, EventArgs e)
        {
            CutthroatDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<CutthroatDecisionSubphase>("Cutthroat Decision", Triggers.FinishTrigger);

            subphase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
            subphase.DescriptionLong = "Choose what to do:";
            subphase.ImageSource = HostUpgrade;

            foreach (GenericToken token in HostShip.Tokens.GetTokensByColor(TokenColors.Red, TokenColors.Orange))
            {
                if (!subphase.GetDecisions().Any(n => n.Name == GetRemoveTokenDescription(token)))
                {
                    subphase.AddDecision(
                        GetRemoveTokenDescription(token),
                        delegate { RemoveToken(token); }
                    );
                }
            }

            if (HostShip.State.MaxCharges > 0
                && HostShip.State.RegensCharges == 0
                && HostShip.State.Charges < HostShip.State.MaxCharges
            )
            {
                subphase.AddDecision(
                    $"Recover charge on {HostShip.PilotInfo.PilotName}",
                    RecoverPilotCharge
                );
            }

            foreach (GenericUpgrade upgrade in HostShip.UpgradeBar.GetUpgradesAll())
            {
                if (upgrade.State.MaxCharges > 0
                    && upgrade.UpgradeInfo.RegensChargesCount == 0
                    && upgrade.State.Charges < upgrade.State.MaxCharges
                    && !upgrade.UpgradeInfo.CannotBeRecharged
                )
                {
                    subphase.AddDecision(
                        $"Recover charge on {upgrade.UpgradeInfo.Name}",
                        delegate { RecoverUpgradeCharge(upgrade); }
                    );
                }
            }

            subphase.ShowSkipButton = true;
            subphase.DecisionOwner = HostShip.Owner;
            subphase.DefaultDecisionName = subphase.GetDecisions().First()?.Name;

            subphase.Start();
        }

        private string GetRemoveTokenDescription(GenericToken token)
        {
            string lockLetter = (token is RedTargetLockToken) ? $" ({(token as RedTargetLockToken).Letter})" : "";
            return $"Remove {token.Name}{lockLetter}";
        }

        private void RemoveToken(GenericToken token)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.Tokens.RemoveToken(token, Triggers.FinishTrigger);
        }

        private void RecoverPilotCharge(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            HostShip.RestoreCharges(1);
            Triggers.FinishTrigger();
        }

        private void RecoverUpgradeCharge(GenericUpgrade upgrade)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            upgrade.State.RestoreCharges(1);
            Triggers.FinishTrigger();
        }

        private class CutthroatDecisionSubphase : DecisionSubPhase { }
    }
}