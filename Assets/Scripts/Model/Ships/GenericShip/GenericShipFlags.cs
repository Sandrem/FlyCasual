using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {
        public bool IsSetupPerformed { get; set; }
        public bool IsManeuverPerformed { get; set; }
        public bool IsAttackPerformed { get; set; }
        public bool IsDestroyed { get; set; }
        public bool IsSkipsActionSubPhase { get; set; }

		public bool IsFreeActionSkipped { get; set; }
    } 

}
