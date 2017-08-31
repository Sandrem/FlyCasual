using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Board;

namespace ActionsList
{

    public class CloakAction : GenericAction
    {
        public CloakAction() {
            Name = "Cloak";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/CloakAction.png";
        }

        public override void ActionTake()
        {
            Selection.ThisShip.AssignToken(new Tokens.CloakToken(), Phases.CurrentSubPhase.CallBack);
        }

    }

}

