using System;
using System.Collections.Generic;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using UnityEngine;
using Upgrade;

namespace Ship.SecondEdition.TIESeBomber
{
    public class TIESeBomber : GenericShip, TIE
    {
        public TIESeBomber() : base()
        {
            ShipInfo = new ShipCardInfo
            (
                "TIE/se Bomber",
                BaseSize.Small,
                Faction.FirstOrder,
                new ShipArcsInfo(ArcType.Front, 2), 2, 4, 2,
                new ShipActionsInfo
                (
                    new ActionInfo(typeof(FocusAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction)),
                    new ActionInfo(typeof(BoostAction), ActionColor.Red),
                    new ActionInfo(typeof(ReloadAction))
                ),
                new ShipUpgradesInfo
                (
                    UpgradeType.Tech,
                    UpgradeType.Torpedo,
                    UpgradeType.Missile,
                    UpgradeType.Gunner,
                    UpgradeType.Device,
                    UpgradeType.Device,
                    UpgradeType.Modification
                )
            );

            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(TargetLockAction)));
            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(ReloadAction), typeof(EvadeAction)));

            ShipAbilities.Add(new Abilities.SecondEdition.PursuitThrustersAbility());

            IconicPilots = new Dictionary<Faction, System.Type>
            {
                { Faction.FirstOrder, typeof(FirstOrderCadet) }
            };

            ModelInfo = new ShipModelInfo(
                "TIE Se Bomber",
                "First Order",
                new Vector3(-3.4f, 7.5f, 5.55f),
                2f
            );

            DialInfo = new ShipDialInfo
            (
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
            );

            SoundInfo = new ShipSoundInfo(
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

            ShipIconLetter = ' '; //TODO
        }
    }
}

namespace Abilities.SecondEdition
{
    public class PursuitThrustersAbility : GenericAbility
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
            RegisterAbilityTrigger(TriggerTypes.OnSystemsAbilityActivation, AskToPerformReposition);
        }

        private void AskToPerformReposition(object sender, EventArgs e)
        {
            HostShip.AskPerformFreeAction
            (
                new BoostAction() { HostShip = HostShip },
                Triggers.FinishTrigger,
                descriptionShort: "Pursuit Thrusters",
                descriptionLong: "You may perform a boost action",
                imageHolder: HostShip
            );
        }
    }
}