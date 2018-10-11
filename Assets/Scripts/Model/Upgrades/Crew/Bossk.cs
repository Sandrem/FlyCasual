using System;
using Upgrade;
using Ship;
using Abilities;
using Tokens;
using UnityEngine;
using RuleSets;
using Abilities.SecondEdition;

namespace UpgradesList
{
    public class Bossk : GenericUpgrade, ISecondEditionUpgrade
    {
        public Bossk() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Bossk";
            Cost = 2;

            isUnique = true;

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(47, 1));

            UpgradeAbilities.Add(new BosskCrewAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 10;

            Types.Clear();
            Types.Add(UpgradeType.Gunner);

            UpgradeAbilities.Clear();
            UpgradeAbilities.Add(new BosskGunnerSe());

            SEImageNumber = 139;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }

    }
}

namespace Abilities.SecondEdition
{
    public class BosskGunnerSe : GenericAbility
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
            if (Combat.ShotInfo.Weapon is PrimaryWeaponClass)
            {
                attackIsPrimaryWeapon = true;
            }
            theShipAttacked = Combat.Defender;
        }

        private void CheckBosskAbility()
        {
            if (!HostShip.Tokens.HasToken(typeof(StressToken)) && attackIsPrimaryWeapon && !HostShip.IsCannotAttackSecondTime)
            {
                Messages.ShowInfoToHuman("Bossk says: GRRRRAAARR, you will receive a stress token and perform another primary attack.");
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
            Combat.StartAdditionalAttack(
                HostShip,
                Cleanup,
                IsPrimaryWeaponShot,
                "Bossk",
                "You must perform an additional primary attack.",
                HostShip,
                false
            );
        }

        private void Cleanup()
        {
            theShipAttacked = null;
            HostShip.IsCannotAttackSecondTime = true;
            Triggers.FinishTrigger();
        }

        private bool IsPrimaryWeaponShot(GenericShip ship, IShipWeapon weapon, bool isSilent)
        {
            if (weapon is PrimaryWeaponClass && ship == theShipAttacked)
            {
                return true;
            }
            else
            {
                Messages.ShowError("Bossk bonus attack must be performed with a primary weapon and be the same target.");
                return false;
            }
        }
    }
}

namespace Abilities
{
    public class BosskCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += RegisterBosskAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= RegisterBosskAbility;
        }

        private void RegisterBosskAbility()
        {
            if (!HostShip.Tokens.HasToken(typeof(StressToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackMissed, PerformBosskAbility);
            }
        }

        private void PerformBosskAbility(object sender, EventArgs e)
        {
            HostShip.ChooseTargetToAcquireTargetLock(
                AssignFocusToken,
                HostShip.PilotName,
                HostShip
            );
        }

        private void AssignFocusToken()
        {
            HostShip.Tokens.AssignToken(typeof(FocusToken), AssignStressToken);
        }

        private void AssignStressToken()
        {
            Messages.ShowInfoToHuman("Bossk: Focus and Stress tokens acquired.");
            HostShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }
    }
}