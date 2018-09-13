﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class VCX100Table : GenericAiTable
    {

        public VCX100Table() : base()
        {
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.L.B");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("5.F.R");
            FrontManeuversInner.Add("5.F.R");

            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("2.F.S");

            FrontSideManeuversInner.Add("1.F.S");
            FrontSideManeuversInner.Add("5.F.R");
            FrontSideManeuversInner.Add("5.F.R");
            FrontSideManeuversInner.Add("2.R.B");
            FrontSideManeuversInner.Add("2.R.B");
            FrontSideManeuversInner.Add("1.R.B");

            FrontSideManeuversOuter.Add("2.F.S");
            FrontSideManeuversOuter.Add("3.R.T");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.B");

            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("2.R.B");
            SideManeuversInner.Add("2.R.B");
            SideManeuversInner.Add("2.R.T");

            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("2.R.B");
            SideManeuversOuter.Add("3.R.T");
            SideManeuversOuter.Add("3.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");

            BackSideManeuversInner.Add("1.R.B");
            BackSideManeuversInner.Add("1.R.B");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("1.R.T");
            BackSideManeuversInner.Add("5.F.R");

            BackSideManeuversOuter.Add("2.R.B");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("3.R.T");
            BackSideManeuversOuter.Add("3.R.T");
            BackSideManeuversOuter.Add("5.F.R");
            BackSideManeuversOuter.Add("5.F.R");

            BackManeuversInner.Add("5.F.R");
            BackManeuversInner.Add("5.F.R");
            BackManeuversInner.Add("2.R.T");
            BackManeuversInner.Add("2.L.T");
            BackManeuversInner.Add("3.L.B");
            BackManeuversInner.Add("3.R.B");

            BackManeuversOuter.Add("5.F.R");
            BackManeuversOuter.Add("5.F.R");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.L.T");
        }

        public override void AdaptToSecondEdition()
        {
            ReplaceManeuver("5.F.R", "4.F.R");
        }

    }
}