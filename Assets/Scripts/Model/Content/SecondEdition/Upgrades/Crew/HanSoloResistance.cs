using Ship;
using Upgrade;
using Movement;
using Actions;
using ActionsList;
using System;
using BoardTools;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class HanSoloResistance : GenericUpgrade
    {
        public HanSoloResistance() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Han Solo (Resistance)",
                UpgradeType.Crew,
                cost: 6,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                addAction: new ActionInfo(typeof(EvadeAction), ActionColor.Red),
                abilityType: typeof(Abilities.SecondEdition.HanSoloResistanceCrewAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/622ea4b573afbb5c95b3e9f2989a8aef.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class HanSoloResistanceCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is EvadeAction)
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, AssignAdditionalEvadeTokens);
            }
        }

        private void AssignAdditionalEvadeTokens(object sender, System.EventArgs e)
        {
            int shipsInRangeCount = 0;
            foreach (GenericShip enemyShip in HostShip.Owner.EnemyShips.Values)
            {
                DistanceInfo distanceInfo = new DistanceInfo(HostShip, enemyShip);
                if (distanceInfo.Range <= 1) shipsInRangeCount++;
            }

            if (shipsInRangeCount > 0) Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Gained additional " + shipsInRangeCount + " Evade token(s)");

            HostShip.Tokens.AssignTokens(
                () => new EvadeToken(HostShip),
                shipsInRangeCount,
                Triggers.FinishTrigger
            );
        }
    }
}