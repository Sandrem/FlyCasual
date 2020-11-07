using System;
using System.Collections.Generic;

namespace Abilities
{
    public class HasTokenCondition : Condition
    {
        private List<Type> TokensToHave;

        public HasTokenCondition(List<Type> tokensToHave)
        {
            TokensToHave = tokensToHave;
        }

        public override bool Passed(ConditionArgs args)
        {
            foreach (Type tokenType in TokensToHave)
            {
                if (args.ShipToCheck.Tokens.HasToken(tokenType, '*')) return true;
            }

            return false;
        }
    }
}
