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
        public string Name { get; private set; }
        public ManeuverBearing Bearing { get; private set; }
        public ManeuverDirection Direction { get; private set; }
        public ManeuverSpeed Speed { get; private set; }
        private GameObject TemplateGO;
        private GameObject FinisherGO;

        public ManeuverTemplate(ManeuverBearing bearing, ManeuverDirection direction, ManeuverSpeed speed)
        {
            Bearing = bearing;
            Direction = direction;
            Speed = speed;

            string bearingString = bearing.ToString();
            string speedString = speed.ToString().Replace("Speed", "");
            Name = bearingString + speedString;
        }

        public void ApplyTemplate(Vector3 position, Vector3 angles)
        {
            if (TemplateGO == null)
            {
                GameObject prefab = Resources.Load<GameObject>("Prefabs/ManeuverTemplates/" + Name);
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

        public Vector3 GetFinalPosition()
        {
            return FinisherGO.transform.position;
        }

        public Vector3 GetFinalAngles()
        {
            Debug.Log(FinisherGO.transform.eulerAngles.x + " " + FinisherGO.transform.eulerAngles.y + " " + FinisherGO.transform.eulerAngles.z);
            return FinisherGO.transform.eulerAngles;
        }

        public void DestroyTemplate()
        {
            GameObject.Destroy(TemplateGO.gameObject);
            TemplateGO = null;
        }
    }
}
