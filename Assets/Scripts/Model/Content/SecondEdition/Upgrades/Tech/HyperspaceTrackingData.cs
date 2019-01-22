using BoardTools;
using Ship;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class HyperspaceTrackingData : GenericUpgrade
    {
        public static readonly int LowestPossibleInitiative = 0;
        public static readonly int HighestPossibleInitiative = 6;
        public static readonly int ShortestTokenDistance = 0;
        public static readonly int LongestTokenDistance = 2;

        public HyperspaceTrackingData() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Hyperspace Tracking Data",
                UpgradeType.Tech,
                cost: 2,
                restrictions: new UpgradeCardRestrictions(
                    new BaseSizeRestriction(BaseSize.Large),
                    new FactionRestriction(Faction.FirstOrder)
                    ),
                abilityType: typeof(Abilities.SecondEdition.HyperspaceTrackingDataAbility)
                );

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/a48713c6784bf33bd3f8b36eb19221c3.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class HyperspaceTrackingDataAbility : GenericAbility, IModifyPilotSkill
    {
        public int NewSetupInitiative;
        public List<GenericShip> CurrentTargets;

        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += RegisterHyperspaceTrackingDataPreSetupAbility;
            Phases.Events.OnSetupEnd += RestoreInitialInitiative;

            Phases.Events.OnSetupEnd += RegisterHyperspaceTrackingDataPostSetupAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= RegisterHyperspaceTrackingDataPreSetupAbility;
            Phases.Events.OnSetupEnd -= RestoreInitialInitiative;

            Phases.Events.OnSetupEnd -= RegisterHyperspaceTrackingDataPostSetupAbility;
        }

        protected void RegisterHyperspaceTrackingDataPreSetupAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Hyperspace Tracking Data Ability",
                TriggerType = TriggerTypes.OnGameStart,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = CreateInitiativeSubPhase,
                Skippable = true
            });
        }

        protected void RegisterHyperspaceTrackingDataPostSetupAbility()
        {
            CurrentTargets = new List<GenericShip>();
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = "Hyperspace Tracking Data Ability",
                TriggerType = TriggerTypes.OnGameStart,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = InitiateTokenSubPhase,
                Skippable = true
            });
        }

        #region Initiative Subphase

        protected void CreateInitiativeSubPhase(object sender, System.EventArgs e)
        {
            var initiativeSubPhase = (HyperspaceTrackingDataInitiativeSubPhase)Phases.StartTemporarySubPhaseNew(
                    Name,
                    typeof(HyperspaceTrackingDataInitiativeSubPhase),
                    Triggers.FinishTrigger
                );

            initiativeSubPhase.InfoText = String.Format("{0}: ({1}) You may choose a number between {2} and {3}. "
                + "Treat your initiative as the chosen value during Setup.",
                HostShip.ShipId,
                HostShip.PilotInfo.PilotName,
                UpgradesList.SecondEdition.HyperspaceTrackingData.LowestPossibleInitiative,
                UpgradesList.SecondEdition.HyperspaceTrackingData.HighestPossibleInitiative);

            for (var i = UpgradesList.SecondEdition.HyperspaceTrackingData.LowestPossibleInitiative;
                i <= UpgradesList.SecondEdition.HyperspaceTrackingData.HighestPossibleInitiative; i++)
            {
                int initiative = i;
                initiativeSubPhase.AddDecision(initiative.ToString(),
                    delegate { SelectInitiative(initiative); }
                    );
            }

            initiativeSubPhase.DefaultDecisionName = UpgradesList.SecondEdition.HyperspaceTrackingData.HighestPossibleInitiative.ToString();
            initiativeSubPhase.RequiredPlayer = HostShip.Owner.PlayerNo;

            initiativeSubPhase.Start();
        }

        protected void SelectInitiative(int initiative)
        {
            this.NewSetupInitiative = initiative;
            this.HostShip.State.AddPilotSkillModifier(this);

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        protected void RestoreInitialInitiative()
        {
            this.HostShip.State.RemovePilotSkillModifier(this);
        }

        public void ModifyPilotSkill(ref int pilotSkill)
        {
            pilotSkill = NewSetupInitiative;
        }

        protected class HyperspaceTrackingDataInitiativeSubPhase : SubPhases.DecisionSubPhase { }

        #endregion

        #region Token Subphase

        private void InitiateTokenSubPhase(object sender, EventArgs e)
        {
            CreateTokenSubPhase();
        }

        private void CreateTokenSubPhase()
        {
            if (TargetsForAbilityExist(FilterAbilityTarget))
            {
                SelectTargetForAbility(
                    SelectAbilityTarget,
                    FilterAbilityTarget,
                    getAiPriority: delegate { return 100; },
                    subphaseOwnerPlayerNo: HostShip.Owner.PlayerNo,
                    name: String.Format("{0} {1} - {2}", HostShip.ShipId, HostShip.PilotInfo.PilotName, HostName),
                    description: "Choose a ship to assign 1 focus or 1 evade token to.",
                    showSkipButton: true
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        protected void SelectAbilityTarget()
        {
            CurrentTargets.Add(TargetShip);

            var tokenSubPhase = (HyperspaceTrackingDataTokenSubPhase)Phases.StartTemporarySubPhaseNew(
                Name,
                typeof(HyperspaceTrackingDataTokenSubPhase),
                CheckMoreTargets
            );

            tokenSubPhase.ShowSkipButton = true;
            tokenSubPhase.InfoText = String.Format("Hyperspace Tracking Data: {0}: ({1}) "
                + "Assign 1 Focus Token or 1 Evade Token",
                TargetShip.ShipId,
                TargetShip.PilotInfo
                );
            tokenSubPhase.DecisionViewType = SubPhases.DecisionViewTypes.TextButtons;

            tokenSubPhase.AddDecision("Focus", delegate {
                TargetShip.Tokens.AssignToken(typeof(FocusToken), CheckMoreTargets);
            });
            tokenSubPhase.AddDecision("Evade", delegate {
                TargetShip.Tokens.AssignToken(typeof(EvadeToken), CheckMoreTargets);
            });

            tokenSubPhase.DefaultDecisionName = tokenSubPhase.GetDecisions().First().Name;

            tokenSubPhase.Start();
        }

        protected virtual bool FilterAbilityTarget(GenericShip ship)
        {
            if (CurrentTargets.Contains(ship))
            {
                return false;
            }

            return
                FilterByTargetType(ship, new List<SubPhases.TargetTypes>() {
                    SubPhases.TargetTypes.OtherFriendly,
                    SubPhases.TargetTypes.This }) &&
                FilterTargetsByRange(
                    ship,
                    UpgradesList.SecondEdition.HyperspaceTrackingData.ShortestTokenDistance,
                    UpgradesList.SecondEdition.HyperspaceTrackingData.LongestTokenDistance
                    );
        }

        private void CheckMoreTargets()
        {
            SubPhases.DecisionSubPhase.ConfirmDecisionNoCallback();
            CreateTokenSubPhase();
        }

        protected class HyperspaceTrackingDataTokenSubPhase : SubPhases.DecisionSubPhase { }

        #endregion
    }
}
