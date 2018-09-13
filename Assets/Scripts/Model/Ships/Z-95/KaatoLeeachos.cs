﻿using Abilities.SecondEdition;
using RuleSets;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;

namespace Ship
{
    namespace Z95
    {
        public class KaatoLeeachos : Z95, ISecondEditionPilot
        {
            public KaatoLeeachos() : base()
            {
                PilotName = "Kaa'to Leeachos";
                PilotSkill = 3;
                Cost = 29;

                IsUnique = true;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Illicit);
                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotRuleType = typeof(SecondEdition);

                faction = Faction.Scum;

                PilotAbilities.Add(new KaatoLeeachosAbilitySE());

                SEImageNumber = 170;
            }

            public void AdaptPilotToSecondEdition()
            {
               // nah
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KaatoLeeachosAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnCombatPhaseStart_Triggers -= RegisterAbility;
        }

        private void RegisterAbility()
        {
            if(TargetsForAbilityExist(FilterAbilityTarget))
                RegisterAbilityTrigger(TriggerTypes.OnCombatPhaseStart, Ability);
        }

        protected virtual string GenerateAbilityMessage()
        {
            return "Choose another friendly ship to transfer focus or evade from.";
        }

        private void Ability(object sender, EventArgs e)
        {
            SelectTargetForAbility(
                SelectAbilityTarget,
                FilterAbilityTarget,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                true,
                null,
                HostShip.PilotName,
                GenerateAbilityMessage(),
                HostShip.ImageUrl
            );
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly, TargetTypes.This })
                && FilterTargetsByRange(ship, 1, 2)
                && ship.Tokens.HasToken(typeof(FocusToken));
        }

        private int GetAiAbilityPriority(GenericShip ship)
        {
            int result = 0;
            return result;
        }

        private void SelectAbilityTarget()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            KaatoLeeachosDecisionSubPhaseSE subphase = (KaatoLeeachosDecisionSubPhaseSE)
            Phases.StartTemporarySubPhaseNew(
                "Select token to transfer to Kaa'to Leeachos.",
                typeof(KaatoLeeachosDecisionSubPhaseSE),
                Triggers.FinishTrigger
            );

            subphase.Start();

        }
    

        public class KaatoLeeachosDecisionSubPhaseSE : DecisionSubPhase
        {
            public GenericShip target;
            Type tokenType = null;

            public override void PrepareDecision(Action callBack)
            {
                InfoText = Selection.AnotherShip.PilotName + ": " + "Select token to transfer to Kaato.";
                DecisionOwner = Selection.AnotherShip.Owner;

                if (Selection.AnotherShip.Tokens.HasToken(typeof(FocusToken)))
                    AddDecision("Transfer focus token.", delegate { tokenType = typeof(FocusToken); Selection.AnotherShip.Tokens.RemoveToken(tokenType, AddTokenToKaato); });

                if (Selection.AnotherShip.Tokens.HasToken(typeof(EvadeToken)))
                    AddDecision("Transfer evade token.", delegate { tokenType = typeof(EvadeToken); Selection.AnotherShip.Tokens.RemoveToken(tokenType, AddTokenToKaato); });

                callBack();
            }

            private void AddTokenToKaato()
            {
                Selection.ThisShip.Tokens.AssignToken(tokenType, DecisionSubPhase.ConfirmDecision);
            }
        }
    }
}