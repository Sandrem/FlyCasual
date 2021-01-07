using Movement;
using System;
using UnityEngine;
using Upgrade;

namespace Abilities
{
    public class FlipCardAction : AbilityPart
    {
        private GenericAbility Ability;

        public Func<GenericDualUpgrade> GetDualCard { get; }
        public FlipCardAction(Func<GenericDualUpgrade> getDualCard)
        {
            GetDualCard = getDualCard;
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;

            GetDualCard().Flip();
            Triggers.FinishTrigger();
        }
    }
}
