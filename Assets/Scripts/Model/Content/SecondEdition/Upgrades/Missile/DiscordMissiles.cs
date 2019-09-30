﻿using Arcs;
using BoardTools;
using Bombs;
using Movement;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class DiscordMissiles : GenericSpecialWeapon
    {
        public DiscordMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Discord Missiles",
                UpgradeType.Missile,
                subType: UpgradeSubType.Remote,
                cost: 4,
                limited: 3,
                charges: 1,
                cannotBeRecharged: true,
                restriction: new FactionRestriction(Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.DiscordMissilesAbility),
                remoteType: typeof(Remote.BuzzDroidSwarm)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/ee2f0b906cb4f1dbcafc274f44b76d3d.png";
        }

        public override List<ManeuverTemplate> GetDefaultDropTemplates()
        {
            return new List<ManeuverTemplate>();
        }

        public override List<ManeuverTemplate> GetDefaultLaunchTemplates()
        {
            return new List<ManeuverTemplate>()
            {
                new ManeuverTemplate(ManeuverBearing.Straight, ManeuverDirection.Forward, ManeuverSpeed.Speed3),
                new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed3),
                new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed3)
            };
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DiscordMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_NoTriggers += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_NoTriggers -= CheckAbility;
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterOwnAbilityTrigger;
        }

        private void CheckAbility()
        {
            if (HostUpgrade.State.Charges > 0 && HostShip.Tokens.HasToken(typeof(CalculateToken)))
            {
                Phases.Events.OnCombatPhaseStart_Triggers += RegisterOwnAbilityTrigger;
            }
        }

        private void RegisterOwnAbilityTrigger()
        {
            Phases.Events.OnEndPhaseStart_Triggers -= RegisterOwnAbilityTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, AskToUseOwnAbility);
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                StartRemoteDeployment,
                descriptionLong: "Do you want to launch 1 Buzz Droid Swarm?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void StartRemoteDeployment(object sender, EventArgs e)
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();

            BombsManager.RegisterBombDropTriggerIfAvailable(
                HostShip,
                TriggerTypes.OnAbilityDirect,
                type: HostUpgrade.GetType()
            );

            Triggers.ResolveTriggers(
                TriggerTypes.OnAbilityDirect,
                FinishRemoteDeployment
            );
        }

        private void FinishRemoteDeployment()
        {
            HostUpgrade.State.SpendCharge();
            HostShip.Tokens.SpendToken(typeof(CalculateToken), Triggers.FinishTrigger);
        }
    }
}