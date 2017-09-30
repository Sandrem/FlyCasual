using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class VT49DecimatorTable : GenericAiTable
    {

        public VT49DecimatorTable() : base()
        {
            FrontManeuversInner.Add("3.R.T");
            FrontManeuversInner.Add("3.L.T");
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
            FrontSideManeuversInner.Add("2.L.B");
            FrontSideManeuversInner.Add("2.L.B");
            FrontSideManeuversInner.Add("2.L.T");
            FrontSideManeuversInner.Add("4.F.S");
            FrontSideManeuversInner.Add("4.F.S");

            FrontSideManeuversOuter.Add("4.F.S");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("3.R.T");

            SideManeuversInner.Add("3.R.B");
            SideManeuversInner.Add("3.R.B");
            SideManeuversInner.Add("3.R.T");
            SideManeuversInner.Add("2.R.B");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("1.R.B");

            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("3.R.T");
            SideManeuversOuter.Add("3.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("1.R.B");

            BackSideManeuversInner.Add("3.R.B");
            BackSideManeuversInner.Add("3.R.B");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.B");
            BackSideManeuversInner.Add("1.R.B");

            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("3.R.T");
            BackSideManeuversOuter.Add("3.R.T");
            BackSideManeuversOuter.Add("1.R.B");

            BackManeuversInner.Add("3.L.T");
            BackManeuversInner.Add("2.L.T");
            BackManeuversInner.Add("3.R.T");
            BackManeuversInner.Add("2.R.T");
            BackManeuversInner.Add("1.L.B");
            BackManeuversInner.Add("1.R.B");

            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.R.T");
        }

    }
}