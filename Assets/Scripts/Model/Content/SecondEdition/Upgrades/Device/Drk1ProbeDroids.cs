using Upgrade;
using System.Collections.Generic;
using ActionsList;
using Ship;
using System;
using Bombs;
using BoardTools;
using Movement;

namespace UpgradesList.SecondEdition
{
    public class Drk1ProbeDroids : GenericUpgrade
    {
        public Drk1ProbeDroids() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "DRK-1 Probe Droids",
                UpgradeType.Device,
                subType: UpgradeSubType.Remote,
                charges: 2,
                cannotBeRecharged: true,
                cost: 5,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.Drk1ProbeDroidsAbility),
                remoteType: typeof(Remote.Drk1ProbeDroid)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/44556cd042e536b41e7e89850e13081a.png";
        }

        public override List<ManeuverTemplate> GetDefaultDropTemplates()
        {
            return new List<ManeuverTemplate>()
            {
                new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Right, ManeuverSpeed.Speed3, isBombTemplate: true),
                new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Left, ManeuverSpeed.Speed3, isBombTemplate: true),
                new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed3, isBombTemplate: true),
                new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed3, isBombTemplate: true),
                new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed3, isBombTemplate: true)
            };
        }

        public override List<ManeuverTemplate> GetDefaultLaunchTemplates()
        {
            return new List<ManeuverTemplate>()
            {
                new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed3),
                new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed3),
                new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed3),
                new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Left, ManeuverSpeed.Speed3),
                new ManeuverTemplate(ManeuverBearing.Turn, ManeuverDirection.Right, ManeuverSpeed.Speed3)
            };

        }
    }
}

namespace Abilities.SecondEdition
{
    public class Drk1ProbeDroidsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEndPhaseStart_NoTriggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEndPhaseStart_NoTriggers -= CheckAbility;
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterOwnAbilityTrigger;
        }

        private void CheckAbility()
        {
            if (HostUpgrade.State.Charges > 0)
            {
                Phases.Events.OnEndPhaseStart_Triggers += RegisterOwnAbilityTrigger;
            }
        }

        private void RegisterOwnAbilityTrigger()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterOwnAbilityTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnEndPhaseStart, AskToUseOwnAbility);
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                DeployRemote,
                descriptionLong: "Do you want to drop or launch 1 DRK-1 Probe Droid using a speed 3 template?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void DeployRemote(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            BombsManager.RegisterBombDropTriggerIfAvailable(
                HostShip,
                TriggerTypes.OnAbilityDirect,
                type: HostUpgrade.GetType()
            );

            Triggers.ResolveTriggers(
                TriggerTypes.OnAbilityDirect,
                delegate {
                    HostUpgrade.State.SpendCharge();
                    Triggers.FinishTrigger();
                }
            );
        }
    }
}