using BoardTools;
using Content;
using Ship;
using System;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.JumpMaster5000
    {
        public class Dengar : JumpMaster5000
        {
            public Dengar() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Dengar",
                    "Vengeful Corellian",
                    Faction.Scum,
                    6,
                    7,
                    22,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.DengarPilotAbility),
                    charges: 1,
                    regensCharges: 1,
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent,
                        UpgradeType.Crew,
                        UpgradeType.Gunner,
                        UpgradeType.Modification,
                        UpgradeType.Title
                    },
                    tags: new List<Tags>
                    {
                        Tags.BountyHunter
                    },
                    seImageNumber: 214
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DengarPilotAbility : Abilities.FirstEdition.DengarPilotAbility
    {
        protected override bool CanCounterattackUsingShotInfo(ShotInfo counterAttackInfo)
        {
            return HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, Arcs.ArcType.Front);
        }

        protected override bool CanUseAbility()
        {
            return HostShip.State.Charges > 0;
        }

        protected override void MarkAbilityAsUsed()
        {
            HostShip.SpendCharge();
        }
    }
}

namespace Abilities.FirstEdition
{
    public class DengarPilotAbility : GenericAbility
    {
        private GenericShip shipToPunish;
        private bool isPerformedRegularAttack;

        public override void ActivateAbility()
        {
            HostShip.OnAttackFinishAsDefender += CheckAbility;
            Phases.Events.OnRoundEnd += ClearIsAbilityUsedFlag;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackFinishAsDefender -= CheckAbility;
            Phases.Events.OnRoundEnd -= ClearIsAbilityUsedFlag;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (!CanUseAbility()) return;

            if (HostShip.IsCannotAttackSecondTime) return;

            ShotInfo counterAttackInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapons);
            if (!CanCounterattackUsingShotInfo(counterAttackInfo)) return;

            // Save his attacker, becuase combat data will be cleared
            shipToPunish = Combat.Attacker;

            Combat.Attacker.OnCombatCheckExtraAttack += RegisterAbility;
        }

        protected virtual bool CanUseAbility()
        {
            return !IsAbilityUsed;
        }

        protected virtual bool CanCounterattackUsingShotInfo(ShotInfo counterAttackInfo)
        {
            return counterAttackInfo.InArc;
        }

        protected virtual void MarkAbilityAsUsed()
        {
            IsAbilityUsed = true;
        }

        private void RegisterAbility(GenericShip ship)
        {
            ship.OnCombatCheckExtraAttack -= RegisterAbility;

            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, DoCounterAttack);
        }

        private void DoCounterAttack(object sender, EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                // Save his "is already attacked" flag
                isPerformedRegularAttack = HostShip.IsAttackPerformed;

                // Plan to set IsAbilityUsed only after attack that was successfully started
                HostShip.OnAttackStartAsAttacker += MarkAbilityAsUsed;

                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    FinishExtraAttack,
                    CounterAttackFilter,
                    HostShip.PilotInfo.PilotName,
                    "You may perform an additional attack against " + shipToPunish.PilotInfo.PilotName,
                    HostShip
                );
            }
            else
            {
                Messages.ShowErrorToHuman(string.Format("{0} cannot attack an additional time", HostShip.PilotInfo.PilotName));
                Triggers.FinishTrigger();
            }
        }

        private void FinishExtraAttack()
        {
            // Restore previous value of "is already attacked" flag
            HostShip.IsAttackPerformed = isPerformedRegularAttack;

            // Set IsAbilityUsed only after attack that was successfully started
            HostShip.OnAttackStartAsAttacker -= MarkAbilityAsUsed;

            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;

            Triggers.FinishTrigger();
        }

        private bool CounterAttackFilter(GenericShip targetShip, IShipWeapon weapon, bool isSilent)
        {
            bool result = true;

            if (targetShip != shipToPunish)
            {
                if (!isSilent) Messages.ShowErrorToHuman(string.Format("{0} can only attack {1}", HostShip.PilotInfo.PilotName, shipToPunish.PilotInfo.PilotName));
                result = false;
            }

            return result;
        }

    }
}