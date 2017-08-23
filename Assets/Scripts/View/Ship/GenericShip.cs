using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {

        private const float HALF_OF_FIRINGARC_SIZE = 0.425f;
        private Transform shipAllParts;
        private Transform modelCenter;

        public void CreateModel(Vector3 position)
        {
            Model = CreateShipModel(position);
            shipAllParts = Model.transform.Find("RotationHelper/RotationHelper2/ShipAllParts").transform;
            modelCenter = shipAllParts.Find("ShipModels/" + Type + "/ModelCenter").transform;
            SetRaycastTarget(true);
            SetSpotlightMask();
            SetShipBaseEdges();
        }

        public GameObject CreateShipModel(Vector3 position)
        {

            Vector3 facing = (Owner.PlayerNo == Players.PlayerNo.Player1) ? ShipFactory.ROTATION_FORWARD : ShipFactory.ROTATION_BACKWARD;

            position = new Vector3(0, 0, (Owner.PlayerNo == Players.PlayerNo.Player1) ? -4 : 4);

            GameObject newShip = MonoBehaviour.Instantiate(Game.PrefabsList.ShipModel, position + new Vector3(0, 0.03f, 0), Quaternion.Euler(facing), Board.BoardManager.GetBoard());
            newShip.transform.Find("RotationHelper/RotationHelper2/ShipAllParts/ShipModels/" + Type).gameObject.SetActive(true);

            ShipId = ShipFactory.lastId;
            ShipFactory.lastId = ShipFactory.lastId + 1;
            SetTagOfChildrenRecursive(newShip.transform, "ShipId:" + ShipId.ToString());
            SetShipIdText(newShip);

            return newShip;
        }

        private void SetShipIdText(GameObject model)
        {
            TextMesh ShipIdText = model.transform.Find("RotationHelper/RotationHelper2/ShipAllParts/ShipIdText").GetComponent<TextMesh>();
            ShipIdText.text = ShipId.ToString();
            ShipIdText.color = (Owner.PlayerNo == Players.PlayerNo.Player1) ? Color.green: Color.red;
        }

        private void SetTagOfChildrenRecursive(Transform parent, string tag)
        {
            parent.gameObject.tag = tag;
            if (parent.childCount > 0)
            {
                foreach (Transform t in parent)
                {
                    SetTagOfChildrenRecursive(t, tag);
                }
            }
        }

        private void SetSpotlightMask()
        {
            foreach (Transform spotlight in Model.transform.Find("RotationHelper/RotationHelper2/ShipAllParts/Spotlight").transform)
            {
                spotlight.GetComponent<Light>().cullingMask |= 1 << LayerMask.NameToLayer("ShipId:" + ShipId);
            }
        }

        public void SetShipInstertImage()
        {
            string materialName = PilotName;
            materialName = materialName.Replace(' ', '_');
            materialName = materialName.Replace('"', '_');
            string pathToResource = "ShipStandInsert/" + Type + "/Materials/" + materialName;

            Material shipBaseInsert = (Material)Resources.Load(pathToResource, typeof(Material));
            if (shipBaseInsert != null)
            {
                shipAllParts.Find("ShipStand/ShipStandInsert/ShipStandInsertImage/default").GetComponent<Renderer>().material = shipBaseInsert;
            }
            else
            {
                Debug.Log("Cannot find: " + pathToResource);
            }
            
        }

        public void ToggleCollisionDetection(bool value)
        {
            shipAllParts.Find("ShipStand/ObstaclesStayDetector").GetComponent<ObstaclesStayDetector>().checkCollisions = value;
            shipAllParts.Find("ShipStand/ObstaclesHitsDetector").GetComponent<ObstaclesHitsDetector>().checkCollisions = value;
        }

        public void ToggleColliders(bool value)
        {
            shipAllParts.Find("ShipStand/ObstaclesStayDetector").GetComponent<Collider>().enabled = value;
            shipAllParts.Find("ShipStand/ObstaclesHitsDetector").GetComponent<Collider>().enabled = value;
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
            int layer = (value) ? LayerMask.NameToLayer("ShipId:" + ShipId) : LayerMask.NameToLayer("Ignore Raycast") ;
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

        public void ToggleIonized(bool isIonized)
        {
            if (isIonized) Sounds.PlaySoundOnce("Ionization");
            shipAllParts.Find("Ionization").gameObject.SetActive(isIonized);
        }

        public void PlayDestroyedAnimSound(System.Action callBack)
        {
            int random = Random.Range(1, 8);
            Sounds.PlaySoundOnce("Explosion-" + random);
            shipAllParts.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            shipAllParts.Find("Explosion/Debris").GetComponent<ParticleSystem>().Play();
            shipAllParts.Find("Explosion/Sparks").GetComponent<ParticleSystem>().Play();
            shipAllParts.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            Game.Wait(1, delegate { callBack(); });
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

        public void HighlightAnyHovered()
        {
            shipAllParts.Find("SelectionProjector").gameObject.SetActive(true);
            shipAllParts.Find("SelectionProjector").GetComponent<Projector>().material = (Material)Resources.Load("Projectors/Materials/SelectionAnyHovered", typeof(Material));
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

        public void AnimateTurretWeapon()
        {
            Transform origin = modelCenter.Find("TurretShots/Rotation");

            Vector3 targetPoint = Selection.AnotherShip.GetModelCenter();
            origin.LookAt(targetPoint);
            ParticleSystem.MainModule particles = origin.GetComponentInChildren<ParticleSystem>().main;
            particles.startLifetimeMultiplier = (Vector3.Distance(origin.position, targetPoint) * 0.25f / (10 / 3));

            origin.gameObject.SetActive(true);
            Game.StartCoroutine(TurnOffTurretShots(ShotsCount));
        }

        public void AnimateMunitionsShot()
        {
            Transform launchOrigin = modelCenter.Find("MunitionsLauncherPoint/MunitionsLauncherDirection");
            if (launchOrigin != null)
            {
                Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender);
                float distance = shotInfo.Distance;

                Vector3 targetPoint = Selection.AnotherShip.GetModelCenter();
                launchOrigin.LookAt(targetPoint);

                GameObject munition = MonoBehaviour.Instantiate(shipAllParts.Find("Munition").gameObject, launchOrigin);
                munition.GetComponent<MunitionMovement>().selfDescructTimer = distance;
                munition.SetActive(true);
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

        private IEnumerator TurnOffTurretShots(float shotsCount)
        {
            yield return new WaitForSeconds(shotsCount * 0.5f + 0.4f);
            Transform shotsTransform = modelCenter.Find("TurretShots/Rotation");
            if (shotsTransform != null)
            {
                shotsTransform.gameObject.SetActive(false);
            }
        }

    }

}
