using Ship;
using SubPhases;
using System;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.JumpMaster5000
    {
        public class TelTrevura : JumpMaster5000
        {
            public TelTrevura() : base()
            {
                PilotInfo = new PilotCardInfo(
                    "Tel Trevura",
                    4,
                    50,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TelTrevuraAbility),
                    charges: 1,
                    extraUpgradeIcon: UpgradeType.Talent,
                    seImageNumber: 216
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TelTrevuraAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnReadyToBeDestroyed += ActivateAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnReadyToBeDestroyed -= ActivateAbility;
        }

        private void ActivateAbility(GenericShip ship)
        {
            if (HostShip.State.Charges > 0)
            {
                Messages.ShowInfo(HostShip.PilotInfo.PilotName + ": Destruction is prevented");

                HostShip.SpendCharge();

                HostShip.OnReadyToBeDestroyed -= ActivateAbility;

                HostShip.PreventDestruction = true;

                Roster.MoveToReserve(HostShip);

                Phases.Events.OnPlanningPhaseStart += RegisterSetup;
            }
        }

        private void RegisterSetup()
        {
            Phases.Events.OnPlanningPhaseStart -= RegisterSetup;

            HostShip.PreventDestruction = false;

            RegisterAbilityTrigger(TriggerTypes.OnPlanningSubPhaseStart, RestoreAndSetup);
        }

        private void RestoreAndSetup(object sender, System.EventArgs e)
        {
            RestoreShip(SetupShip);
        }

        private void RestoreShip(Action callback)
        {
            HostShip.Damage.RemoveAllDamage();

            DamageSourceEventArgs damageArgs = new DamageSourceEventArgs()
            {
                Source = HostShip,
                DamageType = DamageTypes.CardAbility
            };

            HostShip.Damage.TryResolveDamage(5, damageArgs, callback);
        }

        private void SetupShip()
        {
            Roster.ReturnFromReserve(HostShip);

            var subphase = Phases.StartTemporarySubPhaseNew<SetupShipMidgameSubPhase>(
                "Setup",
                delegate {
                    Messages.ShowInfo(HostShip.PilotInfo.PilotName + " returned to the play area");
                    Triggers.FinishTrigger();
                }
            );

            subphase.ShipToSetup = HostShip;
            subphase.SetupSide = (HostShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? Direction.Bottom : Direction.Top;
            subphase.AbilityName = HostShip.PilotInfo.PilotName;
            subphase.Description = "Place yourself within range 1 of your player edge";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }
    }
}