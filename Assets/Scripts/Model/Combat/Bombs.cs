using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Upgrade;
using UnityEngine;

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
        public static GenericBomb CurrentBombToDrop { get; set; }
        private static List<Vector3> generatedBombPoints = new List<Vector3>();

        public static List<Vector3> GetBombPoints()
        {
            if (generatedBombPoints.Count == 0)
            {
                int precision = 10;
                for (int i = 0; i <= precision; i++)
                {
                    generatedBombPoints.Add(new Vector3(-1.6f + (3.2f / precision) * precision, 0, 0.05f));
                    generatedBombPoints.Add(new Vector3(1.6f, 0, 0.05f + (3 / precision) * precision));
                    generatedBombPoints.Add(new Vector3(-1.6f, 0, 0.05f + (3 / precision) * precision));
                    generatedBombPoints.Add(new Vector3(-1.6f + (3.2f / precision) * precision, 0, 3.05f));
                }
            }

            return generatedBombPoints;
        }
    }
}



