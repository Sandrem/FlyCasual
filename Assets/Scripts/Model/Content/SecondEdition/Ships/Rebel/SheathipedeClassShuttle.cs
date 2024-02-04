using System.Collections.Generic;
using Movement;
using ActionsList;
using Ship;
using System;
using Upgrade;
using Actions;
using Arcs;
using UnityEngine;
using Ship.CardInfo;

namespace Ship
{
    namespace SecondEdition.SheathipedeClassShuttle
    {
        public class SheathipedeClassShuttle : GenericShip
        {
            public SheathipedeClassShuttle() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "Sheathipede-class Shuttle",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, System.Type>
                        {
                            { Faction.Rebel, typeof(FennRau) }
                        }
                    ),
                    new ShipArcsInfo
                    (
                        new ShipArcInfo(ArcType.Front, 2),
                        new ShipArcInfo(ArcType.Rear, 2)
                    ),
                    2, 4, 1,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(CoordinateAction))
                    ),
                    new ShipUpgradesInfo()
                );

                ShipAbilities.Add(new Abilities.SecondEdition.CommsShuttle());

                ModelInfo = new ShipModelInfo
                (
                    "Sheathipede-class Shuttle",
                    "Phantom II",
                    new Vector3(-3.7f, 7.8f, 5.55f),
                    1.25f
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.ReverseStraight, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex)
                );

                SoundInfo = new ShipSoundInfo
                (
                    new List<string>()
                    {
                        "XWing-Fly1",
                        "XWing-Fly2",
                        "XWing-Fly3"
                    },
                    "XWing-Laser", 2
                );

                ShipIconLetter = '%';
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class CommsShuttle : GenericAbility
    {
        public override string Name => "Comms Shuttle";

        private bool CoordinateActionWasAdded;

        public override void ActivateAbility()
        {
            HostShip.OnDocked += ApplyAbility;
            HostShip.OnUndocked += RemoveAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnDocked -= ApplyAbility;
            HostShip.OnUndocked -= RemoveAbility;
        }

        private void ApplyAbility(GenericShip ship)
        {
            if (!ship.ActionBar.HasAction(typeof(CoordinateAction)))
            {
                CoordinateActionWasAdded = true;
                ship.ActionBar.AddGrantedAction(new CoordinateAction(), null);
            }

            HostShip.DockingHost.OnMovementActivationStart += RegisterFreeAction;
        }

        private void RegisterFreeAction(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnMovementActivationStart, AskFreeCoordinate);
        }

        private void AskFreeCoordinate(object sender, EventArgs e)
        {
            HostShip.DockingHost.AskPerformFreeAction(
                new CoordinateAction(),
                Triggers.FinishTrigger,
                descriptionShort: Name,
                descriptionLong: "Before your carrier ship activates, it can perfrom a Coordinate action"
            );
        }

        private void RemoveAbility(GenericShip ship)
        {
            if (CoordinateActionWasAdded)
            {
                CoordinateActionWasAdded = false;
                ship.ActionBar.RemoveGrantedAction(typeof(CoordinateAction), null);
            }

            HostShip.DockingHost.OnMovementActivationStart -= RegisterFreeAction;
        }
    }
}
