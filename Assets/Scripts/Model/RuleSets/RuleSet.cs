using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RuleSets
{
    public abstract class RuleSet
    {
        public static RuleSet Instance { get; private set; }

        public abstract string Name { get; }
        public abstract int MaxPoints { get; }
        public abstract int MaxShipsCount { get; }
        public abstract int MinShipsCount { get; }

        public RuleSet()
        {
            Instance = this;
        }

        public abstract void EvadeDiceModification(DiceRoll diceRoll);
    }
}