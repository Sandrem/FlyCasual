﻿using Arcs;
using BoardTools;
using Bombs;
using Movement;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
using UnityEngine;
using Upgrade;

namespace RuleSets
{
    public class FirstEdition : Edition
    {
        public override string Name { get { return "FirstEdition"; } }

        public override int MaxPoints { get { return 100; } }
        public override int MinShipsCount { get { return 1; } }
        public override int MaxShipsCount { get { return 8; } }
        public override string CombatPhaseName { get { return "Combat"; } }
        public override Color MovementEasyColor { get { return Color.green; } }
        public override MovementComplexity IonManeuverComplexity { get { return MovementComplexity.Normal; } }
        public override string PathToSavedSquadrons { get { return "SavedSquadrons"; } }

        public override string RootUrlForImages { get { return "https://raw.githubusercontent.com/guidokessels/xwing-data/master/images/"; } }
        public override Vector2 UpgradeCardSize { get { return new Vector2(196, 300); }  }
        public override Vector2 UpgradeCardCompactOffset { get { return Vector2.zero; } }
        public override Vector2 UpgradeCardCompactSize { get { return UpgradeCardSize; } }

        public override Dictionary<Type, int> DamageDeckContent {
            get
            {
                return new Dictionary<Type, int>()
                {
                    { typeof(DamageDeckCardFE.DirectHit),           7 },
                    { typeof(DamageDeckCardFE.BlindedPilot),        2 },
                    { typeof(DamageDeckCardFE.DamagedCockpit),      2 },
                    { typeof(DamageDeckCardFE.DamagedEngine),       2 },
                    { typeof(DamageDeckCardFE.DamagedSensorArray),  2 },
                    { typeof(DamageDeckCardFE.LooseStabilizer),     2 },
                    { typeof(DamageDeckCardFE.MajorHullBreach),     2 },
                    { typeof(DamageDeckCardFE.ShakenPilot),         2 },
                    { typeof(DamageDeckCardFE.StructuralDamage),    2 },
                    { typeof(DamageDeckCardFE.ThrustControlFire),   2 },
                    { typeof(DamageDeckCardFE.WeaponsFailure),      2 },
                    { typeof(DamageDeckCardFE.ConsoleFire),         2 },
                    { typeof(DamageDeckCardFE.StunnedPilot),        2 },
                    { typeof(DamageDeckCardFE.MajorExplosion),      2 }
                };
            }
        }

        public override Dictionary<BaseSize, int> NegativeTokensToAffectShip {
            get
            {
                return new Dictionary<BaseSize, int>()
                {
                    { BaseSize.Small,   1 },
                    { BaseSize.Large,   2 }
                };
            }
        }

        public override Dictionary<string, string> PreGeneratedAiSquadrons
        {
            get
            {
                return new Dictionary<string, string>()
                {
                    { "BlueSquadron",       "{\"name\":\"Blue Squadron\",\"faction\":\"rebel\",\"points\":100,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"bluesquadronnovice\",\"points\":25,\"ship\":\"t70xwing\",\"upgrades\":{\"mod\":[\"integratedastromech\"],\"amd\":[\"r5astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"name\":\"bluesquadronnovice\",\"points\":25,\"ship\":\"t70xwing\",\"upgrades\":{\"mod\":[\"integratedastromech\"],\"amd\":[\"r5astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"name\":\"bluesquadronnovice\",\"points\":25,\"ship\":\"t70xwing\",\"upgrades\":{\"mod\":[\"integratedastromech\"],\"amd\":[\"r5astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"name\":\"bluesquadronnovice\",\"points\":25,\"ship\":\"t70xwing\",\"upgrades\":{\"mod\":[\"integratedastromech\"],\"amd\":[\"r5astromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}}],\"description\":\"Blue Squadron Novice + Integrated Astromech + R5 Astromech\nBlue Squadron Novice + Integrated Astromech + R5 Astromech\nBlue Squadron Novice + Integrated Astromech + R5 Astromech\nBlue Squadron Novice + Integrated Astromech + R5 Astromech\"}" },
                    { "ScurrgAlpha",        "{\"name\":\"Scurrg Alpha\",\"faction\":\"scum\",\"points\":99,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"lokrevenant\",\"points\":33,\"ship\":\"scurrgh6bomber\",\"upgrades\":{\"mod\":[\"guidancechips\"],\"turret\":[\"autoblasterturret\"],\"missile\":[\"harpoonmissiles\"],\"ept\":[\"deadeye\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Lok Revenant\"}}},{\"name\":\"lokrevenant\",\"points\":33,\"ship\":\"scurrgh6bomber\",\"upgrades\":{\"mod\":[\"guidancechips\"],\"turret\":[\"autoblasterturret\"],\"missile\":[\"harpoonmissiles\"],\"ept\":[\"deadeye\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Lok Revenant\"}}},{\"name\":\"lokrevenant\",\"points\":33,\"ship\":\"scurrgh6bomber\",\"upgrades\":{\"mod\":[\"guidancechips\"],\"turret\":[\"autoblasterturret\"],\"missile\":[\"harpoonmissiles\"],\"ept\":[\"deadeye\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Lok Revenant\"}}}],\"description\":\"Lok Revenant + Guidance Chips + Autoblaster Turret + Harpoon Missiles + Deadeye\nLok Revenant + Guidance Chips + Autoblaster Turret + Harpoon Missiles + Deadeye\nLok Revenant + Guidance Chips + Autoblaster Turret + Harpoon Missiles + Deadeye\"}}" },
                    { "StormSquadron",      "{\"name\":\"Storm Squadron\",\"faction\":\"imperial\",\"points\":96,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"stormsquadronpilot\",\"points\":24,\"ship\":\"tieadv\",\"upgrades\":{\"title\":[\"tiex1\"],\"system\":[\"advtargetingcomputer\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"stormsquadronpilot\",\"points\":24,\"ship\":\"tieadv\",\"upgrades\":{\"title\":[\"tiex1\"],\"system\":[\"advtargetingcomputer\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"stormsquadronpilot\",\"points\":24,\"ship\":\"tieadv\",\"upgrades\":{\"title\":[\"tiex1\"],\"system\":[\"advtargetingcomputer\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"stormsquadronpilot\",\"points\":24,\"ship\":\"tieadv\",\"upgrades\":{\"title\":[\"tiex1\"],\"system\":[\"advtargetingcomputer\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"description\":\"Storm Squadron Pilot + TIE/x1 + Adv. Targeting Computer\nStorm Squadron Pilot + TIE/x1 + Adv. Targeting Computer\nStorm Squadron Pilot + TIE/x1 + Adv. Targeting Computer\nStorm Squadron Pilot + TIE/x1 + Adv. Targeting Computer\"}" },
                    { "TacticalWookiees",   "{\"name\":\"Tactical Wookiees\",\"faction\":\"rebel\",\"points\":98,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"wookieeliberator\",\"points\":32,\"ship\":\"auzituckgunship\",\"upgrades\":{\"crew\":[\"tactician\"],\"ept\":[\"expertise\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Kashyyyk Defender\"}}},{\"name\":\"lowhhrick\",\"points\":34,\"ship\":\"auzituckgunship\",\"upgrades\":{\"crew\":[\"tactician\"],\"ept\":[\"expertise\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Lowhhrick\"}}},{\"name\":\"wookieeliberator\",\"points\":32,\"ship\":\"auzituckgunship\",\"upgrades\":{\"crew\":[\"tactician\"],\"ept\":[\"expertise\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Kashyyyk Defender\"}}}],\"description\":\"Wookiee Liberator + Tactician + Expertise\nLowhhrick + Tactician + Expertise\nWookiee Liberator + Tactician + Expertise\"}"},
                    { "ThugLife",           "{\"name\":\"Thug Life\",\"faction\":\"scum\",\"points\":100,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"syndicatethug\",\"points\":25,\"ship\":\"ywing\",\"upgrades\":{\"turret\":[\"twinlaserturret\"],\"samd\":[\"unhingedastromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Brown\"}}},{\"name\":\"syndicatethug\",\"points\":25,\"ship\":\"ywing\",\"upgrades\":{\"turret\":[\"twinlaserturret\"],\"samd\":[\"unhingedastromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Brown\"}}},{\"name\":\"syndicatethug\",\"points\":25,\"ship\":\"ywing\",\"upgrades\":{\"turret\":[\"twinlaserturret\"],\"samd\":[\"unhingedastromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Brown\"}}},{\"name\":\"syndicatethug\",\"points\":25,\"ship\":\"ywing\",\"upgrades\":{\"turret\":[\"twinlaserturret\"],\"samd\":[\"unhingedastromech\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Brown\"}}}],\"description\":\"Syndicate Thug + Twin Laser Turret + Unhinged Astromech\nSyndicate Thug + Twin Laser Turret + Unhinged Astromech\nSyndicate Thug + Twin Laser Turret + Unhinged Astromech\nSyndicate Thug + Twin Laser Turret + Unhinged Astromech\"}" },
                    { "TripleDefenders",    "{\"name\":\"Triple Defenders\",\"faction\":\"imperial\",\"points\":99,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"glaivesquadronpilot\",\"points\":33,\"ship\":\"tiedefender\",\"upgrades\":{\"title\":[\"tiex7\"],\"ept\":[\"veteraninstincts\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Crimson\"}}},{\"name\":\"glaivesquadronpilot\",\"points\":33,\"ship\":\"tiedefender\",\"upgrades\":{\"title\":[\"tiex7\"],\"ept\":[\"veteraninstincts\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Crimson\"}}},{\"name\":\"glaivesquadronpilot\",\"points\":33,\"ship\":\"tiedefender\",\"upgrades\":{\"title\":[\"tiex7\"],\"ept\":[\"veteraninstincts\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Crimson\"}}}],\"description\":\"Glaive Squadron Pilot + TIE/x7 + Veteran Instincts\nGlaive Squadron Pilot + TIE/x7 + Veteran Instincts\nGlaive Squadron Pilot + TIE/x7 + Veteran Instincts\"}" }
                };
            }
        }

        public override int MinShipCost(Faction faction)
        {
            return 12;
        }

        public override void EvadeDiceModification(DiceRoll diceRoll)
        {
            diceRoll.AddDice(DieSide.Success).ShowWithoutRoll();
        }

        public override bool PilotIsAllowed(GenericShip ship)
        {
            return ship.GetType().ToString().Contains(Name);
        }

        public override bool ShipIsAllowed(GenericShip ship)
        {
            return ship.GetType().ToString().Contains(Name);
        }

        public override bool WeaponHasRangeBonus()
        {
            return Combat.ChosenWeapon is PrimaryWeaponClass;
        }

        public override void SetShipBaseImage(GenericShip ship)
        {
            ship.SetShipBaseImageFirstEdition();
        }

        public override void BarrelRollTemplatePlanning()
        {
            (Phases.CurrentSubPhase as BarrelRollPlanningSubPhase).PerfromTemplatePlanningFirstEdition();
        }

        public override void DecloakTemplatePlanning()
        {
            (Phases.CurrentSubPhase as DecloakPlanningSubPhase).PerfromTemplatePlanningFirstEdition();
        }

        public override void SubScribeToGenericShipEvents(GenericShip ship)
        {
            ship.OnTryAddAvailableDiceModification += Rules.BullseyeArc.CheckBullseyeArc;
        }

        public override void ReloadAction()
        {
            ActionsList.ReloadAction.FlipFaceupRecursive();
        }

        public override bool ReinforceEffectCanBeUsed(ArcFacing facing)
        {
            bool result = false;

            ShotInfo reverseShotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
            result = (facing == ArcFacing.FullFront) ? reverseShotInfo.InArc : !reverseShotInfo.InArc;

            return result;
        }

        public override bool ReinforcePostCombatEffectCanBeUsed(ArcFacing facing)
        {
            return false;
        }

        public override void TimedBombActivationTime(GenericShip ship)
        {
            ship.OnManeuverIsRevealed -= BombsManager.CheckBombDropAvailability;
            ship.OnManeuverIsRevealed += BombsManager.CheckBombDropAvailability;
        }

        public override void ActivateGenericUpgradeAbility(GenericUpgrade upgrade)
        {
            if (upgrade.UpgradeInfo.HasType(UpgradeType.Turret))
            {
                upgrade.HostShip.OnGameStart += delegate { upgrade.HostShip.ArcsInfo.GetArc<OutOfArc>().ShotPermissions.CanShootTurret = true; };
            }
        }

        public override void SquadBuilderIsOpened()
        {
            MainMenu.CurrentMainMenu.ChangePanel("SquadBuilderPanel");
        }

        public override bool IsTokenCanBeDiscardedByJam(GenericToken token)
        {
            List<Type> tokensTypesToDiscard = new List<Type> { typeof(FocusToken), typeof(EvadeToken), typeof(BlueTargetLockToken) };
            return tokensTypesToDiscard.Contains(token.GetType());
        }

        public override string GetPilotImageUrl(GenericShip ship, string filename)
        {
            return RootUrlForImages + "pilots/" + ImageUrls.FormatFaction(ship.SubFaction) + "/" + ImageUrls.FormatShipType(ship.ShipInfo.ShipName) + "/" + (filename ?? (ImageUrls.FormatName(ship.PilotInfo.PilotName) + ".png"));
        }

        public override string GetUpgradeImageUrl(GenericUpgrade upgrade)
        {
            return RootUrlForImages + "upgrades/" + ImageUrls.FormatUpgradeTypes(upgrade.UpgradeInfo.UpgradeTypes) + "/" + ImageUrls.FormatName(ImageUrls.FormatUpgradeName(upgrade.UpgradeInfo.Name)) + ".png";
        }
    }
}
