using Ship;
using System;
using UnityEngine;
using Upgrade;
using Abilities;
using SubPhases;
using RuleSets;
using Obstacles;
using BoardTools;

namespace UpgradesList
{
    public class BobaFett : GenericUpgrade, ISecondEditionUpgrade
    {
        public BobaFett() : base()
        {
            Types.Add(UpgradeType.Crew);
            Name = "Boba Fett";
            Cost = 1;

            isUnique = true;
            UpgradeAbilities.Add(new BobaFettCrewAbility());
        }

        public void AdaptUpgradeToSecondEdition()
        {
            Cost = 4;

            UpgradeAbilities.RemoveAll(a => a is BobaFettCrewAbility);
            UpgradeAbilities.Add(new Abilities.SecondEdition.BobaFettCrewAbilitySE());

            SEImageNumber = 129;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Scum;
        }                       
    }
}

namespace Abilities.SecondEdition
{
    public class BobaFettCrewAbilitySE : GenericAbility
    {
        public override void ActivateAbility()
        {
            Phases.Events.OnSetupStart += MoveToReserve;
            Phases.Events.OnSetupEnd += RegisterReturn;
        }

        public override void DeactivateAbility()
        {
            Phases.Events.OnSetupStart -= MoveToReserve;
            Phases.Events.OnSetupEnd -= RegisterReturn;
        }

        private void MoveToReserve()
        {
            Messages.ShowInfo(HostShip.PilotName + " is moved to Reserve");
            Roster.MoveToReserve(HostShip);
        }

        private void RegisterReturn()
        {
            RegisterAbilityTrigger(TriggerTypes.OnSetupEnd, SetupShip);
        }

        private void SetupShip(object sender, EventArgs e)
        {
            Roster.ReturnFromReserve(HostShip);

            var subphase = Phases.StartTemporarySubPhaseNew<SetupShipMidgameSubPhase>(
                "Setup",
                delegate {
                    Messages.ShowInfo(HostShip.PilotName + " is placed");
                    Triggers.FinishTrigger();
                }
            );

            subphase.ShipToSetup = HostShip;
            subphase.SetupSide = Direction.None;
            subphase.AbilityName = HostUpgrade.Name;
            subphase.Description = "Place yourself at range 0 of an obstacle and beyond range 3 of any enemy ship";
            subphase.ImageUrl = HostUpgrade.ImageUrl;
            subphase.SetupFilter = SetupFilter;

            subphase.Start();
        }

        private bool SetupFilter()
        {
            bool result = true;

            if (HostShip.Model.GetComponentInChildren<ObstaclesStayDetector>().OverlapedAsteroids.Count == 0)
            {
                Messages.ShowErrorToHuman("Cannot setup the ship:\nMust be placed on an asteroid");
                return false;
            }

            foreach (GenericShip enemyShip in Roster.GetPlayer(Roster.AnotherPlayer(HostShip.Owner.PlayerNo)).Ships.Values)
            {
                DistanceInfo distInfo = new DistanceInfo(HostShip, enemyShip);
                if (distInfo.Range < 4)
                {
                    Messages.ShowErrorToHuman("Cannot setup the ship:\nRange to " + enemyShip.PilotName + " is " + distInfo.Range);
                    return false;
                }
            }

            return result;
        }
    }
}

namespace Abilities
{
    // After performing an attack, if the defender was dealt a faceup Damage card, you may discard this card to choose and discard 1 of the defender's Upgrade cards.
    public class BobaFettCrewAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            GenericShip.OnFaceupCritCardReadyToBeDealtGlobal += CheckBobaFettAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnFaceupCritCardReadyToBeDealtGlobal -= CheckBobaFettAbility;
        }

        protected void CheckBobaFettAbility(GenericShip target, GenericDamageCard crit, EventArgs e)
        {
            // Check that the ship getting the crit is the defender of an attack, and Boba Fett's ship is the attacker, and the defender has any card to discard.
            if(Combat.Defender != null && target.ShipId == Combat.Defender.ShipId && Combat.Attacker != null && HostShip.ShipId == Combat.Attacker.ShipId && Combat.Defender.UpgradeBar.GetUpgradesOnlyFaceup().Count > 0)
            {
                RegisterAbilityTrigger(TriggerTypes.OnFaceupCritCardReadyToBeDealt, (s1, e1) =>
                {
                    var phase = Phases.StartTemporarySubPhaseNew<BobaFettCrewDecisionSubPhase>("Boba Fett's ability", Triggers.FinishTrigger);
                    phase.bobaFettUpgradeCard = HostUpgrade;
                    phase.Start();
                });
            }
        }
    }
}

namespace SubPhases
{

    public class BobaFettCrewDecisionSubPhase : DecisionSubPhase
    {
        public GenericUpgrade bobaFettUpgradeCard; 

        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Use Boba Fett's ability?";
            RequiredPlayer = Combat.Attacker.Owner.PlayerNo;

            AddDecision("Yes", UseBobaFettCrewAbility);
            AddDecision("No", DontUseBobaFettCrewAbility);

            DefaultDecisionName = "Yes";

            callBack();
        }

        private void UseBobaFettCrewAbility(object sender, System.EventArgs e)
        {
            Sounds.PlayShipSound(GetRandomLine());
            Messages.ShowInfo("Boba Fett is used");                                    
            bobaFettUpgradeCard.TryDiscard(() =>
            {                
                var selectUpgradePhase = Phases.StartTemporarySubPhaseNew<BobaFettCrewUpgradeDecisionSubPhase>("Discard upgrade", ConfirmDecision);
                selectUpgradePhase.Start();
            });
        }

        private void DontUseBobaFettCrewAbility(object sender, System.EventArgs e)
        {
            ConfirmDecision();
        }

        protected string[] bobaLines = new[] { "BobaFettQ1", "BobaFettQ2", "BobaFettQ3", "BobaFettQ4", "BobaFettQ5", };
        protected string GetRandomLine()
        {
            return bobaLines[UnityEngine.Random.Range(0, bobaLines.Length)];
        }

    }

    public class BobaFettCrewUpgradeDecisionSubPhase: DecisionSubPhase
    {
        public override void PrepareDecision(System.Action callBack)
        {
            InfoText = "Select upgrade to discard:";
            RequiredPlayer = Combat.Attacker.Owner.PlayerNo;

            var upgrades = Combat.Defender.UpgradeBar.GetUpgradesOnlyFaceup();
            foreach(var upgrade in upgrades)
            {
                AddDecision(upgrade.Name, (s, e) => DiscardUpgrade(upgrade), upgrade.ImageUrl);
            }

            DefaultDecisionName = upgrades[0].Name;

            DecisionViewType = DecisionViewTypes.ImagesUpgrade;

            callBack();
        }

        protected void DiscardUpgrade(GenericUpgrade upgrade)
        {
            Sounds.PlayShipSound(GetRandomLine());
            upgrade.TryDiscard(ConfirmDecision);
        }

        protected string[] bobaLines = new[] { "BobaFettA1", "BobaFettA2", "BobaFettA3", "BobaFettA4", "BobaFettA5", };
        protected string GetRandomLine()
        {
            return bobaLines[UnityEngine.Random.Range(0, bobaLines.Length)];
        }

    }

}
