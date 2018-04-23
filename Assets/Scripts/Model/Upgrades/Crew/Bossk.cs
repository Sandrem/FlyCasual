using System;
using Upgrade;
using Ship;
using Abilities;
using Tokens;
using UnityEngine;

namespace UpgradesList
{
    public class Bossk : GenericUpgrade
    {
        public Bossk() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Bossk";
            Cost = 2;

            isUnique = true;

            ImageUrl = ImageUrls.GetImageUrl(this, "bossk-crew.png");

            AvatarOffset = new Vector2(47, 1);

            UpgradeAbilities.Add(new BosskCrewAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
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
                HostShip.ImageUrl
            );            
        }

        private void AssignFocusToken()
        {
            HostShip.Tokens.AssignToken(new FocusToken(HostShip), AssignStressToken);
        }

        private void AssignStressToken()
        {
            Messages.ShowInfoToHuman("Bossk: Focus and Stress tokens acquired.");
            HostShip.Tokens.AssignToken(new StressToken(HostShip), Triggers.FinishTrigger);
        }
    }
}