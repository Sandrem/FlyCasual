using Actions;
using ActionsList;
using Arcs;
using BoardTools;
using Ship;
using SubPhases;
using System;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class UnderslungBlasterCannon : GenericSpecialWeapon
    {
        public UnderslungBlasterCannon() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Underslung Blaster Cannon",
                UpgradeType.Cannon,
                cost: 2,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 2,
                    minRange: 1,
                    maxRange: 1,
                    arc: ArcType.SingleTurret,
                    requiresToken: typeof(BlueTargetLockToken),
                    noRangeBonus: true
                ),
                addArc: new ShipArcInfo(ArcType.SingleTurret),
                addAction: new ActionInfo(typeof(RotateArcAction)),
                restrictions: new UpgradeCardRestrictions(
                    new FactionRestriction(Faction.Resistance),
                    new ShipRestriction(typeof(Ship.SecondEdition.T70XWing.T70XWing))
                ),
                abilityType: typeof(Abilities.SecondEdition.UnderslungBlasterCannonAbility)
            );

            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/b5/6d/b56db2ed-dca8-4cdf-8fa0-a8e35d27ae2b/swz68_underslung-blaster-cannon.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class UnderslungBlasterCannonAbility : GenericAbility
    {
        private GenericShip blasterTarget = null;

        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += AddBlasterRestriction;
            GenericShip.OnMovementFinishGlobal += CheckBlasterAbility;
            Phases.Events.OnActivationPhaseEnd_Triggers += CleanUpBlasterAbility;
            Phases.Events.OnCombatPhaseEnd_Triggers += CleanUpBlasterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= AddBlasterRestriction;
            GenericShip.OnMovementFinishGlobal -= CheckBlasterAbility;
            Phases.Events.OnActivationPhaseEnd_Triggers -= CleanUpBlasterAbility;
            Phases.Events.OnCombatPhaseEnd_Triggers -= CleanUpBlasterAbility;
        }

        private void AddBlasterRestriction()
        {
            if (Combat.Attacker.ShipId == HostShip.ShipId && Combat.ChosenWeapon.GetType() == HostUpgrade.GetType())
            {
                Combat.Attacker.OnTryAddAvailableDiceModification += BlasterRestrictionForAttacker;
                Combat.Attacker.OnAttackFinish += RemoveBlasterRestrictionForAttacker;

                Combat.Defender.OnTryAddAvailableDiceModification += BlasterRestrictionForDefender;
                Combat.Defender.OnAttackFinish += RemoveBlasterRestrictionForDefender;
            }
        }

        protected virtual void BlasterRestrictionForDefender(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (action.DiceModificationTiming == DiceModificationTimingType.Opposite)
            {
                Messages.ShowErrorToHuman("Underslung Blaster Cannon: You cannot modify attack dice");
                canBeUsed = false;
            }
        }

        protected virtual void BlasterRestrictionForAttacker(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (action.DiceModificationTiming != DiceModificationTimingType.Opposite && action.DiceModificationName != "Target Lock")
            {
                Messages.ShowErrorToHuman("Underslung Blaster Cannon: You cannot modify your attack dice in another ways");
                canBeUsed = false;
            }
        }

        private void RemoveBlasterRestrictionForDefender(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= BlasterRestrictionForDefender;
            ship.OnAttackFinish -= RemoveBlasterRestrictionForDefender;
        }

        private void RemoveBlasterRestrictionForAttacker(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification -= BlasterRestrictionForAttacker;
            ship.OnAttackFinish -= RemoveBlasterRestrictionForAttacker;
        }

        private void CleanUpBlasterAbility()
        {
            ClearIsAbilityUsedFlag();
            blasterTarget = null;
            HostShip.IsAttackPerformed = false;
            HostShip.IsAttackSkipped = false;
            HostShip.IsCannotAttackSecondTime = false;
        }

        public void AfterFiresightAttackSubPhase()
        {
            HostShip.IsAttackPerformed = true;
            //if bonus attack was skipped, allow bonus attacks again
            if (HostShip.IsAttackSkipped) HostShip.IsCannotAttackSecondTime = false;
            HostShip.OnAttackFinishAsAttacker -= SetIsAbilityIsUsed;
            Selection.ChangeActiveShip(blasterTarget);
            Phases.FinishSubPhase(Phases.CurrentSubPhase.GetType());
            Triggers.FinishTrigger();
        }

        private void CheckBlasterAbility(GenericShip ship)
        {
            if (!IsAbilityUsed && ship.Owner.PlayerNo != HostShip.Owner.PlayerNo)
            {
                blasterTarget = ship;

                ShotInfo shotInfo = new ShotInfo(HostShip, blasterTarget, (HostUpgrade as IShipWeapon));

                if (shotInfo.Range <= (HostUpgrade as IShipWeapon).WeaponInfo.MaxRange &&
                    shotInfo.Range >= (HostUpgrade as IShipWeapon).WeaponInfo.MinRange &&
                    shotInfo.IsShotAvailable)
                {
                    RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskBlasterAbility);
                }
            }
        }

        private void AskBlasterAbility(object sender, System.EventArgs e)
        {
            Selection.ChangeActiveShip(HostShip);

            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                AlwaysUseByDefault,
                PerformBlasterShot,
                dontUseAbility: CancelBlasterShot,
                callback: delegate {
                    Selection.ChangeActiveShip(blasterTarget);
                    Triggers.FinishTrigger();
                },
                descriptionLong: "Do you want to perform \"Underslung Blaster Cannon\" attack against " + blasterTarget.PilotInfo.PilotName + "?",
                imageHolder: HostUpgrade
            );
        }

        private void CancelBlasterShot(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecision();
        }

        private bool ForesightAttackFilter(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            bool result = true;
            if (defender != blasterTarget)
            {
                if (!isSilent) Messages.ShowErrorToHuman(
                    string.Format("Underslung Blaster Cannon's target must be {0}", blasterTarget.PilotInfo.PilotName));
                result = false;
            }
            else if (!(weapon.GetType() == HostUpgrade.GetType()))
            {
                if (!isSilent) Messages.ShowErrorToHuman("This attack must be Underslung Blaster Cannon attack");
                result = false;
            }

            return result;
        }

        public void ForesightSetUsed()
        {
            IsAbilityUsed = true;
        }

        private void PerformBlasterShot(object sender, EventArgs e)
        {
            if (!HostShip.IsCannotAttackSecondTime)
            {
                HostShip.OnAttackFinishAsAttacker += SetIsAbilityIsUsed;
                HostShip.IsCannotAttackSecondTime = true;

                Combat.StartSelectAttackTarget(
                    HostShip,
                    AfterFiresightAttackSubPhase,
                    ForesightAttackFilter,
                    HostUpgrade.UpgradeInfo.Name,
                    "You may perform a bonus Underslung Blaster Cannon attack against " + blasterTarget.PilotInfo.PilotName,
                    HostUpgrade
                );
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }
    }
}