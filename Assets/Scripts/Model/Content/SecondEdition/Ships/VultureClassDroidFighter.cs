using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Actions;
using ActionsList;
using Arcs;
using Movement;
using Ship;
using SubPhases;
using Upgrade;

namespace Ship.SecondEdition.VultureClassDroidFighter
{
    public class VultureClassDroidFighter : GenericShip
    {
        public VultureClassDroidFighter() : base()
        {
            ShipInfo = new ShipCardInfo
            (
                "Vulture-class Droid Fighter",
                BaseSize.Small,
                Faction.Separatists,
                new ShipArcsInfo(ArcType.Front, 2), 2, 3, 0,
                new ShipActionsInfo(
                    new ActionInfo(typeof(CalculateAction)),
                    new ActionInfo(typeof(TargetLockAction)),
                    new ActionInfo(typeof(BarrelRollAction))
                ),
                new ShipUpgradesInfo(
                    UpgradeType.Title,
                    UpgradeType.Missile,
                    UpgradeType.Modification,
                    UpgradeType.Configuration
                )
            );

            ShipAbilities.Add(new Abilities.SecondEdition.NetworkedCalculationsAbility());

            ShipInfo.ActionIcons.AddLinkedAction(new LinkedActionInfo(typeof(BarrelRollAction), typeof(CalculateAction)));

            IconicPilots = new Dictionary<Faction, System.Type> {
                { Faction.Separatists, typeof(Dfs081) }
            };

            ModelInfo = new ShipModelInfo(
                "Vulture",
                "Default"
            );

            DialInfo = new ShipDialInfo(
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed1, ManeuverDirection.Forward, ManeuverBearing.KoiogranTurn, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Left, ManeuverBearing.TallonRoll, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed2, ManeuverDirection.Right, ManeuverBearing.TallonRoll, MovementComplexity.Complex),

                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Turn, MovementComplexity.Normal),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Left, ManeuverBearing.Bank, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Bank, MovementComplexity.Complex),
                new ManeuverInfo(ManeuverSpeed.Speed3, ManeuverDirection.Right, ManeuverBearing.Turn, MovementComplexity.Normal),

                new ManeuverInfo(ManeuverSpeed.Speed4, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Easy),

                new ManeuverInfo(ManeuverSpeed.Speed5, ManeuverDirection.Forward, ManeuverBearing.Straight, MovementComplexity.Normal)
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

            // TODO: AI Table
            HotacManeuverTable = new AI.EscapeCraftTable();

            // ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/2/20/Maneuver_escape_craft.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    //While you defend or perform an attack. you may spend 1 calculate token from a friendly ship 
    //at range 0-1 to change 1 focus result to an evade or hit result.
    public class NetworkedCalculationsAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Networked Calculations",
                IsDiceModificationAvailable,
                GetDiceModificationAiPriority,
                DiceModificationType.Change,
                1,
                new List<DieSide> {DieSide.Focus},
                DieSide.Success,
                payAbilityCost: PayAbilityCost
            );
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private bool IsDiceModificationAvailable()
        {
            if ((Combat.AttackStep == CombatStep.Attack) || (Combat.AttackStep == CombatStep.Defence))
            {
                return BoardTools.Board
                    .GetShipsAtRange(HostShip, new UnityEngine.Vector2(0, 1), Team.Type.Friendly)
                    .Where(ship => ship.Tokens.HasToken<Tokens.CalculateToken>())
                    .Any();
            }
            return false;
        }
        
        private int GetDiceModificationAiPriority()
        {
            return 40;
        }

        private void PayAbilityCost(Action<bool> callback)
        {
            SelectTargetForAbility(
                () => SpendToken(callback),
                FilterTargets,
                GetAiPriority,
                HostShip.Owner.PlayerNo,
                "Networked Calculations",
                "Choose a friendly ship to spend a calculate token",
                HostUpgrade
            );
        }

        private void SpendToken(Action<bool> callback)
        {
            SelectShipSubPhase.FinishSelectionNoCallback();

            if (TargetShip.Tokens.HasToken<Tokens.CalculateToken>())
            {
                TargetShip.Tokens.SpendToken(typeof(Tokens.CalculateToken), () => callback(true));
            }
            else
            {
                callback(false);
            }
        }

        private bool FilterTargets(GenericShip ship)
        {
            return FilterByTargetType(ship, TargetTypes.This, TargetTypes.OtherFriendly) && FilterTargetsByRange(ship, 0, 1);
        }

        private int GetAiPriority(GenericShip ship)
        {
            //prioritize cheap ships and ships with multiple calculate tokens
            //a more advanced function could take into account which ships have already attacked, and which are likely to be attacked

            int result = 0;

            result += ship.Tokens.CountTokensByType<Tokens.CalculateToken>() * 100;

            result += 200 - ship.PilotInfo.Cost;

            return result;
        }
    }
}