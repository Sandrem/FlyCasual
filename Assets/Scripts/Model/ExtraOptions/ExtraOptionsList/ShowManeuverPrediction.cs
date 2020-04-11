using Bombs;
using Movement;
using Obstacles;
using Remote;
using Ship;
using SubPhases;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ExtraOptions
{
    namespace ExtraOptionsList
    {
        public class ShowManeuverPrediction : ExtraOption
        {
            private static GameObject FakeShip;
            private static GenericShip Ship;
            private static MovementPrediction MovementPrediction;

            public ShowManeuverPrediction()
            {
                Name = "Show Maneuver Prediction";
                Description = "Click on your ship's assigned dial during Planning Phase to see prediction of it's final position and collisions";
            }

            protected override void Activate()
            {
                DebugManager.ManualCollisionPrediction = true;
            }

            protected override void Deactivate()
            {
                DebugManager.ManualCollisionPrediction = false;
            }

            public static void ShowPrediction(GenericShip ship)
            {
                if (DebugManager.ManualCollisionPrediction && Phases.CurrentSubPhase is SubPhases.PlanningSubPhase)
                {
                    StartSubPhase();
                    GameManagerScript.Instance.StartCoroutine(DoPredictionCoroutine(ship));
                }
            }

            private static void StartSubPhase()
            {
                GenericSubPhase subphase = Phases.StartTemporarySubPhaseNew<ManualCollisionPredictionSubPhase>(
                    "Manual collision prediction",
                    Finish
                );
                subphase.Start();
            }

            private static IEnumerator DoPredictionCoroutine(GenericShip ship)
            {
                Ship = ship;
                Selection.ChangeActiveShip(Ship);

                yield return PredictCurrentManeuver();
                ShowFakeShip();
                ShowCollisionResults();

                ShowNextButton();
            }

            private static IEnumerator PredictCurrentManeuver()
            {
                string maneuverCode = Ship.AssignedManeuver.ToString();
                GenericMovement movement = ShipMovementScript.MovementFromString(maneuverCode);

                Ship.SetAssignedManeuver(movement, isSilent: true);
                movement.Initialize();

                MovementPrediction = new MovementPrediction(movement);
                MovementPrediction.GenerateShipStands();
                yield return MovementPrediction.CalculateMovementPredicition();
            }

            private static void ShowFakeShip()
            {
                MovementTemplates.ApplyMovementRuler(Ship);

                FakeShip = GameObject.Instantiate<GameObject>(
                    Ship.Model,
                    MovementPrediction.FinalPosition,
                    new Quaternion() { eulerAngles = MovementPrediction.FinalAngles },
                    Ship.Model.transform.parent
                );

                ShowFakeShipSemiTransparent();
            }

            private static void ShowFakeShipSemiTransparent()
            {
                foreach (Renderer render in FakeShip.GetComponentsInChildren<Renderer>())
                {
                    try
                    {
                        StandardShaderUtils.ChangeRenderMode(render.material, StandardShaderUtils.BlendMode.Fade);

                        Color colorToChange = render.material.color;
                        colorToChange.a = 100f / 256f;
                        render.material.color = colorToChange;
                    }
                    catch (Exception) { }
                }
            }

            private static void ShowCollisionResults()
            {
                string title = "Results of collision detection";

                string description = "";

                if (MovementPrediction.AsteroidsHit.Count > 0)
                {
                    description += "Obstacles hit: ";
                    foreach (GenericObstacle obstacle in MovementPrediction.AsteroidsHit)
                    {
                        description += obstacle.Name + ", ";
                    }
                    description = description.Remove(description.Length - 2, 2);
                    description += "\n";
                }

                if (MovementPrediction.IsLandedOnAsteroid)
                {
                    description += "Landed on obstacles: ";
                    foreach (GenericObstacle obstacle in MovementPrediction.LandedOnObstacles)
                    {
                        description += obstacle.Name + ", ";
                    }
                    description = description.Remove(description.Length - 2, 2);
                    description += "\n";
                }

                if (MovementPrediction.MinesHit.Count > 0)
                {
                    description += "Mines hit: ";
                    foreach (GenericDeviceGameObject mine in MovementPrediction.MinesHit)
                    {
                        description += BombsManager.GetBombByObject(mine).UpgradeInfo.Name + ", ";
                    }
                    description = description.Remove(description.Length - 2, 2);
                    description += "\n";
                }

                if (MovementPrediction.ShipsBumped.Count > 0)
                {
                    description += "Bumped into ships: ";
                    foreach (GenericShip ship in MovementPrediction.ShipsBumped)
                    {
                        description += ship.PilotInfo.PilotName + " (" + ship.ShipId + "), ";
                    }
                    description = description.Remove(description.Length - 2, 2);
                    description += "\n";
                }

                if (MovementPrediction.RemotesMovedThrough.Count > 0)
                {
                    description += "Remotes hit: ";
                    foreach (GenericRemote remote in MovementPrediction.RemotesMovedThrough)
                    {
                        description += remote.PilotInfo.PilotName + ", ";
                    }
                    description = description.Remove(description.Length - 3, 2);
                    description += "\n";
                }

                if (MovementPrediction.RemotesOverlapped.Count > 0)
                {
                    description += "Landed on remotes: ";
                    foreach (GenericRemote remote in MovementPrediction.RemotesOverlapped)
                    {
                        description += remote.PilotInfo.PilotName + ", ";
                    }
                    description = description.Remove(description.Length - 2, 2);
                    description += "\n";
                }

                if (description != "")
                {
                    description = description.Remove(description.Length - 1, 1);
                }
                else
                {
                    description = "No collisions";
                }

                Phases.CurrentSubPhase.ShowSubphaseDescription(title, description);
            }

            private static void ShowNextButton()
            {
                Phases.CurrentSubPhase.IsReadyForCommands = true;
                UI.ShowNextButton();
            }

            private static void Finish()
            {
                GameObject.Destroy(FakeShip);
                MovementTemplates.HideLastMovementRuler();
                GenericSubPhase.HideSubphaseDescription();
            }

            private class ManualCollisionPredictionSubPhase : GenericSubPhase
            {
                public override List<GameCommandTypes> AllowedGameCommandTypes => new List<GameCommandTypes>() { GameCommandTypes.PressNext };

                public override void NextButton()
                {
                    Next();
                }

                public override void Next()
                {
                    CallBack();
                    Phases.CurrentSubPhase = PreviousSubPhase;

                    Phases.CurrentSubPhase.Resume();
                }
            };
        }
    }
}
