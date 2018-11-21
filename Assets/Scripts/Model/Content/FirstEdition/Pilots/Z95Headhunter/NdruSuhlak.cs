using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace FirstEdition.Z95Headhunter
    {
        public class NdruSuhlak : Z95Headhunter
        {
            public NdruSuhlak() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "N'dru Suhlak",
                    7,
                    17,
                    limited: 1,
                    abilityType: typeof(Abilities.FirstEdition.NdruSuhlakAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);

                ShipInfo.Faction = Faction.Scum;
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class NdruSuhlakAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckNdruSuhlakAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckNdruSuhlakAbility;
        }

        protected virtual void CheckNdruSuhlakAbility(ref int value)
        {
            if (BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(1, 2), Team.Type.Friendly).Count == 1) value++;
        }
    }
}