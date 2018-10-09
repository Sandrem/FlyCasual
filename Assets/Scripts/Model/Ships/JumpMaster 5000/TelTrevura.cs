using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Abilities;
using System;
using Ship;
using BoardTools;
using RuleSets;
using SubPhases;

namespace Ship
{
    namespace JumpMaster5000
    {
        public class TelTrevura : JumpMaster5000, ISecondEditionPilot
        {
            public TelTrevura() : base()
            {
                PilotName = "Tel Trevura";
                PilotSkill = 4;
                Cost = 60;

                IsUnique = true;

                UsesCharges = true;
                MaxCharges = 1;

                PrintedUpgradeIcons.Add(Upgrade.UpgradeType.Elite);

                PilotAbilities.Add(new Abilities.SecondEdition.TelTrevuraAbilitySE());

                SEImageNumber = 216;
            }

            public void AdaptPilotToSecondEdition()
            {
                
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class TelTrevuraAbilitySE : GenericAbility
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
            if (HostShip.Charges > 0)
            {
                Messages.ShowInfo(HostShip.PilotName + ": Destruction is prevented");

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
                    Messages.ShowInfo(HostShip.PilotName + " returned to the play area");
                    Triggers.FinishTrigger();
                }
            );

            subphase.ShipToSetup = HostShip;
            subphase.SetupSide = (HostShip.Owner.PlayerNo == Players.PlayerNo.Player1) ? Direction.Bottom : Direction.Top;
            subphase.AbilityName = HostShip.PilotName;
            subphase.Description = "Place yourself within range 1 of your player edge";
            subphase.ImageUrl = HostShip.ImageUrl;

            subphase.Start();
        }
    }
}