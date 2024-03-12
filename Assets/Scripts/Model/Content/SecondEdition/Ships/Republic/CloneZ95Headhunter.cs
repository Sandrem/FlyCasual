using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using Ship.CardInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.CloneZ95Headhunter
    {
        public class CloneZ95Headhunter : GenericShip
        {
            public CloneZ95Headhunter() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "Clone Z-95 Headhunter",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, Type>
                        {
                            { Faction.Republic, typeof(Knack) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 2), 2, 2, 2,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(BarrelRollAction), ActionColor.Red)
                    )
                );

                ModelInfo = new ShipModelInfo
                (
                    "Clone Z-95 Headhunter",
                    "Red",
                    previewScale: 2f
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),

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
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
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

                ShipIconLetter = 'z';

                ShipAbilities.Add(new Abilities.SecondEdition.VersatileFrameAbility());
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class VersatileFrameAbility : GenericAbility
    {
        private readonly List<UpgradeType> HardpointSlotTypes = new List<UpgradeType>
        {
            UpgradeType.Torpedo,
            UpgradeType.Missile
        };

        public override void ActivateAbilityForSquadBuilder()
        {
            foreach (UpgradeType upgradeType in HardpointSlotTypes)
            {
                //HostShip.ShipInfo.UpgradeIcons.Upgrades.Add(upgradeType);
                HostShip.UpgradeBar.AddSlot(upgradeType);
            };

            HostShip.OnPreInstallUpgrade += OnPreInstallUpgrade;
            HostShip.OnRemovePreInstallUpgrade += OnRemovePreInstallUpgrade;
        }

        public override void ActivateAbility()
        {
            HostShip.OnGenerateActions += CheckBoostActionAvailability;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGenerateActions -= CheckBoostActionAvailability;
        }

        public override void DeactivateAbilityForSquadBuilder() { }

        private void OnPreInstallUpgrade(GenericUpgrade upgrade)
        {
            if (HardpointSlotTypes.Contains(upgrade.UpgradeInfo.UpgradeTypes.First()))
            {
                HardpointSlotTypes
                    .Where(slot => slot != upgrade.UpgradeInfo.UpgradeTypes.First())
                    .ToList()
                    .ForEach(slot => HostShip.UpgradeBar.RemoveSlot(slot));
            }
        }

        private void OnRemovePreInstallUpgrade(GenericUpgrade upgrade)
        {
            if (HardpointSlotTypes.Contains(upgrade.UpgradeInfo.UpgradeTypes.First()))
            {
                HardpointSlotTypes
                    .Where(slot => slot != upgrade.UpgradeInfo.UpgradeTypes.First())
                    .ToList()
                    .ForEach(slot => HostShip.UpgradeBar.AddSlot(slot));
            }
        }

        private void CheckBoostActionAvailability(GenericShip ship)
        {
            if (ship.RevealedManeuver != null && ship.RevealedManeuver.ColorComplexity == MovementComplexity.Easy)
            {
                ship.AddAvailableAction(new BoostAction());
            }
        }
    }
}

