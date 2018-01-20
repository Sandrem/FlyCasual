using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class ProtectorateStarfighterTable : GenericAiTable
    {

        public ProtectorateStarfighterTable() : base()
        {
            FrontManeuversInner.Add("2.L.B");
            FrontManeuversInner.Add("2.L.B");
            FrontManeuversInner.Add("2.R.B");
            FrontManeuversInner.Add("2.R.B");
            FrontManeuversInner.Add("2.F.S");
            FrontManeuversInner.Add("4.F.R");

            FrontManeuversOuter.Add("5.F.S");
            FrontManeuversOuter.Add("5.F.S");
            FrontManeuversOuter.Add("5.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("3.F.S");

            FrontSideManeuversInner.Add("2.R.T");
            FrontSideManeuversInner.Add("1.R.T");
            FrontSideManeuversInner.Add("1.R.T");
            FrontSideManeuversInner.Add("2.F.S");
            FrontSideManeuversInner.Add("2.F.S");
            FrontSideManeuversInner.Add("4.F.R");

            FrontSideManeuversOuter.Add("3.F.S");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("3.R.T");
            FrontSideManeuversOuter.Add("3.R.T");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");

            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("2.R.E");
            SideManeuversInner.Add("2.R.E");
            SideManeuversInner.Add("4.F.R");

            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("1.R.T");
            SideManeuversOuter.Add("1.R.T");

            BackSideManeuversInner.Add("4.F.R");
            BackSideManeuversInner.Add("1.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.E");
            BackSideManeuversInner.Add("2.R.E");

            BackSideManeuversOuter.Add("4.F.R");
            BackSideManeuversOuter.Add("4.F.R");
            BackSideManeuversOuter.Add("1.R.T");
            BackSideManeuversOuter.Add("1.R.T");
            BackSideManeuversOuter.Add("2.R.E");
            BackSideManeuversOuter.Add("2.R.E");

            BackManeuversInner.Add("2.L.T");
            BackManeuversInner.Add("2.R.T");
            BackManeuversInner.Add("2.R.E");
            BackManeuversInner.Add("2.L.E");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("5.F.S");

            BackManeuversOuter.Add("4.F.R");
            BackManeuversOuter.Add("4.F.R");
            BackManeuversOuter.Add("2.R.E");
            BackManeuversOuter.Add("2.L.E");
            BackManeuversOuter.Add("1.L.T");
            BackManeuversOuter.Add("1.R.T");
        }

    }
}