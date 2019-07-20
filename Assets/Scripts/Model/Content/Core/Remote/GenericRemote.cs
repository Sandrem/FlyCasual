using Players;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Remote
{
    public abstract class GenericRemote : GenericShip
    {
        public RemoteInfo RemoteInfo { get; protected set; }
        public new RemoteTokensHolder Tokens { get; protected set; } // Assign only Red TLs

        public GenericRemote(GenericPlayer owner)
        {
            Owner = owner;
        }

        public void SpawnModel(Vector3 position, int shipId,Quaternion rotation)
        {
            ShipId = shipId;

            GameObject prefab = Resources.Load<GameObject>("Prefabs/Remotes/" + RemoteInfo.Name);
            Model = MonoBehaviour.Instantiate(prefab, position, rotation, BoardTools.Board.GetBoard());
            ShipAllParts = Model.transform.Find("RotationHelper/RotationHelper2/ShipAllParts").transform;

            SetTagOfChildrenRecursive(Model.transform, "ShipId:" + ShipId.ToString());
            SetRaycastTarget(true);
            SetSpotlightMask();
            SetShipIdText(Model);

            //Roster.AddShipToLists(this);
        }
    }
}
