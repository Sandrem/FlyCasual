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

        public void SpawnModel(Vector3 position, Quaternion rotation)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Remotes/" + RemoteInfo.Name);
            GameObject remoteModel = MonoBehaviour.Instantiate(prefab, position, rotation, BoardTools.Board.GetBoard());
        }
    }
}
