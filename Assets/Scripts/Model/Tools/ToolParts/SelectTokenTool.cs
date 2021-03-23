using Tokens;
using SubPhases;
using System.Collections.Generic;
using Players;
using System.Collections;
using UnityEngine;
using Abilities.Parameters;
using System.Linq;

namespace ToolParts
{
    public class SelectTokenTool
    {
        public GenericToken ChosenToken {get; private set;}
        private bool IsFinished;
        private AbilityDescription AbilityDescription;
        private IImageHolder ImageHolder;
        private GenericPlayer DecisionOwner;
        private List<TokenColors> TokenColorsFilter;

        public SelectTokenTool(
            AbilityDescription abilityDescription,
            GenericPlayer decisionOwner,
            List<TokenColors> tokenColorsFilter = null)
        {
            AbilityDescription = abilityDescription;
            DecisionOwner = decisionOwner;
            TokenColorsFilter = tokenColorsFilter;
        }

        public IEnumerator GetToken()
        {
            CreateSubphase();
            while (!IsFinished)
            {
                yield return new WaitForEndOfFrame();
            }
            yield return true;            
        }

        private void CreateSubphase()
        {
            SelectTokenDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<SelectTokenDecisionSubPhase>("SelectToken", Finish);

            subphase.DescriptionShort = AbilityDescription.Name;
            subphase.DescriptionLong = AbilityDescription.Description;
            subphase.ImageSource = AbilityDescription.ImageSource;

            subphase.DecisionOwner = DecisionOwner;
            subphase.ShowSkipButton = true;

            Dictionary<string, GenericToken> tokens = new Dictionary<string, GenericToken>(); 

            foreach(GenericToken token in Selection.ThisShip.Tokens.GetAllTokens())
            {
                if (token.TokenColor == TokenColors.Orange || token.TokenColor == TokenColors.Red) tokens[token.Name] = token;
            }

            foreach(KeyValuePair<string, GenericToken> kv in tokens)
            {
                subphase.AddDecision
                (
                    kv.Key,
                    delegate { ChooseToken(kv.Value); }
                );
            }

            if (subphase.GetDecisions().Count > 0)
            {
                subphase.DefaultDecisionName = subphase.GetDecisions().First().Name;
                subphase.Start();
            }
            else
            {
                DecisionSubPhase.ConfirmDecisionNoCallback();
                Finish();
            }
        }

        private void ChooseToken(GenericToken token)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            ChosenToken = token;
            Finish();
        }

        private void Finish()
        {
            IsFinished = true;
        }
    }

    public class SelectTokenDecisionSubPhase : DecisionSubPhase {}   
}