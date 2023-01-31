using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using Ship.CardInfo;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M12LKimogilaFighter
    {
        public class M12LKimogilaFighter : GenericShip
        {
            public M12LKimogilaFighter() : base()
            {
                ShipInfo = new ShipCardInfo25
                (
                    "M12-L Kimogila Fighter",
                    BaseSize.Medium,
                    new FactionData
                    (
                        new Dictionary<Faction, System.Type>
                        {
                            { Faction.Scum, typeof(CartelExecutioner) }
                        }
                    ),
                    new ShipArcsInfo(ArcType.Front, 3), 1, 7, 2,
                    new ShipActionsInfo
                    (
                        new ActionInfo(typeof(FocusAction)),
                        new ActionInfo(typeof(TargetLockAction)),
                        new ActionInfo(typeof(BarrelRollAction), ActionColor.Red),
                        new ActionInfo(typeof(ReloadAction))
                    ),
                    new ShipUpgradesInfo(),
                    legality: new List<Content.Legality>() { Content.Legality.ExtendedLegal }
                );

                ModelInfo = new ShipModelInfo
                (
                    "M12-L Kimogila Fighter",
                    "Hutt Cartel",
                    new Vector3(-3.9f, 7.95f, 5.55f),
                    1.25f
                );

                DialInfo = new ShipDialInfo
                (
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

                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                    new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

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
                    "XWing-Laser", 3
                );

                ShipIconLetter = 'K';

                ShipAbilities.Add(new Abilities.SecondEdition.DeadToRights());
            }
        }
    }
}

namespace Abilities.SecondEdition
{
    public class DeadToRights : GenericAbility
    {
        public override string Name { get { return "Dead to Rights"; } }

        public override void ActivateAbility()
        {
            GenericShip.OnTryAddAvailableDiceModificationGlobal += CheckBullseyeArc;
        }

        public override void DeactivateAbility()
        {
            GenericShip.OnTryAddAvailableDiceModificationGlobal -= CheckBullseyeArc;
        }

        private List<System.Type> TokensForbidden = new List<System.Type>()
        {
            typeof(FocusToken),
            typeof(EvadeToken),
            typeof(CalculateToken),
            typeof(ReinforceAftToken),
            typeof(ReinforceForeToken),
        };

        public void CheckBullseyeArc(GenericShip ship, GenericAction action, ref bool canBeUsed)
        {
            if (action.TokensSpend.Any(t => TokensForbidden.Contains(t)))
            {
                if (Combat.AttackStep == CombatStep.Defence && Combat.Defender.ShipId == ship.ShipId)
                {
                    if (Combat.Attacker.ShipId == HostShip.ShipId)
                    {
                        if (Combat.Attacker.SectorsInfo.IsShipInSector(Combat.Defender, ArcType.Bullseye))
                        {
                            Messages.ShowInfo("Dead to Rights: The defender cannot use " + action.DiceModificationName);
                            canBeUsed = false;
                        }
                    }
                }
            }
        }
    }
}
