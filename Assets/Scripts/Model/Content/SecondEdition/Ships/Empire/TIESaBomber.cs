using System.Collections.Generic;
using Movement;
using ActionsList;
using Actions;
using Upgrade;
using BoardTools;
using System.Linq;
using Arcs;
using UnityEngine;
using Ship.CardInfo;

namespace Ship
{
    namespace SecondEdition.TIESaBomber
    {
        public class TIESaBomber : GenericShip
        {
            public TIESaBomber() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "TIE/sa Bomber",
                    BaseSize.Small,
                    new FactionData
                    (
                        new Dictionary<Faction, System.Type>
                        {
                            { Faction.Imperial, typeof(TomaxBren) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 2), 2, 6, 0,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(BarrelRollAction)),
                        new ActionInfo(typeof(ReloadAction), ActionColor.Red)
                    ),
                    new ShipUpgradesInfo(),
                    linkedActions: new List<LinkedActionInfo>
                    {
                        new LinkedActionInfo(typeof(BarrelRollAction), typeof(TargetLockAction))
                    },
                    legality: new List<Content.Legality>() { Content.Legality.StandardLegal, Content.Legality.ExtendedLegal }
                );

                ModelInfo = new ShipModelInfo
                (
                    "TIE Bomber",
                    "Gray",
                    new Vector3(-3.9f, 8f, 5.55f),
                    2f
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

                    new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex)
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
                    "TIE-Fire", 2
                );

                ShipIconLetter = 'B';

                ShipAbilities.Add(new Abilities.SecondEdition.NimbleBomber());
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class NimbleBomber : GenericAbility
    {
        public override string Name { get { return "Nimble Bomber"; } }

        public override void ActivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplatesOneCondition += AddNimbleBomberTemplates;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnGetAvailableBombDropTemplatesOneCondition -= AddNimbleBomberTemplates;
        }

        private void AddNimbleBomberTemplates(List<ManeuverTemplate> availableTemplates, GenericUpgrade upgrade)
        {
            List<ManeuverTemplate> templatesCopy = new List<ManeuverTemplate>(availableTemplates);

            foreach (ManeuverTemplate existingTemplate in templatesCopy)
            {
                if (existingTemplate.Bearing == ManeuverBearing.Straight && existingTemplate.Direction == ManeuverDirection.Forward)
                {
                    List<ManeuverTemplate> newTemplates = new List<ManeuverTemplate>()
                    {
                        new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Right, existingTemplate.Speed, isBombTemplate: true),
                        new ManeuverTemplate(ManeuverBearing.Bank, ManeuverDirection.Left, existingTemplate.Speed, isBombTemplate: true),
                    };

                    foreach (ManeuverTemplate newTemplate in newTemplates)
                    {
                        if (!availableTemplates.Any(t => t.Name == newTemplate.Name))
                        {
                            availableTemplates.Add(newTemplate);
                        }
                    }
                }
            }
        }
    }
}
