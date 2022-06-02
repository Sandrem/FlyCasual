using System.Collections.Generic;
using Upgrade;
using System;
using Ship;
using SubPhases;
using Arcs;
using Tokens;
using Content;

namespace Ship
{
    namespace SecondEdition.TIELnFighter
    {
        public class IsbJingoist : TIELnFighter
        {
            public IsbJingoist() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "ISB Jingoist",
                    "Heartless Enforcer",
                    Faction.Imperial,
                    4,
                    2,
                    3,
                    limited: 2,
                    abilityType: typeof(Abilities.SecondEdition.IsbJingoistAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Illicit
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    }
                );

                ImageUrl = "https://i.imgur.com/6rxtxtb.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IsbJingoistAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCombatActivation += TryRegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCombatActivation -= TryRegisterAbility;
        }
        private void TryRegisterAbility(GenericShip ship)
        {
            if (TargetsForAbilityExist(FilterTargetsOfAbility))
            {
                RegisterAbilityTrigger(TriggerTypes.OnCombatActivation, AskSelectShip);
            }
        }

        private void AskSelectShip(object sender, EventArgs e)
        {
            SelectTargetForAbility
            (
                CheckTargetLegality,
                FilterTargetsOfAbility,
                GetAiPriorityOfTarget,
                HostShip.Owner.PlayerNo,
                HostShip.PilotInfo.PilotName,
                "Choose 1 enemy ship in your front sector at range 0-1",
                HostShip
            );
        }

        private bool FilterTargetsOfAbility(GenericShip ship)
        {
            return FilterByTargetType(ship, new List<TargetTypes>() { TargetTypes.Enemy })
                && FilterTargetsByRangeInArc(ship, 0, 1);
        }

        private int GetAiPriorityOfTarget(GenericShip ship)
        {
            int priority = ship.PilotInfo.Cost;
            if (!ship.Tokens.HasGreenTokens) priority += 50;
            return priority;
        }

        private void CheckTargetLegality()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            int rangeToTarget = HostShip.SectorsInfo.RangeToShipBySector(TargetShip, ArcType.Front);
            if (rangeToTarget >= 0 && rangeToTarget <= 1)
            {
                if (TargetShip.Tokens.HasGreenTokens)
                {
                    AskToRemoveGreenTokenInstead();
                }
                else
                {
                    AskWhichBadTokenToAssign();
                }
            }
        }

        private void AskToRemoveGreenTokenInstead()
        {
            Selection.ThisShip = TargetShip;
            Selection.ActiveShip = HostShip;

            IsbJingoistTokenRemovalDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<IsbJingoistTokenRemovalDecisionSubphase>("ISB Jingoist: Which token to remove",Triggers.FinishTrigger);
            subphase.AbilitySource = HostShip;
            subphase.DoIfSkipped = AskWhichBadTokenToAssign;
            subphase.Start();
        }

        private void AskWhichBadTokenToAssign()
        {
            Selection.ThisShip = HostShip;

            IsbJingoistBadTokenDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<IsbJingoistBadTokenDecisionSubphase>("ISB Jingoist: Which token to assign", Triggers.FinishTrigger);

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "Which token to assign?";
            subphase.ImageSource = HostShip;

            subphase.AddDecision("Deplete Tolen", delegate { AssignToken(new DepleteToken(TargetShip)); });
            subphase.AddDecision("Strain Tolen", delegate { AssignToken(new StrainToken(TargetShip)); });

            subphase.DefaultDecisionName = "Deplete Tolen";
            subphase.DecisionOwner = HostShip.Owner;
            subphase.ShowSkipButton = false;

            subphase.Start();
        }

        private void AssignToken(GenericToken token)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: {TargetShip.PilotInfo.PilotName} gains {token.Name}");
            TargetShip.Tokens.AssignToken(token, Triggers.FinishTrigger);
        }

        public class IsbJingoistBadTokenDecisionSubphase : DecisionSubPhase { }
        public class IsbJingoistTokenRemovalDecisionSubphase : RemoveGreenTokenDecisionSubPhase
        {
            public GenericShip AbilitySource;
            public Action DoIfSkipped;

            public override void PrepareCustomDecisions()
            {
                DescriptionShort = "ISB Jingoist";
                DescriptionLong = "Do you want to remove 1 green token to avoid ISB Jingoist's ability?";
                ImageSource = AbilitySource;

                DecisionOwner = Selection.ThisShip.Owner;
                DefaultDecisionName = "None";

                AddDecision("None", ContinueAbility);
            }

            private void ContinueAbility(object sender, EventArgs e)
            {
                DecisionSubPhase.ConfirmDecisionNoCallback();
                DoIfSkipped();
            }

            protected override void AfterTokenIsDiscarded()
            {
                Selection.ChangeActiveShip(AbilitySource);
                base.AfterTokenIsDiscarded();
            }
        }
    }
}

