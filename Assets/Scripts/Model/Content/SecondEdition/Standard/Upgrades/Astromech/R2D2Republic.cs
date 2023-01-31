using Upgrade;
using Ship;
using System.Collections.Generic;
using System;
using Remote;
using BoardTools;
using SubPhases;
using System.Linq;

namespace UpgradesList.SecondEdition
{
    public class R2D2Republic : GenericUpgrade
    {
        public R2D2Republic() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "R2-D2",
                UpgradeType.Astromech,
                cost: 8,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.R2D2RepublicAbility),
                restriction: new FactionRestriction(Faction.Republic),
                charges: 2
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c3/6f/c36f6f13-6998-4120-acbf-3c85132ea416/swz79_r2d2.png";

            NameCanonical = "r2d2-republic";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class R2D2RepublicAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementActivationFinish += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementActivationFinish -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (HostUpgrade.State.Charges > 0 && HasReasonToUseAbility())
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementActivationFinish, AskToUseR2D2Ability);
            }
        }

        private bool HasReasonToUseAbility()
        {
            return HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax
                || HostShip.Damage.IsDamaged
                || HasRemoteAtRange0to1();
        }

        private bool HasRemoteAtRange0to1()
        {
            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
            {
                if (enemyShip is GenericRemote)
                {
                    DistanceInfo distInfo = new DistanceInfo(HostShip, enemyShip);
                    if (distInfo.Range < 2) return true;
                }
            }

            return false;
        }

        private void AskToUseR2D2Ability(object sender, EventArgs e)
        {
            R2D2RepublicDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<R2D2RepublicDecisionSubphase>("R2D2 Decision", Triggers.FinishTrigger);

            subphase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
            subphase.DescriptionLong = "You may spend 1 charge and gain 1 Deplete token to:";
            subphase.ImageSource = HostUpgrade;

            if (HostShip.State.ShieldsCurrent < HostShip.State.ShieldsMax)
            {
                subphase.AddDecision("Recover 1 shield", RecoverShield);
            }

            if (HostShip.Damage.HasFacedownCards)
            {
                subphase.AddDecision("Repair 1 facedown damage card", RepairFacedownDamageCard);
            }

            if (HostShip.Damage.HasFaceupCards)
            {
                subphase.AddDecision("Repair 1 faceup damage card", RepairFaceupDamageCard);
            }

            if (HasRemoteAtRange0to1())
            {
                subphase.AddDecision("Remove 1 device at range 0-1", RemoveDevice);
            }

            subphase.DecisionOwner = HostShip.Owner;
            subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;
            subphase.ShowSkipButton = true;

            subphase.Start();
        }

        private void RecoverShield(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();

            HostShip.Tokens.AssignToken(typeof(Tokens.DepleteToken), () =>
            {
                if (HostShip.TryRegenShields())
                {
                    Sounds.PlayShipSound("R2D2-Proud");
                    Messages.ShowInfo(HostName + " causes " + HostShip.PilotInfo.PilotName + " to recover 1 shield and gain 1 deplete token");
                }
                Triggers.FinishTrigger();
            });
        }

        private void RepairFacedownDamageCard(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();
            HostShip.Tokens.AssignToken(typeof(Tokens.DepleteToken), () =>
            {
                if (HostShip.Damage.DiscardRandomFacedownCard())
                {
                    Sounds.PlayShipSound("R2D2-Proud");
                    Messages.ShowInfoToHuman("Facedown Damage card is discarded");
                }

                Triggers.FinishTrigger();
            });
        }

        private void RepairFaceupDamageCard(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostUpgrade.State.SpendCharge();
            HostShip.Tokens.AssignToken(typeof(Tokens.DepleteToken), () =>
            {
                List<GenericDamageCard> shipCritsList = HostShip.Damage.GetFaceupCrits();

                if (shipCritsList.Count == 1)
                {
                    HostShip.Damage.FlipFaceupCritFacedown(shipCritsList.First());
                    Sounds.PlayShipSound("R2D2-Proud");
                    Triggers.FinishTrigger();
                }
                else if (shipCritsList.Count > 1)
                {
                    Phases.StartTemporarySubPhaseOld(
                        HostUpgrade.UpgradeInfo.Name + ": Select faceup ship Crit",
                        typeof(SubPhases.FixCritDecisionSubPhase),
                        Triggers.FinishTrigger
                    );
                }
            });
        }

        private void RemoveDevice(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            SelectTargetForAbility(
                RemoveTargetDevice,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostUpgrade.UpgradeInfo.Name,
                description: "Select a device to remove",
                imageSource: HostUpgrade
            );
        }

        private void RemoveTargetDevice()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            HostUpgrade.State.SpendCharge();

            HostShip.Tokens.AssignToken(typeof(Tokens.DepleteToken), () =>
            {
                Sounds.PlayShipSound("R2D2-Proud");
                Messages.ShowInfo(HostName + " removes " + TargetShip.PilotInfo.PilotName);

                TargetShip.DestroyShipForced(Triggers.FinishTrigger);
            });
        }

        private bool FilterTargets(GenericShip ship)
        {
            if (!(ship is GenericRemote)) return false;

            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            return distInfo.Range < 2;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 1;
        }

        private class R2D2RepublicDecisionSubphase : DecisionSubPhase { }
    }
}

namespace SubPhases
{

    public class FixCritDecisionSubPhase : DecisionSubPhase
    {
        public override void PrepareDecision(System.Action callBack)
        {
            DecisionViewType = DecisionViewTypes.ImagesDamageCard;

            foreach (var shipCrit in Selection.ActiveShip.Damage.GetFaceupCrits(CriticalCardType.Ship).ToList())
            {
                AddDecision(shipCrit.Name, delegate { DiscardCrit(shipCrit); }, shipCrit.ImageUrl);
            }

            DefaultDecisionName = GetDecisions().First().Name;

            callBack();
        }

        private void DiscardCrit(GenericDamageCard critCard)
        {
            Selection.ActiveShip.Damage.FlipFaceupCritFacedown(critCard);
            Sounds.PlayShipSound("R2D2-Proud");
            ConfirmDecision();
        }

    }

}