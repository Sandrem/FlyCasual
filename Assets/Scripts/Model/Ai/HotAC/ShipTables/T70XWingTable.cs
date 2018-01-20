using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class T70XWingTable : GenericAiTable
    {

        public T70XWingTable() : base()
        {
            FrontManeuversInner.Add("4.F.R");
            FrontManeuversInner.Add("4.F.R");
            FrontManeuversInner.Add("1.L.B");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("2.F.S");
            FrontManeuversInner.Add("1.F.S");

            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("2.F.S");

            FrontSideManeuversInner.Add("1.F.S");
            FrontSideManeuversInner.Add("3.R.E");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("4.F.S");

            FrontSideManeuversOuter.Add("3.F.S");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.T");
            FrontSideManeuversOuter.Add("2.R.T");

            SideManeuversInner.Add("1.R.B");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("3.R.E");
            SideManeuversInner.Add("3.R.E");
            SideManeuversInner.Add("4.F.R");

            SideManeuversOuter.Add("1.R.B");
            SideManeuversOuter.Add("2.R.B");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("3.R.T");
            SideManeuversOuter.Add("3.R.T");

            BackSideManeuversInner.Add("4.F.R");
            BackSideManeuversInner.Add("4.F.R");
            BackSideManeuversInner.Add("1.R.B");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("3.R.E");

            BackSideManeuversOuter.Add("4.F.R");
            BackSideManeuversOuter.Add("4.F.R");
            BackSideManeuversOuter.Add("3.R.E");
            BackSideManeuversOuter.Add("3.R.E");
            BackSideManeuversOuter.Add("3.R.E");
            BackSideManeuversOuter.Add("2.R.T");

            BackManeuversInner.Add("4.F.S");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("4.F.R");
            BackManeuversInner.Add("3.L.E");
            BackManeuversInner.Add("3.R.E");

            BackManeuversOuter.Add("4.F.R");
            BackManeuversOuter.Add("4.F.R");
            BackManeuversOuter.Add("3.L.E");
            BackManeuversOuter.Add("3.R.E");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.R.T");
        }

    }
}