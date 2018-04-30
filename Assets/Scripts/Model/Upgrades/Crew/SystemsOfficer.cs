using Upgrade;
using Ship;
using System.Linq;
using Tokens;
using System.Collections.Generic;
using UnityEngine;
using SquadBuilderNS;
using System;
using Abilities;
using Board;
using SubPhases;

namespace UpgradesList
{
    public class SystemsOfficer : GenericUpgrade
    {
        public SystemsOfficer() : base()
        {
            Types.Add(UpgradeType.Crew);            
            Name = "Systems Officer";
            Cost = 2;

            isLimited = true;

            AvatarOffset = new Vector2(45, 1);

            UpgradeAbilities.Add(new SystemsOfficerAbility());
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Imperial;
        }
    }
}

namespace Abilities
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
            if (BoardManager.IsOffTheBoard(ship)) return;

            // After executing a green maneuver
            var movementColor = HostShip.GetLastManeuverColor();
            if (movementColor == Movement.ManeuverColor.Green)
            {
                // ...check if there is another firendly ship at range 1
                var friendlyShipsAtRangeOne = HostShip.Owner.Ships.Values
                    .Where(another => another.ShipId != HostShip.ShipId)
                    .Where(another => BoardManager.GetRangeOfShips(HostShip, another) <= 1)
                    .ToArray();
                if(friendlyShipsAtRangeOne.Any())
                {
                    RegisterAbilityTrigger(TriggerTypes.OnShipMovementFinish, SystemsOfficerEffect);
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
                true,
                null,
                HostUpgrade.Name,
                "Choose another ship.\nIt may acquire a Target Lock.",
                HostUpgrade.ImageUrl
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
                HostUpgrade.Name,
                HostUpgrade.ImageUrl
            );
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;

            result += NeedTokenPriority(ship);
            result += ship.Cost + ship.UpgradeBar.GetUpgradesOnlyFaceup().Sum(n => n.Cost);

            return result;
        }

        private int NeedTokenPriority(GenericShip ship)
        {
            if (!ship.Tokens.HasToken(typeof(Tokens.BlueTargetLockToken), '*')) return 100;
            return 0;
        }
    }
}
