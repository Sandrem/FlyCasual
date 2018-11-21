using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class NdruSuhlak : Z95AF4Headhunter
        {
            public NdruSuhlak() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "N'dru Suhlak",
                    4,
                    31,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.NdruSuhlakAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ShipInfo.Faction = Faction.Scum;

                SEImageNumber = 169;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NdruSuhlakAbility : Abilities.FirstEdition.NdruSuhlakAbility
    {
        protected override void CheckNdruSuhlakAbility(ref int value)
        {
            if (Combat.ChosenWeapon.GetType() != HostShip.PrimaryWeapon.GetType())
                return;

            if (BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(1, 2), Team.Type.Friendly).Count == 1)
                value++;
        }
    }
}
