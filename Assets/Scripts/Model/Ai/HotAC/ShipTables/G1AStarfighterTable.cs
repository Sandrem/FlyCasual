using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class G1AStarfighterTable : GenericAiTable
    {

        public G1AStarfighterTable() : base()
        {
            FrontManeuversInner.Add("1.L.B");
            FrontManeuversInner.Add("1.L.B");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.F.S");

            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("2.F.S");

            FrontSideManeuversInner.Add("2.R.T");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.F.S");
            FrontSideManeuversInner.Add("1.F.S");
            FrontSideManeuversInner.Add("4.F.R");

            FrontSideManeuversOuter.Add("2.F.S");
            FrontSideManeuversOuter.Add("2.F.S");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.T");
            FrontSideManeuversOuter.Add("2.R.T");
            FrontSideManeuversOuter.Add("3.R.B");

            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.B");
            SideManeuversInner.Add("2.R.B");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("1.R.T");
            SideManeuversInner.Add("4.F.R");

            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("1.R.T");
            SideManeuversOuter.Add("1.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");

            BackSideManeuversInner.Add("4.F.R");
            BackSideManeuversInner.Add("4.F.R");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("1.R.T");
            BackSideManeuversInner.Add("1.R.T");

            BackSideManeuversOuter.Add("3.F.R");
            BackSideManeuversOuter.Add("3.F.R");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("1.R.T");
            BackSideManeuversOuter.Add("1.R.T");

            BackManeuversInner.Add("2.L.T");
            BackManeuversInner.Add("2.L.T");
            BackManeuversInner.Add("2.R.T");
            BackManeuversInner.Add("2.R.T");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("3.F.R");

            BackManeuversOuter.Add("3.F.R");
            BackManeuversOuter.Add("3.F.R");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.R.T");
        }

    }
}