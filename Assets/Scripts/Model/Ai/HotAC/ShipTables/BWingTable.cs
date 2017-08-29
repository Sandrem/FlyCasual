using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class BWingTable : GenericAiTable
    {

        public BWingTable() : base()
        {
            FrontManeuversInner.Add("1.L.B");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("2.F.R");

            FrontManeuversOuter.Add("1.R.T");
            FrontManeuversOuter.Add("1.F.S");
            FrontManeuversOuter.Add("1.R.B");
            FrontManeuversOuter.Add("1.R.B");
            FrontManeuversOuter.Add("1.R.B");
            FrontManeuversOuter.Add("2.R.B");

            FrontSideManeuversInner.Add("1.R.T");
            FrontSideManeuversInner.Add("1.F.S");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("2.R.B");

            FrontSideManeuversOuter.Add("2.F.S");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.T");
            FrontSideManeuversOuter.Add("2.R.T");
            FrontSideManeuversOuter.Add("3.R.B");

            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.F.R");
            SideManeuversInner.Add("2.F.R");

            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("1.R.T");
            SideManeuversOuter.Add("1.R.T");

            BackSideManeuversInner.Add("2.F.R");
            BackSideManeuversInner.Add("2.F.R");
            BackSideManeuversInner.Add("2.F.R");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("1.R.T");

            BackSideManeuversOuter.Add("2.F.R");
            BackSideManeuversOuter.Add("2.F.R");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("1.R.T");

            BackManeuversInner.Add("1.R.T");
            BackManeuversInner.Add("1.L.T");
            BackManeuversInner.Add("3.F.S");
            BackManeuversInner.Add("2.F.R");
            BackManeuversInner.Add("2.F.R");
            BackManeuversInner.Add("2.F.R");

            BackManeuversOuter.Add("2.F.R");
            BackManeuversOuter.Add("2.F.R");
            BackManeuversOuter.Add("2.F.R");
            BackManeuversOuter.Add("2.F.R");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.R.T");
        }

    }
}