using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class TIESFTable : GenericAiTable
    {

        public TIESFTable() : base()
        {
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.L.B");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("3.L.R");
            FrontManeuversInner.Add("3.R.R");

            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("2.F.S");

            FrontSideManeuversInner.Add("1.F.S");
            FrontSideManeuversInner.Add("3.L.R");
            FrontSideManeuversInner.Add("3.L.R");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("2.R.B");

            FrontSideManeuversOuter.Add("2.F.S");
            FrontSideManeuversOuter.Add("3.R.T");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.B");

            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("3.L.R");
            SideManeuversInner.Add("3.L.R");
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
            BackSideManeuversInner.Add("3.L.R");

            BackSideManeuversOuter.Add("2.R.B");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("3.R.T");
            BackSideManeuversOuter.Add("3.R.T");
            BackSideManeuversOuter.Add("3.L.R");
            BackSideManeuversOuter.Add("3.L.R");

            BackManeuversInner.Add("3.L.R");
            BackManeuversInner.Add("3.R.R");
            BackManeuversInner.Add("2.R.T");
            BackManeuversInner.Add("2.L.T");
            BackManeuversInner.Add("3.R.B");
            BackManeuversInner.Add("3.L.B");

            BackManeuversOuter.Add("3.L.R");
            BackManeuversOuter.Add("3.R.R");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.L.T");
        }

    }
}