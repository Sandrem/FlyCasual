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
                    // WARNING: when updating squads, make sure to leave the names unchanged!
                    // REBELS
                    { "5 X-Wings",  "{\"name\":\"5 X-Wings\",\"faction\":\"rebelalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"bluesquadronescort\",\"points\":40,\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"bluesquadronescort\",\"points\":40,\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"bluesquadronescort\",\"points\":40,\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"bluesquadronescort\",\"points\":40,\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"bluesquadronescort\",\"points\":40,\"ship\":\"t65xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Blue Squadron Escort + Servomotor S-Foils (Open)\nBlue Squadron Escort + Servomotor S-Foils (Open)\nBlue Squadron Escort + Servomotor S-Foils (Open)\nBlue Squadron Escort + Servomotor S-Foils (Open)\nBlue Squadron Escort + Servomotor S-Foils (Open)\"}" },
                    { "Barrage K-Wings",  "{\"name\":\"Barrage K-Wings\",\"faction\":\"rebelalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"wardensquadronpilot\",\"points\":50,\"ship\":\"btls8kwing\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"wardensquadronpilot\",\"points\":50,\"ship\":\"btls8kwing\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"wardensquadronpilot\",\"points\":50,\"ship\":\"btls8kwing\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"wardensquadronpilot\",\"points\":50,\"ship\":\"btls8kwing\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Warden Squadron Pilot + Barrage Rockets + Shield Upgrade\nWarden Squadron Pilot + Barrage Rockets + Shield Upgrade\nWarden Squadron Pilot + Barrage Rockets + Shield Upgrade\nWarden Squadron Pilot + Barrage Rockets + Shield Upgrade\"}" },
                    { "Crackshot A-Wings",  "{\"name\":\"Crackshot A-Wings\",\"faction\":\"rebelalliance\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"greensquadronpilot\",\"points\":33,\"ship\":\"rz1awing\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronpilot\",\"points\":33,\"ship\":\"rz1awing\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronpilot\",\"points\":33,\"ship\":\"rz1awing\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronpilot\",\"points\":33,\"ship\":\"rz1awing\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronpilot\",\"points\":33,\"ship\":\"rz1awing\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronpilot\",\"points\":33,\"ship\":\"rz1awing\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Green Squadron Pilot + Crack Shot\nGreen Squadron Pilot + Crack Shot\nGreen Squadron Pilot + Crack Shot\nGreen Squadron Pilot + Crack Shot\nGreen Squadron Pilot + Crack Shot\nGreen Squadron Pilot + Crack Shot\"}" },
                    { "Death Star Run",  "{\"name\":\"Death Star Run\",\"faction\":\"rebelalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"lukeskywalker\",\"points\":76,\"ship\":\"t65xwing\",\"upgrades\":{\"torpedo\":[\"protontorpedoes\"],\"configuration\":[\"servomotorsfoils\"],\"force-power\":[\"instinctiveaim\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"wedgeantilles\",\"points\":68,\"ship\":\"t65xwing\",\"upgrades\":{\"torpedo\":[\"protontorpedoes\"],\"configuration\":[\"servomotorsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"dutchvander\",\"points\":56,\"ship\":\"btla4ywing\",\"upgrades\":{\"torpedo\":[\"protontorpedoes\"],\"turret\":[\"dorsalturret\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Luke Skywalker + Proton Torpedoes + Servomotor S-Foils (Open) + Instinctive Aim\nWedge Antilles + Proton Torpedoes + Servomotor S-Foils (Open)\n\\\"Dutch\\\" Vander + Proton Torpedoes + Dorsal Turret\"}" },
                    { "E-Wing Swarm",  "{\"name\":\"E-Wing Swarm\",\"faction\":\"rebelalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"knavesquadronescort\",\"points\":50,\"ship\":\"ewing\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"knavesquadronescort\",\"points\":50,\"ship\":\"ewing\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"knavesquadronescort\",\"points\":50,\"ship\":\"ewing\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"knavesquadronescort\",\"points\":50,\"ship\":\"ewing\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Knave Squadron Escort\nKnave Squadron Escort\nKnave Squadron Escort\nKnave Squadron Escort\"}" },
                    { "Gold Squadron Ion Turrets",  "{\"name\":\"Gold Squadron Ion Turrets\",\"faction\":\"rebelalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"goldsquadronveteran\",\"points\":50,\"ship\":\"btla4ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"goldsquadronveteran\",\"points\":50,\"ship\":\"btla4ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"goldsquadronveteran\",\"points\":50,\"ship\":\"btla4ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"goldsquadronveteran\",\"points\":50,\"ship\":\"btla4ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Gold Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + Hull Upgrade\nGold Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + Hull Upgrade\nGold Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + Hull Upgrade\nGold Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + Hull Upgrade\"}" },
                    { "Proton E-Wings",  "{\"name\":\"Proton E-Wings\",\"faction\":\"rebelalliance\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"roguesquadronescort\",\"points\":66,\"ship\":\"ewing\",\"upgrades\":{\"torpedo\":[\"protontorpedoes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"roguesquadronescort\",\"points\":66,\"ship\":\"ewing\",\"upgrades\":{\"torpedo\":[\"protontorpedoes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"roguesquadronescort\",\"points\":66,\"ship\":\"ewing\",\"upgrades\":{\"torpedo\":[\"protontorpedoes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Rogue Squadron Escort + Proton Torpedoes\nRogue Squadron Escort + Proton Torpedoes\nRogue Squadron Escort + Proton Torpedoes\"}" },
                    { "Selfless B-Wing Swarm",  "{\"name\":\"Selfless B-Wing Swarm\",\"faction\":\"rebelalliance\",\"points\":196,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"bladesquadronveteran\",\"points\":49,\"ship\":\"asf01bwing\",\"upgrades\":{\"talent\":[\"selfless\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bladesquadronveteran\",\"points\":49,\"ship\":\"asf01bwing\",\"upgrades\":{\"talent\":[\"selfless\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bladesquadronveteran\",\"points\":49,\"ship\":\"asf01bwing\",\"upgrades\":{\"talent\":[\"selfless\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bladesquadronveteran\",\"points\":49,\"ship\":\"asf01bwing\",\"upgrades\":{\"talent\":[\"selfless\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Blade Squadron Veteran + Selfless + Shield Upgrade\nBlade Squadron Veteran + Selfless + Shield Upgrade\nBlade Squadron Veteran + Selfless + Shield Upgrade\nBlade Squadron Veteran + Selfless + Shield Upgrade\"}" },
                    // GALACTIC EMPIRE
                    { "Barrage TIE SA Swarm",  "{\"name\":\"Barrage TIE SA Swarm\",\"faction\":\"galacticempire\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"scimitarsquadronpilot\",\"points\":40,\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"scimitarsquadronpilot\",\"points\":40,\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"scimitarsquadronpilot\",\"points\":40,\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"scimitarsquadronpilot\",\"points\":40,\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"scimitarsquadronpilot\",\"points\":40,\"ship\":\"tiesabomber\",\"upgrades\":{\"missile\":[\"barragerockets\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Scimitar Squadron Pilot + Barrage Rockets + Hull Upgrade\nScimitar Squadron Pilot + Barrage Rockets + Hull Upgrade\nScimitar Squadron Pilot + Barrage Rockets + Hull Upgrade\nScimitar Squadron Pilot + Barrage Rockets + Hull Upgrade\nScimitar Squadron Pilot + Barrage Rockets + Hull Upgrade\"}" },
                    { "TIE Advanced X1 Swarm",  "{\"name\":\"TIE Advanced Swarm\",\"faction\":\"galacticempire\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"stormsquadronace\",\"points\":40,\"ship\":\"tieadvancedx1\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"stormsquadronace\",\"points\":40,\"ship\":\"tieadvancedx1\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"stormsquadronace\",\"points\":40,\"ship\":\"tieadvancedx1\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"stormsquadronace\",\"points\":40,\"ship\":\"tieadvancedx1\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"stormsquadronace\",\"points\":40,\"ship\":\"tieadvancedx1\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Storm Squadron Ace + Crack Shot\nStorm Squadron Ace + Crack Shot\nStorm Squadron Ace + Crack Shot\nStorm Squadron Ace + Crack Shot\nStorm Squadron Ace + Crack Shot\"}" },
                    { "TIE Aggressor Ion Swarm",  "{\"name\":\"TIE Aggressor Ion Swarm\",\"faction\":\"galacticempire\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"sienarspecialist\",\"points\":44,\"ship\":\"tieagaggressor\",\"upgrades\":{\"gunner\":[\"agilegunner\"],\"turret\":[\"ioncannonturret\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"sienarspecialist\",\"points\":39,\"ship\":\"tieagaggressor\",\"upgrades\":{\"gunner\":[\"agilegunner\"],\"turret\":[\"ioncannonturret\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"sienarspecialist\",\"points\":39,\"ship\":\"tieagaggressor\",\"upgrades\":{\"gunner\":[\"agilegunner\"],\"turret\":[\"ioncannonturret\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"sienarspecialist\",\"points\":39,\"ship\":\"tieagaggressor\",\"upgrades\":{\"gunner\":[\"agilegunner\"],\"turret\":[\"ioncannonturret\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"sienarspecialist\",\"points\":39,\"ship\":\"tieagaggressor\",\"upgrades\":{\"gunner\":[\"agilegunner\"],\"turret\":[\"ioncannonturret\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Sienar Specialist + Agile Gunner + Ion Cannon Turret + Hull Upgrade\nSienar Specialist + Agile Gunner + Ion Cannon Turret\nSienar Specialist + Agile Gunner + Ion Cannon Turret\nSienar Specialist + Agile Gunner + Ion Cannon Turret\nSienar Specialist + Agile Gunner + Ion Cannon Turret\"}" },
                    { "TIE LN Swarm",  "{\"name\":\"TIE LN Swarm\",\"faction\":\"galacticempire\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"blacksquadronace\",\"points\":25,\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"blacksquadronace\",\"points\":25,\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"blacksquadronace\",\"points\":25,\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"blacksquadronace\",\"points\":25,\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"blacksquadronace\",\"points\":25,\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"blacksquadronace\",\"points\":25,\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"blacksquadronace\",\"points\":25,\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"id\":\"blacksquadronace\",\"points\":25,\"ship\":\"tielnfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Black Squadron Ace (TIE/ln Fighter)\nBlack Squadron Ace (TIE/ln Fighter)\nBlack Squadron Ace (TIE/ln Fighter)\nBlack Squadron Ace (TIE/ln Fighter)\nBlack Squadron Ace (TIE/ln Fighter)\nBlack Squadron Ace (TIE/ln Fighter)\nBlack Squadron Ace (TIE/ln Fighter)\nBlack Squadron Ace (TIE/ln Fighter)\"}" },
                    // SCUM
                    { "Crackshot Z-95 Swarm",  "{\"name\":\"Crackshot Z-95 Swarm\",\"faction\":\"scumandvillainy\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"blacksunsoldier\",\"points\":25,\"ship\":\"z95af4headhunter\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun\"}}},{\"id\":\"blacksunsoldier\",\"points\":25,\"ship\":\"z95af4headhunter\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun\"}}},{\"id\":\"blacksunsoldier\",\"points\":25,\"ship\":\"z95af4headhunter\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun\"}}},{\"id\":\"blacksunsoldier\",\"points\":25,\"ship\":\"z95af4headhunter\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun\"}}},{\"id\":\"blacksunsoldier\",\"points\":25,\"ship\":\"z95af4headhunter\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun\"}}},{\"id\":\"blacksunsoldier\",\"points\":25,\"ship\":\"z95af4headhunter\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun\"}}},{\"id\":\"blacksunsoldier\",\"points\":25,\"ship\":\"z95af4headhunter\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun\"}}},{\"id\":\"blacksunsoldier\",\"points\":25,\"ship\":\"z95af4headhunter\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Black Sun Soldier + Crack Shot\nBlack Sun Soldier + Crack Shot\nBlack Sun Soldier + Crack Shot\nBlack Sun Soldier + Crack Shot\nBlack Sun Soldier + Crack Shot\nBlack Sun Soldier + Crack Shot\nBlack Sun Soldier + Crack Shot\nBlack Sun Soldier + Crack Shot\"}" },
                    { "Fang Swarm",  "{\"name\":\"Fang Swarm\",\"faction\":\"scumandvillainy\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"skullsquadronpilot\",\"points\":50,\"ship\":\"fangfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Protectorate Starfighter\"}}},{\"id\":\"skullsquadronpilot\",\"points\":50,\"ship\":\"fangfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Protectorate Starfighter\"}}},{\"id\":\"skullsquadronpilot\",\"points\":50,\"ship\":\"fangfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Protectorate Starfighter\"}}},{\"id\":\"skullsquadronpilot\",\"points\":50,\"ship\":\"fangfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Protectorate Starfighter\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Skull Squadron Pilot + Fearless\nSkull Squadron Pilot + Fearless\nSkull Squadron Pilot + Fearless\nSkull Squadron Pilot + Fearless\"}" },
                    { "Fearless Aggressors",  "{\"name\":\"Fearless Aggressors\",\"faction\":\"scumandvillainy\",\"points\":199,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"ig88b\",\"points\":66,\"ship\":\"aggressorassaultfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"ig88d\",\"points\":66,\"ship\":\"aggressorassaultfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"ig88c\",\"points\":67,\"ship\":\"aggressorassaultfighter\",\"upgrades\":{\"talent\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"IG-88B + Fearless\nIG-88D + Fearless\nIG-88C + Fearless\"}" },
                    { "Heavy Scyk Swarm",  "{\"name\":\"Heavy Scyk Swarm\",\"faction\":\"scumandvillainy\",\"points\":199,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"tansariipointveteran\",\"points\":34,\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"points\":34,\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"points\":34,\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"points\":34,\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"points\":34,\"ship\":\"m3ainterceptor\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}},{\"id\":\"tansariipointveteran\",\"points\":29,\"ship\":\"m3ainterceptor\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Inaldra\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Tansarii Point Veteran + Heavy Laser Cannon\nTansarii Point Veteran + Heavy Laser Cannon\nTansarii Point Veteran + Heavy Laser Cannon\nTansarii Point Veteran + Heavy Laser Cannon\nTansarii Point Veteran + Heavy Laser Cannon\nTansarii Point Veteran\"}" },
                    { "Ion Scurrg Swarm",  "{\"name\":\"Ion Scurrg Swarm\",\"faction\":\"scumandvillainy\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"lokrevenant\",\"points\":50,\"ship\":\"scurrgh6bomber\",\"upgrades\":{\"turret\":[\"ioncannonturret\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Lok Revenant\"}}},{\"id\":\"lokrevenant\",\"points\":50,\"ship\":\"scurrgh6bomber\",\"upgrades\":{\"turret\":[\"ioncannonturret\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Lok Revenant\"}}},{\"id\":\"lokrevenant\",\"points\":50,\"ship\":\"scurrgh6bomber\",\"upgrades\":{\"turret\":[\"ioncannonturret\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Lok Revenant\"}}},{\"id\":\"lokrevenant\",\"points\":50,\"ship\":\"scurrgh6bomber\",\"upgrades\":{\"turret\":[\"ioncannonturret\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Lok Revenant\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Lok Revenant + Ion Cannon Turret\nLok Revenant + Ion Cannon Turret\nLok Revenant + Ion Cannon Turret\nLok Revenant + Ion Cannon Turret\"}" },
                    { "Kihraxz Swarm",  "{\"name\":\"Kihraxz Swarm\",\"faction\":\"scumandvillainy\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"blacksunace\",\"points\":40,\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun (White)\"}}},{\"id\":\"blacksunace\",\"points\":40,\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun (White)\"}}},{\"id\":\"blacksunace\",\"points\":40,\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun (White)\"}}},{\"id\":\"blacksunace\",\"points\":40,\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun (White)\"}}},{\"id\":\"blacksunace\",\"points\":40,\"ship\":\"kihraxzfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun (White)\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Black Sun Ace\nBlack Sun Ace\nBlack Sun Ace\nBlack Sun Ace\nBlack Sun Ace\"}" },
                    { "StarViper Swarm",  "{\"name\":\"StarViper Swarm\",\"faction\":\"scumandvillainy\",\"points\":196,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"blacksunassassin\",\"points\":49,\"ship\":\"starviperclassattackplatform\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun Assassin\"}}},{\"id\":\"blacksunassassin\",\"points\":49,\"ship\":\"starviperclassattackplatform\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun Assassin\"}}},{\"id\":\"blacksunassassin\",\"points\":49,\"ship\":\"starviperclassattackplatform\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun Assassin\"}}},{\"id\":\"blacksunassassin\",\"points\":49,\"ship\":\"starviperclassattackplatform\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black Sun Assassin\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Black Sun Assassin + Crack Shot\nBlack Sun Assassin + Crack Shot\nBlack Sun Assassin + Crack Shot\nBlack Sun Assassin + Crack Shot\"}" },
                    // RESISTANCE
                    { "3 YT-1300",  "{\"name\":\"3 YT-1300\",\"faction\":\"resistance\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"resistancesympathizer\",\"points\":66,\"ship\":\"scavengedyt1300\",\"upgrades\":{\"modification\":[\"shieldupgrade\"],\"illicit\":[\"deadmansswitch\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"YT-1300\"}}},{\"id\":\"resistancesympathizer\",\"points\":66,\"ship\":\"scavengedyt1300\",\"upgrades\":{\"modification\":[\"shieldupgrade\"],\"illicit\":[\"deadmansswitch\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"YT-1300\"}}},{\"id\":\"resistancesympathizer\",\"points\":66,\"ship\":\"scavengedyt1300\",\"upgrades\":{\"modification\":[\"shieldupgrade\"],\"illicit\":[\"deadmansswitch\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"YT-1300\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Resistance Sympathizer + Shield Upgrade + Deadman's Switch\nResistance Sympathizer + Shield Upgrade + Deadman's Switch\nResistance Sympathizer + Shield Upgrade + Deadman's Switch\"}" },
                    { "4 Heroic T-70 With Optics",  "{\"name\":\"4 Heroic T-70 With Optics\",\"faction\":\"resistance\",\"points\":196,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"redsquadronexpert\",\"points\":49,\"ship\":\"t70xwing\",\"upgrades\":{\"tech\":[\"advancedoptics\"],\"talent\":[\"heroic\"],\"configuration\":[\"integratedsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"redsquadronexpert\",\"points\":49,\"ship\":\"t70xwing\",\"upgrades\":{\"tech\":[\"advancedoptics\"],\"talent\":[\"heroic\"],\"configuration\":[\"integratedsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"redsquadronexpert\",\"points\":49,\"ship\":\"t70xwing\",\"upgrades\":{\"tech\":[\"advancedoptics\"],\"talent\":[\"heroic\"],\"configuration\":[\"integratedsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"redsquadronexpert\",\"points\":49,\"ship\":\"t70xwing\",\"upgrades\":{\"tech\":[\"advancedoptics\"],\"talent\":[\"heroic\"],\"configuration\":[\"integratedsfoils\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Red Squadron Expert + Advanced Optics + Heroic + Integrated S-Foils (Open)\nRed Squadron Expert + Advanced Optics + Heroic + Integrated S-Foils (Open)\nRed Squadron Expert + Advanced Optics + Heroic + Integrated S-Foils (Open)\nRed Squadron Expert + Advanced Optics + Heroic + Integrated S-Foils (Open)\"}" },
                    { "4 Heroic T-70",  "{\"name\":\"4 Heroic T-70\",\"faction\":\"resistance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"blacksquadronace-t70xwing\",\"points\":50,\"ship\":\"t70xwing\",\"upgrades\":{\"talent\":[\"heroic\"],\"configuration\":[\"integratedsfoils\"],\"astromech\":[\"r4astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black One\"}}},{\"id\":\"blacksquadronace-t70xwing\",\"points\":50,\"ship\":\"t70xwing\",\"upgrades\":{\"talent\":[\"heroic\"],\"configuration\":[\"integratedsfoils\"],\"astromech\":[\"r4astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black One\"}}},{\"id\":\"blacksquadronace-t70xwing\",\"points\":50,\"ship\":\"t70xwing\",\"upgrades\":{\"talent\":[\"heroic\"],\"configuration\":[\"integratedsfoils\"],\"astromech\":[\"r4astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black One\"}}},{\"id\":\"blacksquadronace-t70xwing\",\"points\":50,\"ship\":\"t70xwing\",\"upgrades\":{\"talent\":[\"heroic\"],\"configuration\":[\"integratedsfoils\"],\"astromech\":[\"r4astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Black One\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Black Squadron Ace (T-70 X-wing) + Heroic + Integrated S-Foils (Open) + R4 Astromech\nBlack Squadron Ace (T-70 X-wing) + Heroic + Integrated S-Foils (Open) + R4 Astromech\nBlack Squadron Ace (T-70 X-wing) + Heroic + Integrated S-Foils (Open) + R4 Astromech\nBlack Squadron Ace (T-70 X-wing) + Heroic + Integrated S-Foils (Open) + R4 Astromech\"}" },
                    { "Heroic A-wing Swarm + Optics",  "{\"name\":\"Heroic A-wing Swarm + Optics\",\"faction\":\"resistance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"greensquadronexpert\",\"points\":40,\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"],\"tech\":[\"advancedoptics\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronexpert\",\"points\":40,\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"],\"tech\":[\"advancedoptics\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronexpert\",\"points\":40,\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"],\"tech\":[\"advancedoptics\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronexpert\",\"points\":40,\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"],\"tech\":[\"advancedoptics\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}},{\"id\":\"greensquadronexpert\",\"points\":40,\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"],\"tech\":[\"advancedoptics\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Green\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Green Squadron Expert + Heroic + Advanced Optics\nGreen Squadron Expert + Heroic + Advanced Optics\nGreen Squadron Expert + Heroic + Advanced Optics\nGreen Squadron Expert + Heroic + Advanced Optics\nGreen Squadron Expert + Heroic + Advanced Optics\"}" },
                    { "Heroic A-Wing Swarm",  "{\"name\":\"Heroic A-Wing Swarm\",\"faction\":\"resistance\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"bluesquadronrecruit\",\"points\":33,\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronrecruit\",\"points\":33,\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronrecruit\",\"points\":33,\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronrecruit\",\"points\":33,\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronrecruit\",\"points\":33,\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"id\":\"bluesquadronrecruit\",\"points\":33,\"ship\":\"rz2awing\",\"upgrades\":{\"talent\":[\"heroic\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Blue Squadron Recruit + Heroic\nBlue Squadron Recruit + Heroic\nBlue Squadron Recruit + Heroic\nBlue Squadron Recruit + Heroic\nBlue Squadron Recruit + Heroic\nBlue Squadron Recruit + Heroic\"}" },
                    // FO
                    { "TIE FO Swarm",  "{\"name\":\"TIE FO Swarm\",\"faction\":\"firstorder\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"epsilonsquadroncadet\",\"points\":25,\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"epsilonsquadroncadet\",\"points\":25,\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"epsilonsquadroncadet\",\"points\":25,\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"epsilonsquadroncadet\",\"points\":25,\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"epsilonsquadroncadet\",\"points\":25,\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"epsilonsquadroncadet\",\"points\":25,\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"epsilonsquadroncadet\",\"points\":25,\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"epsilonsquadroncadet\",\"points\":25,\"ship\":\"tiefofighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Epsilon Squadron Cadet\nEpsilon Squadron Cadet\nEpsilon Squadron Cadet\nEpsilon Squadron Cadet\nEpsilon Squadron Cadet\nEpsilon Squadron Cadet\nEpsilon Squadron Cadet\nEpsilon Squadron Cadet\"}" },
                    { "TIE SF Swarm",  "{\"name\":\"TIE SF Swarm\",\"faction\":\"firstorder\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"omegasquadronexpert\",\"points\":50,\"ship\":\"tiesffighter\",\"upgrades\":{\"gunner\":[\"specialforcesgunner\"],\"modification\":[\"hullupgrade\"],\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronexpert\",\"points\":50,\"ship\":\"tiesffighter\",\"upgrades\":{\"gunner\":[\"specialforcesgunner\"],\"modification\":[\"hullupgrade\"],\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronexpert\",\"points\":50,\"ship\":\"tiesffighter\",\"upgrades\":{\"gunner\":[\"specialforcesgunner\"],\"modification\":[\"hullupgrade\"],\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}},{\"id\":\"omegasquadronexpert\",\"points\":50,\"ship\":\"tiesffighter\",\"upgrades\":{\"gunner\":[\"specialforcesgunner\"],\"modification\":[\"hullupgrade\"],\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"First Order\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Omega Squadron Expert + Special Forces Gunner + Hull Upgrade + Crack Shot\nOmega Squadron Expert + Special Forces Gunner + Hull Upgrade + Crack Shot\nOmega Squadron Expert + Special Forces Gunner + Hull Upgrade + Crack Shot\nOmega Squadron Expert + Special Forces Gunner + Hull Upgrade + Crack Shot\"}" },
                    { "Triple Upsion",  "{\"name\":\"Triple Upsion\",\"faction\":\"firstorder\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"starkillerbasepilot\",\"points\":67,\"ship\":\"upsilonclasscommandshuttle\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Upsilon-class Shuttle\"}}},{\"id\":\"starkillerbasepilot\",\"points\":66,\"ship\":\"upsilonclasscommandshuttle\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Upsilon-class Shuttle\"}}},{\"id\":\"starkillerbasepilot\",\"points\":67,\"ship\":\"upsilonclasscommandshuttle\",\"upgrades\":{\"cannon\":[\"heavylasercannon\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Upsilon-class Shuttle\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Starkiller Base Pilot + Heavy Laser Cannon + Shield Upgrade\nStarkiller Base Pilot + Heavy Laser Cannon + Hull Upgrade\nStarkiller Base Pilot + Heavy Laser Cannon + Shield Upgrade\"}" },
                    // REPUBLIC
                    { "ARC Swarm",  "{\"name\":\"ARC Swarm\",\"faction\":\"galacticrepublic\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"squadsevenveteran\",\"points\":52,\"ship\":\"arc170starfighter\",\"upgrades\":{\"gunner\":[\"veterantailgunner\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"squadsevenveteran\",\"points\":52,\"ship\":\"arc170starfighter\",\"upgrades\":{\"gunner\":[\"veterantailgunner\"],\"modification\":[\"shieldupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"squadsevenveteran\",\"points\":48,\"ship\":\"arc170starfighter\",\"upgrades\":{\"gunner\":[\"veterantailgunner\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"squadsevenveteran\",\"points\":48,\"ship\":\"arc170starfighter\",\"upgrades\":{\"gunner\":[\"veterantailgunner\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Squad Seven Veteran + Veteran Tail Gunner + Shield Upgrade\nSquad Seven Veteran + Veteran Tail Gunner + Shield Upgrade\nSquad Seven Veteran + Veteran Tail Gunner\nSquad Seven Veteran + Veteran Tail Gunner\"}" },
                    { "Delta Swarm",  "{\"name\":\"Delta Swarm\",\"faction\":\"galacticrepublic\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"jediknight\",\"points\":40,\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"calibratedlasertargeting\"],\"force-power\":[\"predictiveshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"jediknight\",\"points\":40,\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"calibratedlasertargeting\"],\"force-power\":[\"predictiveshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"jediknight\",\"points\":40,\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"calibratedlasertargeting\"],\"force-power\":[\"predictiveshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"jediknight\",\"points\":40,\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"calibratedlasertargeting\"],\"force-power\":[\"predictiveshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"id\":\"jediknight\",\"points\":40,\"ship\":\"delta7aethersprite\",\"upgrades\":{\"configuration\":[\"calibratedlasertargeting\"],\"force-power\":[\"predictiveshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Jedi Knight + Calibrated Laser Targeting + Predictive Shot\nJedi Knight + Calibrated Laser Targeting + Predictive Shot\nJedi Knight + Calibrated Laser Targeting + Predictive Shot\nJedi Knight + Calibrated Laser Targeting + Predictive Shot\nJedi Knight + Calibrated Laser Targeting + Predictive Shot\"}" },
                    { "Ion BLB-Y Squad",  "{\"name\":\"Ion BLB-Y Squad\",\"faction\":\"galacticrepublic\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"shadowsquadronveteran\",\"points\":50,\"ship\":\"btlbywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"shadowsquadronveteran\",\"points\":50,\"ship\":\"btlbywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"shadowsquadronveteran\",\"points\":50,\"ship\":\"btlbywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"shadowsquadronveteran\",\"points\":50,\"ship\":\"btlbywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"gunner\":[\"veteranturretgunner\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Shadow Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + Hull Upgrade\nShadow Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + Hull Upgrade\nShadow Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + Hull Upgrade\nShadow Squadron Veteran + Ion Cannon Turret + Veteran Turret Gunner + Hull Upgrade\"}" },
                    { "Naboo Swarm",  "{\"name\":\"Naboo Swarm\",\"faction\":\"galacticrepublic\",\"points\":198,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"bravoflightofficer\",\"points\":33,\"ship\":\"nabooroyaln1starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"bravoflightofficer\",\"points\":33,\"ship\":\"nabooroyaln1starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"bravoflightofficer\",\"points\":33,\"ship\":\"nabooroyaln1starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"bravoflightofficer\",\"points\":33,\"ship\":\"nabooroyaln1starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"bravoflightofficer\",\"points\":33,\"ship\":\"nabooroyaln1starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}},{\"id\":\"bravoflightofficer\",\"points\":33,\"ship\":\"nabooroyaln1starfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Bravo Flight Officer\nBravo Flight Officer\nBravo Flight Officer\nBravo Flight Officer\nBravo Flight Officer\nBravo Flight Officer\"}" },
                    // SEPARATISTS
                    { "Belbullab Swarm",  "{\"name\":\"Belbullab Swarm\",\"faction\":\"separatistalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"skakoanace\",\"points\":44,\"ship\":\"belbullab22starfighter\",\"upgrades\":{\"talent\":[\"crackshot\"],\"modification\":[\"hullupgrade\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"skakoanace\",\"points\":39,\"ship\":\"belbullab22starfighter\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"skakoanace\",\"points\":39,\"ship\":\"belbullab22starfighter\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"skakoanace\",\"points\":39,\"ship\":\"belbullab22starfighter\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"skakoanace\",\"points\":39,\"ship\":\"belbullab22starfighter\",\"upgrades\":{\"talent\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Skakoan Ace + Crack Shot + Hull Upgrade\nSkakoan Ace + Crack Shot\nSkakoan Ace + Crack Shot\nSkakoan Ace + Crack Shot\nSkakoan Ace + Crack Shot\"}" },
                    { "Energy Vulture Swarm",  "{\"name\":\"Energy Vulture Swarm\",\"faction\":\"separatistalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"tradefederationdrone\",\"points\":25,\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"],\"modification\":[\"munitionsfailsafe\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"points\":25,\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"],\"modification\":[\"munitionsfailsafe\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"points\":25,\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"],\"modification\":[\"munitionsfailsafe\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"points\":25,\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"],\"modification\":[\"munitionsfailsafe\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"points\":25,\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"],\"modification\":[\"munitionsfailsafe\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"points\":25,\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"],\"modification\":[\"munitionsfailsafe\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"points\":25,\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"],\"modification\":[\"munitionsfailsafe\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"tradefederationdrone\",\"points\":25,\"ship\":\"vultureclassdroidfighter\",\"upgrades\":{\"missile\":[\"energyshellcharges\"],\"modification\":[\"munitionsfailsafe\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Trade Federation Drone + Energy-Shell Charges + Munitions Failsafe\nTrade Federation Drone + Energy-Shell Charges + Munitions Failsafe\nTrade Federation Drone + Energy-Shell Charges + Munitions Failsafe\nTrade Federation Drone + Energy-Shell Charges + Munitions Failsafe\nTrade Federation Drone + Energy-Shell Charges + Munitions Failsafe\nTrade Federation Drone + Energy-Shell Charges + Munitions Failsafe\nTrade Federation Drone + Energy-Shell Charges + Munitions Failsafe\nTrade Federation Drone + Energy-Shell Charges + Munitions Failsafe\"}" },
                    { "Nantex Swarm",  "{\"name\":\"Nantex Swarm\",\"faction\":\"separatistalliance\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"id\":\"petranakiarenaace\",\"points\":40,\"ship\":\"nantexclassstarfighter\",\"upgrades\":{\"talent\":[\"crackshot\",\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"petranakiarenaace\",\"points\":40,\"ship\":\"nantexclassstarfighter\",\"upgrades\":{\"talent\":[\"crackshot\",\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"petranakiarenaace\",\"points\":40,\"ship\":\"nantexclassstarfighter\",\"upgrades\":{\"talent\":[\"crackshot\",\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"petranakiarenaace\",\"points\":40,\"ship\":\"nantexclassstarfighter\",\"upgrades\":{\"talent\":[\"crackshot\",\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}},{\"id\":\"petranakiarenaace\",\"points\":40,\"ship\":\"nantexclassstarfighter\",\"upgrades\":{\"talent\":[\"crackshot\",\"marksmanship\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Default\"}}}],\"obstacles\":[\"coreasteroid5\",\"core2asteroid5\",\"core2asteroid4\"],\"description\":\"Petranaki Arena Arce + Crack Shot + Marksmanship\nPetranaki Arena Arce + Crack Shot + Marksmanship\nPetranaki Arena Arce + Crack Shot + Marksmanship\nPetranaki Arena Arce + Crack Shot + Marksmanship\nPetranaki Arena Arce + Crack Shot + Marksmanship\"}" }
                };
            }
        }

        public override int MinShipCost(Faction faction)
        {
            switch (faction)
            {
                case Faction.Rebel:
                    return 23;
                case Faction.Imperial:
                    return 23;
                case Faction.Scum:
                    return (HasYv666InSquad()) ? 6 : 23;
                case Faction.Resistance:
                    return 32;
                case Faction.FirstOrder:
                    return 28;
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
                    // if (!overWrittenInstead) Messages.ShowError("Action is skipped");
                    Phases.CurrentSubPhase.IsReadyForCommands = true;
                    Phases.Skip();
                    if (Phases.CurrentSubPhase is ActivationSubPhase)
                    {
                        Selection.DeselectAllShips();
                        Phases.Next();
                    }
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
            return RootUrlForImages + "Card_Pilot_" + ship.PilotInfo.SEImageNumber + ".png";
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
