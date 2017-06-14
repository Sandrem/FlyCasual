using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {

        private const float HALF_OF_FIRINGARC_SIZE = 0.44f;

        public void CreateModel(Vector3 position)
        {
            Model = CreateShipModel(position);
            setShipBaseEdges();
        }

        public GameObject CreateShipModel(Vector3 position)
        {

            Vector3 facing = (Owner.PlayerNo == Players.PlayerNo.Player1) ? ShipFactory.ROTATION_FORWARD : ShipFactory.ROTATION_BACKWARD;

            position = new Vector3(0, 0, (Owner.PlayerNo == Players.PlayerNo.Player1) ? -4 : 4);

            GameObject newShip = MonoBehaviour.Instantiate(Game.PrefabsList.ShipModel, position + new Vector3(0, 0.03f, 0), Quaternion.Euler(facing), Board.GetBoard());
            newShip.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).gameObject.SetActive(true);

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
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipStand").Find("ShipStandInsert").Find("ShipStandInsertImage").Find("default").GetComponent<Renderer>().material = (Material)Resources.Load("ShipStandInsert/Materials/" + materialName, typeof(Material));
        }

        public Vector3 GetAngles()
        {
            return Model.transform.eulerAngles;
        }

        public void SetAngles(Vector3 angles)
        {
            Model.transform.eulerAngles = angles;
        }

        public void UpdateRotationHelperAngles(Vector3 angles)
        {
            Model.transform.Find("RotationHelper").localEulerAngles = new Vector3(angles.x, angles.y + Model.transform.Find("RotationHelper").localEulerAngles.y, angles.z);
        }

        public void SetRotationHelper2Angles(Vector3 angles)
        {
            Model.transform.Find("RotationHelper").Find("RotationHelper2").localEulerAngles = angles;
        }

        public void SetPosition(Vector3 position)
        {
            Model.transform.position = position;
        }

        public Vector3 GetPosition()
        {
            return Model.transform.position;
        }

        public Quaternion GetRotation()
        {
            return Model.transform.rotation;
        }

        public Vector3 TransformDirection(Vector3 direction)
        {
            return Model.transform.TransformDirection(direction);
        }

        public Vector3 TransformVector(Vector3 vector)
        {
            return Model.transform.TransformDirection(vector);
        }

        public Vector3 TransformPoint(Vector3 point)
        {
            return Model.transform.TransformPoint(point);
        }

        public void Rotate(Vector3 point, float progress)
        {
            Model.transform.RotateAround(point, new Vector3(0, 1, 0), progress);
        }

        public void ToggleCollisionDetection(bool value)
        {
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipStand").Find("ObstaclesStayDetector").GetComponent<ObstaclesStayDetector>().checkCollisions = value;
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipStand").Find("ObstaclesHitsDetector").GetComponent<ObstaclesHitsDetector>().checkCollisions = value;
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

        public void ApplyRotationHelpers()
        {
            Model.transform.localEulerAngles += Model.transform.Find("RotationHelper").localEulerAngles + Model.transform.Find("RotationHelper").Find("RotationHelper2").localEulerAngles;
        }

        public void ResetRotationHelpers()
        {
            Model.transform.Find("RotationHelper").localEulerAngles = Vector3.zero;
            Model.transform.Find("RotationHelper").Find("RotationHelper2").localEulerAngles = Vector3.zero;
        }

        public void SimplifyRotationHelpers()
        {
            Model.transform.Find("RotationHelper").localEulerAngles += Model.transform.Find("RotationHelper").Find("RotationHelper2").localEulerAngles;
            Model.transform.Find("RotationHelper").Find("RotationHelper2").localEulerAngles = Vector3.zero;
        }

        public void Rotate180()
        {
            Model.transform.RotateAround(Model.transform.TransformPoint(new Vector3(0, 0, -HALF_OF_SHIPSTAND_SIZE)), Vector3.up, 180);
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
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).Find("ModelCenter").Find("DamageParticles").gameObject.SetActive(isDamaged);
        }

        public void RotateModelDuringTurn(MovementExecutionData currentMovementData, MovementExecutionData previousMovementData)
        {
            if ((currentMovementData.MovementDirection == ManeuverDirection.Forward) && (previousMovementData.Speed == 0)) return;
            if (currentMovementData.CollisionReverting) return;

            float progressCurrent = currentMovementData.CurrentProgress;
            float progressTarget = currentMovementData.TargetProgress;
            float turningDirection = 0;

            if (currentMovementData.MovementDirection != ManeuverDirection.Forward)
            {
                progressTarget += progressTarget * (1f / currentMovementData.Speed);
                turningDirection = (currentMovementData.MovementDirection == ManeuverDirection.Right) ? 1 : -1;
            }
            if (previousMovementData.Speed != 0)
            {
                progressCurrent += progressTarget * previousMovementData.Speed;
                progressTarget += progressTarget * previousMovementData.Speed;
                turningDirection = (previousMovementData.MovementDirection == ManeuverDirection.Right) ? 1 : -1;
            }

            float progress = progressCurrent / progressTarget;
            if (progress > 0.5f)
            {
                progress = 1 - progress;
            }
                Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).Find("ModelCenter").localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, 45 * turningDirection, progress));
        }

        public void RotateModelDuringBarrelRoll(float progress, float turningDirection)
        {
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).Find("ModelCenter").localEulerAngles = new Vector3(0, 0, Mathf.Lerp(0, turningDirection * 360, progress));
        }

        public void MoveUpwards(float progress)
        {
            progress = (progress > 0.5f) ? 1 - progress : progress;
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).Find("ModelCenter").localPosition = new Vector3(0, 3.67f + 4f * progress, 0);
        }

        public Vector3 InverseTransformPoint(Vector3 point)
        {
            return Model.transform.InverseTransformPoint(point);
        }

        public void HighlightThisSelected()
        {
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("SelectionProjector").gameObject.SetActive(true);
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("SelectionProjector").GetComponent<Projector>().material = (Material)Resources.Load("Projectors/Materials/SelectionThisProjector", typeof(Material));
        }

        public void HighlightEnemySelected()
        {
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("SelectionProjector").gameObject.SetActive(true);
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("SelectionProjector").GetComponent<Projector>().material = (Material)Resources.Load("Projectors/Materials/SelectionEnemyProjector", typeof(Material));
        }

        public void HighlightSelectedOff()
        {
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("SelectionProjector").gameObject.SetActive(false);
        }

        public void HighlightCanBeSelectedOn()
        {
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("Spotlight").gameObject.SetActive(true);
        }

        public void HighlightCanBeSelectedOff()
        {
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("Spotlight").gameObject.SetActive(false);
        }

        public Vector3 GetCenter()
        {
            Vector3 result;
            result = Model.transform.TransformPoint(0, 0, -HALF_OF_SHIPSTAND_SIZE);
            return result;
        }

        public Vector3 GetModelCenter()
        {
            Vector3 result;
            result = Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).Find("ModelCenter").position;
            return result;
        }

        public void SetHeight(float height)
        {
            Model.transform.Find("RotationHelper").localPosition = new Vector3(Model.transform.Find("RotationHelper").localPosition.x, height, Model.transform.Find("RotationHelper").localPosition.z);
        }

        public void ToggleShipStandAndPeg(bool value)
        {
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipStand").gameObject.SetActive(value);
            Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipPeg").gameObject.SetActive(value);
        }

        public void AnimatePrimaryWeapon()
        {
            Transform shotsTransform = Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).Find("ModelCenter").Find("Shots");
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
            Transform shotsTransform = Model.transform.Find("RotationHelper").Find("RotationHelper2").Find("ShipAllParts").Find("ShipModels").Find(Type).Find("ModelCenter").Find("Shots");
            if (shotsTransform != null)
            {
                shotsTransform.gameObject.SetActive(false);
            }
        }

    }

}
