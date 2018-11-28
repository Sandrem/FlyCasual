using Ship;
using Upgrade;
using UnityEngine;
using SubPhases;
using BoardTools;
using System.Linq;
using System;
using System.Collections.Generic;

namespace UpgradesList.FirstEdition
{
    public class SystemsOfficer : GenericUpgrade
    {
        public SystemsOfficer() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Systems Officer",
                UpgradeType.Crew,
                cost: 2,
                isLimited: true,
                restrictionFaction: Faction.Imperial,
                abilityType: typeof(Abilities.FirstEdition.SystemsOfficerAbility)
            );

            Avatar = new AvatarInfo(Faction.Imperial, new Vector2(45, 1));
        }        
    }
}

namespace Abilities.FirstEdition
{
    public class SystemsOfficerAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += RegisterSystemsOfficerAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= RegisterSystemsOfficerAbility;
        }

        private void RegisterSystemsOfficerAbility(GenericShip ship)
        {
            if (Board.IsOffTheBoard(ship)) return;

            // After executing a green maneuver
            var movementColor = HostShip.GetLastManeuverColor();
            if (movementColor == Movement.MovementComplexity.Easy)
            {
                // ...check if there is another firendly ship at range 1
                var friendlyShipsAtRangeOne = HostShip.Owner.Ships.Values
                    .Where(another => another.ShipId != HostShip.ShipId)
                    .Where(another => Board.GetRangeOfShips(HostShip, another) <= 1)
                    .ToArray();
                if (friendlyShipsAtRangeOne.Any())
                {
                    RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, SystemsOfficerEffect);
                }
            }
        }

        protected void SystemsOfficerEffect(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                GrantFreeTargetLock,
                IsFriendlyShipAtRangeOne,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostUpgrade.UpgradeInfo.Name,
                "Choose another ship.\nIt may acquire a Target Lock.",
                HostUpgrade
            );
        }

        protected bool IsFriendlyShipAtRangeOne(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) && FilterTargetsByRange(ship, 1, 1);
        }

        protected void GrantFreeTargetLock()
        {
            Selection.ThisShip = TargetShip;
            RegisterAbilityTrigger(TriggerTypes.OnFreeActionPlanned, AcquireFreeTargetLock);
            Triggers.ResolveTriggers(TriggerTypes.OnFreeActionPlanned, SelectShipSubPhase.FinishSelection);
        }

        protected void AcquireFreeTargetLock(object sender, System.EventArgs e)
        {
            TargetShip.ChooseTargetToAcquireTargetLock(() =>
            {
                Selection.ThisShip = HostShip;
                Phases.CurrentSubPhase.Resume();
                Triggers.FinishTrigger();
            },
                HostUpgrade.UpgradeInfo.Name,
                HostUpgrade
            );
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            result += NeedTokenPriority(ship);
            result += ship.PilotInfo.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.UpgradeInfo.Cost);

            return result;
        }

        private int NeedTokenPriority(GenericShip ship)
        {
            if (!ship.Tokens.HasToken(typeof(Tokens.BlueTargetLockToken), '*')) return 100;
            return 0;
        }
    }
}