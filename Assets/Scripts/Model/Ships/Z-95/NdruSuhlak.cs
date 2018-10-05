using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using RuleSets;

namespace Ship
{
    namespace Z95
    {
        public class NdruSuhlak : Z95, ISecondEditionPilot
        {
            public NdruSuhlak() : base()
            {
                PilotName = "N'dru Suhlak";
                PilotSkill = 7;
                Cost = 17;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                faction = Faction.Scum;

                PilotAbilities.Add(new NdruSuhlakAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 31;

                PilotAbilities.RemoveAll(ability => ability is Abilities.NdruSuhlakAbility);
                PilotAbilities.Add(new Abilities.SecondEdition.NdruSuhlakAbilitySE());

                SEImageNumber = 169;
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

        protected virtual void CheckNdruSuhlakAbility(ref int value)
        {
            if (BoardTools.Board.GetShipsAtRange(HostShip, new Vector2(1,2), Team.Type.Friendly).Count == 1) value++;
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NdruSuhlakAbilitySE : NdruSuhlakAbility
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