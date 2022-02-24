using Abilities.SecondEdition;
using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.TIEInterceptor
    {
        public class NashWindrider : TIEInterceptor
        {
            public NashWindrider() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Nash Windrider",
                    "Alderaanian Zealot",
                    Faction.Imperial,
                    2,
                    4,
                    6,
                    isLimited: true,
                    abilityType: typeof(NashWindriderAbility),
                    charges: 1,
                    regensCharges: 1,
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Missile,
                        UpgradeType.Modification
                    },
                    tags: new List<Tags>
                    {
                        Tags.Tie
                    },
                    skinName: "Skystrike Academy"
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/be/1b/be1bc184-02e3-4b23-9006-6c299ec2a7a6/swz84_pilot_nashwindrider.png";
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NashWindriderAbility : GenericAbility
    {
        GenericShip DestroyedShip { get; set; }

        public override void ActivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnShipIsDestroyedGlobal -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship, bool flag)
        {
            if (ConditionsAreMet(ship))
            {
                DestroyedShip = ship;
                RegisterAbilityTrigger(TriggerTypes.OnShipIsDestroyed, AskToChangeEngagementOrder);
            }
        }

        private bool ConditionsAreMet(GenericShip ship)
        {
            DistanceInfo distInfo = new DistanceInfo(HostShip, ship);
            return HostShip.State.Charges >= 1
                && Phases.CurrentPhase is MainPhases.CombatPhase
                && Tools.IsSameTeam(HostShip, ship)
                && ship.ShipInfo.BaseSize == BaseSize.Small
                && distInfo.Range <= 3
                && !ship.IsAttackPerformed;
        }

        private void AskToChangeEngagementOrder(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostShip.PilotInfo.PilotName,
                IfShipCanAttack,
                UseNashWindriderAbility,
                descriptionLong: $"Do you want to spend 1 charge to allow {DestroyedShip.PilotInfo.PilotName} engage at the current initiative?",
                imageHolder: HostShip,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void UseNashWindriderAbility(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            Messages.ShowInfo($"{HostShip.PilotInfo.PilotName}: {DestroyedShip.PilotInfo.PilotName} engages at the current initiative");
            HostShip.SpendCharge();
            DestroyedShip.State.CombatActivationAtInitiative = CombatSubPhase.CurrentInitiative;

            Triggers.FinishTrigger();
        }

        private bool IfShipCanAttack()
        {
            return ActionsHolder.HasTarget(DestroyedShip);
        }
    }
}