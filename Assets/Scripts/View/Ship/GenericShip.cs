using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {

        private const float HALF_OF_FIRINGARC_SIZE = 0.44f;
        private Transform shipAllParts;
        private Transform modelCenter;

        public void CreateModel(Vector3 position)
        {
            Model = CreateShipModel(position);
            shipAllParts = Model.transform.Find("RotationHelper/RotationHelper2/ShipAllParts").transform;
            modelCenter = shipAllParts.Find("ShipModels/" + Type + "/ModelCenter").transform;
            setShipBaseEdges();
        }

        public GameObject CreateShipModel(Vector3 position)
        {

            Vector3 facing = (Owner.PlayerNo == Players.PlayerNo.Player1) ? ShipFactory.ROTATION_FORWARD : ShipFactory.ROTATION_BACKWARD;

            position = new Vector3(0, 0, (Owner.PlayerNo == Players.PlayerNo.Player1) ? -4 : 4);

            GameObject newShip = MonoBehaviour.Instantiate(Game.PrefabsList.ShipModel, position + new Vector3(0, 0.03f, 0), Quaternion.Euler(facing), Board.GetBoard());
            newShip.transform.Find("RotationHelper/RotationHelper2/ShipAllParts/ShipModels/" + Type).gameObject.SetActive(true);

            ShipId = ShipFactory.lastId;
            ShipFactory.lastId = ShipFactory.lastId + 1;
            SetTagOfChildren(newShip.transform, "ShipId:" + ShipId.ToString());

            return newShip;
        }

        private static void SetTagOfChildren(Transform parent, string tag)
        {
            parent.gameObject.tag = tag;
            if (parent.childCount > 0)
            {
                foreach (Transform t in parent)
                {
                    SetTagOfChildren(t, tag);
                }
            }
        }

        public void SetShipInstertImage()
        {
            string materialName = PilotName;
            materialName = materialName.Replace(' ', '_');
            materialName = materialName.Replace('"', '_');
            Material shipBaseInsert = (Material)Resources.Load("ShipStandInsert/Materials/" + materialName, typeof(Material));
            if (shipBaseInsert != null)
            {
                shipAllParts.Find("ShipStand/ShipStandInsert/ShipStandInsertImage/default").GetComponent<Renderer>().material = shipBaseInsert;
            }
            else
            {
                Debug.Log("Cannot find: " + materialName);
            }
            
        }

        public void ToggleCollisionDetection(bool value)
        {
            shipAllParts.Find("ShipStand/ObstaclesStayDetector").GetComponent<ObstaclesStayDetector>().checkCollisions = value;
            shipAllParts.Find("ShipStand/ObstaclesHitsDetector").GetComponent<ObstaclesHitsDetector>().checkCollisions = value;
        }

        public void SetActive(bool argument)
        {
            Model.SetActive(argument);
        }

        public string GetTag()
        {
            return Model.tag;
        }

        public Vector3 GetFrontFacing()
        {
            return Model.transform.TransformDirection(0, 0, 1f);
        }

        public void SetRaycastTarget(bool value)
        {
            int layer = (value) ? LayerMask.NameToLayer("Ships") : LayerMask.NameToLayer("Ignore Raycast") ;
            SetLayerRecursive(Model.transform, layer);
        }

        private void SetLayerRecursive(Transform target, int layer)
        {
            target.gameObject.layer = layer;
            foreach (Transform transform in target)
            {
                SetLayerRecursive(transform, layer);
            }
        }

        public void ToggleDamaged(bool isDamaged)
        {
            shipAllParts.Find("ShipModels/" + Type + "/ModelCenter/DamageParticles").gameObject.SetActive(isDamaged);
        }

        public void MoveUpwards(float progress)
        {
            progress = (progress > 0.5f) ? 1 - progress : progress;
            modelCenter.localPosition = new Vector3(0, 3.67f + 4f * progress, 0);
        }

        public void HighlightThisSelected()
        {
            shipAllParts.Find("SelectionProjector").gameObject.SetActive(true);
            shipAllParts.Find("SelectionProjector").GetComponent<Projector>().material = (Material)Resources.Load("Projectors/Materials/SelectionThisProjector", typeof(Material));
        }

        public void HighlightEnemySelected()
        {
            shipAllParts.Find("SelectionProjector").gameObject.SetActive(true);
            shipAllParts.Find("SelectionProjector").GetComponent<Projector>().material = (Material)Resources.Load("Projectors/Materials/SelectionEnemyProjector", typeof(Material));
        }

        public void HighlightSelectedOff()
        {
            shipAllParts.Find("SelectionProjector").gameObject.SetActive(false);
        }

        public void HighlightCanBeSelectedOn()
        {
            shipAllParts.Find("Spotlight").gameObject.SetActive(true);
        }

        public void HighlightCanBeSelectedOff()
        {
            shipAllParts.Find("Spotlight").gameObject.SetActive(false);
        }

        public void SetHeight(float height)
        {
            Model.transform.Find("RotationHelper").localPosition = new Vector3(Model.transform.Find("RotationHelper").localPosition.x, height, Model.transform.Find("RotationHelper").localPosition.z);
        }

        public void ToggleShipStandAndPeg(bool value)
        {
            shipAllParts.Find("ShipStand").gameObject.SetActive(value);
            shipAllParts.Find("ShipPeg").gameObject.SetActive(value);
        }

        public void AnimatePrimaryWeapon()
        {
            Transform shotsTransform = modelCenter.Find("Shots");
            if (shotsTransform != null)
            {
                foreach (Transform origin in shotsTransform)
                {
                    Vector3 targetPoint = Selection.AnotherShip.GetModelCenter();
                    origin.LookAt(targetPoint);
                    ParticleSystem.MainModule particles = origin.GetComponentInChildren<ParticleSystem>().main;
                    particles.startLifetimeMultiplier = (Vector3.Distance(origin.position, targetPoint) * 0.25f / (10/3));
                }

                shotsTransform.gameObject.SetActive(true);
                Game.StartCoroutine(TurnOffShots(ShotsCount));
            }
        }

        private IEnumerator TurnOffShots(float shotsCount)
        {
            yield return new WaitForSeconds(shotsCount * 0.5f + 0.4f);
            Transform shotsTransform = modelCenter.Find("Shots");
            if (shotsTransform != null)
            {
                shotsTransform.gameObject.SetActive(false);
            }
        }

    }

}
