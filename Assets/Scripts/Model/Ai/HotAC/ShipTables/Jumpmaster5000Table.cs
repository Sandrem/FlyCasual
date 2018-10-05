using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Jumpmaster5000Table : GenericAiTable
    {

        public Jumpmaster5000Table() : base()
        {
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.L.B");
            FrontManeuversInner.Add("2.R.R");
            FrontManeuversInner.Add("2.L.R");
            FrontManeuversInner.Add("2.L.R");
            FrontManeuversInner.Add("4.F.R");

            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("2.F.S");

            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("2.L.R");
            FrontSideManeuversInner.Add("4.F.R");
            FrontSideManeuversInner.Add("4.F.R");
            FrontSideManeuversInner.Add("1.F.S");

            FrontSideManeuversOuter.Add("2.F.S");
            FrontSideManeuversOuter.Add("4.F.S");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.B");

            SideManeuversInner.Add("2.L.R");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("2.R.B");
            SideManeuversInner.Add("2.R.B");
            SideManeuversInner.Add("2.R.T");

            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("2.R.B");
            SideManeuversOuter.Add("2.L.R");
            SideManeuversOuter.Add("2.L.R");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");

            BackSideManeuversInner.Add("3.R.B");
            BackSideManeuversInner.Add("2.L.R");
            BackSideManeuversInner.Add("2.L.R");
            BackSideManeuversInner.Add("1.R.T");
            BackSideManeuversInner.Add("1.R.T");
            BackSideManeuversInner.Add("4.F.R");

            BackSideManeuversOuter.Add("3.R.B");
            BackSideManeuversOuter.Add("4.F.R");
            BackSideManeuversOuter.Add("2.L.R");
            BackSideManeuversOuter.Add("2.L.R");
            BackSideManeuversOuter.Add("1.R.T");
            BackSideManeuversOuter.Add("1.R.T");

            BackManeuversInner.Add("2.L.R");
            BackManeuversInner.Add("2.L.R");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("2.R.T");
            BackManeuversInner.Add("2.L.T");
            BackManeuversInner.Add("2.L.T");

            BackManeuversOuter.Add("1.R.T");
            BackManeuversOuter.Add("4.F.R");
            BackManeuversOuter.Add("2.L.R");
            BackManeuversOuter.Add("2.L.R");
            BackManeuversOuter.Add("1.L.T");
            BackManeuversOuter.Add("1.L.T");
        }

        public override void AdaptToSecondEdition()
        {
            ReplaceManeuver("2.L.R", "4.F.R");
            ReplaceManeuver("2.R.R", "4.F.R");
        }

    }
}