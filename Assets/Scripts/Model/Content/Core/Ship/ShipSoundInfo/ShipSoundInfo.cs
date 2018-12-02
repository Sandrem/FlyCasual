using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ship
{
    public class ShipSoundInfo
    {
        public List<string> MovementSoundNames { get; private set; }
        public int ShotsCount { get; private set; }
        public string ShotsName { get; private set; }

        public ShipSoundInfo(List<string> movementSoundNames, string shotsName, int shotsCount)
        {
            MovementSoundNames = movementSoundNames;

            ShotsName = shotsName;
            ShotsCount = shotsCount;
        }
    }
}
