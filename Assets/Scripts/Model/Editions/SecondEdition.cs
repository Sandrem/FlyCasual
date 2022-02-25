using ActionsList;
using Arcs;
using BoardTools;
using Bombs;
using GameModes;
using Movement;
using Obstacles;
using Ship;
using SquadBuilderNS;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
using UnityEngine;
using Upgrade;

namespace Editions
{
    public class SecondEdition : Edition
    {
        public override string Name { get { return "Second Edition"; } }
        public override string NameShort { get { return "SecondEdition"; } }

        public SecondEdition()
        {
            RuleSet = new RuleSets.RuleSet25();
        }

        public override int MaxPoints { get { return 20; } }
        public override int MinShipsCount { get { return 3; } }
        public override int MaxShipsCount { get { return 8; } }
        public override string CombatPhaseName { get { return "Engagement"; } }
        public override Color MovementEasyColor { get { return new Color(0, 0.5f, 1); } }
        public override bool CanAttackBumpedTarget { get { return true; } }
        public override MovementComplexity IonManeuverComplexity { get { return MovementComplexity.Easy; } }
        public override string PathToSavedSquadrons { get { return "SavedSquadrons"; } } //RandomAiSquadrons

        public override string RootUrlForImages { get { return "https://squadbuilder.fantasyflightgames.com/card_images/"; } }
        public override Vector2 UpgradeCardSize { get { return new Vector2(418, 300); } }
        public override Vector2 UpgradeCardCompactOffset { get { return new Vector2(168, 2); } }
        public override Vector2 UpgradeCardCompactSize { get { return new Vector2(237, 296); } }

        public override Dictionary<Type, int> DamageDeckContent
        {
            get
            {
                return new Dictionary<Type, int>()
                {
                    { typeof(DamageDeckCardSE.BlindedPilot),        2 },
                    { typeof(DamageDeckCardSE.ConsoleFire),         2 },
                    { typeof(DamageDeckCardSE.DamagedEngine),       2 },
                    { typeof(DamageDeckCardSE.DamagedSensorArray),  2 },
                    { typeof(DamageDeckCardSE.DirectHit),           5 },
                    { typeof(DamageDeckCardSE.DisabledPowerRegulator),  2 },                    
                    { typeof(DamageDeckCardSE.FuelLeak),            4 },
                    { typeof(DamageDeckCardSE.HullBreach),          2 },
                    { typeof(DamageDeckCardSE.LooseStabilizer),     2 },
                    { typeof(DamageDeckCardSE.PanickedPilot),       2 },
                    { typeof(DamageDeckCardSE.StructuralDamage),    2 },
                    { typeof(DamageDeckCardSE.StunnedPilot),        2 },
                    { typeof(DamageDeckCardSE.WeaponsFailure),      2 },
                    { typeof(DamageDeckCardSE.WoundedPilot),        2 },
                };
            }
        }

        public override Dictionary<BaseSize, int> NegativeTokensToAffectShip
        {
            get
            {
                return new Dictionary<BaseSize, int>()
                {
                    { BaseSize.None,    int.MaxValue },
                    { BaseSize.Small,   1 },
                    { BaseSize.Medium,  2 },
                    { BaseSize.Large,   3 }
                };
            }
        }

        public override Dictionary<string, string> PreGeneratedAiSquadrons
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    
                };
            }
        }

        public override int MinShipCost(Faction faction)
        {
            return 1;
        }

        private bool HasYv666InSquad()
        {
            return Global.SquadBuilder.CurrentSquad.Ships.Any(n => n.Instance is Ship.SecondEdition.YV666LightFreighter.YV666LightFreighter);
        }

        public override void EvadeDiceModification(DiceRoll diceRoll)
        {
            if (diceRoll.Blanks > 0)
            {
                diceRoll.ChangeOne(DieSide.Blank, DieSide.Success);
            }
            else if (diceRoll.Focuses > 0)
            {
                diceRoll.ChangeOne(DieSide.Focus, DieSide.Success);
            }
            else
            {
                Messages.ShowError("An Evade Token was spent, but there were no valid dice to change to evade");
            }
        }

        public override void ActionIsFailed(GenericShip ship, GenericAction action, bool overWrittenInstead = false, bool hasSecondChance = false)
        {
            base.ActionIsFailed(ship, action, overWrittenInstead, hasSecondChance);

            // Temporary solution for off-the-board problem

            if (!hasSecondChance)
            {
                if (!IsTractorBeamFailed())
                {
                    Phases.CurrentSubPhase.IsReadyForCommands = true;
                    Phases.CurrentSubPhase.SkipButton();
                }
                else
                {
                    (Phases.CurrentSubPhase as TractorBeamPlanningSubPhase).RegisterTractorPlanning();
                }
            }
        }

        private bool IsTractorBeamFailed()
        {
            return Phases.CurrentSubPhase is TractorBeamPlanningSubPhase;
        }

        public override void AdaptShipToRules(GenericShip ship)
        {
            if (Edition.Current is SecondEdition)
            {
                if (ship.HotacManeuverTable != null) ship.HotacManeuverTable.AdaptToSecondEdition();
            }
        }

        public override bool IsWeaponHaveRangeBonus(IShipWeapon weapon)
        {
            List<WeaponTypes> rangeEffectedWeaponTypes = new List<WeaponTypes>()
            {
                WeaponTypes.Cannon,
                WeaponTypes.PrimaryWeapon,
                WeaponTypes.Turret
            };

            return rangeEffectedWeaponTypes.Contains(weapon.WeaponType) && !weapon.WeaponInfo.NoRangeBonus;
        }

        public override void SetShipBaseImage(GenericShip ship)
        {
            ship.SetShipBaseImageSecondEdition();
        }

        public override void RotateMobileFiringArc(GenericArc arc, ArcFacing facing)
        {
            arc.ShipBase.Host.ShowMobileFiringArcHighlight(facing);
        }

        public override void RotateMobileFiringArcAlt(GenericArc arc, ArcFacing facing)
        {
            arc.ShipBase.Host.ShowMobileFiringArcAltHighlight(facing);
        }

        public override void BarrelRollTemplatePlanning()
        {
            (Phases.CurrentSubPhase as BarrelRollPlanningSubPhase).PerfromTemplatePlanningSecondEdition();
        }

        public override void DecloakTemplatePlanning()
        {
            (Phases.CurrentSubPhase as DecloakPlanningSubPhase).PerfromTemplatePlanningSecondEdition();
        }

        public override void SquadBuilderIsOpened()
        {
            MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
            if (IsSquadBuilderLocked && Global.SquadBuilder.CurrentSquad.Points == 0)
            {
                MainMenu.CurrentMainMenu.ChangePanel("BrowseSavedSquadsPanel");
            }
        }

        public override void WhenIonized(GenericShip ship)
        {
            ship.OnPerformActionStepStart += EnableIonizationActionEffect;
            ship.OnMovementActivationFinish += DisableIonizationActionEffect;
        }

        private void EnableIonizationActionEffect(GenericShip ship)
        {
            ship.OnTryAddAction += IonizedShipCanDoOnlyFocus;
        }

        private void DisableIonizationActionEffect(GenericShip ship)
        {
            ship.OnTryAddAction -= IonizedShipCanDoOnlyFocus;

            ship.OnPerformActionStepStart -= EnableIonizationActionEffect;
            ship.OnMovementActivationFinish -= DisableIonizationActionEffect;
        }

        private void IonizedShipCanDoOnlyFocus(GenericShip ship, GenericAction action, ref bool canBePerformed)
        {
            if (canBePerformed)
            {
                bool canPerformActionWhileIonized = ship.CallCanPerformActionWhileIonized(action);
                if (!canPerformActionWhileIonized)
                {
                    canBePerformed = action is FocusAction;
                }
            }
        }

        public override bool ReinforceEffectCanBeUsed(ArcFacing facing)
        {
            return false;
        }

        public override bool DefenderIsReinforcedAgainstAttacker(ArcFacing facing, GenericShip defender, GenericShip attacker)
        {
            bool inFullFrontArc = defender.SectorsInfo.IsShipInSector(attacker, ArcType.FullFront);
            bool inFullRearArc = defender.SectorsInfo.IsShipInSector(attacker, ArcType.FullRear);

            return (facing == ArcFacing.FullFront) ? inFullFrontArc && !inFullRearArc : !inFullFrontArc && inFullRearArc;
        }

        public override bool ReinforcePostCombatEffectCanBeUsed(ArcFacing facing)
        {
            if (Combat.DiceRollAttack.Successes <= 1) return false;

            return DefenderIsReinforcedAgainstAttacker(facing, Combat.Defender, Combat.Attacker);
        }

        public override void TimedBombActivationTime(GenericShip ship)
        {
            ship.OnCheckSystemsAbilityActivation -= BombsManager.CheckBombDropAvailabilitySystemPhase;
            ship.OnCheckSystemsAbilityActivation += BombsManager.CheckBombDropAvailabilitySystemPhase;

            ship.OnSystemsAbilityActivation -= BombsManager.RegisterBombDropAvailabilitySystemPhase;
            ship.OnSystemsAbilityActivation += BombsManager.RegisterBombDropAvailabilitySystemPhase;
        }

        public override bool IsTokenCanBeDiscardedByJam(GenericToken token)
        {
            return token.TokenColor == TokenColors.Green || token is BlueTargetLockToken;
        }

        public override string GetPilotImageUrl(GenericShip ship, string filename)
        {
            return (ship.PilotInfo.SEImageNumber == 0) ? null : RootUrlForImages + "Card_Pilot_" + ship.PilotInfo.SEImageNumber + ".png";
        }

        public override string GetUpgradeImageUrl(GenericUpgrade upgrade, string filename = null)
        {
            return RootUrlForImages
                + "Card_Upgrade_" + upgrade.UpgradeInfo.SEImageNumber
                + ((upgrade.IsSecondSide)?"b":"")
                + ".png";
        }

        public override string FactionToXws(Faction faction)
        {
            string result = "";

            switch (faction)
            {
                case Faction.Rebel:
                    result = "rebelalliance";
                    break;
                case Faction.Imperial:
                    result = "galacticempire";
                    break;
                case Faction.Scum:
                    result = "scumandvillainy";
                    break;
                case Faction.Resistance:
                    result = "resistance";
                    break;
                case Faction.FirstOrder:
                    result = "firstorder";
                    break;
                case Faction.Republic:
                    result = "galacticrepublic";
                    break;
                case Faction.Separatists:
                    result = "separatistalliance";
                    break;
                default:
                    break;
            }

            return result;
        }

        public override Faction XwsToFaction(string factionXWS)
        {
            Faction result = Faction.None;

            switch (factionXWS)
            {
                case "rebelalliance":
                    result = Faction.Rebel;
                    break;
                case "galacticempire":
                    result = Faction.Imperial;
                    break;
                case "scumandvillainy":
                    result = Faction.Scum;
                    break;
                case "resistance":
                    result = Faction.Resistance;
                    break;
                case "firstorder":
                    result = Faction.FirstOrder;
                    break;
                case "galacticrepublic":
                    result = Faction.Republic;
                    break;
                case "separatistalliance":
                    result = Faction.Separatists;
                    break;
                default:
                    break;
            }

            return result;
        }

        public override string UpgradeTypeToXws(UpgradeType upgradeType)
        {
            string result = "";

            switch (upgradeType)
            {
                case UpgradeType.ForcePower:
                    result = "force-power";
                    break;
                case UpgradeType.TacticalRelay:
                    result = "tactical-relay";
                    break;
                default:
                    result = upgradeType.ToString().ToLower();
                    break;
            }

            return result;
        }

        public override UpgradeType XwsToUpgradeType(string upgradeXws)
        {
            UpgradeType result = UpgradeType.Astromech;

            switch (upgradeXws)
            {
                case "force-power":
                    result = UpgradeType.ForcePower;
                    break;
                case "tactical-relay":
                case "tacticalrelay":
                    result = UpgradeType.TacticalRelay;
                    break;
                default:
                    string capitalizedName = upgradeXws.First().ToString().ToUpper() + upgradeXws.Substring(1);
                    result = (UpgradeType)Enum.Parse(typeof(UpgradeType), capitalizedName);
                    break;
            }

            return result;
        }
    }
}
