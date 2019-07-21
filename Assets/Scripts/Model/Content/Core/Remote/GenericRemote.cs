using Arcs;
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

        public void SpawnModel(int shipId, Vector3 position, Quaternion rotation)
        {
            ShipId = shipId;

            GeneratePilotInfo();
            GenerateModel(position, rotation);
            GeneratePseudoShip();
            InitializeRosterPanel();

            Roster.AddShipToLists(this);
        }

        private void GeneratePilotInfo()
        {
            ShipInfo = new ShipCardInfo(
                "Remote",
                BaseSize.None,
                Faction.None,
                new ShipArcsInfo(ArcType.None, 0),
                RemoteInfo.Agility,
                RemoteInfo.Hull,
                0,
                new ShipActionsInfo(),
                new ShipUpgradesInfo()
            );

            PilotInfo = new PilotCardInfo(
                RemoteInfo.Name,
                RemoteInfo.Initiative,
                0
            );
        }

        private void GenerateModel(Vector3 position, Quaternion rotation)
        {
            GameObject prefab = Resources.Load<GameObject>("Prefabs/Remotes/" + RemoteInfo.Name);
            Model = MonoBehaviour.Instantiate(prefab, position, rotation, BoardTools.Board.GetBoard());
            ShipAllParts = Model.transform.Find("RotationHelper/RotationHelper2/ShipAllParts").transform;

            SetTagOfChildrenRecursive(Model.transform, "ShipId:" + ShipId.ToString());
            SetRaycastTarget(true);
            SetSpotlightMask();
            SetShipIdText(Model);

            // InitializeShipBase();
        }

        private void GeneratePseudoShip()
        {
            Damage = new Damage(this);
            ActionBar.Initialize();
            InitializeState();
            InitializeSectors();
            InitializeShipBaseArc();
        }
    }
}
