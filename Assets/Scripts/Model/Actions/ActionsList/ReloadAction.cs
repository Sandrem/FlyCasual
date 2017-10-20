using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionsList
{

    public class ReloadAction : GenericAction
    {

        public ReloadAction() {
            Name = EffectName = "Reload";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/ReloadActionAndJamTokens.png";
        }

        public override void ActionTake()
        {
            Messages.ShowError("Not implemented");
        }

        public override int GetActionPriority()
        {
            int result = 0;
            return result;
        }

    }

}
