using System;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class Eta2Actis : GenericShip
    {
        public Eta2Actis() : base()
        {
            RequiredMods = new List<Type>() { typeof(Mods.ModsList.UnreleasedContentMod) };

            ShipInfo = new ShipCardInfo
            (
                "Eta-2 Actis",
                BaseSize.Small,
                Faction.Republic,
                new ShipArcsInfo(ArcType.Bullseye, 3), 3, 3, 0,
                new ShipActionsInfo(
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(EvadeAction)),
                    new ActionInfo(typeof(TargetLockAction), ActionColor.Purple),
                    new ActionInfo(typeof(BarrelRollAction)),
                    new ActionInfo(typeof(BoostAction))
                ),
                new ShipUpgradesInfo(
                    UpgradeType.Title,
                    UpgradeType.Modification,
                    UpgradeType.Configuration,
                    UpgradeType.Astromech
                )
            );

            ShipInfo.ArcInfo.Arcs.Add(new ShipArcInfo(ArcType.Front, 2));

            ShipAbilities.Add(new Abilities.SecondEdition.IntuitiveControlsAbility());

            IconicPilots = new Dictionary<Faction, System.Type> {
                { Faction.Republic, typeof(JediGeneral) }
            };

            ModelInfo = new ShipModelInfo(
                "Eta-2 Actis",
                "Yellow",
                new Vector3(-3.75f, 7.85f, 5.55f),
                1f
            );

            DialInfo = new ShipDialInfo(
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

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
            );

            SoundInfo = new ShipSoundInfo(
                new List<string>()
                {
                    "XWing-Fly1",
                    "XWing-Fly2",
                    "XWing-Fly3"
                },
                "XWing-Laser", 2
            );

            ShipIconLetter = '*';
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
