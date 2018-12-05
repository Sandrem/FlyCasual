using Ship;
using Upgrade;
using UnityEngine;
using ActionsList;
using System.Linq;
using System.Collections.Generic;

namespace UpgradesList.FirstEdition
{
    public class FlightInstructor : GenericUpgrade
    {
        public FlightInstructor() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Flight Instructor",
                UpgradeType.Crew,
                cost: 4,
                abilityType: typeof(Abilities.FirstEdition.FlightInstructorAbility)
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(32, 1));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class FlightInstructorAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGenerateDiceModifications += FlightInstructorActionEffect;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateDiceModifications -= FlightInstructorActionEffect;
        }

        private void FlightInstructorActionEffect(GenericShip host)
        {
            GenericAction newAction = new FlightInstructorActionEffect
            {
                HostShip = host,
                ImageUrl = HostUpgrade.ImageUrl
            };
            host.AddAvailableDiceModification(newAction);
        }
    }
}

namespace ActionsList
{

    public class FlightInstructorActionEffect : GenericAction
    {

        public FlightInstructorActionEffect()
        {
            Name = DiceModificationName = "Flight Instructor";

            // Used for abilities like Dark Curse's that can prevent rerolls
            IsReroll = true;
        }

        public override bool IsDiceModificationAvailable()
        {
            return Combat.AttackStep == CombatStep.Defence;
        }

        public override int GetDiceModificationPriority()
        {
            int result = 0;

            if (Combat.AttackStep == CombatStep.Defence)
            {
                bool canRerollBlank = Combat.Attacker.State.Initiative <= 2 && Combat.DiceRollDefence.BlanksNotRerolled > 0;
                bool canRerollFocus = Combat.DiceRollDefence.FocusesNotRerolled > 0;

                if (Combat.Defender.GetDiceModificationsGenerated().Count(n => n.IsTurnsAllFocusIntoSuccess) > 0)
                {
                    if (canRerollBlank)
                    {
                        result = 95;
                    }
                }
                else
                {
                    if (canRerollBlank || canRerollFocus)
                    {
                        result = 95;
                    }
                }
            }

            return result;
        }

        public override void ActionEffect(System.Action callBack)
        {
            // Can reroll focus if Attacker PS > 2, focus or blank if Attacker PS <= 2
            List<DieSide> allowedRerolls =
                (Combat.Attacker.State.Initiative > 2) ?
                new List<DieSide> { DieSide.Focus } :
                new List<DieSide> { DieSide.Blank, DieSide.Focus };

            DiceRerollManager diceRerollManager = new DiceRerollManager
            {
                SidesCanBeRerolled = allowedRerolls,
                CallBack = callBack
            };

            diceRerollManager.Start();
        }

    }

}