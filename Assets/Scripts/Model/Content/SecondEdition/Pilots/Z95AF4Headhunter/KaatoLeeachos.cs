using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.Z95AF4Headhunter
    {
        public class KaatoLeeachos : Z95AF4Headhunter
        {
            public KaatoLeeachos() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Ka'ato Leeachos",
                    3,
                    29,
                    limited: 1,
                    abilityType: typeof(Abilities.SecondEdition.KaatoLeeachosAbility)
                );

                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Illicit);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Elite);

                ShipInfo.Faction = Faction.Scum;

                SEImageNumber = 170;
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KaatoLeeachosAbility : GenericAbility
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
            if (TargetsForAbilityExist(FilterAbilityTarget))
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
                HostShip.PilotName,
                GenerateAbilityMessage(),
                HostShip
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
