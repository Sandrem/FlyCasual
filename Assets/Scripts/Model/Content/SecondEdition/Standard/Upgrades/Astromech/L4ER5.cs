using Actions;
using ActionsList;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class L4ER5 : GenericUpgrade
    {
        public L4ER5() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "L4E-R5",
                UpgradeType.Astromech,
                cost: 3,
                abilityType: typeof(Abilities.SecondEdition.L4ER5Ability),
                isLimited: true,
                restrictions: new UpgradeCardRestrictions
                (
                    new FactionRestriction(Faction.Resistance),
                    new ActionBarRestriction(typeof(RotateArcAction))
                ),
                addActionLink: new LinkedActionInfo(typeof(RotateArcAction), typeof(CalculateAction), linkedColor: ActionColor.White)
            );

            ImageUrl = "https://i.imgur.com/3HnRV9Z.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class L4ER5Ability : KyleKatarnAbility
    {
        protected override string GenerateAbilityString()
        {
            return "Choose another ship in arc to assign 1 of your Calculate tokens to it";
        }

        protected override Type GetTokenType()
        {
            return typeof(CalculateToken);
        }

        protected override bool FilterAbilityTarget(GenericShip ship)
        {
            return
                FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly }) &&
                FilterTargetsByRange(ship, 1, 3) &&
                Board.IsShipInArc(HostShip, ship);
        }
    }
}