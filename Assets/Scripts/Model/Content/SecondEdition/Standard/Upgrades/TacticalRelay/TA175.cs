using Ship;
using Upgrade;
using System.Linq;
using System.Collections.Generic;
using ActionsList;
using Actions;
using System;
using BoardTools;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class TA175 : GenericUpgrade
    {
        public TA175() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "TA-175",
                UpgradeType.TacticalRelay,
                cost: 8,
                isLimited: true,
                isSolitary: true,
                restriction: new FactionRestriction(Faction.Separatists),
                abilityType: typeof(Abilities.SecondEdition.TA175Ability)
            );

            Avatar = new AvatarInfo(
                Faction.Separatists,
                new Vector2(211, 14)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/3c/78/3c789eaf-ce5d-443a-8696-f0ad4781346c/swz41_ta-175.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class TA175Ability : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, bool isFled)
        {

            if (ship.Owner.PlayerNo == HostShip.Owner.PlayerNo
                && ship.ActionBar.PrintedActions.Any(n => n is CalculateAction)
                && Board.CheckInRange(HostShip, ship, 0, 3, RangeCheckReason.UpgradeCard))
            {
                RegisterAbilityTrigger(
                    TriggerTypes.OnShipIsDestroyed,
                    delegate { AssignCalculateTokens(ship); }
                );
            }
        }

        private void AssignCalculateTokens(GenericShip destroyedShips)
        {
            List<GenericShip> friendlyShipsInRange = Board.GetShipsAtRange(destroyedShips, new Vector2(0, 3), Team.Type.Friendly)
                .Where(n => n.Owner.PlayerNo == destroyedShips.Owner.PlayerNo)
                .Where(n => n.ShipId != destroyedShips.ShipId)
                .Where(n => n.ActionBar.PrintedActions.Any(a => a is CalculateAction))
                .ToList();

            AssignCalculateTokensRecursive(friendlyShipsInRange);
        }

        private void AssignCalculateTokensRecursive(List<GenericShip> friendlyShipsInRange)
        {
            if (friendlyShipsInRange.Count > 0)
            {
                GenericShip shipToAssign = friendlyShipsInRange.First();
                friendlyShipsInRange.Remove(shipToAssign);

                Messages.ShowInfo("TA-175: " + shipToAssign.PilotInfo.PilotName + " gets Calculate token", allowCopies: true);

                shipToAssign.Tokens.AssignToken(
                    typeof(Tokens.CalculateToken),
                    delegate{ AssignCalculateTokensRecursive(friendlyShipsInRange); }
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}