using Actions;
using ActionsList;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class BattleMeditation : GenericUpgrade, IVariableCost
    {
        public BattleMeditation() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Battle Meditation",
                UpgradeType.ForcePower,
                cost: 2,
                isLimited: false,
                restriction: new FactionRestriction(Faction.Republic),
                addAction: new ActionInfo(typeof(CoordinateAction), ActionColor.Purple),
                abilityType: typeof(Abilities.SecondEdition.BattleMeditationAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e3/0b/e30ba082-91c7-408b-9738-d631079911c7/swz32_battle-meditation.png";
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> initiativeToCost = new Dictionary<int, int>()
            {
                {0, 0},
                {1, 2},
                {2, 4},
                {3, 6},
                {4, 8},
                {5, 10},
                {6, 12}
            };

            UpgradeInfo.Cost = initiativeToCost[ship.PilotInfo.Initiative];
        }
    }
}


namespace Abilities.SecondEdition
{
    // You cannot coordinate limited ships. While you perform a purple coordinate action, you may coordinate
    // 1 additional friendly non-limited ship of the same type. Both ships must perform the same action.
    public class BattleMeditationAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckCanCoordinate += ForbitLimitedShips;
            HostShip.BeforeActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckCanCoordinate -= ForbitLimitedShips;
            HostShip.BeforeActionIsPerformed -= CheckAbility;
        }

        private void ForbitLimitedShips(GenericShip ship, ref bool canBeCoordinated)
        {
            if (ship.PilotInfo.IsLimited) canBeCoordinated = false;
        }

        private void CheckAbility(GenericAction action, ref bool isFree)
        {
            if (action is CoordinateAction && action.Color == ActionColor.Purple)
            {
                HostShip.OnCheckCoordinateModeModification += SetCustomCoordinateMode;
            }
        }

        private void SetCustomCoordinateMode(ref CoordinateActionData coordinateActionData)
        {
            coordinateActionData.MaxTargets = 2;
            coordinateActionData.SameShipTypeLimit = true;
            coordinateActionData.SameActionLimit = true;

            HostShip.OnCheckCoordinateModeModification -= SetCustomCoordinateMode;
        }
    }
}