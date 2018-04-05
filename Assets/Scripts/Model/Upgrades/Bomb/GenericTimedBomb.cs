using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ship;
using Bombs;
using System.Linq;

namespace Upgrade
{

    abstract public class GenericTimedBomb : GenericBomb
    {
        public GenericTimedBomb() : base()
        {

        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            host.OnManeuverIsRevealed -= BombsManager.CheckBombDropAvailability;
            host.OnManeuverIsRevealed += BombsManager.CheckBombDropAvailability;
        }

        public override void ActivateBombs(List<GameObject> bombObjects, Action callBack)
        {
            Phases.OnActivationPhaseEnd_Triggers -= PlanTimedDetonation;
            Phases.OnActivationPhaseEnd_Triggers += PlanTimedDetonation;

            CurrentBombObjects.AddRange(bombObjects);
            base.ActivateBombs(bombObjects, callBack);
        }

        private void PlanTimedDetonation()
        {
            foreach (var bombObject in CurrentBombObjects)
            {
                Triggers.RegisterTrigger(new Trigger()
                {
                    Name = "Detonation of " + Name,
                    TriggerType = TriggerTypes.OnActivationPhaseEnd,
                    TriggerOwner = Host.Owner.PlayerNo,
                    EventHandler = TryDetonate,
                    EventArgs = new BombDetonationEventArgs()
                    {
                        BombObject = bombObject
                    }
                });
            }
        }

        protected override void Detonate()
        {
            Phases.OnActivationPhaseEnd_Triggers -= PlanTimedDetonation;
            foreach (var ship in BombsManager.GetShipsInRange(BombsManager.CurrentBombObject))
            {
                RegisterDetonationTriggerForShip(ship);
            }
        
            base.Detonate();
        }

    }

}