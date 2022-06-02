using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.DroidTriFighter
{
    public class PhlacArphoccPrototype : DroidTriFighter
    {
        public PhlacArphoccPrototype()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Phlac-Arphocc Prototype",
                "Predictive Analysis Protocol",
                Faction.Separatists,
                5,
                5,
                14,
                limited: 2,
                abilityType: typeof(Abilities.SecondEdition.PhlacArphoccPrototypeAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.Talent,
                    UpgradeType.Sensor,
                    UpgradeType.Missile,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Droid
                },
                legality: new List<Legality>
                {
                    Legality.StandartBanned
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/57/f7/57f74986-1cbf-4745-9792-360153f75719/swz81_phlac-prototype_cutout.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PhlacArphoccPrototypeAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            if (HostShip.Tokens.HasToken<BlueTargetLockToken>('*')) flag = true;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToSeeDial);
        }

        private void AskToSeeDial(object sender, EventArgs e)
        {
            if (HostShip.Tokens.HasToken<BlueTargetLockToken>('*'))
            {
                AskToUseAbility(
                    "Phlac-Arphocc Prototype",
                    NeverUseByDefault,
                    AgreeToUseAbility,
                    descriptionLong: "Do you want to spend your lock on a ship to look at that ship's dial?",
                    imageHolder: HostShip,
                    showSkipButton: true,
                    requiredPlayer: HostShip.Owner.PlayerNo
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void AgreeToUseAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            BlueTargetLockToken lockToken = HostShip.Tokens.GetToken<BlueTargetLockToken>('*');
            GenericShip targetShip = lockToken.OtherTargetLockTokenOwner as GenericShip;
            
            HostShip.Tokens.RemoveToken(lockToken, delegate { SeeDial(targetShip); });
        }

        private void SeeDial(GenericShip targetShip)
        { 
            Roster.ToggleManeuverVisibility(targetShip, true);
            targetShip.AlwaysShowAssignedManeuver = true;

            Triggers.FinishTrigger();
        }
    }
}
