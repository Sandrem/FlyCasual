using System.Collections.Generic;
using ActionsList;
using Upgrade;
using Actions;
using System;
using Ship.CardInfo;
using Arcs;
using Movement;

namespace Ship
{
    namespace SecondEdition.FangFighter
    {
        public class FangFighter : GenericShip
        {
            public FangFighter() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "Fang Fighter",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, Type>
                        {
                            { Faction.Scum, typeof(FennRau) },
                            { Faction.Rebel, typeof(ClanWrenVolunteer) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 3), 3, 4, 0,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(BarrelRollAction)),
                        new ActionInfo(typeof(BoostAction))
                    ),
                    new ShipUpgradesInfo(),
                    linkedActions: new List<LinkedActionInfo>
                    {
                        new LinkedActionInfo(typeof(BarrelRollAction), typeof(FocusAction)),
                        new LinkedActionInfo(typeof(BoostAction), typeof(FocusAction))
                    }
                );

                ModelInfo = new ShipModelInfo
                (
                    "Protectorate Starfighter",
                    "Zealous Recruit"
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.TallonRoll, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.TallonRoll, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

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
                    "TIE-Fire", 3
                );

                ShipIconLetter = 'M';

                ShipAbilities.Add(new Abilities.SecondEdition.ConcordiaFaceoffAbility());
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    // While you defend, if the attack range is 1 and you are in the attacker's forward firing arc,
    // change 1 result to an evade result.

    public class ConcordiaFaceoffAbility : GenericAbility
    {
        public override string Name { get { return "Concordia Faceoff"; } }

        public override void ActivateAbility()
        {
            AddDiceModification(
                "Concordia Faceoff",
                IsAvailable,
                AiPriority,
                DiceModificationType.Change,
                1,
                sideCanBeChangedTo: DieSide.Success
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        public bool IsAvailable()
        {
            return
            (
                Combat.AttackStep == CombatStep.Defence &&
                Combat.Defender == HostShip &&
                Combat.ShotInfo.Range == 1 &&
                Combat.Attacker.SectorsInfo.IsShipInSector(HostShip, Arcs.ArcType.Front)
            );
        }

        public int AiPriority()
        {
            //TODO: Change to enum
            return 100;
        }
    }
}