using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship.SecondEdition.Hwk290LightFreighter
{
    public class Tapusk : Hwk290LightFreighter
    {
        public Tapusk() : base()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Tapusk",
                "Order 66 Informant",
                Faction.Scum,
                5,
                4,
                10,
                isLimited: true,
                abilityType: typeof(Abilities.SecondEdition.TapuskAbility),
                charges: 2,
                regensCharges: 1,
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Crew,
                    UpgradeType.Device,
                    UpgradeType.Illicit,
                    UpgradeType.Modification,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Freighter
                },
                skinName: "Black"
            );

            ImageUrl = "https://i.imgur.com/oIZlcvg.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TapuskAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= CheckAbility;
        }

        private void CheckAbility()
        {
            if (HostShip.State.Charges > 1
                && GetAvailableTargets().Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, StartSelectShipSubphase);
            }
        }

        private void StartSelectShipSubphase(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                ChooseAbility,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "You may spend 2 charges to select 1 enemy ship in your Turret Arc to not recover a Charge or Force.",
                imageSource: HostShip
            );
        }

        private void ChooseAbility()
        {
            SubPhases.SelectShipSubPhase.FinishSelectionNoCallback();

            if (TargetShip.State.RegensCharges > 0
                && TargetShip.State.Charges < TargetShip.State.MaxCharges
                && TargetShip.State.Force < TargetShip.State.MaxForce)
            {
                ChooseWhichDoesntRecover();
            }
            else if (TargetShip.State.Force >= TargetShip.State.MaxForce)
            {
                TargetShip.BeforeChargeRecovers += DontRecoverCharge;
                Triggers.FinishTrigger();
            }
            else
            {
                TargetShip.BeforeForceRecovers += DontRecoverForce;
                Triggers.FinishTrigger();
            }
        }

        private void ChooseWhichDoesntRecover()
        {
            TapuskDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<TapuskDecisionSubphase>(
                "Tapusk Decision",
                Triggers.FinishTrigger
            );

            subphase.Name = HostShip.PilotInfo.PilotName;
            subphase.DescriptionShort = "Select Whether the Targeted enemy ship will not recover a Force or a Charge";
            subphase.ImageSource = HostShip;

            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = true;

            subphase.AddDecision(
                "Charge",
                delegate { TargetShip.BeforeChargeRecovers += DontRecoverCharge; DecisionSubPhase.ConfirmDecision(); }
            );

            subphase.AddDecision(
                "Force",
                delegate { TargetShip.BeforeForceRecovers += DontRecoverForce; DecisionSubPhase.ConfirmDecision(); }
            );

            subphase.Start();

        }

        private void DontRecoverForce(ref bool recover)
        {
            HostShip.SpendCharges(2);
            Messages.ShowInfo(TargetShip.PilotInfo.PilotName + " prevented from recovering Force.");
            TargetShip.BeforeForceRecovers -= DontRecoverForce;
            recover = false;
        }

        private void DontRecoverCharge(ref bool recover)
        {
            HostShip.SpendCharges(2);
            Messages.ShowInfo(TargetShip.PilotInfo.PilotName + " prevented from recovering Charge.");
            TargetShip.BeforeChargeRecovers -= DontRecoverCharge;
            recover = false;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }

        private bool FilterTargets(GenericShip ship)
        {
            return GetAvailableTargets().Contains(ship);
        }

        public List<GenericShip> GetAvailableTargets()
        {
            return BoardTools.Board.GetShipsAtRange(HostShip, new UnityEngine.Vector2(0, 3), Team.Type.Enemy)
                .Where(n => HostShip.ArcsInfo.HasShipInTurretArc(n) && ((n.State.RegensCharges > 0 && n.State.Charges < n.State.MaxCharges) || n.State.Force < n.State.MaxForce))
                .ToList();
        }

        private class TapuskDecisionSubphase : DecisionSubPhase { }
    }
}
