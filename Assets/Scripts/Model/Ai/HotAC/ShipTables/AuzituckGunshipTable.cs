using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class AuzituckGunshipTable : GenericAiTable
    {

        public AuzituckGunshipTable() : base()
        {
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("1.L.B");
            FrontManeuversInner.Add("1.L.B");

            FrontManeuversOuter.Add("5.F.S");
            FrontManeuversOuter.Add("5.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");

            FrontSideManeuversInner.Add("1.F.S");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("2.R.B");
            FrontSideManeuversInner.Add("2.R.T");

            FrontSideManeuversOuter.Add("4.F.S");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.T");

            SideManeuversInner.Add("3.R.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.B");
            SideManeuversInner.Add("1.R.B");

            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("3.R.T");
            SideManeuversOuter.Add("2.R.B");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("2.R.B");

            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.B");
            BackSideManeuversInner.Add("3.R.T");
            BackSideManeuversInner.Add("3.R.T");
            BackSideManeuversInner.Add("1.R.B");

            BackSideManeuversOuter.Add("3.R.T");
            BackSideManeuversOuter.Add("3.R.T");
            BackSideManeuversOuter.Add("3.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");

            BackManeuversInner.Add("3.R.T");
            BackManeuversInner.Add("2.R.T");
            BackManeuversInner.Add("3.L.T");
            BackManeuversInner.Add("2.L.T");
            BackManeuversInner.Add("4.F.S");
            BackManeuversInner.Add("5.F.S");

            BackManeuversOuter.Add("3.R.T");
            BackManeuversOuter.Add("3.L.T");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.R.T");
        }

        public override void AdaptToSecondEdition()
        {
            ReplaceManeuver("5.F.S", "4.F.S");

            FrontManeuversInner.Remove("1.R.B");
            FrontManeuversInner.Remove("1.L.B");
            FrontManeuversInner.Add("0.S.S");
            FrontManeuversInner.Add("0.S.S");
        }

    }
}