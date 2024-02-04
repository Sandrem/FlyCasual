using Abilities.SecondEdition;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship.CardInfo;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Ship.SecondEdition.LaatIGunship
{
    public class LaatIGunship : GenericShip
    {
        public LaatIGunship() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "LAAT/i Gunship",
                BaseSize.Medium,
                new FactionData
                (
                    new Dictionary<Faction, Type>
                    {
                        { Faction.Republic, typeof(Warthog) }
                    }
                ),
                new ShipArcsInfo(ArcType.DoubleTurret, 2), 1, 8, 2,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(ReinforceAction), ActionColor.Red),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(RotateArcAction)),
                    new ActionInfo(typeof(ReloadAction))
                ),
                new ShipUpgradesInfo()
            );

            ShipInfo.Charges = 2;
            ShipInfo.RegensCharges = 1;

            ShipAbilities.Add(new FireConvergenceAbility());

            ModelInfo = new ShipModelInfo
            (
                "LAAT Gunship",
                "212th Battalion",
                new Vector3(-4f, 8.65f, 5.55f),
                2.3f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex)
            );

            SoundInfo = new ShipSoundInfo
            (
                new List<string>()
                {
                    "Falcon-Fly1",
                    "Falcon-Fly2",
                    "Falcon-Fly3"
                },
                "Falcon-Fire", 2
            );

            ShipIconLetter = '/';
        }
    }
}

namespace Abilities.SecondEdition
{
    //When a friendly ship performs a non-turret attack, if the defender is in your turret arc, you may spend 1 charge.
    //If you do, the attacker rerolls up to 2 attack dice
    public class FireConvergenceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Fire Convergence (" + HostShip.ShipId + ")",
                IsAvailable,
                AiPriority,
                DiceModificationType.Reroll,
                2,
                isGlobal: true,
                payAbilityCost: SpendCharge
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        protected virtual bool IsAvailable()
        {
            return HostShip.State.Charges > 0
                && Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker.Owner == HostShip.Owner
                && !Combat.ArcForShot.IsTurretArc
                && HostShip.ArcsInfo.HasShipInTurretArc(Combat.Defender);
        }
        

        private int AiPriority()
        {
            return 81; //slightly higher than TL
        }

        private void SpendCharge(Action<bool> callback)
        {
            if (HostShip.State.Charges > 0)
            {
                HostShip.SpendCharge();
                callback(true);
            }
            else
                callback(false);
        }
    }
}