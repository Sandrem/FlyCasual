using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    public enum BaseSize
    {
        Small,
        Large
    }

    public class ShipBase
    {
        public BaseSize Size { get; private set; }
    }
}
