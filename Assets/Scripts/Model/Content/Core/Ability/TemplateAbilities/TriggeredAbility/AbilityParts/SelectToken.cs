using Abilities.Parameters;
using Players;
using Ship;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using ToolParts;

namespace Abilities
{
    public class SelectToken : AbilityPart
    {
        private GenericAbility Ability;
        private ShipRole TokenOwner;
        private AbilityDescription AbilityDescription;
        private List<TokenColors> ColorsFilter;
        private GenericPlayer DecisionOwner;
        private AbilityPart Next = null;

        public SelectToken
        (
            AbilityDescription abilityDescription,
            GenericPlayer decisionOwner,
            List<TokenColors> colorsFilter,
            ShipRole tokenOwner = ShipRole.ThisShip,
            AbilityPart next = null
        )
        {
            TokenOwner = tokenOwner;
            AbilityDescription = abilityDescription;
            ColorsFilter = colorsFilter;
            Next = next;
        }

        public override void DoAction(GenericAbility ability)
        {
            Ability = ability;
            GameManagerScript.Instance.StartCoroutine(GetToken(Ability.GetShip(TokenOwner), ColorsFilter));
        }

        private IEnumerator GetToken(GenericShip ship, List<TokenColors> colorsFilter)
        {
            if (ship != null)
            {
                SelectTokenTool selectToken = new SelectTokenTool(
                    AbilityDescription,
                    DecisionOwner,
                    colorsFilter               
                );

                yield return selectToken.GetToken();
                Ability.TargetToken = selectToken.ChosenToken;

                FinishAbilityPart();
            }
            else
            {
                Messages.ShowInfo("Ability: Ship is not set");
                FinishAbilityPart();
            }
        }

        private void FinishAbilityPart()
        {
            if (Next != null)
            {
                Next.DoAction(Ability);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}
