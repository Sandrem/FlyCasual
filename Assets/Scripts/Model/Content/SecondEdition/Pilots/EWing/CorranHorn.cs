using Arcs;
using Ship;
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
                PilotInfo = new PilotCardInfo(
                    "Corran Horn",
                    5,
                    66,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CorranHornAbility),
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 50
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
    public class CorranHornAbility : CorranHornBaseAbility
    {
        public CorranHornAbility()
        {
            TriggerType = TriggerTypes.OnEngagementInitiativeChanged;
            Description = "You may perform a bonus bullseye primary attack\nGain 1 disarm token next round";
            ExtraAttackFilter = IsBullsEyePrimary;
        }

        public override void ActivateAbility()
        {
            Phases.Events.OnEngagementInitiativeChanged += RegisterCorranHornAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnEngagementInitiativeChanged -= RegisterCorranHornAbility;
        }

        protected override void RegisterCorranHornAbility()
        {
            if (!HostShip.Tokens.HasToken(typeof(WeaponsDisabledToken)) && Phases.CurrentSubPhase.RequiredInitiative == 0)
            {
                RegisterAbilityTrigger(TriggerType, UseCorranHornAbility);
            }
        }

        private bool IsBullsEyePrimary(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = false;
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon && HostShip.SectorsInfo.IsShipInSector(defender, ArcType.Bullseye))
            {
                result = true;
            }
            else
            {
                if(Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon)
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