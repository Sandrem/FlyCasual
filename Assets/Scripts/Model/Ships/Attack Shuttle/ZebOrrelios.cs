using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using RuleSets;

namespace Ship
{
    namespace AttackShuttle
    {
        public class ZebOrrelios : AttackShuttle, ISecondEditionPilot
        {
            public ZebOrrelios() : base()
            {
                PilotName = "\"Zeb\" Orrelios";
                PilotSkill = 3;
                Cost = 18;

                IsUnique = true;

                PilotAbilities.Add(new Abilities.ZebOrreliosPilotAbility());
            }

            public void AdaptPilotToSecondEdition()
            {
                PilotSkill = 2;
                Cost = 34;
            }
        }
    }
}

namespace Abilities
{
    public class ZebOrreliosPilotAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckCancelCritsFirst += CancelCritsFirstIfDefender;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckCancelCritsFirst -= CancelCritsFirstIfDefender;
        }

        private void CancelCritsFirstIfDefender(GenericShip ship)
        {
            if (ship.ShipId == Combat.Defender.ShipId)
            {
                Combat.DiceRollAttack.CancelCritsFirst = true;
            }
        }
    }
}
