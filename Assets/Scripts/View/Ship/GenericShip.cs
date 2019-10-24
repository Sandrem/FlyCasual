﻿using Arcs;
using BoardTools;
using Editions;
using Remote;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {
        protected Transform ShipAllParts;
        private Transform modelCenter;

        private string originalSkinName;

        protected string SpecialModel;

        public void CreateModel(Vector3 position)
        {
            Model = CreateShipModel(position);
            ShipAllParts = Model.transform.Find("RotationHelper/RotationHelper2/ShipAllParts").transform;
            modelCenter = ShipAllParts.Find("ShipModels/" + (SpecialModel ?? FixTypeName(ModelInfo.ModelName)) + "/ModelCenter").transform;
            InitializeShipBase();
            SetRaycastTarget(true);
            SetSpotlightMask();
        }

        private void InitializeShipBase()
        {
            switch (ShipInfo.BaseSize)
            {
                case BaseSize.Small:
                    ShipBase = new ShipBaseSmall(this);
                    break;
                case BaseSize.Medium:
                    ShipBase = new ShipBaseMedium(this);
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

            GameObject shipPrefab = (GameObject)Resources.Load("Prefabs/ShipModel/ShipModel", typeof(GameObject));
            GameObject newShip = MonoBehaviour.Instantiate(shipPrefab, position + new Vector3(0, 0.03f, 0), Quaternion.Euler(facing), Board.GetBoard());

            GameObject modelPrefab = GetShipModelPrefab();
            if (modelPrefab != null)
            {
                GameObject newModel = MonoBehaviour.Instantiate(modelPrefab, newShip.transform.Find("RotationHelper/RotationHelper2/ShipAllParts/ShipModels"));
                newModel.name = SpecialModel ?? FixTypeName(ModelInfo.ModelName);
            }
            else
            {
                Console.Write("<b>Missing model: " + (SpecialModel ?? FixTypeName(ModelInfo.ModelName)) + "</b>", LogTypes.Errors, true, "red");
            }

            SetShipIdText(newShip);

            return newShip;
        }

        public GameObject GetShipModelPrefab()
        {
            return Resources.Load<GameObject>("Prefabs/ShipModel/ShipModels/" + (SpecialModel ?? FixTypeName(ModelInfo.ModelName)));
        }

        protected void SetShipIdText(GameObject model)
        {
            TextMesh ShipIdText = model.transform.Find("RotationHelper/RotationHelper2/ShipAllParts/ShipIdText").GetComponent<TextMesh>();
            ShipIdText.text = ShipId.ToString();
            ShipIdText.color = (Owner.PlayerNo == Players.PlayerNo.Player1) ? Color.green: Color.red;
        }

        protected void SetTagOfChildrenRecursive(Transform parent, string tag)
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

        protected void SetSpotlightMask()
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
            Edition.Current.SetShipBaseImage(this);
        }

        public void SetShipBaseImageFirstEdition()
        {
            string materialName = PilotInfo.PilotName;
            materialName = materialName.Replace(' ', '_');
            materialName = materialName.Replace('"', '_');
            materialName = materialName.Replace("'", "");

            var pathToResource = "ShipStandInsert/" + FixTypeName(ModelInfo.ModelName) + "/" + materialName;
            var shipBaseInsert = CreateMaterial(pathToResource);

            if (shipBaseInsert != null)
            {
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/default").GetComponent<Renderer>().material = shipBaseInsert;
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/default").GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            }
            else
            {
                string materialNameAlt = materialName + "_" + Faction.ToString();

                var pathToResourceAlt = "ShipStandInsert/" + FixTypeName(ModelInfo.ModelName) + "/" + materialNameAlt;
                shipBaseInsert = CreateMaterial(pathToResourceAlt);

                if (shipBaseInsert != null)
                {
                    ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/default").GetComponent<Renderer>().material = shipBaseInsert;
                    ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/default").GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                }
                else
                {
                    Debug.Log("Cannot find: " + pathToResource + " or " + pathToResourceAlt);
                    ShipAllParts.Find("ShipBase/ShipStandInsert").gameObject.SetActive(false);
                }
            }
        }

        public void SetShipBaseImageSecondEdition()
        {
            string newTexture = "ShipStandInsert/Universal/SecondEdition/" + this.ShipInfo.BaseSize.ToString() + "/Base";
            Material newMaterial = CreateMaterial(newTexture);
            ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/default").GetComponent<Renderer>().material = newMaterial;
            ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/default").GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

            //Ship Info
            newTexture = "ShipStandInsert/Universal/SecondEdition/" + this.ShipInfo.BaseSize.ToString() + "/" + this.Faction.ToString() + "/Info";
            newMaterial = CreateMaterial(newTexture);
            StandardShaderUtils.ChangeRenderMode(newMaterial, StandardShaderUtils.BlendMode.Cutout);
            ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/ShipInfo").GetComponent<Renderer>().material = newMaterial;
            ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/ShipInfo").GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/ShipInfo").GetComponent<MeshRenderer>().enabled = true;

            //Forward arc
            ArcFront arcPrimary = ArcsInfo.GetArc<ArcFront>();
            if (arcPrimary != null && !ArcsInfo.HasArc(ArcType.FullFront))
            {
                newTexture = "ShipStandInsert/Universal/SecondEdition/" + this.ShipInfo.BaseSize.ToString() + "/" + this.Faction.ToString() + "/Front";
                newMaterial = CreateMaterial(newTexture);
                StandardShaderUtils.ChangeRenderMode(newMaterial, StandardShaderUtils.BlendMode.Fade);
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/FirstArc").GetComponent<Renderer>().material = newMaterial;
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/FirstArc").GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/FirstArc").GetComponent<MeshRenderer>().enabled = true;
            }
            else if (ArcsInfo.HasArc(ArcType.FullFront))
            {
                newTexture = "ShipStandInsert/Universal/SecondEdition/" + this.ShipInfo.BaseSize.ToString() + "/" + this.Faction.ToString() + "/FullFront";
                newMaterial = CreateMaterial(newTexture);
                StandardShaderUtils.ChangeRenderMode(newMaterial, StandardShaderUtils.BlendMode.Fade);
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/FirstArc").GetComponent<Renderer>().material = newMaterial;
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/FirstArc").GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/FirstArc").GetComponent<MeshRenderer>().enabled = true;
            }
            else if (ArcsInfo.HasArc(ArcType.Bullseye))
            {
                newTexture = "ShipStandInsert/Universal/SecondEdition/" + this.ShipInfo.BaseSize.ToString() + "/" + this.Faction.ToString() + "/Bullseye";
                newMaterial = CreateMaterial(newTexture);
                StandardShaderUtils.ChangeRenderMode(newMaterial, StandardShaderUtils.BlendMode.Fade);
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/FirstArc").GetComponent<Renderer>().material = newMaterial;
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/FirstArc").GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/FirstArc").GetComponent<MeshRenderer>().enabled = true;
            }

            //Rear arc
            if (ArcsInfo.Arcs.Any(n => n.Facing == ArcFacing.Rear))
            {
                newTexture = "ShipStandInsert/Universal/SecondEdition/" + this.ShipInfo.BaseSize.ToString() + "/" + this.Faction.ToString() + "/Rear";
                newMaterial = CreateMaterial(newTexture);
                StandardShaderUtils.ChangeRenderMode(newMaterial, StandardShaderUtils.BlendMode.Fade);
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/SecondArc").GetComponent<Renderer>().material = newMaterial;
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/SecondArc").GetComponent<Renderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/SecondArc").GetComponent<MeshRenderer>().enabled = true;
            }
            else
            {
                ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/SecondArc").GetComponent<MeshRenderer>().enabled = false;
            }

            ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/PilotName").GetComponent<TextMesh>().text = ShortenPilotName(this.PilotInfo.PilotName);
            ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/PilotSkill").GetComponent<TextMesh>().text = this.State.Initiative.ToString();
            ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/ShipIcon").GetComponent<TextMesh>().text = this.ShipIconLetter.ToString();
        }

        private string ShortenPilotName(string pilotName)
        {
            return pilotName.Replace("Squadron", "Sq.")
                .Replace("Station", "St.");
        }

        public void ShowMobileFiringArcHighlight(ArcFacing facing)
        {
            //Mobile arc
            string newTexture = "ShipStandInsert/Universal/SecondEdition/" + this.ShipInfo.BaseSize.ToString() + "/" + this.Faction.ToString() + "/" + facing.ToString();
            Material newMaterial = CreateMaterial(newTexture);

            StandardShaderUtils.ChangeRenderMode(newMaterial, StandardShaderUtils.BlendMode.Fade);
            ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/SecondArc").GetComponent<Renderer>().material = newMaterial;
            ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/SecondArc").GetComponent<MeshRenderer>().enabled = true;
        }

        public void ShowMobileFiringArcAltHighlight(ArcFacing facing)
        {
            //Mobile arc alt
            string newTexture = "ShipStandInsert/Universal/SecondEdition/" + this.ShipInfo.BaseSize.ToString() + "/" + this.Faction.ToString() + "/" + facing.ToString();
            Material newMaterial = CreateMaterial(newTexture);

            StandardShaderUtils.ChangeRenderMode(newMaterial, StandardShaderUtils.BlendMode.Fade);
            ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/ThirdArc").GetComponent<Renderer>().material = newMaterial;
            ShipAllParts.Find("ShipBase/ShipStandInsert/ShipStandInsertImage/ThirdArc").GetComponent<MeshRenderer>().enabled = true;
        }

        private void SetIdMarker()
        {
            string[] goNames = { "Front", "Rear" };
            string idColor = (Owner.PlayerNo == Players.PlayerNo.Player1) ? "ID_White" : "ID_Black";

            foreach (string goName in goNames)
            {
                TextMesh ShipIdText = ShipAllParts.Find("ShipBase/ShipPeg/XWingNumberMarker" + goName + "/" + idColor).GetComponent<TextMesh>();
                ShipIdText.text = ShipId.ToString();
                if (Owner.PlayerNo == Players.PlayerNo.Player2)
                {
                    ShipAllParts.Find("ShipBase/ShipPeg/XWingNumberMarker" + goName).transform.localEulerAngles += new Vector3(0, 180, 0);
                }
            }

        }

        public void SetShipSkin(Transform shipTransform, Texture skin)
        {
            if (!string.IsNullOrEmpty(ModelInfo.SkinName))
            {
                foreach (Renderer renderer in shipTransform.GetComponentsInChildren<Renderer>())
                {
                    if (renderer != null && renderer.gameObject.name != "Particle System")
                    {
                        renderer.material.SetTexture("_MainTex", skin);
                        renderer.material.SetColor("_Color", Color.white);
                    }
                }
            }
        }

        public Texture GetSkinTexture()
        {
            Texture skin = GetAvailableSkins().FirstOrDefault(n => n.name == ModelInfo.SkinName);

            if (skin == null)
            {
                Debug.Log("Warning: Skin \"" + ModelInfo.SkinName + "\" not found, default skin is used ");
                string defaultSkinName = GetDefaultSkinName();
                skin = (Texture)Resources.Load("ShipSkins/" + FixTypeName(ModelInfo.ModelName) + "/" + defaultSkinName, typeof(Texture));
            }

            return skin;
        }

        public List<Texture> GetAvailableSkins()
        {
            List<Texture> textures = new List<Texture>();
            textures.AddRange(Resources.LoadAll<Texture>("ShipSkins/" + FixTypeName(ModelInfo.ModelName)).ToList());

            string customizableSkinsPath = Application.dataPath + "/../Customizable/Ship Skins/";
            if (Directory.Exists(customizableSkinsPath))
            {
                foreach (string filePath in Directory.GetFiles(customizableSkinsPath + FixTypeName(ModelInfo.ModelName) + "/"))
                {
                    string fileName = Path.GetFileNameWithoutExtension(filePath);

                    Texture existsingTexture = textures.FirstOrDefault(n => n.name == fileName);
                    if (existsingTexture != null) textures.Remove(existsingTexture);

                    byte[] fileData = File.ReadAllBytes(filePath);
                    Texture2D texture = new Texture2D(1, 1);
                    texture.LoadImage(fileData);
                    texture.name = fileName;
                    textures.Add(texture);
                }
            }

            return textures;
        }

        private string GetDefaultSkinName()
        {
            GenericShip parentShip = (GenericShip) System.Activator.CreateInstance(this.GetType().BaseType);
            return parentShip.ModelInfo.SkinName;
        }

        public void ToggleCollisionDetection(bool value)
        {
            if (!(this is GenericRemote))
            {
                ShipAllParts.Find("ShipBase/ShipBaseCollider/ObstaclesStayDetector").GetComponent<ObstaclesStayDetector>().checkCollisions = value;
                ShipAllParts.Find("ShipBase/ObstaclesHitsDetector").GetComponent<ObstaclesHitsDetector>().checkCollisions = value;
            }
        }

        public void ToggleColliders(bool value)
        {
            ShipAllParts.Find("ShipBase/ShipBaseCollider/ObstaclesStayDetector").GetComponent<Collider>().enabled = value;
            ShipAllParts.Find("ShipBase/ObstaclesHitsDetector").GetComponent<Collider>().enabled = value;
        }

        public MeshCollider GetCollider()
        {
            return ShipAllParts.Find("ShipBase/ShipBaseCollider/ObstaclesStayDetector").GetComponent<MeshCollider>();
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
            if (!(this is GenericRemote))
            {
                ShipAllParts.Find("ShipModels/" + (SpecialModel ?? FixTypeName(ModelInfo.ModelName)) + "/ModelCenter/DamageParticles").gameObject.SetActive(isDamaged);
            }
        }

        public void ToggleIonized(bool isIonized)
        {
            if (isIonized) Sounds.PlayShipSound("Ionization");
            ShipAllParts.Find("Ionization").gameObject.SetActive(isIonized);
        }

        public void ToggleCloaked(bool isTransparent)
        {
            foreach (Transform transform in GetModelTransform())
            {
                Renderer renderer = transform.GetComponent<Renderer>();
                if (renderer != null)
                {
                    renderer.material.shader = (isTransparent) ? Shader.Find("VR/SpatialMapping/Occlusion") : Shader.Find("Standard");
                }

                ShipAllParts.Find("ShipBase/ShipPeg").gameObject.SetActive(!isTransparent);
            }
        }

        public void PlayDestroyedAnimSound(System.Action callBack)
        {
            int random = Random.Range(1, 8);
            float playSoundDelay = Sounds.PlayShipSound("Explosion-" + random);
            playSoundDelay = Mathf.Max(playSoundDelay, 1f);

            ShipAllParts.Find("Explosion/Explosion").GetComponent<ParticleSystem>().Play();
            ShipAllParts.Find("Explosion/Debris").GetComponent<ParticleSystem>().Play();
            ShipAllParts.Find("Explosion/Sparks").GetComponent<ParticleSystem>().Play();
            ShipAllParts.Find("Explosion/Ring").GetComponent<ParticleSystem>().Play();

            GameManagerScript.Wait(playSoundDelay, delegate { callBack(); });
        }

        public void MoveUpwards(float progress)
        {
            progress = (progress > 0.5f) ? 1 - progress : progress;
            modelCenter.localPosition = new Vector3(0, 3.67f + 4f * progress, 0);
        }

        public Transform GetSelectionProjector()
        {
            return ShipAllParts.Find("ShipBase").Find("SelectionProjector");
        }

        public Transform GetMultiSelectionProjector()
        {
            return ShipAllParts.Find("ShipBase").Find("MultiSelectionProjector");
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

        public void ToggleMultiSelectionProjector()
        {
            Transform projector = GetMultiSelectionProjector();
            projector.gameObject.SetActive(!projector.gameObject.activeSelf);
        }

        public void TurnOffMultiSelectionProjector()
        {
            Transform projector = GetMultiSelectionProjector();
            projector.gameObject.SetActive(false);
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
                ShipAllParts.Find("Spotlight").gameObject.SetActive(true);
            }
        }

        public void HighlightCanBeSelectedOff()
        {
            ShipAllParts.Find("Spotlight").gameObject.SetActive(false);
        }

        public void SetHeight(float height)
        {
            Model.transform.Find("RotationHelper").localPosition = new Vector3(Model.transform.Find("RotationHelper").localPosition.x, height, Model.transform.Find("RotationHelper").localPosition.z);
        }

        public void ToggleShipStandAndPeg(bool value)
        {
            ShipAllParts.Find("ShipBase").gameObject.SetActive(value);
            TogglePeg(value);
        }

        public void TogglePeg(bool value)
        {
            ShipAllParts.Find("ShipBase").Find("ShipPeg").gameObject.SetActive(value);
        }

        public Transform GetBoosterHelper()
        {
            return ShipAllParts.Find("ShipBase/BoostHelper");
        }

        public Transform GetBarrelRollHelper()
        {
            return ShipAllParts.Find("ShipBase/BarrelRollHelper");
        }

        public Transform GetBombDropHelper()
        {
            return ShipAllParts.Find("ShipBase/BombDropHelper");
        }

        public Transform GetBombLaunchHelper()
        {
            return ShipAllParts.Find("ShipBase/BombLaunchHelper");
        }

        public Transform GetDecloakHelper()
        {
            return ShipAllParts.Find("ShipBase/DecloakHelper");
        }

        private void AnimatePrimaryWeapon()
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
                Game.StartCoroutine(TurnOffShots(SoundInfo.ShotsCount));
            }
        }

        private void AnimateTurretWeapon()
        {
            Transform origin = modelCenter.Find("TurretShots/Rotation");

            Vector3 targetPoint = Selection.AnotherShip.GetModelCenter();
            origin.LookAt(targetPoint);
            ParticleSystem.MainModule particles = origin.GetComponentInChildren<ParticleSystem>().main;
            particles.startLifetimeMultiplier = (Vector3.Distance(origin.position, targetPoint) * 0.25f / (10 / 3));

            origin.gameObject.SetActive(true);

            GameManagerScript Game = GameObject.Find("GameManager").GetComponent<GameManagerScript>();
            Game.StartCoroutine(TurnOffTurretShots(SoundInfo.ShotsCount));
        }

        private void AnimateMunitionsShot()
        {
            Transform launchOrigin = modelCenter.Find("MunitionsLauncherPoint/MunitionsLauncherDirection");
            if (launchOrigin != null)
            {
                ShotInfo shotInfo = new ShotInfo(Combat.Attacker, Combat.Defender, Combat.ChosenWeapon);
                float distance = shotInfo.DistanceReal;

                Vector3 targetPoint = Selection.AnotherShip.GetModelCenter();
                launchOrigin.LookAt(targetPoint);

                GameObject munition = MonoBehaviour.Instantiate(ShipAllParts.Find("Munition").gameObject, launchOrigin);
                munition.GetComponent<MunitionMovement>().selfDescructTimer = distance;
                munition.SetActive(true);
            }
        }

        private void AnimateShields()
        {
            Transform shields = modelCenter.Find("Shields");
            if (shields != null) shields.gameObject.SetActive(true);
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
            return ShipAllParts;
        }

        public virtual Transform GetModelTransform()
        {
            return ShipAllParts.Find("ShipModels/" + (SpecialModel ?? FixTypeName(ModelInfo.ModelName)) + "/ModelCenter/Model");
        }

        public string FixTypeName(string inputName)
        {
            string result = inputName.Replace('/', ' ');
            return result;
        }

    }

}
