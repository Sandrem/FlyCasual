using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class TIEDefenderTable : GenericAiTable
    {

        public TIEDefenderTable() : base()
        {
            FrontManeuversInner.Add("1.L.B");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("2.F.S");
            FrontManeuversInner.Add("2.F.S");
            FrontManeuversInner.Add("4.F.R");
            FrontManeuversInner.Add("4.F.R");

            FrontManeuversOuter.Add("5.F.S");
            FrontManeuversOuter.Add("5.F.S");
            FrontManeuversOuter.Add("5.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("3.F.S");

            FrontSideManeuversInner.Add("2.F.S");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("4.F.R");
            FrontSideManeuversInner.Add("4.F.R");
            FrontSideManeuversInner.Add("1.R.T");

            FrontSideManeuversOuter.Add("3.F.S");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.T");

            SideManeuversInner.Add("1.R.B");
            SideManeuversInner.Add("4.F.R");
            SideManeuversInner.Add("4.F.R");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("1.R.T");

            SideManeuversOuter.Add("1.R.B");
            SideManeuversOuter.Add("2.R.B");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("3.R.T");
            SideManeuversOuter.Add("3.R.T");
            SideManeuversOuter.Add("3.R.T");

            BackSideManeuversInner.Add("4.F.R");
            BackSideManeuversInner.Add("4.F.R");
            BackSideManeuversInner.Add("4.F.R");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("1.R.T");

            BackSideManeuversOuter.Add("4.F.R");
            BackSideManeuversOuter.Add("4.F.R");
            BackSideManeuversOuter.Add("4.F.R");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("1.R.T");

            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("3.L.T");
            BackManeuversInner.Add("2.R.T");

            BackManeuversOuter.Add("4.F.R");
            BackManeuversOuter.Add("4.F.R");
            BackManeuversOuter.Add("4.F.R");
            BackManeuversOuter.Add("4.F.R");
            BackManeuversOuter.Add("1.L.T");
            BackManeuversOuter.Add("1.R.T");
        }

    }
}