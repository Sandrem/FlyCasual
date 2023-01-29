using Content;
using Ship;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class BodicaVenj : FangFighter
        {
            public BodicaVenj() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Bodica Venj",
                    "Wrathful Warrior",
                    Faction.Rebel,
                    4,
                    5,
                    6,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.BodicaVenjAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Talent,
                        UpgradeType.Talent,
                        UpgradeType.Torpedo
                    },
                    tags: new List<Tags>()
                    {
                        Tags.Mandalorian 
                    },
                    skinName: "Bodica Venj"
                );

                ImageUrl = "https://infinitearenas.com/xw2/images/pilots/bodicavenj.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class BodicaVenjAbility : GenericAbility
    {
        GenericShip bonusAttackTarget;

        public override void ActivateAbility()
        {
            GenericShip.OnAttackFinishGlobal += CheckBodicaVenjAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnAttackFinishGlobal -= CheckBodicaVenjAbility;
        }

        private void CheckBodicaVenjAbility(GenericShip ship)
        {

            if (!HostShip.IsDepleted && !HostShip.IsCannotAttackSecondTime && Tools.IsSameTeam(Combat.Defender, HostShip) && Combat.Defender.ShipId != HostShip.ShipId)
            {
                bonusAttackTarget = Combat.Attacker;
                bonusAttackTarget.OnCombatCheckExtraAttack += RegisterBodicaVenjAbility;
            }
        }

        private void RegisterBodicaVenjAbility(GenericShip ship)
        {
            bonusAttackTarget.OnCombatCheckExtraAttack -= RegisterBodicaVenjAbility;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseBodicaVenjAbility);
        }

        private void UseBodicaVenjAbility(object sender, EventArgs e)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " can perform a bonus attack against " + bonusAttackTarget.PilotInfo.PilotName + " and then gain 1 deplete token.");

            Combat.StartSelectAttackTarget
            (
                HostShip,
                Cleanup,
                IsPrimaryWeaponShot,
                HostShip.PilotInfo.PilotName,
                "You may perform a bonus attack against " + bonusAttackTarget.PilotInfo.PilotName + " and then gain 1 deplete token.",
                HostShip
            );
        }

        private bool IsPrimaryWeaponShot(GenericShip ship, IShipWeapon weapon, bool isSilent)
        {
            if (weapon.WeaponType == WeaponTypes.PrimaryWeapon && ship == bonusAttackTarget)
            {
                return true;
            }
            else if (weapon.WeaponType != WeaponTypes.PrimaryWeapon)
            {
                Messages.ShowError(HostShip.PilotInfo.PilotName + "'s bonus attack must be performed using primary weapon");
                return false;
            }
            else
            {
                Messages.ShowError(HostShip.PilotInfo.PilotName + "'s bonus attack must target " + bonusAttackTarget.PilotInfo.PilotName);
                return false;
            }
        }

        private void Cleanup()
        {
            bonusAttackTarget = null;
            HostShip.IsAttackPerformed = true;
            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped)
            {
                HostShip.IsCannotAttackSecondTime = false;
                Triggers.FinishTrigger();
            }
            else
            {
                HostShip.IsCannotAttackSecondTime = true;
                HostShip.Tokens.AssignToken(typeof(DepleteToken), Triggers.FinishTrigger);
            }
        }
    }
}