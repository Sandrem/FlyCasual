using Ship;
using SubPhases;
using SubPhases.SecondEdition;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class DiamondBoronMissiles : GenericSpecialWeapon
    {
        public DiamondBoronMissiles() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Diamond-Boron Missiles",
                types: new List<UpgradeType>(){
                    UpgradeType.Missile,
                    UpgradeType.Missile
                },
                cost: 5,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 2,
                    maxRange: 3,
                    requiresToken: typeof(BlueTargetLockToken),
                    charges: 3
                ),
                abilityType: typeof(Abilities.SecondEdition.DiamondBoronMissilesAbility),
                isLimited: true
            );
            
            ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/f1/c4/f1c4559f-1817-4256-a229-132b40d83ec9/swz41_diamond-boron-missiles.png";
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class DiamondBoronMissilesAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackHitAsAttacker += AskForAbilityUse;
        }
        public override void DeactivateAbility()
        {
            HostShip.OnAttackHitAsAttacker -= AskForAbilityUse;
        }

        private void AskForAbilityUse()
        {
            if (Combat.ChosenWeapon.GetType() == HostUpgrade.GetType() && HostUpgrade.State.Charges > 0)
            {
                var affectedPilots = BoardTools.Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1))
                    .Where(ship => ship.State.Agility <= Combat.Defender.State.Agility)
                    .Select(ship => ship.PilotInfo.IsLimited ? ship.PilotInfo.PilotName : $"{ship.PilotInfo.PilotName} ({ship.ShipId})")
                    .ToArray();
                
                var allButLast = affectedPilots.Take(affectedPilots.Length - 1)
                    .ToArray();
                var last = affectedPilots.LastOrDefault();
                var affectedPilotsText = last;
                if (allButLast.Any())
                {
                    affectedPilotsText = string.Join(", ", allButLast) + " and " + last;
                }                

                RegisterAbilityTrigger(TriggerTypes.OnShotHit,
                    (s, e) => AskToUseAbility(
                        HostUpgrade.UpgradeInfo.Name,
                        AiDecidesUse,
                        SpendChargeToMakeNearbyRollDamage,
                        descriptionLong: $"Do you want to spend 1 Charge? (If you do, {affectedPilotsText} roll 1 die for damage)",
                        imageHolder: HostUpgrade,
                        requiredPlayer: Combat.Attacker.Owner.PlayerNo,
                        callback: () => Triggers.ResolveTriggers(TriggerTypes.OnAttackHit, Triggers.FinishTrigger)                        
                    )
                );
            }
        }

        private bool AiDecidesUse()
        {
            var shipsAtRange = BoardTools.Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1));
            var friendlies = shipsAtRange.Where(ship => ship.Owner.PlayerNo == Combat.Attacker.Owner.PlayerNo);
            var enemies = shipsAtRange.Where(ship => ship.Owner.PlayerNo != Combat.Attacker.Owner.PlayerNo);
            var friendliesCount = friendlies.Count();
            var enemiesCount = enemies.Count();

            return friendliesCount < enemiesCount;            
        }

        private void SpendChargeToMakeNearbyRollDamage(object sender, EventArgs ev)
        {
            HostUpgrade.State.SpendCharge();

            var shipsAtRange = BoardTools.Board.GetShipsAtRange(Combat.Defender, new Vector2(0, 1))
                .Where(ship => ship.State.Agility <= Combat.Defender.State.Agility)
                .ToArray();

            foreach(var ship in shipsAtRange)
            {
                var currentShip = ship;
                var trigger = RegisterAbilityTrigger(TriggerTypes.OnAttackHit,
                    (s, e) => ShipDamageRollEffect(currentShip, Triggers.FinishTrigger)
                );
                trigger.Name = $"{HostUpgrade.UpgradeInfo.Name} damage {currentShip.PilotInfo.PilotName}";
            }

            SubPhases.DecisionSubPhase.ConfirmDecision();
        }

        private void ShipDamageRollEffect(GenericShip ship, Action callback)
        {            
            Selection.ActiveShip = ship;

            var subphase = Phases.StartTemporarySubPhaseNew<DiamondBoronMissilesCheckSubPhase>(
                $"{HostUpgrade.UpgradeInfo.Name}: {ship.PilotInfo.PilotName} rolls for damage.",
                delegate {
                    Phases.FinishSubPhase(typeof(DiamondBoronMissilesCheckSubPhase));
                    Triggers.FinishTrigger();                                        
                }
            );
            subphase.HostUpgrade = HostUpgrade;
            subphase.RequiredPlayer = ship.Owner.PlayerNo;
            subphase.Start();
        }
    }

}

namespace SubPhases.SecondEdition
{
    public class DiamondBoronMissilesCheckSubPhase : DiceRollCheckSubPhase
    {
        public GenericUpgrade HostUpgrade;

        public override void Prepare()
        {
            DiceKind = DiceKind.Attack;
            DiceCount = 1;

            AfterRoll = FinishAction;
        }

        protected override void FinishAction()
        {
            HideDiceResultMenu();

            CurrentDiceRoll.RemoveAllFailures();
            if (!CurrentDiceRoll.IsEmpty)
            {
                SufferDamage();
            }
            else
            {
                NoDamage();
            }

        }

        private void SufferDamage()
        {
            DamageSourceEventArgs diamondBoronDamage = new DamageSourceEventArgs()
            {
                Source = HostUpgrade,
                DamageType = DamageTypes.CardAbility
            };

            var damageType = string.Join(", ", CurrentDiceRoll.DiceList.Select(die => die.Side == DieSide.Crit ? "critical damage" : "damage"));
            Messages.ShowInfo($"Diamond-Boron Missiles: {Selection.ActiveShip.PilotInfo.PilotName} suffers {damageType}.");

            Selection.ActiveShip.Damage.TryResolveDamage(CurrentDiceRoll.DiceList, diamondBoronDamage, CallBack);
        }

        private void NoDamage()
        {
            Messages.ShowInfo($"Diamond-Boron Missiles: {Selection.ActiveShip.PilotInfo.PilotName} suffers no damage.");
            CallBack();
        }
    }
}
