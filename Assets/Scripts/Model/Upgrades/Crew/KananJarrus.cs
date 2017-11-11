using Ship;
using Upgrade;
using System;
using SubPhases;
using UpgradesList;
using Tokens;

namespace UpgradesList
{ 
    public class KananJarrus : GenericUpgrade
    {
        public bool IsAbilityUsed;

        public KananJarrus() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Kanan Jarrus";
            Cost = 3;

            isUnique = true;
        }

        public override bool IsAllowedForShip(GenericShip ship)
        {
            return ship.faction == Faction.Rebels;
        }

        public override void AttachToShip(GenericShip host)
        {
            base.AttachToShip(host);

            GenericShip.OnMovementFinishGlobal += CheckAbility;
            Phases.OnRoundEnd += ResetKananJarrusAbilityFlag;

            host.OnDestroyed += RemoveKananJarrusAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (!IsAbilityUsed && ship.Owner.PlayerNo == Host.Owner.PlayerNo && ship.AssignedManeuver.ColorComplexity == Movement.ManeuverColor.White)
            {
                Board.ShipDistanceInformation distanceInfo = new Board.ShipDistanceInformation(Host, ship);
                if (distanceInfo.Range < 3)
                {
                    Triggers.RegisterTrigger(new Trigger()
                    {
                        Name = "Kanan Jarrus's ability",
                        TriggerType = TriggerTypes.OnShipMovementFinish,
                        TriggerOwner = Host.Owner.PlayerNo,
                        EventHandler = AskKananJarrusAbility,
                        EventArgs = new KananJarrusAbilityArgs()
                        {
                            KananJarrusUpgradeCard = this,
                            ShipToRemoveStress = ship
                        }
                    });
                }
            }
        }

        private class KananJarrusAbilityArgs: EventArgs
        {
            public KananJarrus KananJarrusUpgradeCard;
            public GenericShip ShipToRemoveStress;
        }

        private void AskKananJarrusAbility(object sender, System.EventArgs e)
        {
            if ((e as KananJarrusAbilityArgs).ShipToRemoveStress.HasToken(typeof(StressToken)))
            {
                KananJarrusDecisionSubPhase newSubphase = (KananJarrusDecisionSubPhase)Phases.StartTemporarySubPhaseNew("Remove stress from ship?", typeof(KananJarrusDecisionSubPhase), Triggers.FinishTrigger);
                newSubphase.KananJarrusUpgradeCard = (e as KananJarrusAbilityArgs).KananJarrusUpgradeCard;
                newSubphase.ShipToRemoveStress = (e as KananJarrusAbilityArgs).ShipToRemoveStress;
                newSubphase.Start();
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private void ResetKananJarrusAbilityFlag()
        {
            IsAbilityUsed = false;
        }

        private void RemoveKananJarrusAbility(GenericShip ship)
        {
            GenericShip.OnMovementFinishGlobal -= CheckAbility;
            Phases.OnRoundEnd -= ResetKananJarrusAbilityFlag;
        }
    }
}

namespace SubPhases
{

    public class KananJarrusDecisionSubPhase : DecisionSubPhase
    {
        public KananJarrus KananJarrusUpgradeCard;
        public GenericShip ShipToRemoveStress;

        public override void PrepareDecision(Action callBack)
        {
            InfoText = "Remove Stress token from ship?";

            AddDecision("Yes", RemoveStress);
            AddDecision("No", DontRemoveStress);

            DefaultDecision = "No";

            callBack();
        }

        private void RemoveStress(object sender, EventArgs e)
        {
            ShipToRemoveStress.RemoveToken(typeof(StressToken));
            KananJarrusUpgradeCard.IsAbilityUsed = true;

            ConfirmDecision();
        }

        private void DontRemoveStress(object sender, EventArgs e)
        {
            ConfirmDecision();
        }

    }

}
