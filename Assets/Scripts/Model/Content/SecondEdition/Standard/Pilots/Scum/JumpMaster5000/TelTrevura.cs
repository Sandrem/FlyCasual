using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.JumpMaster5000
    {
        public class TelTrevura : JumpMaster5000
        {
            public TelTrevura() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Tel Trevura",
                    "Escape Artist",
                    Faction.Scum,
                    4,
                    6,
                    20,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.TelTrevuraAbility),
                    charges: 1,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Cannon,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Modification
                    },
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
            HostShip.OnCheckPreventDestruction += ActivateAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckPreventDestruction -= ActivateAbility;
        }

        private void ActivateAbility(GenericShip ship, ref bool preventDestruction)
        {
            if (HostShip.State.Charges > 0)
            {
                HostShip.SpendCharge();

                Messages.ShowInfo(HostShip.PilotInfo.PilotName + " has prevented his own destruction");

                HostShip.OnCheckPreventDestruction -= ActivateAbility;
                preventDestruction = true;
                HostShip.IsDestroyed = false;
                Roster.MoveToReserve(HostShip);

                Phases.Events.OnPlanningPhaseStart += RegisterSetup;
            }
        }

        private void RegisterSetup()
        {
            Phases.Events.OnPlanningPhaseStart -= RegisterSetup;

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
                    Messages.ShowInfo(HostShip.PilotInfo.PilotName + " has returned to the play area");
                    Triggers.FinishTrigger();
                }
            );

            subphase.ShipToSetup = HostShip;
            subphase.SetupSide = (HostShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? Direction.Bottom : Direction.Top;
            subphase.DescriptionShort = HostShip.PilotInfo.PilotName;
            subphase.DescriptionLong = "Place yourself within range 1 of your player edge.";
            subphase.ImageSource = HostShip;

            subphase.Start();
        }
    }
}