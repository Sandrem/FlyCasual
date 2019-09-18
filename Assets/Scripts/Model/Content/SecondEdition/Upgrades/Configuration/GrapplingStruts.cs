using BoardTools;
using Obstacles;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class GrapplingStrutsClosed : GenericDualUpgrade
    {
        public GrapplingStrutsClosed() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Grappling Struts (Closed)",
                UpgradeType.Configuration,
                cost: 1,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.VultureClassDroidFighter.VultureClassDroidFighter)),
                abilityType: typeof(Abilities.SecondEdition.GrapplingStrutsClosedAbility)
            );

            AnotherSide = typeof(GrapplingStrutsOpen);
            SelectSideOnSetup = false;

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/ca74271f47c42b390ca0ba0b389144a5.png";
        }
    }

    public class GrapplingStrutsOpen : GenericDualUpgrade
    {
        public GrapplingStrutsOpen() : base()
        {
            IsHidden = true;

            UpgradeInfo = new UpgradeCardInfo(
                "Grappling Struts (Open)",
                UpgradeType.Configuration,
                cost: 1,
                restriction: new ShipRestriction(typeof(Ship.SecondEdition.VultureClassDroidFighter.VultureClassDroidFighter)),
                abilityType: typeof(Abilities.SecondEdition.GrapplingStrutsOpenAbility)
            );

            AnotherSide = typeof(GrapplingStrutsClosed);

            ImageUrl = "https://sb-cdn.fantasyflightgames.com/card_images/en/be82da17cfd39003fda380bead210eb9.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class GrapplingStrutsClosedAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            HostShip.OnMovementFinish += CheckAbility;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnMovementFinish -= CheckAbility;
        }

        private void CheckAbility(GenericShip ship)
        {
            if (ConditionsAreMet())
            {
                RegisterAbilityTrigger(TriggerTypes.OnMovementFinish, AskToUseOwnAbility);
            }
        }

        private void AskToUseOwnAbility(object sender, EventArgs e)
        {
            AskToUseAbility(
                HostUpgrade.UpgradeInfo.Name,
                NeverUseByDefault,
                FlipThisCard,
                descriptionLong: "Do you want to flip this card?",
                imageHolder: HostUpgrade,
                requiredPlayer: HostShip.Owner.PlayerNo
            );
        }

        private void FlipThisCard(object sender, EventArgs e)
        {
            DecisionSubPhase.ConfirmDecisionNoCallback();

            HostShip.TogglePeg(false);

            PlayLandingAnimation();

            HostShip.GetModelCenterTransform().localPosition -= new Vector3(0, 2, 0);
            HostShip.GetModelTransform().localPosition += new Vector3(0, 2, 0);
            HostShip.IsLandedModel = true;

            (HostUpgrade as GenericDualUpgrade).Flip();

            Triggers.FinishTrigger();
        }

        protected virtual void PlayLandingAnimation()
        {
            Animation animation = HostShip.GetModelTransform().Find("Vulture/Body").GetComponent<Animation>();
            animation.Play("Landing");
        }

        private bool ConditionsAreMet()
        {
            List<GenericObstacle> obstaclesLanded = HostShip.ObstaclesLanded.Where(n => n.GetTypeName == "Asteroid" || n.GetTypeName == "Debris").ToList();

            foreach (GenericObstacle obstacle in obstaclesLanded)
            {
                int friendlyShipsInRangeOfObstacle = 0;
                foreach (GenericShip friendlyShip in HostShip.Owner.Ships.Values)
                {
                    if (friendlyShip.ShipId == HostShip.ShipId) continue;

                    if (friendlyShip.ObstaclesLanded.Contains(obstacle)) friendlyShipsInRangeOfObstacle++;
                }

                if (friendlyShipsInRangeOfObstacle <= 1)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public class GrapplingStrutsOpenAbility : GenericAbility
    {
        List<GenericObstacle> IgnoreObstaclesList;

        public override void ActivateAbility()
        {
            IgnoreObstaclesList = new List<GenericObstacle>();
            IgnoreObstaclesList.AddRange(HostShip.ObstaclesLanded);

            HostShip.IgnoreObstaclesList.AddRange(IgnoreObstaclesList);
            HostShip.IsIgnoreObstaclesDuringBarrelRoll = true;

            HostShip.OnManeuverIsRevealed += CheckSpecialManeuvers;
            HostShip.OnMovementFinish += FlipThisCard;
        }

        public override void DeactivateAbility()
        {
            HostShip.OnManeuverIsRevealed -= CheckSpecialManeuvers;
            HostShip.OnMovementFinish -= FlipThisCard;

            HostShip.IgnoreObstaclesList.RemoveAll(n => IgnoreObstaclesList.Contains(n));
            HostShip.IsIgnoreObstaclesDuringBarrelRoll = false;
        }

        private void FlipThisCard(GenericShip ship)
        {
            HostShip.ObstaclesHit.RemoveAll(n => IgnoreObstaclesList.Contains(n));

            (HostUpgrade as GenericDualUpgrade).Flip();
        }

        private void CheckSpecialManeuvers(GenericShip ship)
        {
            if (HostShip.RevealedManeuver == null) return;

            if (!RevealedManeuverIs2FS() && IsOnObstacle())
            {
                RegisterOwnTrigger();
            }
            else
            {
                PlayTakeoffAnimation();

                HostShip.GetModelCenterTransform().localPosition += new Vector3(0, 2, 0);
                HostShip.GetModelTransform().localPosition -= new Vector3(0, 2, 0);
                HostShip.IsLandedModel = false;

                GameManagerScript.Wait(0.5f, delegate { HostShip.TogglePeg(true); });
            }
        }

        protected virtual void PlayTakeoffAnimation()
        {
            Animation animation = HostShip.GetModelTransform().Find("Vulture/Body").GetComponent<Animation>();
            animation.Play("Takeoff");
        }

        private void RegisterOwnTrigger()
        {
            RegisterAbilityTrigger(TriggerTypes.OnManeuverIsRevealed, SkipManeuverAndRemoveStress);
        }

        private void SkipManeuverAndRemoveStress(object sender, EventArgs e)
        {
            HostShip.IsManeuverSkipped = true;
            HostShip.Tokens.RemoveToken(typeof(Tokens.StressToken), CheckRotate);
        }

        private void CheckRotate()
        {
            if (HostShip.RevealedManeuver.Direction == Movement.ManeuverDirection.Left)
            {
                HostShip.Rotate90Counterclockwise(Triggers.FinishTrigger);
            }
            else if (HostShip.RevealedManeuver.Direction == Movement.ManeuverDirection.Right)
            {
                HostShip.Rotate90Clockwise(Triggers.FinishTrigger);
            }
            else
            {
                Triggers.FinishTrigger();
            }
        }

        private bool IsOnObstacle()
        {
            return HostShip.ObstaclesLanded.Any(n => n.GetTypeName == "Asteroid" || n.GetTypeName == "Debris");
        }

        private bool RevealedManeuverIs2FS()
        {
            return HostShip.RevealedManeuver.ManeuverSpeed == Movement.ManeuverSpeed.Speed2
                && HostShip.RevealedManeuver.Bearing == Movement.ManeuverBearing.Straight
                && HostShip.RevealedManeuver.Direction == Movement.ManeuverDirection.Forward;
        }

    }
}