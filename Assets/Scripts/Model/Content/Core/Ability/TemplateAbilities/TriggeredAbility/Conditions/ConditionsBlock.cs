using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Abilities
{
    public class ConditionsBlock
    {
        private Condition[] AllConditions;
        
        public ConditionsBlock(params Condition[] condition)
        {
            AllConditions = condition;
        }

        public bool Passed(ConditionArgs args)
        {
            foreach (Condition condition in AllConditions)
            {
                if (!condition.Passed(args)) return false;
            }

            return true;
        }
    }
}
