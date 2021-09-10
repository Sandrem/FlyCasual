using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Ship
{
    public class ShipModelInfo
    {
        public string ModelName { get; set; }
        public string SkinName { get; set; }
        public Vector3 PreviewCameraPosition { get; }
        public float PreviewScale { get; }
        public bool IsMetallic { get; }

        public ShipModelInfo(string modelName, string skinName, Vector3 previewCameraPosition = default, float previewScale = 0, WingsPositions wingsPositions = WingsPositions.None, bool isMetallic = false)
        {
            ModelName = modelName;
            SkinName = skinName;
            PreviewCameraPosition = previewCameraPosition;
            PreviewScale = previewScale;
            IsMetallic = isMetallic;
        }
    }
}
