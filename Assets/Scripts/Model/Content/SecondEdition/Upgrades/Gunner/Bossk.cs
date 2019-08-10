﻿using Ship;
using Upgrade;
using System;
using UnityEngine;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class Bossk : GenericUpgrade
    {
        public Bossk() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Bossk",
                UpgradeType.Gunner,
                cost: 10,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.SecondEdition.BosskGunnerAbility),
                seImageNumber: 139
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class BosskGunnerAbility : GenericAbility
    {
        bool attackIsPrimaryWeapon = false;
        GenericShip theShipAttacked;

        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += CheckBosskAbility;
            HostShip.OnAttackStartAsAttacker += CheckForPrimaryArc;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= CheckBosskAbility;
            HostShip.OnAttackStartAsAttacker -= CheckForPrimaryArc;
        }

        private void CheckForPrimaryArc()
        {
            if (Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon)
            {
                attackIsPrimaryWeapon = true;
            }
            theShipAttacked = Combat.Defender;
        }

        private void CheckBosskAbility()
        {
            if (!HostShip.Tokens.HasToken(typeof(StressToken)) && attackIsPrimaryWeapon && !HostShip.IsCannotAttackSecondTime)
            {
                Messages.ShowInfoToHuman("Bossk: You will receive a stress token and perform another primary attack");
                HostShip.OnCombatCheckExtraAttack += RegisterBosskAbility;
            }
        }

        private void RegisterBosskAbility(GenericShip ship)
        {
            HostShip.OnCombatCheckExtraAttack -= RegisterBosskAbility;
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack,
                (sender, e) => HostShip.Tokens.AssignToken(typeof(StressToken), () => UseBosskAbility(sender, e)));
        }

        private void UseBosskAbility(object sender, System.EventArgs e)
        {
            HostShip.IsCannotAttackSecondTime = true;

            Combat.StartSelectAttackTarget(
                HostShip,
                Cleanup,
                IsPrimaryWeaponShot,
                "Bossk",
                "You must perform an additional primary attack",
                HostShip,
                false
            );
        }

        private void Cleanup()
        {
            theShipAttacked = null;
            HostShip.IsAttackPerformed = true;
            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;
            Triggers.FinishTrigger();
        }

        private bool IsPrimaryWeaponShot(GenericShip ship, IShipWeapon weapon, bool isSilent)
        {
            if (weapon.WeaponType == WeaponTypes.PrimaryWeapon && ship == theShipAttacked)
            {
                return true;
            }
            else if(weapon.WeaponType != WeaponTypes.PrimaryWeapon)
            {
                Messages.ShowError("Bossk's bonus attack must be performed using Bossk's primary weapon");
                return false;
            }
            else
            {
                Messages.ShowError("Bossk's bonus attack must target " + ship.PilotInfo.PilotName);
                return false;
            }
        }
    }
}