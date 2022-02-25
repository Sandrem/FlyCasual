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

        public override int MaxPoints { get { return 200; } }
        public override int MinShipsCount { get { return 1; } }
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
                    { "RandomSquad1", "{\"name\":\"RandomSquad1\",\"faction\":\"rebelalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"knavesquadronescort\",\"ship\":\"ewing\",\"upgrades\":{\"sensor\":[\"firecontrolsystem\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"knavesquadronescort\",\"ship\":\"ewing\",\"upgrades\":{\"sensor\":[\"firecontrolsystem\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"knavesquadronescort\",\"ship\":\"ewing\",\"upgrades\":{\"sensor\":[\"firecontrolsystem\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"knavesquadronescort\",\"ship\":\"ewing\",\"upgrades\":{\"sensor\":[\"firecontrolsystem\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Knave Squadron Escort\nKnave Squadron Escort\nKnave Squadron Escort + Fire-Control System\nKnave Squadron Escort + Fire-Control System\"}" },
                    { "RandomSquad2", "{\"name\":\"RandomSquad2\",\"faction\":\"rebelalliance\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"roguesquadronescort\",\"ship\":\"ewing\",\"upgrades\":{\"torpedo\":[\"protontorpedoes\"],\"astromech\":[\"r3astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"roguesquadronescort\",\"ship\":\"ewing\",\"upgrades\":{\"torpedo\":[\"protontorpedoes\"],\"astromech\":[\"r3astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"roguesquadronescort\",\"ship\":\"ewing\",\"upgrades\":{\"torpedo\":[\"protontorpedoes\"],\"astromech\":[\"r3astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Rogue Squadron Escort + Proton Torpedoes + R3 Astromech\nRogue Squadron Escort + Proton Torpedoes + R3 Astromech\nRogue Squadron Escort + Proton Torpedoes + R3 Astromech\"}" },
                    { "RandomSquad3", "{\"name\":\"RandomSquad3\",\"faction\":\"rebelalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"redsquadronveteran\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"redsquadronveteran\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"redsquadronveteran\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"redsquadronveteran\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"redsquadronveteran\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Red Squadron Veteran + Servomotor S-Foils (Open)\nRed Squadron Veteran + Servomotor S-Foils (Open)\nRed Squadron Veteran + Servomotor S-Foils (Open)\nRed Squadron Veteran + Servomotor S-Foils (Open)\nRed Squadron Veteran + Servomotor S-Foils (Open)\"}" },
                    { "RandomSquad4", "{\"name\":\"RandomSquad4\",\"faction\":\"rebelalliance\",\"points\":199,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"cavernangelszealot\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"],\"illicit\":[\"falsetranspondercodes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Partisan\"}}},{\"id\":\"cavernangelszealot\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"],\"illicit\":[\"falsetranspondercodes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Partisan\"}}},{\"id\":\"cavernangelszealot\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"],\"illicit\":[\"falsetranspondercodes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Partisan\"}}},{\"id\":\"cavernangelszealot\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Partisan\"}}},{\"id\":\"cavernangelszealot\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Partisan\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Caven Angels + FCSs\"}" },
                    { "RandomSquad5", "{\"name\":\"RandomSquad5\",\"faction\":\"rebelalliance\",\"points\":199,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"bluesquadronescort\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronescort\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronescort\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronescort\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"hefftobber\",\"ship\":\"ut60duwing\",\"upgrades\":{\"configuration\":[\"pivotwing\"],\"talent\":[\"intimidation\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Blue Squadron Escort + Servomotor S-Foils (Open)\nBlue Squadron Escort + Servomotor S-Foils (Open)\nBlue Squadron Escort + Servomotor S-Foils (Open)\nBlue Squadron Escort + Servomotor S-Foils (Open)\nHeff Tobber + Pivot Wing (Open) + Intimidation\"}" },
                    { "RandomSquad6", "{\"name\":\"RandomSquad6\",\"faction\":\"rebelalliance\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"greensquadronpilot\",\"ship\":\"rz1awing\",\"upgrades\":{\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronpilot\",\"ship\":\"rz1awing\",\"upgrades\":{\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronpilot\",\"ship\":\"rz1awing\",\"upgrades\":{\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronpilot\",\"ship\":\"rz1awing\",\"upgrades\":{\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronpilot\",\"ship\":\"rz1awing\",\"upgrades\":{\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronpilot\",\"ship\":\"rz1awing\",\"upgrades\":{\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Green Squadron Pilot + Elusive\nGreen Squadron Pilot + Elusive\nGreen Squadron Pilot + Elusive\nGreen Squadron Pilot + Elusive\nGreen Squadron Pilot + Elusive\nGreen Squadron Pilot + Elusive\"}" },
                    { "RandomSquad7", "{\"name\":\"RandomSquad7\",\"faction\":\"rebelalliance\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"phoenixsquadronpilot\",\"ship\":\"rz1awing\",\"upgrades\":{\"missile\":[\"protonrockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Phoenix Squadron\"}}},{\"id\":\"phoenixsquadronpilot\",\"ship\":\"rz1awing\",\"upgrades\":{\"missile\":[\"protonrockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Phoenix Squadron\"}}},{\"id\":\"phoenixsquadronpilot\",\"ship\":\"rz1awing\",\"upgrades\":{\"missile\":[\"protonrockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Phoenix Squadron\"}}},{\"id\":\"phoenixsquadronpilot\",\"ship\":\"rz1awing\",\"upgrades\":{\"missile\":[\"protonrockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Phoenix Squadron\"}}},{\"id\":\"phoenixsquadronpilot\",\"ship\":\"rz1awing\",\"upgrades\":{\"missile\":[\"protonrockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Phoenix Squadron\"}}},{\"id\":\"phoenixsquadronpilot\",\"ship\":\"rz1awing\",\"upgrades\":{\"missile\":[\"protonrockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Phoenix Squadron\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Phoenix Squadron Pilot + Proton Rockets\nPhoenix Squadron Pilot + Proton Rockets\nPhoenix Squadron Pilot + Proton Rockets\nPhoenix Squadron Pilot + Proton Rockets\nPhoenix Squadron Pilot + Proton Rockets\nPhoenix Squadron Pilot + Proton Rockets\"}" },
                    { "RandomSquad8", "{\"name\":\"RandomSquad8\",\"faction\":\"rebelalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"wardensquadronpilot\",\"ship\":\"btls8kwing\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"wardensquadronpilot\",\"ship\":\"btls8kwing\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"wardensquadronpilot\",\"ship\":\"btls8kwing\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"wardensquadronpilot\",\"ship\":\"btls8kwing\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Warden Squadron Pilot + Barrage Rockets + Shield Upgrade\nWarden Squadron Pilot + Barrage Rockets + Shield Upgrade\nWarden Squadron Pilot + Barrage Rockets + Shield Upgrade\nWarden Squadron Pilot + Barrage Rockets + Shield Upgrade\"}" },
                    { "RandomSquad9", "{\"name\":\"RandomSquad9\",\"faction\":\"rebelalliance\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"lukeskywalker\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"],\"torpedo\":[\"protontorpedoes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Luke Skywalker\"}}},{\"id\":\"wedgeantilles\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"],\"torpedo\":[\"protontorpedoes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Wedge Antilles\"}}},{\"id\":\"thanekyrell\",\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"],\"torpedo\":[\"protontorpedoes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Luke Skywalker + Servomotor S-Foils (Open) + Proton Torpedoes\nWedge Antilles + Servomotor S-Foils (Open) + Proton Torpedoes\nThane Kyrell + Servomotor S-Foils (Open) + Proton Torpedoes\"}" },
                    { "RandomSquad10", "{\"name\":\"RandomSquad10\",\"faction\":\"galacticempire\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"inquisitor\",\"ship\":\"tieadvancedv1\",\"upgrades\":{\"force-power\":[\"brilliantevasion\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"White\"}}},{\"id\":\"inquisitor\",\"ship\":\"tieadvancedv1\",\"upgrades\":{\"force-power\":[\"brilliantevasion\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"White\"}}},{\"id\":\"inquisitor\",\"ship\":\"tieadvancedv1\",\"upgrades\":{\"force-power\":[\"brilliantevasion\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"White\"}}},{\"id\":\"inquisitor\",\"ship\":\"tieadvancedv1\",\"upgrades\":{\"force-power\":[\"brilliantevasion\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"White\"}}},{\"id\":\"inquisitor\",\"ship\":\"tieadvancedv1\",\"upgrades\":{\"force-power\":[\"brilliantevasion\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"White\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Inquisitor + Foresight\nInquisitor + Foresight\nInquisitor + Foresight\nInquisitor + Foresight\nInquisitor + Foresight\"}" },
                    { "RandomSquad11", "{\"name\":\"RandomSquad11\",\"faction\":\"galacticempire\",\"points\":199,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"howlrunner\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"idenversio\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inferno\"}}},{\"id\":\"obsidiansquadronpilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"obsidiansquadronpilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"obsidiansquadronpilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"obsidiansquadronpilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"obsidiansquadronpilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"\"Howlrunner\"\nIden Versio\nObsidian Squadron Pilot\nObsidian Squadron Pilot\nObsidian Squadron Pilot\nObsidian Squadron Pilot\nObsidian Squadron Pilot\"}" },
                    { "RandomSquad12", "{\"name\":\"RandomSquad12\",\"faction\":\"galacticempire\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"howlrunner\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"academypilot\",\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"\"Howlrunner\"\nAcademy Pilot\nAcademy Pilot\nAcademy Pilot\nAcademy Pilot\nAcademy Pilot\nAcademy Pilot\nAcademy Pilot\"}" },
                    { "RandomSquad13", "{\"name\":\"RandomSquad13\",\"faction\":\"galacticempire\",\"points\":199,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"captainjonus\",\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"scimitarsquadronpilot\",\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"scimitarsquadronpilot\",\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"scimitarsquadronpilot\",\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"scimitarsquadronpilot\",\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Captain Jonus + Barrage Rockets + Shield Upgrade\nScimitar Squadron Pilot + Barrage Rockets\nScimitar Squadron Pilot + Barrage Rockets\nScimitar Squadron Pilot + Barrage Rockets\nScimitar Squadron Pilot + Barrage Rockets\"}" },
                    { "RandomSquad14", "{\"name\":\"RandomSquad14\",\"faction\":\"galacticempire\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"gammasquadronace\",\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gamma Squadron\"}}},{\"id\":\"gammasquadronace\",\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gamma Squadron\"}}},{\"id\":\"gammasquadronace\",\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gamma Squadron\"}}},{\"id\":\"gammasquadronace\",\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gamma Squadron\"}}},{\"id\":\"gammasquadronace\",\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gamma Squadron\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Gamma Squadron Ace + Barrage Rockets + Hull Upgrade\nGamma Squadron Ace + Barrage Rockets + Hull Upgrade\nGamma Squadron Ace + Barrage Rockets\nGamma Squadron Ace + Barrage Rockets\nGamma Squadron Ace + Barrage Rockets\"}" },
                    { "RandomSquad15", "{\"name\":\"RandomSquad15\",\"faction\":\"galacticempire\",\"points\":195,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"stormsquadronace\",\"ship\":\"tieadvancedx1\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"stormsquadronace\",\"ship\":\"tieadvancedx1\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"stormsquadronace\",\"ship\":\"tieadvancedx1\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"stormsquadronace\",\"ship\":\"tieadvancedx1\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"stormsquadronace\",\"ship\":\"tieadvancedx1\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Storm Squadron Ace\nStorm Squadron Ace\nStorm Squadron Ace\nStorm Squadron Ace\nStorm Squadron Ace\"}" },
                    { "RandomSquad16", "{\"name\":\"RandomSquad16\",\"faction\":\"rebelalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"goldsquadronveteran\",\"ship\":\"btla4ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"astromech\":[\"r4astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"goldsquadronveteran\",\"ship\":\"btla4ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"astromech\":[\"r4astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"goldsquadronveteran\",\"ship\":\"btla4ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"astromech\":[\"r4astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"goldsquadronveteran\",\"ship\":\"btla4ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"astromech\":[\"r4astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Gold Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + R4 Astromech\nGold Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + R4 Astromech\nGold Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + R4 Astromech\nGold Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + R4 Astromech\"}" },
                    { "RandomSquad17", "{\"name\":\"RandomSquad17\",\"faction\":\"scumandvillainy\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"ig88c\",\"ship\":\"aggressorassaultfighter\",\"upgrades\":{\"title\":[\"ig2000\"],\"cannon\":[\"autoblasters\"],\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"ig88b\",\"ship\":\"aggressorassaultfighter\",\"upgrades\":{\"title\":[\"ig2000\"],\"cannon\":[\"autoblasters\"],\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"4lom\",\"ship\":\"g1astarfighter\",\"upgrades\":{\"crew\":[\"ig88d\"],\"title\":[\"misthunter\"],\"cannon\":[\"autoblasters\"],\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"G-1A Starfighter\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"IG-88C + IG-2000 + Autoblasters + Elusive\nIG-88B + IG-2000 + Autoblasters + Elusive\n4-LOM + IG-88D + Mist Hunter + Autoblasters + Elusive\"}" },
                    { "RandomSquad18", "{\"name\":\"RandomSquad18\",\"faction\":\"scumandvillainy\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"skullsquadronpilot\",\"ship\":\"fangfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Skull Squadron\"}}},{\"id\":\"skullsquadronpilot\",\"ship\":\"fangfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Skull Squadron\"}}},{\"id\":\"skullsquadronpilot\",\"ship\":\"fangfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Skull Squadron\"}}},{\"id\":\"skullsquadronpilot\",\"ship\":\"fangfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Skull Squadron\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Skull Squadron Pilot + Fearless\nSkull Squadron Pilot + Fearless\nSkull Squadron Pilot + Fearless\nSkull Squadron Pilot + Fearless\"}" },
                    { "RandomSquad19", "{\"name\":\"RandomSquad19\",\"faction\":\"scumandvillainy\",\"points\":197,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"fennrau\",\"ship\":\"fangfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Zealous Recruit\"}}},{\"id\":\"oldteroch\",\"ship\":\"fangfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Zealous Recruit\"}}},{\"id\":\"guri\",\"ship\":\"starviperclassattackplatform\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun Enforcer\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Fenn Rau (Fang Fighter) + Fearless\nOld Teroch + Fearless\nGuri + Fearless\"}" },
                    { "RandomSquad20", "{\"name\":\"RandomSquad20\",\"faction\":\"scumandvillainy\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"graz\",\"ship\":\"kihraxzfighter\",\"upgrades\":{\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}},{\"id\":\"blacksunace\",\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun (White)\"}}},{\"id\":\"blacksunace\",\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun (White)\"}}},{\"id\":\"blacksunace\",\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun (White)\"}}},{\"id\":\"blacksunace\",\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun (White)\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Graz + Elusive\nBlack Sun Ace\nBlack Sun Ace\nBlack Sun Ace\nBlack Sun Ace\"}" },
                    { "RandomSquad21", "{\"name\":\"RandomSquad21\",\"faction\":\"scumandvillainy\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"tansariipointveteran\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"cartelspacer\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Tansarii Point Veteran + Heavy Laser Cannon\nTansarii Point Veteran + Heavy Laser Cannon\nTansarii Point Veteran + Heavy Laser Cannon\nTansarii Point Veteran + Heavy Laser Cannon\nTansarii Point Veteran + Heavy Laser Cannon\nCartel Spacer + Heavy Laser Cannon\"}" },
                    { "RandomSquad22", "{\"name\":\"RandomSquad22\",\"faction\":\"scumandvillainy\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"tansariipointveteran\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Tansarii Point Veteran + Cluster Missiles\nTansarii Point Veteran + Cluster Missiles\nTansarii Point Veteran + Cluster Missiles\nTansarii Point Veteran + Cluster Missiles\nTansarii Point Veteran + Cluster Missiles\nTansarii Point Veteran + Cluster Missiles\"}" },
                    { "RandomSquad23", "{\"name\":\"RandomSquad23\",\"faction\":\"scumandvillainy\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"cartelmarauder\",\"ship\":\"kihraxzfighter\",\"upgrades\":{\"illicit\":[\"falsetranspondercodes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}},{\"id\":\"cartelmarauder\",\"ship\":\"kihraxzfighter\",\"upgrades\":{\"illicit\":[\"falsetranspondercodes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}},{\"id\":\"cartelmarauder\",\"ship\":\"kihraxzfighter\",\"upgrades\":{\"illicit\":[\"falsetranspondercodes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}},{\"id\":\"cartelmarauder\",\"ship\":\"kihraxzfighter\",\"upgrades\":{\"illicit\":[\"falsetranspondercodes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}},{\"id\":\"cartelmarauder\",\"ship\":\"kihraxzfighter\",\"upgrades\":{\"illicit\":[\"falsetranspondercodes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"5 Cartel Maradeurs + FCSs\"}" },
                    { "RandomSquad24", "{\"name\":\"RandomSquad24\",\"faction\":\"scumandvillainy\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"drearenthal\",\"ship\":\"btla4ywing\",\"upgrades\":{\"turret\":[\"dorsalturret\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"cartelmarauder\",\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}},{\"id\":\"cartelmarauder\",\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}},{\"id\":\"cartelmarauder\",\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}},{\"id\":\"cartelmarauder\",\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Hutt Cartel\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Drea Renthal + Dorsal Turret + Shield Upgrade\nCartel Marauder\nCartel Marauder\nCartel Marauder\nCartel Marauder\"}" },
                    { "RandomSquad25", "{\"name\":\"RandomSquad25\",\"faction\":\"scumandvillainy\",\"points\":199,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"drearenthal\",\"ship\":\"btla4ywing\",\"upgrades\":{\"turret\":[\"dorsalturret\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"cartelspacer\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"cartelspacer\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"cartelspacer\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"cartelspacer\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"sunnybounder\",\"ship\":\"m3ainterceptor\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Drea Renthal + Scyks\"}" },
                    { "RandomSquad26", "{\"name\":\"RandomSquad26\",\"faction\":\"scumandvillainy\",\"points\":199,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"drearenthal\",\"ship\":\"btla4ywing\",\"upgrades\":{\"turret\":[\"dorsalturret\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"cartelspacer\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"cartelspacer\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"cartelspacer\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"cartelspacer\",\"ship\":\"m3ainterceptor\",\"upgrades\":{\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"sunnybounder\",\"ship\":\"m3ainterceptor\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Drea Renthal + Cluster Scyks\"}" },
                    { "RandomSquad27", "{\"name\":\"RandomSquad27\",\"faction\":\"scumandvillainy\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"miningguildsurveyor\",\"ship\":\"modifiedtielnfighter\",\"upgrades\":{\"talent\":[\"margsablclosure\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Mining Guild Yellow\"}}},{\"id\":\"miningguildsurveyor\",\"ship\":\"modifiedtielnfighter\",\"upgrades\":{\"talent\":[\"margsablclosure\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Mining Guild Yellow\"}}},{\"id\":\"miningguildsurveyor\",\"ship\":\"modifiedtielnfighter\",\"upgrades\":{\"talent\":[\"margsablclosure\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Mining Guild Yellow\"}}},{\"id\":\"miningguildsurveyor\",\"ship\":\"modifiedtielnfighter\",\"upgrades\":{\"talent\":[\"margsablclosure\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Mining Guild Yellow\"}}},{\"id\":\"miningguildsurveyor\",\"ship\":\"modifiedtielnfighter\",\"upgrades\":{\"talent\":[\"margsablclosure\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Mining Guild Yellow\"}}},{\"id\":\"miningguildsurveyor\",\"ship\":\"modifiedtielnfighter\",\"upgrades\":{\"talent\":[\"margsablclosure\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Mining Guild Yellow\"}}},{\"id\":\"miningguildsurveyor\",\"ship\":\"modifiedtielnfighter\",\"upgrades\":{\"talent\":[\"margsablclosure\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Mining Guild Yellow\"}}},{\"id\":\"miningguildsurveyor\",\"ship\":\"modifiedtielnfighter\",\"upgrades\":{\"talent\":[\"margsablclosure\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Mining Guild Yellow\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Mining Guild Surveyor + Marg Sabl Closure + Shield Upgrade\nMining Guild Surveyor + Marg Sabl Closure\nMining Guild Surveyor + Marg Sabl Closure\nMining Guild Surveyor + Marg Sabl Closure\nMining Guild Surveyor + Marg Sabl Closure\nMining Guild Surveyor + Marg Sabl Closure\nMining Guild Surveyor + Marg Sabl Closure\nMining Guild Surveyor + Marg Sabl Closure\"}" },
                    { "RandomSquad28", "{\"name\":\"RandomSquad28\",\"faction\":\"scumandvillainy\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"blacksunassassin\",\"ship\":\"starviperclassattackplatform\",\"upgrades\":{\"talent\":[\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun Assassin\"}}},{\"id\":\"blacksunassassin\",\"ship\":\"starviperclassattackplatform\",\"upgrades\":{\"talent\":[\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun Assassin\"}}},{\"id\":\"blacksunassassin\",\"ship\":\"starviperclassattackplatform\",\"upgrades\":{\"talent\":[\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun Assassin\"}}},{\"id\":\"blacksunassassin\",\"ship\":\"starviperclassattackplatform\",\"upgrades\":{\"talent\":[\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun Assassin\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"4x Black Sun Assassin + Marksmanship\"}" },
                    { "RandomSquad29", "{\"name\":\"RandomSquad29\",\"faction\":\"resistance\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"bluesquadronrecruit\",\"ship\":\"rz2awing\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronrecruit\",\"ship\":\"rz2awing\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronrecruit\",\"ship\":\"rz2awing\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronrecruit\",\"ship\":\"rz2awing\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronrecruit\",\"ship\":\"rz2awing\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronrecruit\",\"ship\":\"rz2awing\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Blue Squadron Recruit\nBlue Squadron Recruit\nBlue Squadron Recruit\nBlue Squadron Recruit\nBlue Squadron Recruit\nBlue Squadron Recruit\"}" },
                    { "RandomSquad30", "{\"name\":\"RandomSquad30\",\"faction\":\"resistance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"greensquadronexpert\",\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"],\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronexpert\",\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"],\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronexpert\",\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"],\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronexpert\",\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"],\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronexpert\",\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"],\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Green Squadron Expert + Heroic + Cluster Missiles\nGreen Squadron Expert + Heroic + Cluster Missiles\nGreen Squadron Expert + Heroic + Cluster Missiles\nGreen Squadron Expert + Heroic + Cluster Missiles\nGreen Squadron Expert + Heroic + Cluster Missiles\"}" },
                    { "RandomSquad31", "{\"name\":\"RandomSquad31\",\"faction\":\"resistance\",\"points\":196,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"blacksquadronace-t70xwing\",\"ship\":\"t70xwing\",\"upgrades\":{\"configuration\":[\"integratedsfoils\"],\"talent\":[\"heroic\"],\"astromech\":[\"r4astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black One\"}}},{\"id\":\"blacksquadronace-t70xwing\",\"ship\":\"t70xwing\",\"upgrades\":{\"configuration\":[\"integratedsfoils\"],\"talent\":[\"heroic\"],\"astromech\":[\"r4astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black One\"}}},{\"id\":\"blacksquadronace-t70xwing\",\"ship\":\"t70xwing\",\"upgrades\":{\"configuration\":[\"integratedsfoils\"],\"talent\":[\"heroic\"],\"astromech\":[\"r4astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black One\"}}},{\"id\":\"blacksquadronace-t70xwing\",\"ship\":\"t70xwing\",\"upgrades\":{\"configuration\":[\"integratedsfoils\"],\"talent\":[\"heroic\"],\"astromech\":[\"r4astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black One\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Black Squadron Ace (T-70 X-wing) + Integrated S-Foils (Open) + Heroic + R4 Astromech\nBlack Squadron Ace (T-70 X-wing) + Integrated S-Foils (Open) + Heroic + R4 Astromech\nBlack Squadron Ace (T-70 X-wing) + Integrated S-Foils (Open) + Heroic + R4 Astromech\nBlack Squadron Ace (T-70 X-wing) + Integrated S-Foils (Open) + Heroic + R4 Astromech\"}" },
                    { "RandomSquad32", "{\"name\":\"RandomSquad32\",\"faction\":\"firstorder\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"epsilonsquadroncadet\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronace\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronace\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronace\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronace\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronace\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronace\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Epsilon Squadron Cadet + 6x Omega Squadron Ace\"}" },
                    { "RandomSquad33", "{\"name\":\"RandomSquad33\",\"faction\":\"firstorder\",\"points\":197,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"omegasquadronexpert\",\"ship\":\"tiesffighter\",\"upgrades\":{\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronexpert\",\"ship\":\"tiesffighter\",\"upgrades\":{\"missile\":[\"clustermissiles\"],\"sensor\":[\"passivesensors\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronexpert\",\"ship\":\"tiesffighter\",\"upgrades\":{\"missile\":[\"clustermissiles\"],\"sensor\":[\"passivesensors\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronexpert\",\"ship\":\"tiesffighter\",\"upgrades\":{\"missile\":[\"clustermissiles\"],\"sensor\":[\"passivesensors\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronexpert\",\"ship\":\"tiesffighter\",\"upgrades\":{\"missile\":[\"clustermissiles\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"5x Omega Squadron Expert + Cluster Missiles + Passive Sensors\"}" },
                    { "RandomSquad34", "{\"name\":\"RandomSquad34\",\"faction\":\"firstorder\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"sienarjaemusengineer\",\"ship\":\"tievnsilencer\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black\"}}},{\"id\":\"sienarjaemusengineer\",\"ship\":\"tievnsilencer\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black\"}}},{\"id\":\"sienarjaemusengineer\",\"ship\":\"tievnsilencer\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black\"}}},{\"id\":\"sienarjaemusengineer\",\"ship\":\"tievnsilencer\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"4x Sienar-Jaemus Engineer\"}" },
                    { "RandomSquad35", "{\"name\":\"RandomSquad35\",\"faction\":\"firstorder\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"starkillerbasepilot\",\"ship\":\"upsilonclasscommandshuttle\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Upsilon-class Shuttle\"}}},{\"id\":\"starkillerbasepilot\",\"ship\":\"upsilonclasscommandshuttle\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Upsilon-class Shuttle\"}}},{\"id\":\"starkillerbasepilot\",\"ship\":\"upsilonclasscommandshuttle\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Upsilon-class Shuttle\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Starkiller Base Pilot + Heavy Laser Cannon + Hull Upgrade\nStarkiller Base Pilot + Heavy Laser Cannon + Hull Upgrade\nStarkiller Base Pilot + Heavy Laser Cannon + Hull Upgrade\"}" },
                    { "RandomSquad36", "{\"name\":\"RandomSquad36\",\"faction\":\"firstorder\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"commandermalarus-xiclasslightshuttle\",\"ship\":\"xiclasslightshuttle\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"epsilonsquadroncadet\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"epsilonsquadroncadet\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"epsilonsquadroncadet\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"epsilonsquadroncadet\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"epsilonsquadroncadet\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"epsilonsquadroncadet\",\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Commander Malarus (Xi-class Light Shuttle) + Hull Upgrade\nEpsilon Squadron Cadet\nEpsilon Squadron Cadet\nEpsilon Squadron Cadet\nEpsilon Squadron Cadet\nEpsilon Squadron Cadet\nEpsilon Squadron Cadet\"}" },
                    { "RandomSquad37", "{\"name\":\"RandomSquad37\",\"faction\":\"resistance\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"resistancesympathizer\",\"ship\":\"scavengedyt1300\",\"upgrades\":{\"crew\":[\"perceptivecopilot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"YT-1300\"}}},{\"id\":\"resistancesympathizer\",\"ship\":\"scavengedyt1300\",\"upgrades\":{\"crew\":[\"perceptivecopilot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"YT-1300\"}}},{\"id\":\"resistancesympathizer\",\"ship\":\"scavengedyt1300\",\"upgrades\":{\"crew\":[\"perceptivecopilot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"YT-1300\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Resistance Sympathizer + Perceptive Copilot\nResistance Sympathizer + Perceptive Copilot\nResistance Sympathizer + Perceptive Copilot\"}" },
                    { "RandomSquad38", "{\"name\":\"RandomSquad38\",\"faction\":\"separatistalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"tradefederationdrone\",\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Trade Federation Drone + Energy-Shell Charges\nTrade Federation Drone + Energy-Shell Charges\nTrade Federation Drone + Energy-Shell Charges\nTrade Federation Drone + Energy-Shell Charges\nTrade Federation Drone + Energy-Shell Charges\nTrade Federation Drone + Energy-Shell Charges\nTrade Federation Drone + Energy-Shell Charges\nTrade Federation Drone + Energy-Shell Charges\"}" },
                    { "RandomSquad39", "{\"name\":\"RandomSquad39\",\"faction\":\"separatistalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"petranakiarenaace\",\"ship\":\"nantexclassstarfighter\",\"upgrades\":{\"talent\":[\"elusive\",\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"petranakiarenaace\",\"ship\":\"nantexclassstarfighter\",\"upgrades\":{\"talent\":[\"elusive\",\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"petranakiarenaace\",\"ship\":\"nantexclassstarfighter\",\"upgrades\":{\"talent\":[\"elusive\",\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"petranakiarenaace\",\"ship\":\"nantexclassstarfighter\",\"upgrades\":{\"talent\":[\"elusive\",\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"petranakiarenaace\",\"ship\":\"nantexclassstarfighter\",\"upgrades\":{\"talent\":[\"elusive\",\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"5x Petranaki Arena Ace + Elusive + Marksmanship\"}" },
                    { "RandomSquad40", "{\"name\":\"RandomSquad40\",\"faction\":\"separatistalliance\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"separatistbomber\",\"ship\":\"hyenaclassdroidbomber\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"separatistbomber\",\"ship\":\"hyenaclassdroidbomber\",\"upgrades\":{\"missile\":[\"energyshellcharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"separatistbomber\",\"ship\":\"hyenaclassdroidbomber\",\"upgrades\":{\"missile\":[\"energyshellcharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"separatistbomber\",\"ship\":\"hyenaclassdroidbomber\",\"upgrades\":{\"missile\":[\"energyshellcharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"separatistbomber\",\"ship\":\"hyenaclassdroidbomber\",\"upgrades\":{\"missile\":[\"energyshellcharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"separatistbomber\",\"ship\":\"hyenaclassdroidbomber\",\"upgrades\":{\"missile\":[\"energyshellcharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Separatist Bomber + Energy-Shell Charges\nSeparatist Bomber + Energy-Shell Charges\nSeparatist Bomber + Energy-Shell Charges\nSeparatist Bomber + Energy-Shell Charges\nSeparatist Bomber + Energy-Shell Charges\nSeparatist Bomber + Energy-Shell Charges\"}" },
                    { "RandomSquad41", "{\"name\":\"RandomSquad41\",\"faction\":\"galacticempire\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"deltasquadronpilot\",\"ship\":\"tieddefender\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"deltasquadronpilot\",\"ship\":\"tieddefender\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"darthvader\",\"ship\":\"tieadvancedx1\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Darth Vader (TIE Advanced x1) + 2x Delta Squadron Pilot\"}" },
                    { "RandomSquad42", "{\"name\":\"RandomSquad42\",\"faction\":\"separatistalliance\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"separatistracketeer\",\"ship\":\"firesprayclasspatrolcraft\",\"upgrades\":{\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Jango Fett\"}}},{\"id\":\"separatistracketeer\",\"ship\":\"firesprayclasspatrolcraft\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Jango Fett\"}}},{\"id\":\"separatistracketeer\",\"ship\":\"firesprayclasspatrolcraft\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Jango Fett\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Separatist Racketeer + Shield Upgrade\nSeparatist Racketeer\nSeparatist Racketeer\"}" },
                    { "RandomSquad43", "{\"name\":\"RandomSquad43\",\"faction\":\"separatistalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"separatistinterceptor\",\"ship\":\"droidtrifighter\",\"upgrades\":{\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"separatistinterceptor\",\"ship\":\"droidtrifighter\",\"upgrades\":{\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"separatistinterceptor\",\"ship\":\"droidtrifighter\",\"upgrades\":{\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"separatistinterceptor\",\"ship\":\"droidtrifighter\",\"upgrades\":{\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"separatistinterceptor\",\"ship\":\"droidtrifighter\",\"upgrades\":{\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Separatist Interceptor + Elusive\nSeparatist Interceptor + Elusive\nSeparatist Interceptor + Elusive\nSeparatist Interceptor + Elusive\nSeparatist Interceptor + Elusive\"}" },
                    { "RandomSquad44", "{\"name\":\"RandomSquad44\",\"faction\":\"separatistalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"generalgrievous\",\"ship\":\"belbullab22starfighter\",\"upgrades\":{\"modification\":[\"imperviumplating\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"skakoanace\",\"ship\":\"belbullab22starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"skakoanace\",\"ship\":\"belbullab22starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"skakoanace\",\"ship\":\"belbullab22starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"skakoanace\",\"ship\":\"belbullab22starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"General Grievous + Impervium Plating\nSkakoan Ace\nSkakoan Ace\nSkakoan Ace\nSkakoan Ace\"}" },
                    { "RandomSquad45", "{\"name\":\"RandomSquad45\",\"faction\":\"galacticrepublic\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"squadsevenveteran\",\"ship\":\"arc170starfighter\",\"upgrades\":{\"gunner\":[\"veterantailgunner\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"squadsevenveteran\",\"ship\":\"arc170starfighter\",\"upgrades\":{\"gunner\":[\"veterantailgunner\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"squadsevenveteran\",\"ship\":\"arc170starfighter\",\"upgrades\":{\"gunner\":[\"veterantailgunner\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"squadsevenveteran\",\"ship\":\"arc170starfighter\",\"upgrades\":{\"gunner\":[\"veterantailgunner\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Squad Seven Veteran + Veteran Tail Gunner + Shield Upgrade\nSquad Seven Veteran + Veteran Tail Gunner + Shield Upgrade\nSquad Seven Veteran + Veteran Tail Gunner\nSquad Seven Veteran + Veteran Tail Gunner\"}" },
                    { "RandomSquad46", "{\"name\":\"RandomSquad46\",\"faction\":\"galacticrepublic\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"shadowsquadronveteran\",\"ship\":\"btlbywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"shadowsquadronveteran\",\"ship\":\"btlbywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"shadowsquadronveteran\",\"ship\":\"btlbywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"shadowsquadronveteran\",\"ship\":\"btlbywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"talent\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Shadow Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + Elusive\nShadow Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + Elusive\nShadow Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + Elusive\nShadow Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + Elusive\"}" },
                    { "RandomSquad47", "{\"name\":\"RandomSquad47\",\"faction\":\"galacticrepublic\",\"points\":195,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"jediknight\",\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"calibratedlasertargeting\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"jediknight\",\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"calibratedlasertargeting\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"jediknight\",\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"calibratedlasertargeting\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"jediknight\",\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"calibratedlasertargeting\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"jediknight\",\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"calibratedlasertargeting\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Jedi Knight + Calibrated Laser Targeting\nJedi Knight + Calibrated Laser Targeting\nJedi Knight + Calibrated Laser Targeting\nJedi Knight + Calibrated Laser Targeting\nJedi Knight + Calibrated Laser Targeting\"}" },
                    { "RandomSquad48", "{\"name\":\"RandomSquad48\",\"faction\":\"galacticrepublic\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"jediknight\",\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"delta7b\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"jediknight\",\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"delta7b\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"jediknight\",\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"delta7b\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"jediknight\",\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"delta7b\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Jedi Knight + Delta-7B + R4 Astromech\nJedi Knight + Delta-7B\nJedi Knight + Delta-7B + R4 Astromech\nJedi Knight + Delta-7B\"}" },
                    { "RandomSquad49", "{\"name\":\"RandomSquad49\",\"faction\":\"galacticrepublic\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"bravoflightofficer\",\"ship\":\"nabooroyaln1starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"bravoflightofficer\",\"ship\":\"nabooroyaln1starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"bravoflightofficer\",\"ship\":\"nabooroyaln1starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"bravoflightofficer\",\"ship\":\"nabooroyaln1starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"bravoflightofficer\",\"ship\":\"nabooroyaln1starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"bravoflightofficer\",\"ship\":\"nabooroyaln1starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Bravo Flight Officer\nBravo Flight Officer\nBravo Flight Officer\nBravo Flight Officer\nBravo Flight Officer\nBravo Flight Officer\"}" },
                    { "RandomSquad50", "{\"name\":\"RandomSquad50\",\"faction\":\"galacticrepublic\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"goldsquadrontrooper\",\"ship\":\"v19torrentstarfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"goldsquadrontrooper\",\"ship\":\"v19torrentstarfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"goldsquadrontrooper\",\"ship\":\"v19torrentstarfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"goldsquadrontrooper\",\"ship\":\"v19torrentstarfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"goldsquadrontrooper\",\"ship\":\"v19torrentstarfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"goldsquadrontrooper\",\"ship\":\"v19torrentstarfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"goldsquadrontrooper\",\"ship\":\"v19torrentstarfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"goldsquadrontrooper\",\"ship\":\"v19torrentstarfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Gold Squadron Trooper\nGold Squadron Trooper\nGold Squadron Trooper\nGold Squadron Trooper\nGold Squadron Trooper\nGold Squadron Trooper\nGold Squadron Trooper\nGold Squadron Trooper\"}" }
                };
            }
        }

        public override int MinShipCost(Faction faction)
        {
            switch (faction)
            {
                case Faction.Rebel:
                    return 22;
                case Faction.Imperial:
                    return 22;
                case Faction.Scum:
                    return (HasYv666InSquad()) ? 6 : 22;
                case Faction.Resistance:
                    return 26;
                case Faction.FirstOrder:
                    return 25;
                case Faction.Republic:
                    return 25;
                case Faction.Separatists:
                    return 20;
                default:
                    return 0;
            }
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
