using Content;
using Ship;
using System.Collections;
using System.Collections.Generic;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.KihraxzFighter
    {
        public class CaptainJostero : KihraxzFighter
        {
            public CaptainJostero() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Captain Jostero",
                    "Aggressive Opportunist",
                    Faction.Scum,
                    3,
                    4,
                    12,
                    isLimited: true,
                    abilityType: typeof(Abilities.SecondEdition.CaptainJosteroAbility),
                    extraUpgradeIcons: new List<UpgradeType>()
                    {
                        UpgradeType.Missile,
                        UpgradeType.Illicit,
                        UpgradeType.Illicit,
                        UpgradeType.Modification,
                        UpgradeType.Modification
                    },
                    seImageNumber: 194,
                    legality: new List<Legality>() { Legality.ExtendedLegal }
                );
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CaptainJosteroAbility : GenericAbility
    {
        private bool performedRegularAttack;

        public override void ActivateAbility()
        {
            GenericShip.OnDamageInstanceResolvedGlobal += CheckJosteroAbility;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnDamageInstanceResolvedGlobal -= CheckJosteroAbility;
        }

        private void CheckJosteroAbility(GenericShip damaged, DamageSourceEventArgs damage)
        {
            // Can we even bonus attack?
            if (HostShip.IsCannotAttackSecondTime)
                return;

            // Make sure the opposing ship is an enemy.
            if (damaged.Owner == HostShip.Owner)
                return;

            // If the ship is defending we're not interested.
            if (Combat.Defender == damaged || damage.DamageType == DamageTypes.ShipAttack)
                return;

            // Save the value for whether we've attacked or not.
            performedRegularAttack = HostShip.IsAttackPerformed;

            TargetShip = damaged;

            // It may be possible in the future for a non-defender to be damaged in combat so we've got to future proof here.
            if (Combat.AttackStep == CombatStep.None)
            {
                RegisterAbilityTrigger(TriggerTypes.OnDamageInstanceResolved, RegisterBonusAttack);
            }
            else
            {
                Combat.Attacker.OnCombatCheckExtraAttack += StartBonusAttack;
            }
        }

        private void StartBonusAttack(GenericShip ship)
        {
            ship.OnCombatCheckExtraAttack -= StartBonusAttack;
            RegisterAbilityTrigger(TriggerTypes.OnCombatCheckExtraAttack, RegisterBonusAttack);
        }

        private void RegisterBonusAttack(object sender, System.EventArgs e)
        {
            HostShip.StartBonusAttack(CleanupBonusAttack, IsTargetShip);
        }

        private bool IsTargetShip(GenericShip defender, IShipWeapon weapon, bool isSilent)
        {
            if (defender == TargetShip)
            {
                return true;
            }
            else
            {
                if (!isSilent) Messages.ShowErrorToHuman("Your bonus attack must be against the ship that just suffered damage");
                return false;
            }
        }

        private void CleanupBonusAttack()
        {
            // Restore previous value of "has already attacked" flag
            HostShip.IsAttackPerformed = performedRegularAttack;

            // Restore ship selection
            Selection.ChangeActiveShip(TargetShip);

            Triggers.FinishTrigger();
        }
    }
}

