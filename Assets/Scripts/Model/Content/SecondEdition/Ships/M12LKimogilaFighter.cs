using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship
{
    namespace SecondEdition.M12LKimogilaFighter
    {
        public class M12LKimogilaFighter : FirstEdition.M12LKimogilaFighter.M12LKimogilaFighter
        {
            public M12LKimogilaFighter() : base()
            {
                ShipInfo.BaseSize = BaseSize.Medium;

                ShipInfo.ArcInfo = new ShipArcsInfo(ArcType.Front, 3);
                ShipInfo.Hull = 7;

                ShipInfo.ActionIcons.RemoveActions(typeof(BarrelRollAction));
                ShipInfo.ActionIcons.AddActions(new ActionInfo(typeof(BarrelRollAction), ActionColor.Red));

                ShipInfo.UpgradeIcons.Upgrades.Remove(UpgradeType.SalvagedAstromech);
                ShipInfo.UpgradeIcons.Upgrades.Add(UpgradeType.Astromech);

                IconicPilots = new Dictionary<Faction, System.Type> {
                    { Faction.Scum, typeof(CartelExecutioner) }
                };

                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn), MovementComplexity.Normal);
                DialInfo.ChangeManeuverComplexity(new ManeuverHolder(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn), MovementComplexity.Normal);

                ShipAbilities.Add(new Abilities.SecondEdition.DeadToRights());

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/3/3e/Maneuver_kimogila.png";
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
                            Messages.ShowInfo("Dead to Rights: The defender cannot use " + action.DiceModificationName + ".");
                            canBeUsed = false;
                        }
                    }
                }
            }
        }
    }
}
