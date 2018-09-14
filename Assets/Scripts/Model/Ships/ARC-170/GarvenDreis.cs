using RuleSets;
using Ship;
using SubPhases;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace ARC170
    {
        public class GarvenDreis : ARC170, ISecondEditionPilot
        {
            public GarvenDreis() : base()
            {
                PilotName = "Garven Dreis";
                PilotSkill = 4;
                Cost = 51;

                IsUnique = true;

                PilotRuleType = typeof(SecondEdition);
                PilotAbilities.Add(new Abilities.SecondEdition.GarvenDreisAbilitySE());

                SEImageNumber = 66;
            }

            public void AdaptPilotToSecondEdition()
            {
                // Not required
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GarvenDreisAbilitySE : GarvenDreisAbility
    {
        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            // Change the ability to work at r3.
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 3);
        }
    }
}