using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Arcs;
using Upgrade;
using Ship;
using System;
using UnityEngine;
using Ship.CardInfo;

namespace Ship
{
    namespace SecondEdition.TIEAdvancedX1
    {
        public class TIEAdvancedX1 : GenericShip
        {
            public TIEAdvancedX1() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "TIE Advanced x1",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, Type>
                        {
                            { Faction.Imperial, typeof(DarthVader) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 2), 3, 3, 2,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(BarrelRollAction)),
                        new ActionInfo(typeof(TargetLockAction))
                    ),
                    new ShipUpgradesInfo(),
                    linkedActions: new List<LinkedActionInfo>
                    {
                        new LinkedActionInfo(typeof(FocusAction), typeof(BarrelRollAction))
                    }
                );

                ShipAbilities.Add(new Abilities.SecondEdition.AdvancedTargetingComputer());

                ModelInfo = new ShipModelInfo
                (
                    "TIE Advanced",
                    "Gray",
                    new Vector3(-3.85f, 8f, 5.55f),
                    1.5f
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.TallonRoll, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.TallonRoll, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
                );

                SoundInfo = new ShipSoundInfo
                (
                    new List<string>()
                    {
                        "TIE-Fly1",
                        "TIE-Fly2",
                        "TIE-Fly3",
                        "TIE-Fly4",
                        "TIE-Fly5",
                        "TIE-Fly6",
                        "TIE-Fly7"
                    },
                    "TIE-Fire", 2
                );

                ShipIconLetter = 'A';
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you perform a primary attack against a defender you have locked, roll 1 additional attack die and change 1 hit result to a critical hit result.
    public class AdvancedTargetingComputer : GenericAbility
    {
        public override string Name { get { return "Advanced Targeting Computer"; } }

        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAbility;
            HostShip.Ai.OnGetActionPriority += IncreaseAILockPriority;

            AddDiceModification(
                "Advanced Targeting Computer",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide>() { DieSide.Success },
                DieSide.Crit
            );
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckAbility;
            HostShip.Ai.OnGetActionPriority -= IncreaseAILockPriority;
            RemoveDiceModification();
        }

        private int GetAiPriority()
        {
            return 100;
        }

        private void IncreaseAILockPriority(GenericAction action, ref int priority)
        {
            if (action is TargetLockAction && TargetLockAction.HasValidLockTargetsAndNoLockOnShipInRange(HostShip)) priority = 55;
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack
                && ActionsHolder.HasTargetLockOn(Combat.Attacker, Combat.Defender)
                && Combat.ChosenWeapon.WeaponType == WeaponTypes.PrimaryWeapon;
        }

        private void CheckAbility(ref int value)
        {
            if (IsAvailable())
            {
                Messages.ShowInfo(Combat.Attacker.PilotInfo.PilotName + "'s target lock and Advanced Targeting Computer grants them +1 attack die");
                value++;
            }
        }

        private void ModifyDice(DiceRoll diceroll)
        {
            HostShip.OnImmediatelyAfterRolling -= ModifyDice;
            if (diceroll.Change(DieSide.Success, DieSide.Crit, 1) > 0)
            {
                Messages.ShowInfo("Advanced Targeting Computer converts one Hit to a Critical Hit");
            }
        }
    }
}
