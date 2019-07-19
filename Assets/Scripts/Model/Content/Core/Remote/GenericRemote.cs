using Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Remote
{
    public abstract class GenericRemote
    {
        public RemoteInfo RemoteInfo { get; protected set; }
        public RemoteTokensHolder Tokens { get; protected set; }
        public RemoteState State { get; protected set; }
        public PlayerNo Owner { get; protected set; }

        public GenericRemote(PlayerNo owner)
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
