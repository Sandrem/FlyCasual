using Ship;
using Upgrade;
using System.Linq;
using ActionsList;
using System;
using System.Collections.Generic;
using Actions;

namespace UpgradesList.SecondEdition
{
    public class LarmaDAcy : GenericUpgrade
    {
        public LarmaDAcy() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Larma D'Acy",
                UpgradeType.Crew,
                cost: 4,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Resistance),
                abilityType: typeof(Abilities.SecondEdition.LarmaDAcyAbility)
            );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/555a815e60cd7bf4612109c3fd65beaa.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class LarmaDAcyAbility : GenericAbility
    {
        private bool set = false;

        private readonly List<Type> AllowedActionTypes = new List<Type>()
        {
            typeof(CoordinateAction),
            typeof(ReinforceAction),
            typeof(JamAction),
        };

        public override void ActivateAbility()
        {
            HostShip.OnTokenIsAssigned += CheckAbilityRestrictions;
            HostShip.OnTokenIsRemoved += CheckAbilityRestrictions;

            HostShip.OnCheckActionComplexity += CheckWhiteActionsWhileStressed;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnTokenIsAssigned -= CheckAbilityRestrictions;
            HostShip.OnTokenIsRemoved -= CheckAbilityRestrictions;

            HostShip.OnCheckActionComplexity -= CheckWhiteActionsWhileStressed;
        }

        private void CheckWhiteActionsWhileStressed(GenericAction action, ref ActionColor color)
        {
            if (AllowedActionTypes.Contains(action.GetType())
                && action.Color == ActionColor.White
                && HostShip.IsStressed)
            {
                Messages.ShowInfo(HostUpgrade.UpgradeInfo.Name + ": Action is treated as red");
                color = ActionColor.Red;
            }
        }

        private void CheckAbilityRestrictions(GenericShip ship, System.Type type)
        {
            if (type == typeof(Tokens.StressToken))
            {
                if (!set && HostShip.Tokens.CountTokensByType(typeof(Tokens.StressToken)) <= 2)
                {
                    foreach (Type actionType in AllowedActionTypes)
                    {
                        HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Add(actionType);
                    }
                    set = true;
                }
                else if (set && HostShip.Tokens.CountTokensByType(typeof(Tokens.StressToken)) > 2)
                {
                    foreach (Type actionType in AllowedActionTypes)
                    {
                        HostShip.ActionBar.ActionsThatCanbePreformedwhileStressed.Remove(actionType);
                    }
                    set = false;
                }
            }
        }
    }
}