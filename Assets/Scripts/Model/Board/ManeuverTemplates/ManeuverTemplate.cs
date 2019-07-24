using Movement;
using Remote;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BoardTools
{
    public class ManeuverTemplate
    {
        public string TemplatePrefabName { get; private set; }
        public string Name { get; private set; }
        public ManeuverBearing Bearing { get; private set; }
        public ManeuverDirection Direction { get; private set; }
        public ManeuverSpeed Speed { get; private set; }
        private GameObject TemplateGO;
        private GameObject FinisherGO;
        public string NameNoDirection { get; private set; }
        public bool IsSideTemplate { get; private set; }
        public ObstaclesStayDetectorForced Collider { get { return TemplateGO.GetComponentInChildren<ObstaclesStayDetectorForced>(); } }

        public ManeuverTemplate(ManeuverBearing bearing, ManeuverDirection direction, ManeuverSpeed speed, bool isBombTemplate = false, bool isSideTemplate = false)
        {
            Bearing = bearing;
            Direction = direction;
            Speed = speed;
            IsSideTemplate = isSideTemplate;

            string bearingString = bearing.ToString();
            string speedString = speed.ToString().Replace("Speed", "");
            if (isBombTemplate)
            {
                if (direction == ManeuverDirection.Left) direction = ManeuverDirection.Right;
                else if (direction == ManeuverDirection.Right) direction = ManeuverDirection.Left;
            }

            TemplatePrefabName = bearingString + speedString + ((IsSideTemplate) ? "Alt" : "");

            NameNoDirection = bearingString + " " + speedString;
            string directionString = (direction == ManeuverDirection.Forward) ? "" : " " + direction.ToString();
            Name = NameNoDirection + directionString;
        }

        public void ApplyTemplate(GenericRemote remote, int jointNumber)
        {
            if (TemplateGO == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/ManeuverTemplates/" + TemplatePrefabName);
                TemplateGO = GameObject.Instantiate<GameObject>(prefab,  Board.GetBoard());
                FinisherGO = TemplateGO.transform.Find("Finish").gameObject;
            }

            TemplateGO.transform.position = remote.GetJointPosition(jointNumber);

            float directionFix = (Direction == ManeuverDirection.Left) ? 180 : 0;

            TemplateGO.transform.eulerAngles = remote.GetJointAngles(jointNumber) + new Vector3(0, 180, 0);

            TemplateGO.transform.localEulerAngles = new Vector3(
                TemplateGO.transform.localEulerAngles.x,
                TemplateGO.transform.localEulerAngles.y,
                directionFix
            );

            FinisherGO.transform.localEulerAngles = new Vector3(
                FinisherGO.transform.localEulerAngles.x,
                FinisherGO.transform.localEulerAngles.y,
                directionFix
            );
        }

        public void ApplyTemplate(GenericShip ship, Vector3 position, Direction direction)
        {
            if (TemplateGO == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/ManeuverTemplates/" + TemplatePrefabName);
                TemplateGO = GameObject.Instantiate<GameObject>(prefab, Board.GetBoard());
                FinisherGO = TemplateGO.transform.Find("Finish").gameObject;
            }

            TemplateGO.transform.position = position;

            float directionFix = (Direction == ManeuverDirection.Left) ? 180 : 0;

            Vector3 angles = ship.GetAngles();
            switch (direction)
            {
                case global::Direction.Top:
                    break;
                case global::Direction.Bottom:
                    angles += new Vector3(0, 180, 0);
                    break;
                case global::Direction.Left:
                    angles += new Vector3(0, -90, 0);
                    break;
                case global::Direction.Right:
                    angles += new Vector3(0, 90, 0);
                    break;
                default:
                    break;
            }
            TemplateGO.transform.eulerAngles = angles;

            TemplateGO.transform.localEulerAngles = new Vector3(
                TemplateGO.transform.localEulerAngles.x,
                TemplateGO.transform.localEulerAngles.y,
                directionFix
            );

            FinisherGO.transform.localEulerAngles = new Vector3(
                FinisherGO.transform.localEulerAngles.x,
                FinisherGO.transform.localEulerAngles.y,
                directionFix
            );
        }

        internal bool Any()
        {
            throw new NotImplementedException();
        }

        public Vector3 GetFinalPosition()
        {
            return FinisherGO.transform.position;
        }

        public Vector3 GetFinalAngles()
        {
            return FinisherGO.transform.eulerAngles;
        }

        public Quaternion GetFinalRotation()
        {
            return FinisherGO.transform.rotation;
        }

        public void DestroyTemplate()
        {
            GameObject.Destroy(TemplateGO.gameObject);
            TemplateGO = null;
        }

        public bool ValidTemplate()
        {
            bool result = true;

            switch (Speed)
            {
                case ManeuverSpeed.AdditionalMovement:
                    result = false;
                    break;
                case ManeuverSpeed.Speed0:
                    result = false;
                    break;
                case ManeuverSpeed.Speed1:
                    break;
                case ManeuverSpeed.Speed2:
                    break;
                case ManeuverSpeed.Speed3:
                    break;
                case ManeuverSpeed.Speed4:
                    result = Direction == ManeuverDirection.Forward;
                    break;
                case ManeuverSpeed.Speed5:
                    result = Direction == ManeuverDirection.Forward;
                    break;
                default:
                    result = false;
                    break;
            }

            return result;
        }
    }
}
