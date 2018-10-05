using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class YT1300Table : GenericAiTable
    {

        public YT1300Table() : base()
        {
            FrontManeuversInner.Add("2.R.T");
            FrontManeuversInner.Add("2.L.T");
            FrontManeuversInner.Add("3.R.B");
            FrontManeuversInner.Add("3.L.B");
            FrontManeuversInner.Add("4.F.S");
            FrontManeuversInner.Add("4.F.S");

            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("2.F.S");

            FrontSideManeuversInner.Add("3.R.B");
            FrontSideManeuversInner.Add("3.L.B");
            FrontSideManeuversInner.Add("2.L.B");
            FrontSideManeuversInner.Add("2.L.T");
            FrontSideManeuversInner.Add("4.F.S");
            FrontSideManeuversInner.Add("4.F.S");

            FrontSideManeuversOuter.Add("4.F.S");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.T");

            SideManeuversInner.Add("3.R.B");
            SideManeuversInner.Add("3.L.B");
            SideManeuversInner.Add("2.L.B");
            SideManeuversInner.Add("2.L.T");
            SideManeuversInner.Add("4.F.S");
            SideManeuversInner.Add("4.F.S");

            SideManeuversOuter.Add("4.F.S");
            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("2.R.B");
            SideManeuversOuter.Add("2.R.T");

            BackSideManeuversInner.Add("4.F.R");
            BackSideManeuversInner.Add("3.F.R");
            BackSideManeuversInner.Add("1.R.T");
            BackSideManeuversInner.Add("1.R.T");
            BackSideManeuversInner.Add("1.R.B");
            BackSideManeuversInner.Add("1.R.B");

            BackSideManeuversOuter.Add("4.F.R");
            BackSideManeuversOuter.Add("3.F.R");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("1.R.T");

            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("1.F.S");
            BackManeuversInner.Add("1.L.B");
            BackManeuversInner.Add("1.R.B");

            BackManeuversOuter.Add("4.F.R");
            BackManeuversOuter.Add("3.F.R");
            BackManeuversOuter.Add("3.F.R");
            BackManeuversOuter.Add("3.F.R");
            BackManeuversOuter.Add("1.L.T");
            BackManeuversOuter.Add("1.R.T");
        }

        public override void AdaptToSecondEdition()
        {
            ReplaceManeuver("1.L.T", "1.L.B");
            ReplaceManeuver("1.R.T", "1.R.B");

            FrontManeuversInner.Remove("2.R.T");
            FrontManeuversInner.Remove("2.L.T");
            FrontManeuversInner.Add("3.R.T");
            FrontManeuversInner.Add("3.L.T");

            FrontSideManeuversInner.Remove("2.L.B");
            FrontSideManeuversInner.Add("2.L.T");

            FrontSideManeuversOuter.Remove("3.R.B");
            FrontSideManeuversOuter.Remove("3.R.B");
            FrontSideManeuversOuter.Add("3.R.T");
            FrontSideManeuversOuter.Add("3.R.T");

            SideManeuversOuter.Remove("3.R.B");
            SideManeuversOuter.Remove("3.R.B");
            SideManeuversOuter.Add("3.R.T");
            SideManeuversOuter.Add("3.R.T");
        }
    }
}