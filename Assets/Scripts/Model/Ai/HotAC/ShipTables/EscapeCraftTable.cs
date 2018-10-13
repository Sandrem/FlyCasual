using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class EscapeCraftTable : GenericAiTable
    {

        public EscapeCraftTable() : base()
        {
            FrontManeuversInner.Add("0.S.S");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.L.B");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("3.F.R");

            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("2.F.S");
            FrontManeuversOuter.Add("2.F.S");

            FrontSideManeuversInner.Add("1.F.S");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("2.R.B");
            FrontSideManeuversInner.Add("3.F.R");
            FrontSideManeuversInner.Add("3.F.R");

            FrontSideManeuversOuter.Add("2.F.S");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.B");

            SideManeuversInner.Add("1.R.B");
            SideManeuversInner.Add("1.R.B");
            SideManeuversInner.Add("2.R.B");
            SideManeuversInner.Add("2.R.B");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("3.F.R");

            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("2.R.B");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");

            BackSideManeuversInner.Add("3.F.R");
            BackSideManeuversInner.Add("3.F.R");
            BackSideManeuversInner.Add("3.F.R");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.T");

            BackSideManeuversOuter.Add("2.R.B");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("3.F.R");
            BackSideManeuversOuter.Add("3.F.R");

            BackManeuversInner.Add("3.F.R");
            BackManeuversInner.Add("3.F.R");
            BackManeuversInner.Add("3.F.R");
            BackManeuversInner.Add("3.F.R");
            BackManeuversInner.Add("2.R.T");
            BackManeuversInner.Add("2.L.T");

            BackManeuversOuter.Add("3.F.R");
            BackManeuversOuter.Add("3.F.R");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.L.T");
        }

    }
}