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
        public bool IsActivatedDuringCombat { get; set; }

        public bool IsSkipsActionSubPhase { get; set; }
        public bool IsBombAlreadyDropped { get; set; }

        public bool IsFreeActionSkipped { get; set; }

        public bool CanPerformActionsWhileStressed { get; set; }
        public bool CanPerformActionsWhenBumped { get; set; }
        public bool CanPerformRedManeuversWhileStressed { get; set; }
        public int CanLaunchBombsWithTemplate { get; set; }

        public bool IsReadyToBeDestroyed { get; set; }
        public bool IsDestroyed { get; set; }

        public bool IsSystemsAbilityCanBeActivated { get { return OnSystemsAbilityActivation != null && !IsSystemsAbilityInactive; } }
        public bool IsSystemsAbilityInactive { get; set; }

        public bool AlwaysShowAssignedManeuver { get; set; }
    } 

}
