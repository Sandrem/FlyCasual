using ActionsList;
using Arcs;
using BoardTools;
using Bombs;
using GameModes;
using Movement;
using Ship;
using SquadBuilderNS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
using UnityEngine;
using Upgrade;

namespace RuleSets
{
    interface ISecondEditionShip
    {
        void AdaptShipToSecondEdition();
    }

    interface ISecondEditionPilot
    {
        void AdaptPilotToSecondEdition();
    }

    interface ISecondEditionUpgrade
    {
        void AdaptUpgradeToSecondEdition();
    }

    public class SecondEdition : RuleSet
    {
        public override string Name { get { return "Second Edition"; } }
        //public override bool IsSquadBuilderLocked { get { return true; } }

        public override int MaxPoints { get { return 200; } }
        public override int MinShipCost { get { return 28; } }
        public override int MinShipsCount { get { return 1; } }
        public override int MaxShipsCount { get { return 8; } }
        public override string CombatPhaseName { get { return "Engagement"; } }
        public override Color MovementEasyColor { get { return new Color(0, 0.5f, 1); } }
        public override MovementComplexity IonManeuverComplexity { get { return MovementComplexity.Easy; } }
        public override string PathToSavedSquadrons { get { return "RandomAiSquadrons"; } }

        public override string RootUrlForImages { get { return "https://raw.githubusercontent.com/sandrem/xwing-data2-test/master/images/"; } }
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
                    { "Vader + Mini-Swarm",  "{\"name\":\"Vader + Mini-Swarm\",\"faction\":\"imperial\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"darthvader\",\"points\":76,\"ship\":\"tieadv\",\"upgrades\":{\"system\":[\"firecontrolsystem\"],\"missile\":[\"clustermissiles\"],\"force\":[\"sense\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Blue\"}}},{\"name\":\"blacksquadronace\",\"points\":31,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":31,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":31,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":31,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"description\":\"Darth Vader + Fire-Control System + Cluster Missiles + Sense\nBlack Squadron Ace + Crack Shot\nBlack Squadron Ace + Crack Shot\nBlack Squadron Ace + Crack Shot\nBlack Squadron Ace + Crack Shot\"}" },
                    { "Boba + Joy + Zealot",  "{\"name\":\"Boba + Joy + Zealot\",\"faction\":\"scum\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"bobafett\",\"points\":46,\"ship\":\"firespray31\",\"upgrades\":{\"bomb\":[\"seismiccharges\"],\"crew\":[\"perceptivecopilot\"],\"illicit\":[\"inertialdampeners\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Boba Fett\"}}},{\"name\":\"joyrekkoff\",\"points\":32,\"ship\":\"protectoratestarfighter\",\"upgrades\":{\"torpedo\":[\"iontorpedoes\"],\"ept\":[\"fearless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Protectorate Starfighter\"}}},{\"name\":\"zealousrecruit\",\"points\":20,\"ship\":\"protectoratestarfighter\",\"upgrades\":{},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Protectorate Starfighter\"}}}],\"description\":\"Boba Fett (Firespray-31) + Seismic Charges + Perceptive Copilot + Inertial Dampeners\nJoy Rekkoff + Ion Torpedoes + Fearless\nZealous Recruit\"}" },
                    { "Luke + Wedge + Gold", "{\"name\":\"Luke + Wedge + Gold\",\"faction\":\"rebel\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"lukeskywalker\",\"points\":69,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(open)\"],\"amd\":[\"r2d2\"],\"torpedo\":[\"protontorpedoes\"],\"force\":[\"instinctiveaim\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"wedgeantilles\",\"points\":81,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(open)\",\"afterburners\"],\"ept\":[\"predator\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"goldsquadronveteran\",\"points\":31,\"ship\":\"ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"amd\":[\"r5astromech\"],\"bomb\":[\"protonbombs\"],\"ept\":[\"experthandling\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}}],\"description\":\"Luke Skywalker + Servomotor S-Foils (Attack) + R2-D2 + Proton Torpedoes + Instinctive Aim\nWedge Antilles + Servomotor S-Foils (Attack) + AfterBurners + Predator\nGold Squadron Veteran + Ion Cannon Turret + R5 Astromech + Proton Bombs + Expert Handling\"}" },
                    { "Luke + Wedge + Dutch", "{\"name\":\"Luke + Wedge + Dutch\",\"faction\":\"rebel\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"lukeskywalker\",\"points\":69,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(open)\"],\"amd\":[\"r2d2\"],\"torpedo\":[\"protontorpedoes\"],\"force\":[\"instinctiveaim\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"wedgeantilles\",\"points\":81,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(open)\"],\"amd\":[\"r4astromech\"],\"ept\":[\"outmaneuver\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"dutchvander\",\"points\":31,\"ship\":\"ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"bomb\":[\"seismiccharges\"],\"ept\":[\"selfless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Yellow\"}}}],\"description\":\"Luke Skywalker + Servomotor S-Foils (Attack) + R2-D2 + Proton Torpedoes + Instinctive Aim\nWedge Antilles + Servomotor S-Foils (Attack) + R4 Astromech + Outmaneuver\n\\\"Dutch\\\" Vander + Ion Cannon Turret + Seismic Charges + Selfless\"}" },
                    { "Maarek + Krennic + Mini-Swarm", "{\"name\":\"Maarek + Krennic + Mini-Swarm\",\"faction\":\"imperial\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"maarekstele\",\"points\":7,\"ship\":\"tieadv\",\"upgrades\":{\"system\":[\"firecontrolsystem\"],\"ept\":[\"ruthless\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"captainferoph\",\"points\":32,\"ship\":\"tiereaper\",\"upgrades\":{\"crew\":[\"tacticalofficer\",\"directorkrennic\"],\"ept\":[\"elusive\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":31,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":31,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"blacksquadronace\",\"points\":31,\"ship\":\"tiefighter\",\"upgrades\":{\"ept\":[\"crackshot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}}],\"description\":\"Maarek Stele (TIE Advanced) + Fire-Control System + Ruthless\nCaptain Feroph + Tactical Officer + Director Krennic + Elusive\nBlack Squadron Ace + Crack Shot\nBlack Squadron Ace + Crack Shot\nBlack Squadron Ace + Crack Shot\"}" },
                    { "Two Tubes + Porkins + Gray Sq. Pilot", "{\"name\":\"Two Tubes + Porkins + Gray Sq. Pilot\",\"faction\":\"rebel\",\"points\":200,\"version\":\"0.3.0\",\"pilots\":[{\"name\":\"jekporkins\",\"points\":64,\"ship\":\"xwing\",\"upgrades\":{\"configuration\":[\"servomotorsfoils(open)\"],\"torpedo\":[\"protontorpedoes\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Red\"}}},{\"name\":\"benthictwotubes\",\"points\":30,\"ship\":\"uwing\",\"upgrades\":{\"configuration\":[\"pivotwing(open)\"],\"system\":[\"advsensors\"],\"crew\":[\"perceptivecopilot\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Partisan\"}}},{\"name\":\"graysquadronbomber\",\"points\":47,\"ship\":\"ywing\",\"upgrades\":{\"turret\":[\"ioncannonturret\"],\"bomb\":[\"seismiccharges\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Gray\"}}},{\"name\":\"edriotwotubes\",\"points\":60,\"ship\":\"xwing\",\"upgrades\":{\"mod\":[\"servomotorsfoils(open)\"]},\"vendor\":{\"Sandrem.FlyCasual\":{\"skin\":\"Partisan\"}}}],\"description\":\"Jek Porkins + Servomotor S-Foils (Attack) + Proton Torpedoes\nBenthic Two Tubes + Pivot Wing (Attack) + Advanced Sensors + Perceptive Copilot\nGray Squadron Bomber + Ion Cannon Turret + Seismic Charges\nEdrio Two Tubes + Servomotor S-Foils (Attack)\"}" }
                };
            }
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
                Messages.ShowError("Evade Token is spent, but no effect");
            }
        }

        public override void ActionIsFailed(GenericShip ship, Type actionType)
        {
            Messages.ShowError("Action is failed and skipped");

            base.ActionIsFailed(ship, actionType);

            Phases.Skip();
        }

        public override bool ShipIsAllowed(GenericShip ship)
        {
            return ship.ShipRuleType == typeof(SecondEdition);
        }

        public override bool PilotIsAllowed(GenericShip ship)
        {
            return ship.PilotRuleType == typeof(SecondEdition);
        }

        public override void AdaptShipToRules(GenericShip ship)
        {
            if (ship is ISecondEditionShip)
            {
                (ship as ISecondEditionShip).AdaptShipToSecondEdition();
                ship.ShipRuleType = typeof(SecondEdition);
            }
        }

        public override void AdaptPilotToRules(GenericShip ship)
        {
            if (ship is ISecondEditionPilot)
            {
                (ship as ISecondEditionPilot).AdaptPilotToSecondEdition();
                ship.PilotRuleType = typeof(SecondEdition);
            }
        }

        public override void AdaptUpgradeToRules(GenericUpgrade upgrade)
        {
            if (upgrade is ISecondEditionUpgrade)
            {
                (upgrade as ISecondEditionUpgrade).AdaptUpgradeToSecondEdition();
                upgrade.UpgradeRuleType = typeof(SecondEdition);
            }

            upgrade.SetChargesToMax();
        }

        public override void AdaptArcsToRules(GenericShip ship)
        {
            ship.ArcInfo.Arcs.Add(new ArcBullseye(ship.ShipBase));
        }

        public override bool WeaponHasRangeBonus()
        {
            return Combat.ChosenWeapon is PrimaryWeaponClass || (Combat.ChosenWeapon as GenericUpgrade).Types.Contains(UpgradeType.Cannon) || (Combat.ChosenWeapon as GenericUpgrade).Types.Contains(UpgradeType.Turret);
        }

        public override void SetShipBaseImage(GenericShip ship)
        {
            ship.SetShipBaseImageSecondEdition();
        }

        public override void RotateMobileFiringArc(ArcFacing facing)
        {
            Selection.ThisShip.ShowMobileFiringArcHighlight(facing);
        }

        public override void ActivateGenericUpgradeAbility(GenericUpgrade upgrade)
        {
            if (upgrade.Types.Contains(UpgradeType.Turret))
            {
                (upgrade as IShipWeapon).CanShootOutsideArc = false;

                upgrade.Host.ShipBaseArcsType = BaseArcsType.ArcMobile;
                upgrade.Host.InitializeShipBaseArc();
            }
        }

        public override void BarrelRollTemplatePlanning()
        {
            (Phases.CurrentSubPhase as SubPhases.BarrelRollPlanningSubPhase).PerfromTemplatePlanningSecondEdition();
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
            ship.OnTryAddAvailableAction += IonizedShipCanDoOnlyFocus;
            ship.OnRoundEnd += DisableIonizationActionEffect;
        }

        private void DisableIonizationActionEffect(GenericShip ship)
        {
            ship.OnTryAddAvailableAction -= IonizedShipCanDoOnlyFocus;
            ship.OnRoundEnd -= DisableIonizationActionEffect;
        }

        private void IonizedShipCanDoOnlyFocus(GenericAction action, ref bool canBePerformed)
        {
            canBePerformed = action is FocusAction;
        }

        public override bool ReinforceEffectCanBeUsed(ArcFacing facing)
        {
            return false;
        }

        public override bool ReinforcePostCombatEffectCanBeUsed(ArcFacing facing)
        {
            if (Combat.DiceRollAttack.Successes <= 1) return false;

            bool result = false;

            List<GenericArc> savedArcs = Combat.Defender.ArcInfo.Arcs;
            Combat.Defender.ArcInfo.Arcs = new List<GenericArc>() { new ArcSpecial180(Combat.Defender.ShipBase) };
            ShotInfo reverseShotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
            bool inForward180Arc = reverseShotInfo.InArc;

            Combat.Defender.ArcInfo.Arcs = new List<GenericArc>() { new ArcSpecial180Rear(Combat.Defender.ShipBase) };
            reverseShotInfo = new ShotInfo(Combat.Defender, Combat.Attacker, Combat.Defender.PrimaryWeapon);
            bool inRear180Arc = reverseShotInfo.InArc;

            Combat.Defender.ArcInfo.Arcs = savedArcs;

            result = (facing == ArcFacing.Front180) ? inForward180Arc && !inRear180Arc : !inForward180Arc && inRear180Arc;

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

    }
}
