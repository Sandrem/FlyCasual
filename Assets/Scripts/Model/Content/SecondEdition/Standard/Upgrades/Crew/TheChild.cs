using Upgrade;
using Ship;
using System;
using SubPhases;
using Tokens;

namespace UpgradesList.SecondEdition
{
    public class TheChild : GenericUpgrade
    {
        public TheChild() : base()
        {
            UpgradeInfo = new UpgradeCardInfo
            (
                "The Child",
                UpgradeType.Crew,
                cost: 7,
                abilityType: typeof(Abilities.SecondEdition.TheChildAbility),
                restriction: new FactionRestriction(Faction.Imperial, Faction.Rebel, Faction.Scum),
                addForce: 2
            );

            ImageUrl = "https://i.imgur.com/8pqkhJr.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TheChildAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnCheckForceRecurring += DenyForceRecurring;
            HostShip.OnAttackFinishAsDefender += CheckForceRegenAbility;
            Phases.Events.OnSetupEnd += RegisterAskToAssignConditions;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckForceRecurring -= DenyForceRecurring;
            HostShip.OnAttackFinishAsDefender -= CheckForceRegenAbility;
            Phases.Events.OnSetupEnd -= RegisterAskToAssignConditions;
        }

        private void DenyForceRecurring(ref bool isForceRecurring)
        {
            isForceRecurring = false;
        }

        private void CheckForceRegenAbility(GenericShip ship)
        {
            if (Combat.DamageInfo.IsDefenderSufferedDamage && HostShip.State.Force < HostShip.State.MaxForce)
            {
                Messages.ShowInfo($"{HostUpgrade.UpgradeInfo.Name}: {HostShip.PilotInfo.PilotName} recovers 1 Force");
                HostShip.State.RestoreForce();
            }
        }

        private void RegisterAskToAssignConditions()
        {
            RegisterAbilityTrigger(TriggerTypes.OnSetupEnd, AskOpponentToSelect2Ships);
        }

        private void AskOpponentToSelect2Ships(object sender, EventArgs e)
        {
            MultiSelectionSubphase subphase = Phases.StartTemporarySubPhaseNew<MultiSelectionSubphase>("The Child", Triggers.FinishTrigger);

            subphase.RequiredPlayer = HostShip.Owner.AnotherPlayer.PlayerNo;

            subphase.Filter = FilterSelection;
            subphase.GetAiPriority = GetAiPriority;
            subphase.MaxToSelect = 2;
            subphase.WhenDone = AssignConditions;

            subphase.DescriptionShort = HostUpgrade.UpgradeInfo.Name;
            subphase.DescriptionLong = "Choose 2 ships to assign Merciless Pursuit condition";
            subphase.ImageSource = HostUpgrade;

            subphase.Start();
        }

        private bool FilterSelection(GenericShip ship)
        {
            return ship.Owner.PlayerNo == HostShip.Owner.AnotherPlayer.PlayerNo;
        }

        private int GetAiPriority(GenericShip ship)
        {
            return ship.PilotInfo.Cost;
        }

        private void AssignConditions(Action callback)
        {
            foreach (GenericShip ship in Selection.MultiSelectedShips)
            {
                Messages.ShowInfo($"Merciless Pursuit condition is assigned to {ship.PilotInfo.PilotName}");
                ship.Tokens.AssignCondition
                (
                    new Conditions.MercilessPursuitCondition(ship) { TheChild = HostUpgrade as UpgradesList.SecondEdition.TheChild }
                );
            }
            callback();
        }
    }
}

namespace Conditions
{
    public class MercilessPursuitCondition : GenericToken
    {
        public GenericUpgrade SourceUpgrade;
        public UpgradesList.SecondEdition.TheChild TheChild;

        private GenericShip CachedAttacker;
        private GenericShip CachedDefender;

        public MercilessPursuitCondition(GenericShip host) : base(host)
        {
            Name = ImageName = "Merciless Pursuit Condition";
            Temporary = false;

            Tooltip = "https://i.imgur.com/K9qa95i.png";
        }

        public override void WhenAssigned()
        {
            Host.OnAttackFinishAsAttacker += CheckBonus;
        }

        public override void WhenRemoved()
        {
            Host.OnAttackFinishAsAttacker -= CheckBonus;
        }

        private void CheckBonus(GenericShip ship)
        {
            if (Combat.Defender.UpgradeBar.HasUpgradeInstalled(typeof(UpgradesList.SecondEdition.TheChild)))
            {
                CachedAttacker = Combat.Attacker;
                CachedDefender = Combat.Defender;
                Triggers.RegisterTrigger
                (
                    new Trigger()
                    {
                        Name = "Merciless Pursuit",
                        TriggerType = TriggerTypes.OnAttackFinish,
                        TriggerOwner = Host.Owner.PlayerNo,
                        EventHandler = AskToAcqureLock
                    }
                );
            }
        }

        private void AskToAcqureLock(object sender, EventArgs e)
        {
            MercilessPursuitDecisionSubphase subphase = Phases.StartTemporarySubPhaseNew<MercilessPursuitDecisionSubphase>(
                "Merciless Pursuit Decision",
                delegate
                {
                    Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                    Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
                    Triggers.FinishTrigger();
                });

            subphase.DescriptionShort = "Merciless Pursuit";
            subphase.DescriptionLong = "Do you want to acquire a lock on the defender?";
            subphase.ImageSource = TheChild;

            subphase.AddDecision("Yes", AcquireLock);
            subphase.AddDecision("No", SkipLock);

            subphase.DecisionOwner = Host.Owner;
            subphase.DefaultDecisionName = "Yes";

            subphase.Start();
        }

        private void AcquireLock(object sender, EventArgs e)
        {
            Messages.ShowInfo($"Merciless Pursuit: {CachedAttacker.PilotInfo.PilotName} aquires a lock on {CachedDefender.PilotInfo.PilotName}");
            ActionsHolder.AcquireTargetLock(CachedAttacker, CachedDefender, DecisionSubPhase.ConfirmDecision, DecisionSubPhase.ConfirmDecision);
        }

        private void SkipLock(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }

        public class MercilessPursuitDecisionSubphase : DecisionSubPhase { };
    }
}