using Abilities.SecondEdition;
using ActionsList;
using BoardTools;
using Conditions;
using Ship;
using Content;
using Tokens;
using System.Collections.Generic;

namespace Ship.SecondEdition.ResistanceTransportPod
{
    public class ViMoradi : ResistanceTransportPod
    {
        public ViMoradi()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Vi Moradi",
                "Starling",
                Faction.Resistance,
                1,
                3,
                6,
                isLimited: true,
                abilityType: typeof(ViMoradiAbility),
                legality: new List<Legality>
                {
                    Legality.StandardBanned,
                    Legality.ExtendedLegal
                }
            );
        }
    }
}

namespace Abilities.SecondEdition
{
    public class ViMoradiAbility : GenericAbility
    {
        public GenericShip ViMoradiAbilitySelectedTarget;

        public override void ActivateAbility()
        {
            Phases.Events.OnSetupEnd += RegisterViMoradiAbility;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupEnd -= RegisterViMoradiAbility;
        }

        protected void RegisterViMoradiAbility()
        {
            Triggers.RegisterTrigger(new Trigger()
            {
                Name = HostShip.PilotInfo.PilotName,
                TriggerType = TriggerTypes.OnSetupEnd,
                TriggerOwner = HostShip.Owner.PlayerNo,
                EventHandler = SelectViMoradiTarget
            });
        }

        protected void SelectViMoradiTarget(object Sender, System.EventArgs e)
        {
            ViMoradiDecisionSubPhase subphase = Phases.StartTemporarySubPhaseNew<ViMoradiDecisionSubPhase>(
                Name,
                Triggers.FinishTrigger
            );

            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "Assign the Compromising Intel condition to 1 enemy ship";
            subphase.ImageSource = HostShip;

            foreach (var enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Ships)
            {
                subphase.AddDecision(
                    enemyShip.Value.ShipId + ": " + enemyShip.Value.PilotInfo.PilotName,
                    delegate { SelectTarget(enemyShip.Value); }
                );
            }

            GenericShip bestEnemyAce = GetEnemyPilotWithHighestSkill();
            subphase.DefaultDecisionName = bestEnemyAce.ShipId + ": " + bestEnemyAce.PilotInfo.PilotName;

            subphase.RequiredPlayer = HostShip.Owner.PlayerNo;

            subphase.Start();
        }

        protected virtual void SelectTarget(GenericShip targetShip)
        {
            Messages.ShowInfo(HostShip.PilotInfo.PilotName + " has Compromising Intel on " + targetShip.PilotInfo.PilotName + " (" + targetShip.ShipId + ")");

            ViMoradiAbilitySelectedTarget = targetShip;
            ViMoradiAbilitySelectedTarget.Tokens.AssignCondition(
                new CompromisingIntelCondition(ViMoradiAbilitySelectedTarget) { Assigner = HostShip }
            );

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private GenericShip GetEnemyPilotWithHighestSkill()
        {
            GenericShip bestAce = null;
            int maxPilotSkill = 0;
            foreach (var enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Ships)
            {
                if (enemyShip.Value.State.Initiative > maxPilotSkill)
                {
                    bestAce = enemyShip.Value;
                    maxPilotSkill = enemyShip.Value.State.Initiative;
                }
            }
            return bestAce;
        }

        protected class ViMoradiDecisionSubPhase : SubPhases.DecisionSubPhase { }
    }
}

namespace Conditions
{
    public class CompromisingIntelCondition : GenericToken
    {
        public GenericShip Assigner;

        public CompromisingIntelCondition(GenericShip host) : base(host)
        {
            Name = ImageName = "Compromising Intel Condition";
            Temporary = false;

            Tooltip = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/b/be/Swz45_compromising-intel.png";
        }

        public override void WhenAssigned()
        {
            Host.OnSystemsPhaseStart += CheckFlipDial;
            Host.OnTryAddAvailableDiceModification += CheckFocusRestriction;
        }

        public override void WhenRemoved()
        {
            Host.OnSystemsPhaseStart -= CheckFlipDial;
            Host.OnTryAddAvailableDiceModification -= CheckFocusRestriction;
        }

        private void CheckFocusRestriction(GenericShip ship, GenericAction diceModification, ref bool diceModificationIsAllowed)
        {
            if (Combat.Attacker == Assigner || Combat.Defender == Assigner)
            {
                if (diceModification is FocusAction) diceModificationIsAllowed = false;
            }
        }

        private void CheckFlipDial(GenericShip ship)
        {
            if (Assigner != null && !Assigner.IsDestroyed)
            {
                DistanceInfo disInfo = new DistanceInfo(Host, Assigner);
                if (disInfo.Range < 4)
                {
                    Messages.ShowInfo("Compromising Inter: dial of " + Host.PilotInfo.PilotName + " is flipped faceup");
                    Roster.ToggleManeuverVisibility(Host, true);
                }
            }
        }
    }
}