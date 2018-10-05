using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tokens;
using RuleSets;

namespace Ship
{
    namespace AlphaClassStarWing
    {
        public class MajorVynder : AlphaClassStarWing, ISecondEditionPilot
        {
            public MajorVynder() : base()
            {
                PilotName = "Major Vynder";
                PilotSkill = 7;
                Cost = 26;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.MajorVynderAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 4;
                Cost = 41;

                SEImageNumber = 135;
            }
        }
    }
}

namespace Abilities
{
    public class MajorVynderAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfDefenceDice += IncreaseDefenceDiceNumber;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfDefenceDice -= IncreaseDefenceDiceNumber;
        }

        private void IncreaseDefenceDiceNumber(ref int diceNumber)
        {
            if (HostShip.Tokens.HasToken(typeof(WeaponsDisabledToken))) diceNumber++;
        }
    }
}
