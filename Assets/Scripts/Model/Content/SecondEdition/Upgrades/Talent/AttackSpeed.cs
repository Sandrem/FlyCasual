using Upgrade;
using Ship;
using System.Collections.Generic;
using System;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class AttackSpeed : GenericUpgrade
    {
        public AttackSpeed() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "Attack Speed",
                UpgradeType.Talent,
                cost: 0,
                abilityType: typeof(Abilities.SecondEdition.AttackSpeedAbility)
            );

            IsHidden = true;

            ImageUrl = "https://i.imgur.com/YWhDshn.jpg";
        }
    }
}

namespace Abilities.SecondEdition
{
    //After you fully execute a speed 3-4 straight maneuver you may perform a straight boost action.
    public class AttackSpeedAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            //AI doesn't use ability
            if (HostShip.Owner.UsesHotacAiRules) return;

            if (HostShip.AssignedManeuver.Speed >= 3
                && HostShip.AssignedManeuver.Speed <= 4
                && HostShip.AssignedManeuver.Bearing == Movement.ManeuverBearing.Straight
                && !HostShip.IsBumped)
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskUseAbility);
            }
        }

        private void AskUseAbility(object sender, EventArgs e)
        {
            HostShip.BeforeActionIsPerformed += RegisterAbilityTrigger;

            HostShip.OnGetAvailableBoostTemplates += ForbidNonStraightBoost;

            HostShip.AskPerformFreeAction(
                new BoostAction(),
                CleanUp,
                HostUpgrade.UpgradeInfo.Name,
                "After you fully execute a speed 3-4 straight maneuver you may boost using the 1 straight template."
            );
        }

        private void ForbidNonStraightBoost(List<BoostMove> availableTemplates, GenericAction action)
        {
            availableTemplates.RemoveAll(n => n.Template != ActionsHolder.BoostTemplates.Straight1);
        }

        private void RegisterAbilityTrigger(GenericAction action, ref bool isFreeAction)
        {
            HostShip.BeforeActionIsPerformed -= RegisterAbilityTrigger;

            RegisterAbilityTrigger(
                TriggerTypes.OnFreeAction,
                delegate { Triggers.FinishTrigger(); }
            );
        }

        private void CleanUp()
        {
            HostShip.BeforeActionIsPerformed -= RegisterAbilityTrigger;
            HostShip.OnGetAvailableBoostTemplates -= ForbidNonStraightBoost;
            Triggers.FinishTrigger();
        }
    }
}