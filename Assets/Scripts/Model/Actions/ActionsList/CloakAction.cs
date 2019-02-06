﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BoardTools;
using Tokens;
using SubPhases;

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
            Selection.ThisShip.Tokens.AssignToken(typeof(CloakToken), Phases.CurrentSubPhase.CallBack);
        }

        public override bool IsActionAvailable()
        {
            return !Selection.ThisShip.Tokens.HasToken(typeof(CloakToken));
        }

        public override void RevertActionOnFail(bool hasSecondChance = false)
        {
            Phases.GoBack();
        }

    }

}

