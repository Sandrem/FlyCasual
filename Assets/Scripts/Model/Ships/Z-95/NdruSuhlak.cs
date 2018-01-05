using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;

namespace Ship
{
    namespace Z95
    {
        public class NdruSuhlak : Z95
        {
            public NdruSuhlak() : base()
            {
                PilotName = "N'dru Suhlak";
                PilotSkill = 7;
                Cost = 17;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                SkinName = "N'dru Suhlak";

                faction = Faction.Scum;

                PilotAbilities.Add(new NdruSuhlakAbility());
            }
        }
    }
}

namespace Abilities
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

        private void CheckNdruSuhlakAbility(ref int value)
        {
            if (Board.BoardManager.GetShipsAtRange(HostShip, new Vector2(1,2), Team.Type.Friendly).Count == 1) value++;
        }
    }
}
