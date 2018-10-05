using BoardTools;
using RuleSets;
using Ship;
using System;
using Upgrade;

namespace Ship
{
    namespace UWing
    {
        public class MagvaYarro : UWing, ISecondEditionPilot
        {
            public MagvaYarro() : base()
            {
                PilotName = "Magva Yarro";
                PilotSkill = 3;
                Cost = 50;

                IsUnique = true;

                PrintedUpgradeIcons.Add(UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                PilotAbilities.Add(new Abilities.SecondEdition.MagvaYarroPilotAbilitySE());

                SEImageNumber = 57;
            }

            public void AdaptPilotToSecondEdition()
            {
                // not needed
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class MagvaYarroPilotAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            DiceRerollManager.OnMaxDiceRerollAllowed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            DiceRerollManager.OnMaxDiceRerollAllowed -= CheckAbility;
        }

        private void CheckAbility(ref int maxDiceRerollCount)
        {
            if (Combat.Defender.Owner.PlayerNo != HostShip.Owner.PlayerNo) return;

            DistanceInfo distInfo = new DistanceInfo(HostShip, Combat.Defender);
            if (distInfo.Range > 2) return;

            if (maxDiceRerollCount > 1) maxDiceRerollCount = 1;
        }
    }
}
