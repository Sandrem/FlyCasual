using Ship;
using Upgrade;
using ActionsList;
using SubPhases;
using Actions;
using Tokens;
using System;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class K2SO : GenericUpgrade
    {
        public K2SO() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "K-2SO",
                UpgradeType.Crew,
                cost: 8,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Rebel),
                addActions: new List<ActionInfo> { new ActionInfo(typeof(CalculateAction)), new ActionInfo(typeof(JamAction)) },
                abilityType: typeof(Abilities.SecondEdition.K2SOAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e9/71/e97130c3-368d-4453-a0ff-51a63d30394c/swz66_k-2so_upgrade.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class K2SOAbility : GenericAbility
    {
        //During the System Phase, you may choose a friendly ship at range 0-3. That ship gains 1 calculate and 1 stress token
        public override void ActivateAbility()
        {
            HostShip.OnSystemsPhaseStart += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnSystemsPhaseStart -= RegisterAbility;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsPhaseStart, Ability);
        }

        protected bool FilterAbilityTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.This, TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 0, 3);
        }

        private void Ability(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                AssignToken,
                FilterAbilityTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                HostName,
                "You may choose a friendly ship at range 0-3. That ship gains 1 calculate and 1 stress token.",
                imageSource: HostUpgrade
            );
        }

        private int GetAiPriority(GenericShip ship)
        {
            //Prioritize high cost pilots that have planned a blue maneuver or are ionized
            var priority = ship.PilotInfo.Cost;

            if ((ship.AssignedManeuver != null && ship.AssignedManeuver.ColorComplexity == Movement.MovementComplexity.Easy) || ship.State.IsIonized)
                priority += 200;
            
            return priority;
        }

        private void AssignToken()
        {
            Messages.ShowInfo("K-2SO: " + TargetShip.PilotInfo.PilotName + " gains 1 calculate and 1 stress token");
            TargetShip.Tokens.AssignToken(typeof(CalculateToken), () =>
            {
                TargetShip.Tokens.AssignToken(typeof(StressToken), SelectShipSubPhase.FinishSelection);
            });
        }
    }
}