using System;
using System.Collections.Generic;
using Ship;
using Upgrade;

namespace Ship.SecondEdition.SithInfiltrator
{
    public class DarthMaulPilot: SithInfiltrator
    {
        public DarthMaulPilot()
        {
            PilotInfo = new PilotCardInfo(
                "Darth Maul",
                5,
                65,
                true,
                abilityType: typeof(Abilities.SecondEdition.DarthMaulPilotAbility),
                pilotTitle: "Sith Assassin",
                force: 3,
                extraUpgradeIcon: UpgradeType.Force
            );
            
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/35/d8/35d8295c-1018-4ed7-94a0-c0bff4e6fbbc/swz30_darth-maul.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you perform an attack, you may spend 2 force to perform a bonus primary attack against a different target. 
    //If your attack missed, you may perform that bonus primary attack against the same target instead.
    public class DarthMaulPilotAbility : GenericAbility
    {
        private bool FirstAttackMissed;
        private GenericShip OriginalDefender;

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += SaveOriginalDefender;
            HostShip.OnAttackHitAsAttacker += RegisterHitAbility;
            HostShip.OnAttackMissedAsAttacker += RegisterMissedAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= SaveOriginalDefender;
            HostShip.OnAttackHitAsAttacker -= RegisterHitAbility;
            HostShip.OnAttackMissedAsAttacker -= RegisterMissedAbility;
        }

        private void SaveOriginalDefender()
        {
            OriginalDefender = Combat.Defender;
        }

        private void RegisterMissedAbility()
        {
            FirstAttackMissed = true;
            StartSecondAttack();
        }

        private void RegisterHitAbility()
        {
            FirstAttackMissed = false;
            StartSecondAttack();
        }

        private void StartSecondAttack()
        {
            if (HostShip.State.Force >= 2)
            {
                HostShip.OnCombatCheckExtraAttack += RegisterSecondAttackTrigger;
            }
        }

        private void RegisterSecondAttackTrigger(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterSecondAttackTrigger;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, UseAbility);
        }

        private void UseAbility(object sender, EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishAdditionalAttack,
                    IsAllowedAttack,
                    HostName,
                    "You may spend 2 force to perform a bonus primary attack" + (FirstAttackMissed ? "" : " against a different target"),
                    HostUpgrade
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack an additional time.", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private bool IsAllowedAttack(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            if (weapon.WeaponType != WeaponTypes.PrimaryWeapon)
            {
                if (!isSilent) Messages.ShowError("Your bonus attack must be a primary weapon attack.");
                return false;
            }

            if (!FirstAttackMissed && defender == OriginalDefender)
            {

                if (!isSilent) Messages.ShowError("Your bonus attack must be against a different target.");
                return false;
            }

            return true;
        }

        private void FinishAdditionalAttack()
        {
            if (HostShip.IsAttackPerformed)
            {
                HostShip.State.Force -= 2;
            }
            else
            {
                // If attack is skipped, set this flag, otherwise regular attack can be performed second time
                HostShip.IsAttackPerformed = true;
            }

            Triggers.FinishTrigger();
        }
    }
}
