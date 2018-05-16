using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardTools;
using Tokens;

namespace ActionsList
{

    public class CloakAction : GenericAction
    {
        public CloakAction()
        {
            Name = "Cloak";
            ImageUrl = "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/reference-cards/CloakAction.png";
        }

        public override void ActionTake()
        {
            Selection.ThisShip.Tokens.AssignToken(new CloakToken(Selection.ThisShip), Phases.CurrentSubPhase.CallBack);
        }

    }

}

