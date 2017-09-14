using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bombs
{
    public enum BombDropTemplates
    {
        Straight1,
        Straight2,
        Straight3,
        Turn1Left,
        Turn1Right,
        Turn3Left,
        Turn3Right
    }

    public static class BombsManager
    {
        public static Upgrade.GenericBomb CurrentBombToDrop { get; set; }
    }
}



