using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    public class ShipModelInfo
    {
        public string ModelName { get; private set; }
        public string SkinName { get; set; }

        public ShipModelInfo(string modelName, string shipName, WingsPositions wingsPositions = WingsPositions.None)
        {
            ModelName = modelName;
            SkinName = shipName;
        }
    }
}
