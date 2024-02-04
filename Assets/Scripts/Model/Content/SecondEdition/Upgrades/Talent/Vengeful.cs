using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Vengeful : GenericUpgrade
    {
        public Vengeful() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Vengeful",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.VengefulAbility)
            );

            IsHidden = true;

            ImageUrl = "https://i.imgur.com/Y0aJSkT.jpg";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class VengefulAbility : GenericAbility
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
            if (!Tools.IsSameTeam(HostShip, ship)) return;
            
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
                || (HostShip.State.MaxCharges > 0
                    && HostShip.State.RegensCharges != 0
                    && HostShip.State.Charges < HostShip.State.MaxCharges);
        }

        private void AskWhatToDo(object sender, EventArgs e)
        {
            VengefulDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<VengefulDecisionSubphase>("Vengeful Decision", Triggers.FinishTrigger);

            subphase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
            subphase.DescriptionLong = "Choose what to do:";

            foreach (GenericToken token in HostShip.Tokens.GetTokensByColor(TokenColors.Red))
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
                && HostShip.State.RegensCharges != 0
                && HostShip.State.Charges < HostShip.State.MaxCharges
            )
            {
                subphase.AddDecision(
                    $"Recover charge on {HostShip.PilotInfo.PilotName}",
                    RecoverPilotCharge
                );
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

        private class VengefulDecisionSubphase : DecisionSubPhase { }
    }
}