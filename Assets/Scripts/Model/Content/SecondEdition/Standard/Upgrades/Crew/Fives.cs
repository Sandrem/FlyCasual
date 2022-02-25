using Ship;
using Upgrade;
using SubPhases;
using System.Linq;
using Movement;
using System;
using Tokens;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradesList.SecondEdition
{
    public class Fives : GenericUpgrade
    {
        public Fives() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "\"Fives\"",
                UpgradeType.Crew,
                cost: 3,
                isLimited: true,
                restriction: new FactionRestriction(Faction.Republic),
                abilityType: typeof(Abilities.SecondEdition.FivesAbility)
            );

            Avatar = new AvatarInfo(
                Faction.Republic,
                new Vector2(227, 9)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/fa/fe/fafeeec4-919f-4968-b26f-93f3cdda03b3/swz70_a1_fives_upgrade.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class FivesAbility : GenericAbility
    {
        Dictionary<Type, int> StoredTokens = new Dictionary<Type, int>()
        {
            {typeof(FocusToken), 0 },
            {typeof(EvadeToken), 0 }
        };

        public override void ActivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker += CheckMissCondition;
            HostShip.OnCombatActivation += CheckGetTokenCondition;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackMissedAsAttacker -= CheckMissCondition;
            HostShip.OnCombatActivation -= CheckGetTokenCondition;
        }

        private void CheckMissCondition()
        {
            if (Combat.Defender.State.Initiative >= HostShip.State.Initiative)
            {
                RegisterAbilityTrigger(TriggerTypes.OnAttackMissed, AskWhatTokenToAssign);
            }
        }

        private void AskWhatTokenToAssign(object sender, EventArgs e)
        {
            WhatTokenToAssignSubPhase subphase = Phases.StartTemporarySubPhaseNew<WhatTokenToAssignSubPhase>("What token to assign", Triggers.FinishTrigger);

            subphase.DescriptionShort = "\"Fives\"";
            subphase.DescriptionLong = "Select which token to place on \"Fives\" card";
            subphase.ImageSource = HostUpgrade;

            subphase.AddDecision("Focus Token", delegate { PlaceTokenOnCard(typeof(FocusToken)); });
            subphase.AddDecision("Evade Token", delegate { PlaceTokenOnCard(typeof(EvadeToken)); });

            subphase.ShowSkipButton = false;
            subphase.DefaultDecisionName = "Focus Token";
            subphase.DecisionOwner = HostShip.Owner;

            subphase.Start();
        }

        private void PlaceTokenOnCard(Type tokenType)
        {
            StoredTokens[tokenType]++;
            HostUpgrade.NamePostfix = $"(F:{StoredTokens[typeof(FocusToken)]} E:{StoredTokens[typeof(EvadeToken)]})";
            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);

            DecisionSubPhase.ConfirmDecision();
        }

        private void CheckGetTokenCondition(GenericShip ship)
        {
            if (StoredTokens.Any(n => n.Value != 0))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, AskWhatTokenToGain);
            }
        }

        private void AskWhatTokenToGain(object sender, EventArgs e)
        {
            WhatTokenToGainSubPhase subphase = Phases.StartTemporarySubPhaseNew<WhatTokenToGainSubPhase>("What token to gain", Triggers.FinishTrigger);

            subphase.DescriptionShort = "\"Fives\"";
            subphase.DescriptionLong = "Do you want to gain a token from \"Fives\" card?";
            subphase.ImageSource = HostUpgrade;

            if (StoredTokens[typeof(FocusToken)] > 0)
            {
                subphase.AddDecision($"Focus Token {StoredTokens[typeof(FocusToken)]}", delegate { GetTokenFromcard(typeof(FocusToken)); });
            }

            if (StoredTokens[typeof(EvadeToken)] > 0)
            {
                subphase.AddDecision($"Evade Token {StoredTokens[typeof(EvadeToken)]}", delegate { GetTokenFromcard(typeof(EvadeToken)); });
            }

            subphase.ShowSkipButton = true;
            subphase.DefaultDecisionName = "Focus Token";
            subphase.DecisionOwner = HostShip.Owner;

            subphase.Start();
        }

        private void GetTokenFromcard(Type type)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();
            
            StoredTokens[type]--;
            HostUpgrade.NamePostfix = $"(F:{StoredTokens[typeof(FocusToken)]} E:{StoredTokens[typeof(EvadeToken)]})";
            Roster.UpdateUpgradesPanel(HostShip, HostShip.InfoPanel);

            HostShip.Tokens.AssignToken(type, Triggers.FinishTrigger);
        }

        public class WhatTokenToAssignSubPhase : DecisionSubPhase { }
        public class WhatTokenToGainSubPhase : DecisionSubPhase { }
    }
}