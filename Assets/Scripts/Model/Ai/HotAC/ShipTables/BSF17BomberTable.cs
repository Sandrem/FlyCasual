using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AI
{
    public class BSF17BomberTable : GenericAiTable
    {

        public BSF17BomberTable() : base()
        {
            FrontManeuversInner.Add("0.S.S");
            FrontManeuversInner.Add("0.S.S");
            FrontManeuversInner.Add("0.S.S");
            FrontManeuversInner.Add("1.F.S");
            FrontManeuversInner.Add("1.R.B");
            FrontManeuversInner.Add("1.L.B");

            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("3.F.S");
            FrontManeuversOuter.Add("2.F.S");
            FrontManeuversOuter.Add("2.F.S");
            FrontManeuversOuter.Add("1.F.S");

            FrontSideManeuversInner.Add("0.S.S");
            FrontSideManeuversInner.Add("1.F.S");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("1.R.B");
            FrontSideManeuversInner.Add("2.R.T");

            FrontSideManeuversOuter.Add("2.R.T");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("3.R.B");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.B");
            FrontSideManeuversOuter.Add("2.R.B");

            SideManeuversInner.Add("0.S.S");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("2.R.T");
            SideManeuversInner.Add("1.R.B");
            SideManeuversInner.Add("2.R.B");

            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.T");
            SideManeuversOuter.Add("2.R.B");
            SideManeuversOuter.Add("3.R.B");

            BackSideManeuversInner.Add("0.S.S");
            BackSideManeuversInner.Add("2.R.T"); 
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.T");
            BackSideManeuversInner.Add("2.R.B");
            BackSideManeuversInner.Add("3.R.B");

            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("2.R.T");
            BackSideManeuversOuter.Add("1.R.B");
            BackSideManeuversOuter.Add("1.R.B");

            BackManeuversInner.Add("0.S.S");
            BackManeuversInner.Add("0.S.S");
            BackManeuversInner.Add("2.R.T");
            BackManeuversInner.Add("2.L.T");
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