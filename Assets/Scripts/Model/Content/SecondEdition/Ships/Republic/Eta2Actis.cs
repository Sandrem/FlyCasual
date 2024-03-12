using System;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using Ship.CardInfo;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class Eta2Actis : GenericShip
    {
        public Eta2Actis() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "Eta-2 Actis",
                BaseSize.Small,
                new FactionData
                (
                    new Dictionary<Faction, Type>
                    {
                        { Faction.Republic, typeof(ObiWanKenobi) }
                    }
                ),
                new ShipArcsInfo
                (
                    new ShipArcInfo(ArcType.Front, 2),
                    new ShipArcInfo(ArcType.Bullseye, 3)
                ),
                3, 3, 0,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(EvadeAction)),
                    new ActionInfo(typeof(TargetLockAction), ActionColor.Purple),
                    new ActionInfo(typeof(BarrelRollAction)),
                    new ActionInfo(typeof(BoostAction))
                )
            );

            ShipAbilities.Add(new Abilities.SecondEdition.IntuitiveControlsAbility());

            ModelInfo = new ShipModelInfo
            (
                "Eta-2 Actis",
                "Yellow",
                new Vector3(-3.75f, 7.85f, 5.55f),
                1f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.TallonRoll, MovementComplexity.Purple),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.TallonRoll, MovementComplexity.Purple),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
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

            ShipIconLetter = '-';
        }
    }
}


namespace Abilities.SecondEdition
{
    public class IntuitiveControlsAbility : GenericAbility
    {
        public override string Name { get { return "Intuitive Controls"; } }

        public override void ActivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation += CheckAbility;
            HostShip.OnSystemsAbilityActivation += RegisterAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnCheckSystemsAbilityActivation -= CheckAbility;
            HostShip.OnSystemsAbilityActivation -= RegisterAbility;
        }

        private void CheckAbility(GenericShip ship, ref bool flag)
        {
            flag = true;
        }

        private void RegisterAbility(GenericShip ship)
        {
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToPerformPurpleReposition);
        }

        private void AskToPerformPurpleReposition(object sender, EventArgs e)
        {
            if (HostShip.State.Force > 0)
            {
                HostShip.AskPerformFreeAction(
                    new List<GenericAction>() {
                        new BarrelRollAction(){ HostShip = HostShip, Color = ActionColor.Purple },
                        new BoostAction(){ HostShip = HostShip, Color = ActionColor.Purple }
                    },
                    Triggers.FinishTrigger,
                    descriptionShort: "Intuitive Controls",
                    descriptionLong: "You may perform a purple barrel roll or boost action",
                    imageHolder: HostShip
                );
            }
            else
            {
                Messages.ShowError("No force tokens left");
                Triggers.FinishTrigger();
            }
        }
    }
}
