using Ship;
using Upgrade;
using Movement;
using Actions;
using ActionsList;
using System;
using BoardTools;
using Tokens;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class HanSoloResistance : GenericUpgrade
    {
        public HanSoloResistance() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Han Solo",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                addAction: new ActionInfo(typeof(EvadeAction), ActionColor.Red),
                abilityType: typeof(Abilities.SecondEdition.HanSoloResistanceCrewAbility)
            );

            NameCanonical = "hansolo-crew";

            Avatar = new AvatarInfo(
                Faction.Resistance,
                new Vector2(298, 1),
                new Vector2(150, 150)
            );
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

            if (shipsInRangeCount > 0) Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + " caused " + HostShip.PilotInfo.PilotName + " to gain " + shipsInRangeCount + " Evade token(s)");

            HostShip.Tokens.AssignTokens(
                () => new EvadeToken(HostShip),
                shipsInRangeCount,
                Triggers.FinishTrigger
            );
        }
    }
}