using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship.CardInfo;
using System.Collections.Generic;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.UpsilonClassCommandShuttle
    {
        public class UpsilonClassCommandShuttle : GenericShip
        {
            public UpsilonClassCommandShuttle() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "Upsilon-class Command Shuttle",
                    BaseSize.Large,
                    new FactionData
                    (
                        new Dictionary<Faction, System.Type>
                        {
                            { Faction.FirstOrder, typeof(LieutenantDormitz) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 4), 1, 6, 6,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(CoordinateAction)),
                        new ActionInfo(typeof(ReinforceAction)),
                        new ActionInfo(typeof(JamAction))
                    ),
                    new ShipUpgradesInfo(),
                    legality: new List<Content.Legality>() { Content.Legality.ExtendedLegal }
                );

                ModelInfo = new ShipModelInfo
                (
                    "Upsilon-class Shuttle",
                    "Upsilon-class Shuttle",
                    new Vector3(-3.7f, 10.11f, 5.55f),
                    4f
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed0, ManeuverDirection.Stationary, ManeuverBearing.Stationary, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex)
                );

                SoundInfo = new ShipSoundInfo
                (
                    new List<string>()
                    {
                        "Slave1-Fly1",
                        "Slave1-Fly2"
                    },
                    "Slave1-Fire", 4
                );

                ShipIconLetter = 'U';

                ShipAbilities.Add(new Abilities.SecondEdition.LinkedBattery());
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class LinkedBattery : GenericAbility
    {
        public override string Name { get { return "Linked Battery"; } }

        public override void ActivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice += CheckAddDice;
        }

        public override void DeactivateAbility()
        {
            HostShip.AfterGotNumberOfAttackDice -= CheckAddDice;
        }

        private void CheckAddDice(ref int count)
        {
            if (Combat.ChosenWeapon is GenericSpecialWeapon && (Combat.ChosenWeapon as GenericSpecialWeapon).HasType(UpgradeType.Cannon)) count++;
        }
    }
}