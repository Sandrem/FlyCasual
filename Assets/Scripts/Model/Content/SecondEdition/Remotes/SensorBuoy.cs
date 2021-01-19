using Players;
using System.Collections.Generic;
using UnityEngine;

namespace Remote
{
    public class SensorBuoy : GenericRemote
    {
        public SensorBuoy(GenericPlayer owner) : base(owner)
        {
            RemoteInfo = new RemoteInfo(
                "Sensor Buoy",
                0, 3, 2,
                ""
            );
        }

        public override Dictionary<string, Vector3> BaseEdges
        {
            get
            {
                return new Dictionary<string, Vector3>()
                {
                    { "R0", new Vector3(0f, 0f, 1.419f) },
                    { "R1", new Vector3(0.991f, 0, 0.986f) },
                    { "R2", new Vector3(1.349f, 0, 0.075f) },
                    { "R3", new Vector3(1.123f, 0f, -0.679f) },
                    { "R4", new Vector3(0.855f, 0f, -1.28f) },
                    { "R5", new Vector3(0.653f, 0f, -1.822f) },
                    { "R6", new Vector3(0f, 0f, -2.136f) },
                    { "R7", new Vector3(-0.653f, 0f, -1.822f) },
                    { "R8", new Vector3(-0.855f, 0f, -1.28f) },
                    { "R9", new Vector3(-1.123f, 0f, -0.679f) },
                    { "R10", new Vector3(-1.349f, 0, 0.075f) },
                    { "R11", new Vector3(-0.991f, 0, 0.986f) }
                };
            }
        }

        protected override void SetPlayerCustomization()
        {
            if (Owner.PlayerNo == PlayerNo.Player2)
            {
                GetShipAllPartsTransform().Find("ShipBase/model").localEulerAngles += new Vector3(0, 0, 180);
            }
        }
    }
}