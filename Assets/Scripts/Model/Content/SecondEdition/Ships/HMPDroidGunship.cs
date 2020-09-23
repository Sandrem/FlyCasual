using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.HMPDroidGunship
{
    public class HMPDroidGunship : GenericShip
    {
        public HMPDroidGunship() : base()
        {
            ShipInfo = new ShipCardInfo
            (
                "HMP Droid Gunship",
                BaseSize.Small,
                Faction.Separatists,
                new ShipArcsInfo(ArcType.FullFront, 2), 1, 5, 3,
                new ShipActionsInfo(
                    new ActionInfo(typeof(CalculateAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction), ActionColor.Red),
                    new ActionInfo(typeof(ReloadAction))
                ),
                new ShipUpgradesInfo(
                    UpgradeType.Title,
                    UpgradeType.Missile,
                    UpgradeType.Missile,
                    UpgradeType.TacticalRelay,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                )
            );

            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(ReloadAction), typeof(CalculateAction), ActionColor.Red));

            ShipAbilities.Add(new Abilities.SecondEdition.NetworkedAimAbility());

            IconicPilots = new Dictionary<Faction, System.Type> {
                { Faction.Separatists, typeof(BaktoidDrone) }
            };

            ModelInfo = new ShipModelInfo(
                "HMP Droid Gunship",
                "Default",
                new Vector3(-3.62f, 7.62f, 5.55f),
                1.5f
            );

            DialInfo = new ShipDialInfo(
                new ManeuverInfo(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Easy),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex)
            );

            SoundInfo = new ShipSoundInfo(
                new List<string>()
                {
                    "XWing-Fly1",
                    "XWing-Fly2",
                    "XWing-Fly3"
                },
                "XWing-Laser", 3
            );
        }
    }
}


namespace Abilities.SecondEdition
{
    //You cannot spend your locks to reroll attack dice.
    //While you perform an attack, you may reroll a number of attack dice up to the number of friendly locks on the defender
    public class NetworkedAimAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnAttackStartAsAttacker += SetTargetLockCannotBeUsed;
            HostShip.OnAttackFinishAsAttacker += SetTargetLockCanBeUsed;

            AddDiceModification(
                "Networked Aim",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Reroll,
                GetRerollCount);
        }

        public override void DeactivateAbility()
        {
            HostShip.OnAttackStartAsAttacker -= SetTargetLockCannotBeUsed;
            HostShip.OnAttackFinishAsAttacker -= SetTargetLockCanBeUsed;

            RemoveDiceModification();
        }

        private int GetRerollCount()
        {
            return Combat.Defender.Tokens
                .GetTokens<RedTargetLockToken>('*')
                .Count(token => (token.OtherTargetLockTokenOwner as GenericShip)?.Owner == HostShip.Owner);
        }

        private int GetAiPriority()
        {
            return 90;
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && Combat.Attacker == HostShip
                && GetRerollCount() > 0;
        }

        private void SetTargetLockCanBeUsed(GenericShip ship)
        {
            HostShip.Tokens.GetTokens<BlueTargetLockToken>('*').ForEach(targetLock => targetLock.CanBeUsed = true);
        }

        private void SetTargetLockCannotBeUsed()
        {
            HostShip.Tokens.GetTokens<BlueTargetLockToken>('*').ForEach(targetLock => targetLock.CanBeUsed = false);
        }

    }
}
