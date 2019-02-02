using Ship;
using Upgrade;
using UnityEngine;
using System.Collections.Generic;
using ActionsList;
using Tokens;

namespace UpgradesList.FirstEdition
{
    public class ReconSpecialist : GenericUpgrade
    {
        public ReconSpecialist() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Recon Specialist",
                UpgradeType.Crew,
                cost: 3,
                abilityType: typeof(Abilities.FirstEdition.ReconSpecialistAbility)
            );

            Avatar = new AvatarInfo(Faction.None, new Vector2(42, 3));
        }        
    }
}

namespace Abilities.FirstEdition
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

        private void RegisterTrigger(GenericShip ship)
        {
            HostShip.OnActionDecisionSubphaseEnd -= RegisterTrigger;

            Triggers.RegisterTrigger(new Trigger()
            {
                Name = HostName + "'s ability",
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