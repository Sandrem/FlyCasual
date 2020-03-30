using Arcs;
using BoardTools;
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
        public override bool HasCombatActivation { get { return false; } }
        public RemoteInfo RemoteInfo { get; protected set; }
        public new RemoteTokensHolder Tokens { get; protected set; } // Assign only Red TLs
        public abstract Dictionary<string, Vector3> BaseEdges { get; }

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
            GeneratePseudoBase();
            InitializeRosterPanel();

            ActivatePilotAbilities();

            Board.RegisterRemote(this);

            Roster.AddShipToLists(this);
        }

        private void GeneratePseudoBase()
        {
            ShipBase = new RemoteShipBase(this);
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
                0,
                abilityType: RemoteInfo.AbilityType
            );

            ImageUrl = RemoteInfo.ImageUrl;
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

        public Vector3 GetJointAngles(int jointIndex)
        {
            return ShipAllParts.Find("ShipBase/ManeuverJoints").Find("ManeuverJoint" + jointIndex).Find("Rotation").eulerAngles;
        }

        public Vector3 GetJointPosition(int jointIndex)
        {
            return ShipAllParts.Find("ShipBase/ManeuverJoints").Find("ManeuverJoint" + jointIndex).Find("Rotation").position;
        }

        private void GeneratePseudoShip()
        {
            Damage = new Damage(this);
            ActionBar.Initialize();
            InitializeState();
            InitializeSectors();
            InitializeShipBaseArc();
        }

        public void ToggleJointArrow(int jointIndex, bool isVisible)
        {
            ShipAllParts.Find("ShipBase/ManeuverJoints").Find("ManeuverJoint" + jointIndex).gameObject.SetActive(isVisible);
        }

        public override Vector3 GetCenter()
        {
            return Model.transform.TransformPoint(0, 0, 2f);
        }

        public override Transform GetModelTransform()
        {
            return ShipAllParts.Find("ShipModels/DRK-1 Probe Droid/ModelCenter/Lights");
        }

        public override Vector3 GetModelCenter()
        {
            return GetModelTransform().position;
        }

        public class RemoteShipBase : GenericShipBase
        {
            public RemoteShipBase(GenericShip host) : base(host)
            {
                baseEdges = new Dictionary<string, Vector3>((host as GenericRemote).BaseEdges);
            }

            public override List<ManeuverTemplate> BoostTemplatesAvailable => throw new NotImplementedException();
            public override List<ManeuverTemplate> BarrelRollTemplatesAvailable => throw new NotImplementedException();
            public override List<ManeuverTemplate> DecloakBoostTemplatesAvailable => throw new NotImplementedException();
            public override List<ManeuverTemplate> DecloakBarrelRollTemplatesAvailable => throw new NotImplementedException();
        }
    }
}
