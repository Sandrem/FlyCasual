using Arcs;
using Content;
using Ship;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.EWing
    {
        public class CorranHorn : EWing
        {
            public CorranHorn() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Corran Horn",
                    "Tenacious Investigator",
                    Faction.Rebel,
                    5,
                    7,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CorranHornAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Tech,
                        UpgradeType.Sensor,
                        UpgradeType.Sensor,
                        UpgradeType.Torpedo,
                        UpgradeType.Astromech,
                        UpgradeType.Modification
                    },
                    seImageNumber: 50,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );

                ModelInfo.SkinName = "Green";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //At initiative 0, you may perform a bonus primary attack against an enemy ship in your bullseye firing arc. 
    //If you do, at the start of the next Planning Phase, gain 1 disarm token.

    public abstract class CorranHornAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnEngagementInitiativeChanged += RegisterCorranHornAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEngagementInitiativeChanged -= RegisterCorranHornAbility;
        }

        private void RegisterCorranHornAbility()
        {
            if (!HostShip.Tokens.HasToken(typeof(WeaponsDisabledToken)) && Phases.CurrentSubPhase.RequiredInitiative == 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnEngagementInitiativeChanged, UseCorranHornAbility);
            }
        }

        private void UseCorranHornAbility(object sender, System.EventArgs e)
        {
            Combat.StartSelectAttackTarget(
                HostShip,
                AfterExtraAttackSubPhase,
                IsBullsEyePrimary,
                HostShip.PilotInfo.PilotName,
                "You may perform a bonus bullseye primary attack\nGain 1 disarm token next round",
                HostShip
            );
        }

        private void AfterExtraAttackSubPhase()
        {
            // "Weapons disabled" token is assigned only if attack was successfully performed
            if (!HostShip.IsAttackSkipped) Phases.Events.OnRoundStart += RegisterAssignWeaponsDisabledTrigger;

            Triggers.FinishTrigger();
        }

        private void RegisterAssignWeaponsDisabledTrigger()
        {
            Phases.Events.OnRoundStart -= RegisterAssignWeaponsDisabledTrigger;
            RegisterAbilityTrigger(TriggerTypes.OnRoundStart, AssignWeaponsDisabledTrigger);
        }

        private void AssignWeaponsDisabledTrigger(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(WeaponsDisabledToken), Triggers.FinishTrigger);
        }

        private bool IsBullsEyePrimary(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;
            if (weapon.WeaponType == WeaponTypes.PrimaryWeapon && HostShip.SectorsInfo.IsShipInSector(defender, ArcType.Bullseye))
            {
                result = true;
            }
            else
            {
                if (weapon.WeaponType != WeaponTypes.PrimaryWeapon)
                {
                    if (!isSilent) Messages.ShowError("This attack must be performed with the primary weapon");
                }
                else
                {
                    if (!isSilent) Messages.ShowError("This attack must be performed against a target in the ship's Bullseye arc");
                }
            }
            return result;
        }
    }
}