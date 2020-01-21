using Ship;
using Upgrade;
using ActionsList;
using SubPhases;
using Actions;
using Tokens;
using System;
using BoardTools;
using System.Collections.Generic;

namespace UpgradesList.SecondEdition
{
    public class C3POResistance : GenericUpgrade
    {
        public C3POResistance() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "C-3PO",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                addActions: new List<ActionInfo>(){
                    new ActionInfo(typeof(CalculateAction)),
                    new ActionInfo(typeof(CoordinateAction), ActionColor.Red)
                },
                abilityType: typeof(Abilities.SecondEdition.C3POResistanceCrewAbility)
            );

            NameCanonical = "c3po-crew";

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/a0e80bb89748e7415e945a050f82ba84.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class C3POResistanceCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckCalculateBonus;
            HostShip.OnCheckRange += CheckCoordinateRangeModification;
        }
                
        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckCalculateBonus;
            HostShip.OnCheckRange -= CheckCoordinateRangeModification;
        }

        private void CheckCoordinateRangeModification(GenericShip anotherShip, int minRange, int maxRange, RangeCheckReason reason, ref bool isInRange)
        {
            if (reason == RangeCheckReason.CoordinateAction && anotherShip.ActionBar.HasAction(typeof(CalculateAction)))
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, anotherShip);
                if (distInfo.Range > 2) isInRange = true;
            }
        }

        private void CheckCalculateBonus(GenericAction action)
        {
            if (action is CalculateAction || action is CoordinateAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AssignBonusCalculateToken);
            }
        }

        private void AssignBonusCalculateToken(object sender, EventArgs e)
        {
            Messages.ShowInfo(string.Format("{0}: {1} gains Calculate token", HostUpgrade.UpgradeInfo.Name, HostShip.PilotInfo.PilotName));

            HostShip.Tokens.AssignToken(typeof(CalculateToken), Triggers.FinishTrigger);
        }
    }
}