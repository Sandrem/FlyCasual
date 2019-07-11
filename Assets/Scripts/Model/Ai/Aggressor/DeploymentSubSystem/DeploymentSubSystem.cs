using BoardTools;
using Editions;
using GameCommands;
using GameModes;
using Players;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tokens;
using UnityEngine;
using Upgrade;

namespace AI.Aggressor
{
    public class DeploymentPlans
    {
        public PlayerNo Player { get; private set; }
        public Dictionary<int, bool> EncounteredObstacles { get; private set; } = new Dictionary<int, bool>();
        public Vector2 BestRange { get; private set; } = new Vector2(0, 0);
        public float CenterOfBestRange { get; private set; }
        public int Direction { get; private set; }

        // TODO: Remove
        public float CurrentPosition { get; set; }

        public DeploymentPlans(PlayerNo playerNo)
        {
            Player = playerNo;
            Direction = (playerNo == PlayerNo.Player1) ? 1 : -1;
        }

        public void AddEncounteredObstaclesData(int key, bool hitObstacle)
        {
            EncounteredObstacles.Add(key, hitObstacle);
        }

        public void CalculateRanges()
        {
            AddEncounteredObstaclesData(91, true);

            bool inRange = false;
            Vector2 currentRange = new Vector2(0, 0);

            for (int i = 0; i <= 91; i++)
            {
                if (inRange)
                {
                    if (EncounteredObstacles[i])
                    {
                        inRange = false;
                        currentRange.y = i;

                        if (currentRange.y - currentRange.x > BestRange.y - BestRange.x)
                        {
                            BestRange = currentRange;
                        }
                    }
                }
                else
                {
                    if (!EncounteredObstacles[i])
                    {
                        inRange = true;
                        currentRange.x = i;
                    }
                }
            }

            CenterOfBestRange = BestRange.x + (BestRange.y - BestRange.x)/2f;
        }

        public void ScanBoard()
        {
            for (int i = 0; i <= 90; i++)
            {
                RaycastHit hitInfo = new RaycastHit();
                if (Physics.Raycast(
                        new Vector3(
                            Board.BoardIntoWorld(-91.44f / 2f) + (Board.BoardIntoWorld(91.44f) * (i / 90f)),
                            0.003f,
                            Board.BoardIntoWorld(Direction * -91.44f / 2f)
                        ),
                        new Vector3(0, 0, Direction),
                        out hitInfo,
                        Board.BoardIntoWorld(91.44f / 2f)
                    )
                )
                {
                    AddEncounteredObstaclesData(i, true);
                }
                else
                {
                    AddEncounteredObstaclesData(i, false);
                }
            }
        }
    }

    public static class DeploymentSubSystem
    {
        private static Dictionary<PlayerNo, DeploymentPlans> DeploymentPlans = new Dictionary<PlayerNo, DeploymentPlans>();

        private static void ScanDeploymentZone()
        {
            // Cleanup in case of old data
            if (DeploymentPlans.ContainsKey(Phases.CurrentSubPhase.RequiredPlayer))
            {
                DeploymentPlans.Remove(Phases.CurrentSubPhase.RequiredPlayer);
            }

            DeploymentPlans currentDeploymentPlans = new DeploymentPlans(Phases.CurrentSubPhase.RequiredPlayer);
            DeploymentPlans.Add(Phases.CurrentSubPhase.RequiredPlayer, currentDeploymentPlans);

            currentDeploymentPlans.ScanBoard();
            currentDeploymentPlans.CalculateRanges();
        }

        public static void SetupShip()
        {
            //If first time for player - scan board
            if (!Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Any(n => n.Value.IsSetupPerformed))
            {
                ScanDeploymentZone();
            }

            DeploymentPlans currentDeploymentPlans = DeploymentPlans[Phases.CurrentSubPhase.RequiredPlayer];

            List<GenericShip> PlayerShips = Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Values.ToList();

            // TODO: Remove blocks
            if (currentDeploymentPlans.CurrentPosition == 0)
            {
                float requiredZone = PlayerShips.Count * Board.BoardIntoWorld(4) + (PlayerShips.Count - 1) * Board.BoardIntoWorld(2);
                float centerToBoard = Board.BoardIntoWorld(-91.44f / 2f) + (Board.BoardIntoWorld(91.44f) * (currentDeploymentPlans.CenterOfBestRange / 90f));
                float requiredZoneStart = centerToBoard - requiredZone / 2f;
                currentDeploymentPlans.CurrentPosition = requiredZoneStart + Board.BoardIntoWorld(2);
            }

            foreach (GenericShip shipToSetup in PlayerShips)
            {
                if (!shipToSetup.IsSetupPerformed && shipToSetup.State.Initiative == Phases.CurrentSubPhase.RequiredInitiative)
                {
                    Selection.ChangeActiveShip(shipToSetup);

                    Vector3 position = new Vector3(
                        currentDeploymentPlans.CurrentPosition,
                        0,
                        Board.BoardIntoWorld(currentDeploymentPlans.Direction * -91.44f / 2f) + currentDeploymentPlans.Direction * Board.BoardIntoWorld(Board.RANGE_1)
                    );
                    currentDeploymentPlans.CurrentPosition += Board.BoardIntoWorld(6);

                    GameManagerScript.Wait(
                        0.5f,
                        delegate {
                            GameCommand command = SetupSubPhase.GeneratePlaceShipCommand(shipToSetup.ShipId, position, shipToSetup.GetAngles());
                            GameMode.CurrentGameMode.ExecuteCommand(command);
                        }
                    );

                    return;
                }
            }

            Phases.Next();
        }
    }
}
