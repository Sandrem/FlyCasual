using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    public class ShipModelInfo
    {
        public string ModelName { get; set; }
        public string SkinName { get; set; }

        public ShipModelInfo(string modelName, string skinName, WingsPositions wingsPositions = WingsPositions.None)
        {
            ModelName = modelName;
            SkinName = skinName;
        }
    }
}
