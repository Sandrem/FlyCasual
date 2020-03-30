using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    public partial class GenericShip
    {
        public virtual bool HasCombatActivation { get { return !IsAttackPerformed; } }
        public bool IsSetupPerformed { get; set; }
        public bool IsManeuverPerformed { get; set; }
        public bool IsAttackPerformed { get; set; }
        public bool IsAttackSkipped { get; set; }
        public bool IsManeuverSkipped { get; set; }
        public bool IsActivatedDuringCombat { get; set; }

        public bool IsSkipsActionSubPhase { get; set; }
        public bool IsBombAlreadyDropped { get; set; }

        public bool IsFreeActionSkipped { get; set; }

        public bool CanPerformActionsWhenBumped { get; set; }
        public bool CanPerformActionsWhenOverlapping { get; set; }
        public bool CanBeCoordinated
        {
            get
            {
                bool result = true;
                if (OnCanBeCoordinated != null) OnCanBeCoordinated(this, ref result);
                return result;
            }
        }

        public bool IsReadyToBeDestroyed { get; set; }
        public bool IsDestroyed { get; set; }

        public bool IsSystemsAbilityCanBeActivated { get { return CheckSystemsAbilityActivation() && !IsSystemsAbilityInactive; } }
        public bool IsSystemsAbilityInactive { get; set; }

        public bool AlwaysShowAssignedManeuver { get; set; }
    } 

}
