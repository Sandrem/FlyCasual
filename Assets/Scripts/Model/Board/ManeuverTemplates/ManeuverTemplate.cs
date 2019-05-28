using Movement;
using System;
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

        public ManeuverTemplate(ManeuverBearing bearing, ManeuverDirection direction, ManeuverSpeed speed, bool isBombTemplate = false)
        {
            Bearing = bearing;

            Direction = direction;

            Speed = speed;

            string bearingString = bearing.ToString();
            string speedString = speed.ToString().Replace("Speed", "");
            if (isBombTemplate)
            {
                if (direction == ManeuverDirection.Left) direction = ManeuverDirection.Right;
                else if (direction == ManeuverDirection.Right) direction = ManeuverDirection.Left;
            }
            string directionString = (direction == ManeuverDirection.Forward) ? "" : direction.ToString();

            TemplatePrefabName = bearingString + speedString;
            Name = bearingString + " " + speedString + " " + directionString;
        }

        public void ApplyTemplate(Vector3 position, Vector3 angles)
        {
            if (TemplateGO == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/ManeuverTemplates/" + TemplatePrefabName);
                TemplateGO = GameObject.Instantiate<GameObject>(prefab,  Board.GetBoard());
                FinisherGO = TemplateGO.transform.Find("Finish").gameObject;
            }

            TemplateGO.transform.position = position;
            TemplateGO.transform.eulerAngles = angles;

            float directionFix = (Direction == ManeuverDirection.Left) ? 180 : 0;

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
