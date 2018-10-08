﻿using ActionsList;
using UnityEngine;
using Upgrade;
using Abilities;
using RuleSets;
using Tokens;

namespace UpgradesList
{
    class ReconSpecialist : GenericUpgrade
    {
        public ReconSpecialist() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Recon Specialist";
            Cost = 3;

            Avatar = new AvatarInfo(Faction.None, new Vector2(42, 3));

            UpgradeAbilities.Add(new ReconSpecialistAbility());
        }
    }
}

namespace Abilities
{
    public class ReconSpecialistAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckConditions;
        }

        private void CheckConditions(GenericAction action)
        {
            if (action is FocusAction)
            {
                HostShip.OnActionDecisionSubphaseEnd += RegisterTrigger;
            }
        }

        private void RegisterTrigger(Ship.GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= RegisterTrigger;

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Recon Specialist's ability",
                TriggerType = TriggerTypes.OnActionDecisionSubPhaseEnd,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = DoReconSpecialistAbility
            });
        }

        private void DoReconSpecialistAbility(object sender, System.EventArgs e)
        {
            HostShip.Tokens.AssignToken(typeof(FocusToken), Triggers.FinishTrigger);
        }
    }
}
