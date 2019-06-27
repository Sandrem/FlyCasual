using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class Delta7B : GenericUpgrade, IVariableCost
    {
        public Delta7B() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Delta-7B",
                UpgradeType.Configuration,
                cost: 15,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.Delta7Aethersprite.Delta7Aethersprite)),
                abilityType: typeof(Abilities.SecondEdition.Delta7BAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/d6/97/d697602c-8614-4192-a44d-986fa2d2fd7a/swz_delta-7b.png";
        }

        public void UpdateCost(GenericShip ship)
        {
            Dictionary<int, int> initiativeToCost = new Dictionary<int, int>()
            {
                {0, 14},
                {1, 15},
                {2, 16},
                {3, 17},
                {4, 18},
                {5, 19},
                {6, 20}
            };

            UpgradeInfo.Cost = initiativeToCost[ship.PilotInfo.Initiative];
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Delta7BAbility : GenericAbility
    {
        public override void ActivateAbility() {}

        public override void DeactivateAbility() {}

        public override void ActivateAbilityForSquadBuilder()
        {
            HostShip.ShipInfo.ArcInfo.Arcs.First().Firepower++;
            HostShip.ShipInfo.Agility--;
            HostShip.ShipInfo.Shields += 2;
        }

        public override void DeactivateAbilityForSquadBuilder()
        {
            HostShip.ShipInfo.ArcInfo.Arcs.First().Firepower--;
            HostShip.ShipInfo.Agility++;
            HostShip.ShipInfo.Shields -= 2;
        }
    }
}