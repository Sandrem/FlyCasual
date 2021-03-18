using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.ASF01BWing
{
    public class NetremPollard : ASF01BWing
    {
        public NetremPollard() : base()
        {
            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            PilotInfo = new PilotCardInfo(
                "Netrem Pollard",
                3,
                44,
                isLimited: true,
                abilityType: typeof(NetremPollardAbility),
                extraUpgradeIcon: UpgradeType.Talent,
                abilityText: "After you barrel rolled you may choose 1 friendly ship that is not stressed at range 0-1 - that ship gains 1 stress token and can rotate 90 degrees"
            );

            ImageUrl = "https://i.imgur.com/tDMmS4S.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NetremPollardAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action is BarrelRollAction && HasTargetsForAbility())
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, SelectTarget);
            }
        }

        private bool HasTargetsForAbility()
        {
            foreach (GenericShip ship in HostShip.Owner.Ships.Values)
            {
                if (FilterTargets(ship)) return true;
            }

            return false;
        }

        private void SelectTarget(object sender, EventArgs e)
        {
            SelectTargetForAbility
            (
                AskWhatToDo,
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                name: HostShip.PilotInfo.PilotName,
                description: "You may choose 1 friendly ship - that ship gains 1 stress token and can rotate",
                imageSource: HostShip
            );
        }

        private void AskWhatToDo()
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            NetremPollardDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<NetremPollardDecisionSubphase>("Rotate the ship?", Triggers.FinishTrigger);

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "Gain 1 Stress to rotate the ship?";
            subphase.ImageSource = HostUpgrade;

            subphase.AddDecision("90 Counterclockwise", Rotate90Counterclockwise);
            subphase.AddDecision("90 Clockwise", Rotate90Clockwise);
            subphase.AddDecision("No", delegate { DecisionSubPhase.ConfirmDecision(); }, isCentered: true);

            subphase.Start();
        }

        private void Rotate90Clockwise(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Selection.ThisShip = TargetShip;
            Selection.ThisShip.Tokens.AssignToken(
                typeof(StressToken),
                delegate { Selection.ThisShip.Rotate90Clockwise(FinishAbility); }
            );
        }

        private void Rotate90Counterclockwise(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Selection.ThisShip = TargetShip;
            Selection.ThisShip.Tokens.AssignToken(
                typeof(StressToken),
                delegate { Selection.ThisShip.Rotate90Counterclockwise(Triggers.FinishTrigger); }
            );
        }

        private void FinishAbility()
        {
            Selection.ThisShip = HostShip;
            Triggers.FinishTrigger();
        }

        private bool FilterTargets(GenericShip ship)
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            return (distInfo.Range <= 1 && !ship.IsStressed);
        }

        private int GetAiPriority(GenericShip ship)
        {
            return 0;
        }

        private class NetremPollardDecisionSubphase : DecisionSubPhase { };
    }
}