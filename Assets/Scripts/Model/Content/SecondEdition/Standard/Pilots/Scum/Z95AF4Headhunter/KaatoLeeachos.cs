using Content;
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
                PilotInfo = new PilotCardInfo25
                (
                    "Ka'ato Leeachos",
                    "Imposing Marauder",
                    Faction.Scum,
                    3,
                    3,
                    11,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.KaatoLeeachosAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Missile,
                        UpgradeType.Modification,
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    seImageNumber: 170
                );

                ModelInfo.SkinName = "Kaa'to Leeachos";
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
            return "Choose another friendly ship to transfer focus or evade from";
        }

        private void Ability(object sender, EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            SelectTargetForAbility(
                SelectAbilityTarget,
                FilterAbilityTarget,
                GetAiAbilityPriority,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                GenerateAbilityMessage(),
                HostShip
            );
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.OtherFriendly, TargetTypes.This })
                && FilterTargetsByRange(ship, 0, 2)
                && (ship.Tokens.HasToken<FocusToken>() || ship.Tokens.HasToken<EvadeToken>());
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
            subphase.TargetShip = TargetShip;
            subphase.HostShip = HostShip;
            subphase.Start();

        }

        public class KaatoLeeachosDecisionSubPhaseSE : DecisionSubPhase
        {
            public GenericShip HostShip;
            public GenericShip TargetShip;
            Type tokenType = null;

            public override void PrepareDecision(Action callBack)
            {
                DescriptionShort = HostShip.PilotInfo.PilotName;
                DescriptionLong = TargetShip.PilotInfo.PilotName + ": " + "Select token to transfer to Kaato.";
                ImageSource = HostShip;

                DecisionOwner = TargetShip.Owner;

                if (TargetShip.Tokens.HasToken(typeof(FocusToken)))
                    AddDecision("Transfer focus token.", delegate { tokenType = typeof(FocusToken); TargetShip.Tokens.RemoveToken(tokenType, AddTokenToKaato); });

                if (TargetShip.Tokens.HasToken(typeof(EvadeToken)))
                    AddDecision("Transfer evade token.", delegate { tokenType = typeof(EvadeToken); TargetShip.Tokens.RemoveToken(tokenType, AddTokenToKaato); });

                callBack();
            }

            private void AddTokenToKaato()
            {
                HostShip.Tokens.AssignToken(tokenType, DecisionSubPhase.ConfirmDecision);
            }
        }
    }
}
