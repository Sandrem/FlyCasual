using System.Collections;
using System.Collections.Generic;
using Movement;
using ActionsList;
using Upgrade;
using Actions;
using Ship.CardInfo;
using Arcs;
using UnityEngine;
using BoardTools;
using System.Linq;

namespace Ship
{
    namespace SecondEdition.StarViperClassAttackPlatform
    {
        public class StarViperClassAttackPlatform : GenericShip
        {
            public StarViperClassAttackPlatform() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "StarViper-class Attack Platform",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, System.Type>
                        {
                            { Faction.Scum, typeof(Guri) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 3), 3, 4, 1,
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
                    },
                    legality: new List<Content.Legality>() { Content.Legality.ExtendedLegal }
                );

                ModelInfo = new ShipModelInfo
                (
                    "StarViper",
                    "Black Sun Enforcer",
                    new Vector3(-3.95f, 8.1f, 5.55f),
                    2f
                );

                DialInfo = new ShipDialInfo
                (
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.SegnorsLoop, MovementComplexity.Complex),

                    new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
                );

                SoundInfo = new ShipSoundInfo
                (
                    new List<string>()
                    {
                        "XWing-Fly1",
                        "XWing-Fly2",
                        "XWing-Fly3"
                    },
                    "XWing-Laser", 3
                );

                ShipIconLetter = 'v';

                ShipAbilities.Add(new Abilities.FirstEdition.StarViperMkIIAbility());
            }
        }
    }
}

namespace Abilities.FirstEdition
{
    public class StarViperMkIIAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates += ChangeBarrelRollTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBarrelRollTemplates -= ChangeBarrelRollTemplates;
        }

        private void ChangeBarrelRollTemplates(List<ManeuverTemplate> availableTemplates)
        {
            if (availableTemplates.Any(n => n.Name == "Straight 1"))
            {
                availableTemplates.RemoveAll(n => n.Name == "Straight 1");
                availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, ManeuverSpeed.Speed1));
                availableTemplates.Add(new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, ManeuverSpeed.Speed1));
            }
        }
    }
}