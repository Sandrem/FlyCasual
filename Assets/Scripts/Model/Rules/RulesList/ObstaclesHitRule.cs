using System.Collections;
using UnityEngine;
using Ship;
using SubPhases;

namespace RulesList
{
    public class ObstaclesHitRule
    {
        static bool RuleIsInitialized = false;

        public ObstaclesHitRule()
        {
            SubscribeEvents();
        }

        private void SubscribeEvents()
        {
            if (!RuleIsInitialized)
            {
                GenericShip.OnPositionFinishGlobal += CheckHits;
                RuleIsInitialized = true;
            }
        }

        public void CheckHits(GenericShip ship)
        {
            if (ship.IsHitObstacles)
            {
                foreach (var obstacle in ship.ObstaclesHit)
                {
                    if (ship.IgnoreObstaclesList.Contains(obstacle)) continue;

                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Apply effect of hit obstacle",
                        TriggerOwner = ship.Owner.PlayerNo,
                        TriggerType = TriggerTypes.OnPositionFinish,
                        EventHandler = delegate { obstacle.OnHit(ship); }
                    });
                }
            }
        }
    }
}