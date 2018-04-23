using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {

        private Transform shipAllParts;
        private Transform modelCenter;

        private string originalSkinName;
        private string skinName;
        public string SkinName
        {
            get { return skinName; }
            set
            {
                if (originalSkinName == null) originalSkinName = value;
                skinName = value;
            }
        }

        public void CreateModel(Vector3 position)
        {
            Model = CreateShipModel(position);
            shipAllParts = Model.transform.Find("RotationHelper/RotationHelper2/ShipAllParts").transform;
            modelCenter = shipAllParts.Find("ShipModels/" + FixTypeName(Type) + "/ModelCenter").transform;
            InitializeShipBase();
            SetRaycastTarget(true);
            SetSpotlightMask();
        }

        private void InitializeShipBase()
        {
            switch (ShipBaseSize)
            {
                case BaseSize.Small:
                    ShipBase = new ShipBaseSmall(this);
                    break;
                case BaseSize.Large:
                    ShipBase = new ShipBaseLarge(this);
                    break;
                default:
                    break;
            }
        }

        public GameObject CreateShipModel(Vector3 position)
        {

            Vector3 facing = (Owner.PlayerNo == Players.PlayerNo.Player1) ? ShipFactory.ROTATION_FORWARD : ShipFactory.ROTATION_BACKWARD;

            position = new Vector3(0, 0, (Owner.PlayerNo == Players.PlayerNo.Player1) ? -4 : 4);

            GameObject prefab = (GameObject)Resources.Load("Prefabs/ShipModel/ShipModel", typeof(GameObject));
            GameObject newShip = MonoBehaviour.Instantiate(prefab, position + new Vector3(0, 0.03f, 0), Quaternion.Euler(facing), Board.BoardManager.GetBoard());
            Transform modelTranform = newShip.transform.Find("RotationHelper/RotationHelper2/ShipAllParts/ShipModels/" + FixTypeName(Type));
            if (modelTranform == null) Console.Write("<b>Missing model: " + FixTypeName(Type) + "</b>", LogTypes.Errors, true, "red");
            modelTranform.gameObject.SetActive(true);

            ShipId = ShipFactory.lastId;
            ShipFactory.lastId = ShipFactory.lastId + 1;
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

        private Material CreateMaterial(string texturePath)
        {
            var texture = Resources.Load<Texture2D>(texturePath);

            if (texture == null)
                return null;

            var material = new Material(Shader.Find("Standard"));
            material.SetTexture("_MainTex", texture);

            return material;
        }

        public void SetShipInsertImage()
        {
            string materialName = PilotName;
            materialName = materialName.Replace(' ', '_');
            materialName = materialName.Replace('"', '_');
            materialName = materialName.Replace("'", "");

            var pathToResource = "ShipStandInsert/" + FixTypeName(Type) + "/" + materialName;
            var shipBaseInsert = CreateMaterial(pathToResource);

            if (shipBaseInsert != null)
            {
                shipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/default").GetComponent<Renderer>().material = shipBaseInsert;
            }
            else
            {
                string materialNameAlt = materialName + "_" + faction.ToString();

                var pathToResourceAlt = "ShipStandInsert/" + FixTypeName(Type) + "/" + materialNameAlt;
                shipBaseInsert = CreateMaterial(pathToResourceAlt);

                if (shipBaseInsert != null)
                {
                    shipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/default").GetComponent<Renderer>().material = shipBaseInsert;
                }
                else
                {
                    Debug.Log("Cannot find: " + pathToResource + " or " + pathToResourceAlt);
                    shipAllParts.Find("ShipBase/ShipStandInsert").gameObject.SetActive(false);
                }
            }
        }

        public void SetShipSkin()
        {
            if (!string.IsNullOrEmpty(SkinName))
            {
                Texture skin = (Texture)Resources.Load("ShipSkins/" + FixTypeName(Type) + "/" + SkinName, typeof(Texture));

                if (skin == null)
                {
                    SkinName = originalSkinName;
                    skin = (Texture)Resources.Load("ShipSkins/" + FixTypeName(Type) + "/" + SkinName, typeof(Texture));
                }

                foreach (Transform modelPart in GetModelTransform())
                {
                    Renderer renderer = modelPart.GetComponent<Renderer>();
                    if (renderer != null) renderer.material.SetTexture("_MainTex", skin);

                    // Second level
                    foreach (Transform modelPartLevel2 in modelPart.transform)
                    {
                        renderer = modelPartLevel2.GetComponent<Renderer>();
                        if (renderer != null) renderer.material.SetTexture("_MainTex", skin);
                    }
                }
            }
        }


        public void ToggleCollisionDetection(bool value)
        {
            shipAllParts.Find("ShipBase/ObstaclesStayDetector").GetComponent<ObstaclesStayDetector>().checkCollisions = value;
            shipAllParts.Find("ShipBase/ObstaclesHitsDetector").GetComponent<ObstaclesHitsDetector>().checkCollisions = value;
        }

        public void ToggleColliders(bool value)
        {
            shipAllParts.Find("ShipBase/ObstaclesStayDetector").GetComponent<Collider>().enabled = value;
            shipAllParts.Find("ShipBase/ObstaclesHitsDetector").GetComponent<Collider>().enabled = value;
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
            shipAllParts.Find("ShipModels/" + FixTypeName(Type) + "/ModelCenter/DamageParticles").gameObject.SetActive(isDamaged);
        }

        public void ToggleIonized(bool isIonized)
        {
            if (isIonized) Sounds.PlayShipSound("Ionization");
            shipAllParts.Find("Ionization").gameObject.SetActive(isIonized);
        }

        public void PlayDestroyedAnimSound(System.Action callBack)
        {
            int random = Random.Range(1, 8);
            float playSoundDelay = Sounds.PlayShipSound("Explosion-" + random);
            playSoundDelay = Mathf.Max(playSoundDelay, 1f);

            shipAllParts.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            shipAllParts.Find("Explosion/Debris").GetComponent<ParticleSystem>().Play();
            shipAllParts.Find("Explosion/Sparks").GetComponent<ParticleSystem>().Play();
            shipAllParts.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.Wait(playSoundDelay, delegate { callBack(); });
        }

        public void MoveUpwards(float progress)
        {
            progress = (progress > 0.5f) ? 1 - progress : progress;
            modelCenter.localPosition = new Vector3(0, 3.67f + 4f * progress, 0);
        }

        public Transform GetSelectionProjector()
        {
            return shipAllParts.Find("ShipBase").Find("SelectionProjector");
        }

        public void HighlightThisSelected()
        {
            Transform projector = GetSelectionProjector();
            projector.gameObject.SetActive(true);
            projector.GetComponent<Projector>().material = (Material)Resources.Load("Projectors/Materials/SelectionThisProjector", typeof(Material));
        }

        public void HighlightAnyHovered()
        {
            Transform projector = GetSelectionProjector();
            projector.gameObject.SetActive(true);
            projector.GetComponent<Projector>().material = (Material)Resources.Load("Projectors/Materials/SelectionAnyHovered", typeof(Material));
        }

        public void HighlightEnemySelected()
        {
            Transform projector = GetSelectionProjector();
            projector.gameObject.SetActive(true);
            projector.GetComponent<Projector>().material = (Material)Resources.Load("Projectors/Materials/SelectionEnemyProjector", typeof(Material));
        }

        public void HighlightSelectedOff()
        {
            Transform projector = GetSelectionProjector();
            projector.gameObject.SetActive(false);
        }

        public void HighlightCanBeSelectedOn()
        {
            if (!DebugManager.DebugTemporary)
            {
                shipAllParts.Find("Spotlight").gameObject.SetActive(true);
            }
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
            shipAllParts.Find("ShipBase").gameObject.SetActive(value);
            shipAllParts.Find("ShipBase").Find("ShipPeg").gameObject.SetActive(value);
        }

        public Transform GetBoosterHelper()
        {
            return shipAllParts.Find("ShipBase/BoostHelper");
        }

        public Transform GetBarrelRollHelper()
        {
            return shipAllParts.Find("ShipBase/BarrelRollHelper");
        }

        public Transform GetBombDropHelper()
        {
            return shipAllParts.Find("ShipBase/BombDropHelper");
        }

        public Transform GetBombLaunchHelper()
        {
            return shipAllParts.Find("ShipBase/BombLaunchHelper");
        }

        public Transform GetDecloakHelper()
        {
            return Model.transform.Find("RotationHelper/RotationHelper2/DecloakHelper");
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
                GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
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

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.StartCoroutine(TurnOffTurretShots(ShotsCount));
        }

        public void AnimateMunitionsShot()
        {
            Transform launchOrigin = modelCenter.Find("MunitionsLauncherPoint/MunitionsLauncherDirection");
            if (launchOrigin != null)
            {
                Board.ShipShotDistanceInformation shotInfo = new Board.ShipShotDistanceInformation(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
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

        public Transform GetShipAllPartsTransform()
        {
            return shipAllParts;
        }

        public Transform GetModelTransform()
        {
            return shipAllParts.Find("ShipModels/" + FixTypeName(Type) + "/ModelCenter/Model");
        }

        public string FixTypeName(string inputName)
        {
            string result = inputName.Replace('/', ' ');
            return result;
        }

    }

}
