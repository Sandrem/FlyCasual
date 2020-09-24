using Movement;
using System;
using UnityEngine;
using Upgrade;

namespace Abilities
{
    public class FlipCardAction : AbilityPart
    {
        private TriggeredAbility Ability;

        public Func<GenericDualUpgrade> GetDualCard { get; }
        public FlipCardAction(Func<GenericDualUpgrade> getDualCard)
        {
            GetDualCard = getDualCard;
        }

        public override void DoAction(TriggeredAbility ability)
        {
            Ability = ability;

            GetDualCard().Flip();
            Triggers.FinishTrigger();
        }
    }
}
