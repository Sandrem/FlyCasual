using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class YV666Table : GenericAiTable
    {

        public YV666Table() : base()
        {
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.L.B");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("0.S.S");
            FrontManeuversInner.Add("0.S.S");
            FrontManeuversInner.Add("0.S.S");

            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("4.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("2.F.S");

            FrontSideManeuversInner.Add("1.F.S");
            FrontSideManeuversInner.Add("2.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("0.S.S");

            FrontSideManeuversOuter.Add("2.F.S");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.T");
            FrontSideManeuversOuter.Add("3.R.T");

            SideManeuversInner.Add("2.R.B");
            SideManeuversInner.Add("1.R.B");
            SideManeuversInner.Add("1.R.B");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("0.S.S");

            SideManeuversOuter.Add("3.R.B");
            SideManeuversOuter.Add("2.R.B");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("3.R.T");
            SideManeuversOuter.Add("3.R.T");

            BackSideManeuversInner.Add("3.R.B");
            BackSideManeuversInner.Add("3.R.T"); 
            BackSideManeuversInner.Add("3.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.B");
            BackSideManeuversInner.Add("0.S.S");

            BackSideManeuversOuter.Add("3.R.B");
            BackSideManeuversOuter.Add("1.R.B");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("3.R.T");
            BackSideManeuversOuter.Add("3.R.T");

            BackManeuversInner.Add("0.S.S");
            BackManeuversInner.Add("0.S.S");
            BackManeuversInner.Add("3.R.T");
            BackManeuversInner.Add("3.L.T");
            BackManeuversInner.Add("3.R.B");
            BackManeuversInner.Add("3.L.B");

            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.L.T");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.R.T");
            BackManeuversOuter.Add("2.R.T");
        }

    }
}