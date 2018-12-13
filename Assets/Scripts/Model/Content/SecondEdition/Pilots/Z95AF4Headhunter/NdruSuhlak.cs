using Ship;
using System.Collections.Generic;
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
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.NdruSuhlakAbility),
                    extraUpgradeIcons: new List<UpgradeType>() { UpgradeType.Talent, UpgradeType.Illicit },
                    factionOverride: Faction.Scum,
                    seImageNumber: 169
                );
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
            if (Combat.ChosenWeapon.WeaponType != WeaponTypes.PrimaryWeapon)
                return;

            if (BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(1, 2), Team.Type.Friendly).Count == 1)
                value++;
        }
    }
}
