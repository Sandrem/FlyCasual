using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class AgressorTable : GenericAiTable
    {

        public AgressorTable() : base()
        {
            FrontManeuversInner.Add("3.L.R");
            FrontManeuversInner.Add("3.R.R");
            FrontManeuversInner.Add("1.L.B");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("4.F.R");

            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("2.F.S");
            FrontManeuversOuter.Add("2.F.S");

            FrontSideManeuversInner.Add("1.F.S");
            FrontSideManeuversInner.Add("3.L.R");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("4.F.R");

            FrontSideManeuversOuter.Add("2.F.S");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("1.R.B");

            SideManeuversInner.Add("3.L.R");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("4.F.R");

            SideManeuversOuter.Add("2.R.B");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("1.R.T");
            SideManeuversOuter.Add("1.R.T");
            SideManeuversOuter.Add("1.R.B");

            BackSideManeuversInner.Add("3.L.R");
            BackSideManeuversInner.Add("3.R.R");
            BackSideManeuversInner.Add("1.R.T");
            BackSideManeuversInner.Add("1.R.T");
            BackSideManeuversInner.Add("4.F.R");
            BackSideManeuversInner.Add("4.F.R");

            BackSideManeuversOuter.Add("4.F.R");
            BackSideManeuversOuter.Add("4.F.R");
            BackSideManeuversOuter.Add("3.L.R");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("1.R.T");

            BackManeuversInner.Add("3.L.R");
            BackManeuversInner.Add("3.R.R");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("1.L.T");
            BackManeuversInner.Add("1.R.T");

            BackManeuversOuter.Add("4.F.R");
            BackManeuversOuter.Add("4.F.R");
            BackManeuversOuter.Add("3.L.R");
            BackManeuversOuter.Add("3.R.R");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.R.T");
        }

    }
}