using Upgrade;
using Ship;
using System;
using SubPhases;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class WeaponsSystemsOfficer : GenericUpgrade
    {
        public WeaponsSystemsOfficer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Weapons Systems Officer",
                UpgradeType.Gunner,
                cost: 5,
                abilityType: typeof(Abilities.SecondEdition.WeaponsSystemsOfficerAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Rebel,
                new Vector2(248, 12)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/c8/de/c8de95b1-c5f8-467e-b9ef-0d81cc92f72e/swz82_a1_upgrade_weapon-systems.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class WeaponsSystemsOfficerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsAttacker -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (Combat.ChosenWeapon.WeaponInfo.RequiresTokens.Contains(typeof(Tokens.BlueTargetLockToken)))
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackFinish, AskToAquireLock);
            }
        }

        private void AskToAquireLock(object sender, EventArgs e)
        {
            AskToUseAbility(
                "Weapons Systems Officer",
                AlwaysUseByDefault,
                GainLockOnDefender,
                descriptionLong: "Do you want to gain a acquire on the defender?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void GainLockOnDefender(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            ActionsHolder.AcquireTargetLock(
                HostShip,
                Combat.Defender,
                Triggers.FinishTrigger,
                Triggers.FinishTrigger
            );
        }
    }
}