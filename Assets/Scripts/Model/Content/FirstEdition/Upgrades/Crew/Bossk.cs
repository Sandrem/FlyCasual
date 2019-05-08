using Ship;
using Upgrade;
using System;
using UnityEngine;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class Bossk : GenericUpgrade
    {
        public Bossk() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Bossk",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum),
                abilityType: typeof(Abilities.FirstEdition.BosskCrewAbility)
            );

            Avatar = new AvatarInfo(Faction.Scum, new Vector2(47, 1));
        }        
    }
}

namespace Abilities.FirstEdition
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
                HostShip.PilotInfo.PilotName,
                HostShip
            );
        }

        private void AssignFocusToken()
        {
            HostShip.Tokens.AssignToken(typeof(FocusToken), AssignStressToken);
        }

        private void AssignStressToken()
        {
            Messages.ShowInfoToHuman("Bossk: Focus and Stress tokens acquired");
            HostShip.Tokens.AssignToken(typeof(StressToken), Triggers.FinishTrigger);
        }
    }
}