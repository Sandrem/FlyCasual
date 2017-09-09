using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class Firespray31Table : GenericAiTable
    {

        public Firespray31Table() : base()
        {
            FrontManeuversInner.Add("4.F.R");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("3.L.B");
            FrontManeuversInner.Add("3.R.B");

            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("2.F.S");

            FrontSideManeuversInner.Add("3.L.T");
            FrontSideManeuversInner.Add("1.F.S");
            FrontSideManeuversInner.Add("2.R.T");
            FrontSideManeuversInner.Add("2.R.T");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");

            FrontSideManeuversOuter.Add("4.F.S");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.T");

            SideManeuversInner.Add("1.L.B");
            SideManeuversInner.Add("3.L.T");
            SideManeuversInner.Add("2.L.T");
            SideManeuversInner.Add("2.L.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.T");

            SideManeuversOuter.Add("1.R.B");
            SideManeuversOuter.Add("2.R.B");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("3.R.T");
            SideManeuversOuter.Add("3.R.T");

            BackSideManeuversInner.Add("4.F.R");
            BackSideManeuversInner.Add("1.L.B");
            BackSideManeuversInner.Add("2.L.B");
            BackSideManeuversInner.Add("2.L.B");
            BackSideManeuversInner.Add("2.L.T");
            BackSideManeuversInner.Add("1.F.S");

            BackSideManeuversOuter.Add("4.F.R");
            BackSideManeuversOuter.Add("3.F.R");
            BackSideManeuversOuter.Add("3.F.R");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("1.R.B");

            BackManeuversInner.Add("3.F.R");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("1.F.S");
            BackManeuversInner.Add("1.F.S");
            BackManeuversInner.Add("1.L.B");
            BackManeuversInner.Add("1.R.B");

            BackManeuversOuter.Add("4.F.S");
            BackManeuversOuter.Add("3.F.S");
            BackManeuversOuter.Add("3.F.S");
            BackManeuversOuter.Add("3.F.S");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.R.T");
        }

    }
}