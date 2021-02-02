using System;
using Obstacles;
using Ship;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class QiRa : GenericUpgrade
    {
        public QiRa()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Qi'Ra",
                UpgradeType.Crew,
                cost: 2,
                abilityType: typeof(Abilities.SecondEdition.QiRaAbility),
                seImageNumber: 161,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Scum)
            );

            Avatar = new AvatarInfo(
                Faction.Scum,
                new Vector2(269, 9)
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class QiRaAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnTargetLockIsAcquired += CheckAbility;
            HostShip.OnBeforeTokenIsRemoved += CheckRemoveAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTargetLockIsAcquired -= CheckAbility;
            HostShip.OnBeforeTokenIsRemoved -= CheckRemoveAbility;
        }

        private void CheckAbility(ITargetLockable target)
        {
            if (target is GenericObstacle)
            {
                Messages.ShowInfo("Qi'Ra: While you move or perform attack, you ignore this obstacle");
                HostShip.IgnoreObstaclesList.Add(target as GenericObstacle);
            }
        }

        private void CheckRemoveAbility(GenericShip ship, GenericToken token, ref bool data)
        {
            BlueTargetLockToken lockToken = token as BlueTargetLockToken;
            if (lockToken != null)
            {
                GenericObstacle lockedObstacle = lockToken.OtherTargetLockTokenOwner as GenericObstacle;
                if (lockedObstacle != null)
                {
                    if (HostShip.Tokens.GetTargetLockLetterPairsOn(lockedObstacle).Count == 1)
                    {
                        Messages.ShowInfo("Qi'Ra: You don't ignore obstacle anymore");
                        HostShip.IgnoreObstaclesList.Remove(lockedObstacle);
                    }
                }
            }
        }
    }
}