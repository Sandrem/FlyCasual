using ActionsList;
using Arcs;
using BoardTools;
using Bombs;
using GameModes;
using Movement;
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
        //public override bool IsSquadBuilderLocked { get { return true; } }

        public override int MaxPoints { get { return 200; } }
        public override int MinShipsCount { get { return 1; } }
        public override int MaxShipsCount { get { return 8; } }
        public override string CombatPhaseName { get { return "Engagement"; } }
        public override Color MovementEasyColor { get { return new Color(0, 0.5f, 1); } }
        public override MovementComplexity IonManeuverComplexity { get { return MovementComplexity.Easy; } }
        public override string PathToSavedSquadrons { get { return "SavedSquadrons"; } } //RandomAiSquadrons

        public override string RootUrlForImages { get { return "https://sb-cdn.fantasyflightgames.com/card_images/"; } }
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
                    { "Vader + Mini-Swarm",  "{\"name\":\"Vader + Mini-Swarm\",\"faction\":\"imperial\",\"points\":197,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"darthvader\",\"points\":75,\"ship\":\"tieadv\",\"upgrades\":{\"missile\":[\"clustermissiles\"],\"system\":[\"firecontrolsystem\"],\"force\":[\"hate\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"name\":\"howlrunner\",\"points\":41,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":27,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":27,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":27,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"description\":\"Darth Vader + Cluster Missiles + Fire-Control System + Hate\n\\\"Howlrunner\\\" + Crack Shot\nBlack Squadron Ace (TIE/ln Fighter) + Crack Shot\nBlack Squadron Ace (TIE/ln Fighter) + Crack Shot\nBlack Squadron Ace (TIE/ln Fighter) + Crack Shot\"}" },
                    { "Boba + Joy + Zealot",  "{\"name\":\"Boba + Joy + Zealot\",\"faction\":\"scum\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"bobafett\",\"points\":46,\"ship\":\"firespray31\",\"upgrades\":{\"bomb\":[\"seismiccharges\"],\"crew\":[\"perceptivecopilot\"],\"illicit\":[\"inertialdampeners\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Boba Fett\"}}},{\"name\":\"joyrekkoff\",\"points\":32,\"ship\":\"protectoratestarfighter\",\"upgrades\":{\"torpedo\":[\"iontorpedoes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Protectorate Starfighter\"}}},{\"name\":\"zealousrecruit\",\"points\":20,\"ship\":\"protectoratestarfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Protectorate Starfighter\"}}}],\"description\":\"Boba Fett (Firespray-31) + Seismic Charges + Perceptive Copilot + Inertial Dampeners\nJoy Rekkoff + Ion Torpedoes\nZealous Recruit\"}" },
                    { "Luke + Wedge + Gold", "{\"name\":\"Luke + Wedge + Gold\",\"faction\":\"rebel\",\"points\":190,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"lukeskywalker\",\"points\":69,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(open)\"],\"amd\":[\"r2d2\"],\"torpedo\":[\"protontorpedoes\"],\"force\":[\"instinctiveaim\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"wedgeantilles\",\"points\":81,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(open)\",\"afterburners\"],\"ept\":[\"predator\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"goldsquadronveteran\",\"points\":31,\"ship\":\"ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"amd\":[\"r5astromech\"],\"bomb\":[\"protonbombs\"],\"ept\":[\"experthandling\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}}],\"description\":\"Luke Skywalker + Servomotor S-Foils (Attack) + R2-D2 + Proton Torpedoes + Instinctive Aim\nWedge Antilles + Servomotor S-Foils (Attack) + AfterBurners + Predator\nGold Squadron Veteran + Ion Cannon Turret + R5 Astromech + Proton Bombs + Expert Handling\"}" },
                    { "Luke + Wedge + Dutch", "{\"name\":\"Luke + Wedge + Dutch\",\"faction\":\"rebel\",\"points\":191,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"lukeskywalker\",\"points\":69,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(open)\"],\"amd\":[\"r2d2\"],\"torpedo\":[\"protontorpedoes\"],\"force\":[\"instinctiveaim\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"wedgeantilles\",\"points\":81,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(open)\"],\"amd\":[\"r4astromech\"],\"ept\":[\"outmaneuver\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"dutchvander\",\"points\":31,\"ship\":\"ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"bomb\":[\"seismiccharges\"],\"ept\":[\"selfless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}}],\"description\":\"Luke Skywalker + Servomotor S-Foils (Attack) + R2-D2 + Proton Torpedoes + Instinctive Aim\nWedge Antilles + Servomotor S-Foils (Attack) + R4 Astromech + Outmaneuver\n\\\"Dutch\\\" Vander + Ion Cannon Turret + Seismic Charges + Selfless\"}" },
                    { "Maarek + Krennic + Mini-Swarm", "{\"name\":\"Maarek + Krennic + Mini-Swarm\",\"faction\":\"imperial\",\"points\":187,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"maarekstele\",\"points\":7,\"ship\":\"tieadv\",\"upgrades\":{\"system\":[\"firecontrolsystem\"],\"ept\":[\"ruthless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"captainferoph\",\"points\":32,\"ship\":\"tiereaper\",\"upgrades\":{\"crew\":[\"tacticalofficer\",\"directorkrennic\"],\"ept\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":31,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":31,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":31,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"description\":\"Maarek Stele (TIE Advanced) + Fire-Control System + Ruthless\nCaptain Feroph + Tactical Officer + Director Krennic + Elusive\nBlack Squadron Ace + Crack Shot\nBlack Squadron Ace + Crack Shot\nBlack Squadron Ace + Crack Shot\"}" },
                    { "Two Tubes + Porkins + Gray Sq. Pilot", "{\"name\":\"Two Tubes + Porkins + Gray Sq. Pilot\",\"faction\":\"rebel\",\"points\":184,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"jekporkins\",\"points\":64,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(open)\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"benthictwotubes\",\"points\":30,\"ship\":\"uwing\",\"upgrades\":{\"configuration\":[\"pivotwing(open)\"],\"crew\":[\"perceptivecopilot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Partisan\"}}},{\"name\":\"graysquadronbomber\",\"points\":47,\"ship\":\"ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"bomb\":[\"seismiccharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"edriotwotubes\",\"points\":60,\"ship\":\"xwing\",\"upgrades\":{\"mod\":[\"servomotorsfoils(open)\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Partisan\"}}}],\"description\":\"Jek Porkins + Servomotor S-Foils (Attack)\nBenthic Two Tubes + Pivot Wing (Attack) + Advanced Sensors + Perceptive Copilot\nGray Squadron Bomber + Ion Cannon Turret + Seismic Charges\nEdrio Two Tubes + Servomotor S-Foils (Attack)\"}" },
                    { "Sloane + Swarm", "{\"name\":\"Sloane + Swarm\",\"faction\":\"imperial\",\"points\":199,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"omicrongrouppilot\",\"points\":61,\"ship\":\"lambdaclassshuttle\",\"upgrades\":{\"crew\":[\"admiralsloane\",\"perceptivecopilot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"name\":\"academypilot\",\"points\":23,\"ship\":\"tiefighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"academypilot\",\"points\":23,\"ship\":\"tiefighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"academypilot\",\"points\":23,\"ship\":\"tiefighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"academypilot\",\"points\":23,\"ship\":\"tiefighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"academypilot\",\"points\":23,\"ship\":\"tiefighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"academypilot\",\"points\":23,\"ship\":\"tiefighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"description\":\"Omicron Group Pilot + Admiral Sloane + Perceptive Copilot\nAcademy Pilot\nAcademy Pilot\nAcademy Pilot\nAcademy Pilot\nAcademy Pilot\nAcademy Pilot\"}" },
                    { "Cartel Marauders", "{\"name\":\"Cartel Marauders\",\"faction\":\"scum\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"cartelmarauder\",\"points\":40,\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}},{\"name\":\"cartelmarauder\",\"points\":40,\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}},{\"name\":\"cartelmarauder\",\"points\":40,\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}},{\"name\":\"cartelmarauder\",\"points\":40,\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}},{\"name\":\"cartelmarauder\",\"points\":40,\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}}],\"description\":\"Cartel Marauder\nCartel Marauder\nCartel Marauder\nCartel Marauder\nCartel Marauder\"}" },
                    { "Blue Sq. Rookies", "{\"name\":\"Blue Sq. Rookies\",\"faction\":\"resistance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"bluesquadronrookie\",\"points\":50,\"ship\":\"t70xwing\",\"upgrades\":{\"tech\":[\"advoptics\"],\"configuration\":[\"integratedsfoils(closed)\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"name\":\"bluesquadronrookie\",\"points\":50,\"ship\":\"t70xwing\",\"upgrades\":{\"tech\":[\"advoptics\"],\"configuration\":[\"integratedsfoils(closed)\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"name\":\"bluesquadronrookie\",\"points\":50,\"ship\":\"t70xwing\",\"upgrades\":{\"tech\":[\"advoptics\"],\"configuration\":[\"integratedsfoils(closed)\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"name\":\"bluesquadronrookie\",\"points\":50,\"ship\":\"t70xwing\",\"upgrades\":{\"tech\":[\"advoptics\"],\"configuration\":[\"integratedsfoils(closed)\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}}],\"description\":\"Blue Squadron Rookie + Advanced Optics + Integrated S-Foils (Closed)\nBlue Squadron Rookie + Advanced Optics + Integrated S-Foils (Closed)\nBlue Squadron Rookie + Advanced Optics + Integrated S-Foils (Closed)\nBlue Squadron Rookie + Advanced Optics + Integrated S-Foils (Closed)\"}" },
                    { "Backdraft + Mini-Swarm", "{\"name\":\"Backdraft + Mini-Swarm\",\"faction\":\"firstorder\",\"points\":197,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"backdraft\",\"points\":53,\"ship\":\"tiesffighter\",\"upgrades\":{\"gunner\":[\"specialforcesgunner\"],\"ept\":[\"fanatical\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"name\":\"omegasquadronexpert\",\"points\":48,\"ship\":\"tiesffighter\",\"upgrades\":{\"gunner\":[\"specialforcesgunner\"],\"ept\":[\"fanatical\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"name\":\"omegasquadronexpert\",\"points\":48,\"ship\":\"tiesffighter\",\"upgrades\":{\"gunner\":[\"specialforcesgunner\"],\"ept\":[\"fanatical\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"name\":\"omegasquadronexpert\",\"points\":48,\"ship\":\"tiesffighter\",\"upgrades\":{\"gunner\":[\"specialforcesgunner\"],\"ept\":[\"fanatical\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}}],\"description\":\"\\\"Backdraft\\\" + Special Forces Gunner + Fanatical\nOmega Squadron Expert + Special Forces Gunner + Fanatical\nOmega Squadron Expert + Special Forces Gunner + Fanatical\nOmega Squadron Expert + Special Forces Gunner + Fanatical\"}" },
                    { "Luke + Wedge + Red Sq. Veterans", "{\"name\":\"Luke + Wedge + Red Sq. Veterans\",\"faction\":\"rebel\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"lukeskywalker\",\"points\":62,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(closed)\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"wedgeantilles\",\"points\":52,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(closed)\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"redsquadronveteran\",\"points\":43,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(closed)\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"redsquadronveteran\",\"points\":43,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(closed)\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"description\":\"Luke Skywalker + Servomotor S-Foils (Closed)\nWedge Antilles + Servomotor S-Foils (Closed)\nRed Squadron Veteran + Servomotor S-Foils (Closed)\nRed Squadron Veteran + Servomotor S-Foils (Closed)\"}" },
                    { "Omega Sq. Experts", "{\"name\":\"Omega Sq. Experts\",\"faction\":\"firstorder\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"omegasquadronexpert\",\"points\":50,\"ship\":\"tiesffighter\",\"upgrades\":{\"system\":[\"firecontrolsystem\"],\"gunner\":[\"specialforcesgunner\"],\"ept\":[\"fanatical\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"name\":\"omegasquadronexpert\",\"points\":50,\"ship\":\"tiesffighter\",\"upgrades\":{\"system\":[\"firecontrolsystem\"],\"gunner\":[\"specialforcesgunner\"],\"ept\":[\"fanatical\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"name\":\"omegasquadronexpert\",\"points\":50,\"ship\":\"tiesffighter\",\"upgrades\":{\"system\":[\"firecontrolsystem\"],\"gunner\":[\"specialforcesgunner\"],\"ept\":[\"fanatical\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"name\":\"omegasquadronexpert\",\"points\":50,\"ship\":\"tiesffighter\",\"upgrades\":{\"system\":[\"firecontrolsystem\"],\"gunner\":[\"specialforcesgunner\"],\"ept\":[\"fanatical\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}}],\"description\":\"Omega Squadron Expert + Fire-Control System + Special Forces Gunner + Fanatical\nOmega Squadron Expert + Fire-Control System + Special Forces Gunner + Fanatical\nOmega Squadron Expert + Fire-Control System + Special Forces Gunner + Fanatical\nOmega Squadron Expert + Fire-Control System + Special Forces Gunner + Fanatical\"}" }
                };
            }
        }

        public override int MinShipCost(Faction faction)
        {
            if (faction != Faction.Scum)
            {
                return 23;
            }
            else
            {
                return (HasYv666InSquad()) ? 6 : 12;
            }
        }

        private bool HasYv666InSquad()
        {
            return SquadBuilder.CurrentSquadList.GetShips().Any(n => n.Instance is Ship.SecondEdition.YV666LightFreighter.YV666LightFreighter);
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
                Messages.ShowError("An Evade Token was spent, but there were no valid dice to change to evade.");
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
                    // if (!overWrittenInstead) Messages.ShowError("Action is skipped");
                    Phases.Skip();
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
                ship.HotacManeuverTable.AdaptToSecondEdition();
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

            return rangeEffectedWeaponTypes.Contains(weapon.WeaponType);
        }

        public override void SetShipBaseImage(GenericShip ship)
        {
            ship.SetShipBaseImageSecondEdition();
        }

        public override void RotateMobileFiringArc(ArcFacing facing)
        {
            Selection.ThisShip.ShowMobileFiringArcHighlight(facing);
        }

        public override void RotateMobileFiringArcAlt(ArcFacing facing)
        {
            Selection.ThisShip.ShowMobileFiringArcAltHighlight(facing);
        }

        public override void BarrelRollTemplatePlanning()
        {
            (Phases.CurrentSubPhase as BarrelRollPlanningSubPhase).PerfromTemplatePlanningSecondEdition();
        }

        public override void DecloakTemplatePlanning()
        {
            (Phases.CurrentSubPhase as DecloakPlanningSubPhase).PerfromTemplatePlanningSecondEdition();
        }

        public override void ReloadAction()
        {
            ActionsList.ReloadAction.RestoreOneCharge();
        }

        public override void SquadBuilderIsOpened()
        {
            MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
            if (IsSquadBuilderLocked && SquadBuilder.CurrentSquadList.Points == 0)
            {
                MainMenu.CurrentMainMenu.ChangePanel("BrowseSavedSquadsPanel");
            }
        }

        public override void WhenIonized(GenericShip ship)
        {
            ship.OnTryAddAction += IonizedShipCanDoOnlyFocus;
            ship.OnRoundEnd += DisableIonizationActionEffect;
        }

        private void DisableIonizationActionEffect(GenericShip ship)
        {
            ship.OnTryAddAction -= IonizedShipCanDoOnlyFocus;
            ship.OnRoundEnd -= DisableIonizationActionEffect;
        }

        private void IonizedShipCanDoOnlyFocus(GenericAction action, ref bool canBePerformed)
        {
            if (canBePerformed) canBePerformed = action is FocusAction;
        }

        public override bool ReinforceEffectCanBeUsed(ArcFacing facing)
        {
            return false;
        }

        public override bool ReinforcePostCombatEffectCanBeUsed(ArcFacing facing)
        {
            if (Combat.DiceRollAttack.Successes <= 1) return false;

            bool result = false;

            bool inFullFrontArc = Combat.Defender.SectorsInfo.IsShipInSector(Combat.Attacker, ArcType.FullFront);
            bool inFullRearArc = Combat.Defender.SectorsInfo.IsShipInSector(Combat.Attacker, ArcType.FullRear);

            result = (facing == ArcFacing.FullFront) ? inFullFrontArc && !inFullRearArc : !inFullFrontArc && inFullRearArc;

            return result;
        }

        public override void TimedBombActivationTime(GenericShip ship)
        {
            ship.OnSystemsAbilityActivation -= BombsManager.CheckBombDropAvailability;
            ship.OnSystemsAbilityActivation += BombsManager.CheckBombDropAvailability;
        }

        public override bool IsTokenCanBeDiscardedByJam(GenericToken token)
        {
            return token.TokenColor == TokenColors.Green || token is BlueTargetLockToken;
        }

        public override string GetPilotImageUrl(GenericShip ship, string filename)
        {
            return RootUrlForImages + "Card_Pilot_" + ship.PilotInfo.SEImageNumber + ".png";
        }

        public override string GetUpgradeImageUrl(GenericUpgrade upgrade)
        {
            return RootUrlForImages + "Card_Upgrade_" + upgrade.UpgradeInfo.SEImageNumber + ((upgrade.IsSecondSide)?"b":"") + ".png";
        }

    }
}
