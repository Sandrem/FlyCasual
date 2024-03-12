using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship.CardInfo;
using System.Collections.Generic;
using UnityEngine;

namespace Ship.SecondEdition.BTLBYWing
{
    public class BTLBYWing : GenericShip
    {
        public BTLBYWing() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "BTL-B Y-wing",
                BaseSize.Small,
                new FactionData
                (
                    new Dictionary<Faction, System.Type>
                    {
                        { Faction.Republic, typeof(ShadowSquadronVeteran) }
                    }
                ),
                new ShipArcsInfo(ArcType.Front, 2), 1, 5, 3,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction), ActionColor.Red),
                    new ActionInfo(typeof(ReloadAction), ActionColor.Red)
                ),
                new ShipUpgradesInfo(),
                abilityText: "<b>Plated Hull:</b> While you defend, if you are not critically damaged, change 1 crit result to a hit result."
            );

            ModelInfo = new ShipModelInfo
            (
                "BTL-B Y-wing",
                "Yellow",
                new Vector3(-4f, 7.9f, 5.55f),
                1.75f
            );

            DialInfo = new ShipDialInfo
            (
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

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
            );

            SoundInfo = new ShipSoundInfo
            (
                new List<string>()
                {
                    "YWing-Fly1",
                    "YWing-Fly2"
                },
                "XWing-Laser", 3
            );

            ShipAbilities.Add(new Abilities.SecondEdition.PlatedHull());

            ShipIconLetter = ':';
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PlatedHull : GenericAbility
    {
        // TODO: You MUST use this ability

        public override string Name { get { return "Plated Hull"; } }

        public override void ActivateAbility()
        {
            AddDiceModification(
                "Plated Hull",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide>() { DieSide.Crit },
                DieSide.Success,
                DiceModificationTimingType.Opposite
            );
        }

        private bool IsAvailable()
        {
            return Combat.AttackStep == CombatStep.Attack && !HostShip.Damage.HasFaceupCards && Combat.DiceRollAttack.CriticalSuccesses > 0;
        }

        private int GetAiPriority()
        {
            return 100;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }
    }
}
