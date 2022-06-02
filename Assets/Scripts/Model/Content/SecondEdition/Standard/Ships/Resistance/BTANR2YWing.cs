using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;
using Arcs;
using UnityEngine;
using System;
using Ship.CardInfo;

namespace Ship.SecondEdition.BTANR2YWing
{
    public class BTANR2YWing : GenericShip
    {
        public BTANR2YWing() : base()
        {
            ShipInfo = new ShipCardInfo25
            (
                "BTA-NR2 Y-wing",
                BaseSize.Small,
                new FactionData
                (
                    new Dictionary<Faction, Type>
                    {
                        { Faction.Resistance, typeof(CaiThrenalli) }
                    }
                ),
                new ShipArcsInfo(ArcType.Front, 2), 1, 4, 3,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction), ActionColor.Red),
                    new ActionInfo(typeof(BarrelRollAction), ActionColor.Red),
                    new ActionInfo(typeof(BoostAction), ActionColor.Red)
                ),
                new ShipUpgradesInfo
                (
                    UpgradeType.Tech,
                    UpgradeType.Turret,
                    UpgradeType.Astromech,
                    UpgradeType.Device,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                )
            );

            ModelInfo = new ShipModelInfo
            (
                "BTA-NR2 Y-wing",
                "Blue",
                new Vector3(-4f, 7.9f, 5.55f),
                1.75f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
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

            ShipAbilities.Add(new Abilities.SecondEdition.IntuitiveInterfaceAbility());

            ShipIconLetter = ' ';
        }
    }
}

namespace Abilities.SecondEdition
{
    public class IntuitiveInterfaceAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnActionIsPerformed += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnActionIsPerformed -= CheckAbility;
        }

        private void CheckAbility(GenericAction action)
        {
            if (action.Source != null
                && (action.Source.UpgradeInfo.HasType(UpgradeType.Talent)
                    || action.Source.UpgradeInfo.HasType(UpgradeType.Illicit)
                    || action.Source.UpgradeInfo.HasType(UpgradeType.Modification))
                )
            {
                RegisterAbilityTrigger(TriggerTypes.OnActionIsPerformed, PerformCalculateAction);
            }
        }

        private void PerformCalculateAction(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction
            (
                new CalculateAction(),
                Triggers.FinishTrigger,
                descriptionShort: "Intuitive Interface",
                descriptionLong: "You may perform a Calculate action"
            );
        }
    }
}
