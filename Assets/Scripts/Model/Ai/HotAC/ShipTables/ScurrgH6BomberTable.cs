using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class ScurrgH6BomberTable : GenericAiTable
    {

        public ScurrgH6BomberTable() : base()
        {
            FrontManeuversInner.Add("3.L.E");
            FrontManeuversInner.Add("3.R.E");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("1.L.B");

            FrontManeuversOuter.Add("5.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("2.F.S");

            FrontSideManeuversInner.Add("3.R.E");
            FrontSideManeuversInner.Add("2.R.B");
            FrontSideManeuversInner.Add("2.F.S");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.F.S");

            FrontSideManeuversOuter.Add("3.F.S");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.T");

            SideManeuversInner.Add("3.R.E");
            SideManeuversInner.Add("3.R.E");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.B");
            SideManeuversInner.Add("1.R.B");

            SideManeuversOuter.Add("3.R.T");
            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.B");
            SideManeuversOuter.Add("2.R.B");

            BackSideManeuversInner.Add("3.R.E");
            BackSideManeuversInner.Add("3.R.E");
            BackSideManeuversInner.Add("3.F.S");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("1.R.B");

            BackSideManeuversOuter.Add("3.R.E");
            BackSideManeuversOuter.Add("3.R.T");
            BackSideManeuversOuter.Add("3.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("1.F.S");

            BackManeuversInner.Add("3.R.E");
            BackManeuversInner.Add("3.L.E");
            BackManeuversInner.Add("3.F.S");
            BackManeuversInner.Add("3.F.S");
            BackManeuversInner.Add("2.R.T");
            BackManeuversInner.Add("2.L.T");

            BackManeuversOuter.Add("3.R.E");
            BackManeuversOuter.Add("3.L.E");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.R.T");
        }

        public override void AdaptToSecondEdition()
        {
            ReplaceManeuver("5.F.S", "3.F.S");
        }

    }
}