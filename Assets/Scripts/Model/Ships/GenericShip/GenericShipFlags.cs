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
        public bool IsBumped { get; set; }
        public bool IsSkipsAction { get; set; }
    } 

}
