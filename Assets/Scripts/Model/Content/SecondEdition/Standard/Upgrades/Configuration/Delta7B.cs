using Ship;
using System.Collections.Generic;
using System.Linq;
using Upgrade;
using Content;

namespace UpgradesList.SecondEdition
{
    public class Delta7B : GenericUpgrade
    {
        public Delta7B() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Delta-7B",
                UpgradeType.Configuration,
                cost: 15,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.Delta7Aethersprite.Delta7Aethersprite)),
                abilityType: typeof(Abilities.SecondEdition.Delta7BAbility),
                legalityInfo: new List<Legality>
                {
                    Legality.StandardBanned,
                    Legality.ExtendedBanned
                }
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/d6/97/d697602c-8614-4192-a44d-986fa2d2fd7a/swz_delta-7b.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class Delta7BAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.ShipInfo.ArcInfo.Arcs.First().Firepower++;
            HostShip.PrimaryWeapons.First().WeaponInfo.AttackValue++;
            HostShip.ChangeFirepowerBy(1);
            HostShip.ShipInfo.Agility--;
            HostShip.ChangeAgilityBy(-1);
            HostShip.ShipInfo.Shields += 2;
            HostShip.ChangeShieldBy(2);
        }

        public override void DeactivateAbility()
        {
            HostShip.ShipInfo.ArcInfo.Arcs.First().Firepower--;
            HostShip.PrimaryWeapons.First().WeaponInfo.AttackValue--;
            HostShip.ChangeFirepowerBy(-1);
            HostShip.ShipInfo.Agility++;
            HostShip.ChangeAgilityBy(1);
            HostShip.ShipInfo.Shields -= 2;
            HostShip.ChangeShieldBy(-2);
        }
    }
}