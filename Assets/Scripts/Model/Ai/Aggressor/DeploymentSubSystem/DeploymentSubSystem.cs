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
    public class DeploymentScannedData
    {
        public Dictionary<int, bool> EncounteredObstacles { get; private set; } = new Dictionary<int, bool>();
        public Vector2 BestRange { get; private set; } = new Vector2(0, 0);
        public float CenterOfBestRange { get; private set; }

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
                            Debug.Log("Best range is " + BestRange);
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
    }

    public static class DeploymentSubSystem
    {
        //TODO: Separate
        static DeploymentScannedData Data;
        static float CurrentPosition;

        public static void ScanDeploymentZone()
        {
            if (Data == null)
            {
                Data = new DeploymentScannedData();

                for (int i = 0; i <= 90; i++)
                {
                    RaycastHit hitInfo = new RaycastHit();
                    if (Physics.Raycast(new Vector3(Board.BoardIntoWorld(-91.44f / 2f) + (Board.BoardIntoWorld(91.44f) * (i / 90f)), 0.003f, Board.BoardIntoWorld(91.44f / 2f)), new Vector3(0, 0, -1), out hitInfo, Board.BoardIntoWorld(91.44f / 2f)))
                    {
                        Data.AddEncounteredObstaclesData(i, true);
                    }
                    else
                    {
                        Data.AddEncounteredObstaclesData(i, false);
                    }
                }

                Data.CalculateRanges();

                Debug.Log("Center: " + Data.CenterOfBestRange);
            }
        }

        public static void SetupShipsInOneLine()
        {
            List<GenericShip> PlayerShips = Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Values.ToList();

            if (CurrentPosition == 0)
            {
                float requiredZone = PlayerShips.Count * Board.BoardIntoWorld(4) + (PlayerShips.Count - 1) * Board.BoardIntoWorld(2);
                float centerToBoard = Board.BoardIntoWorld(-91.44f / 2f) + (Board.BoardIntoWorld(91.44f) * (Data.CenterOfBestRange / 90f));
                Debug.Log("Center to board: " + centerToBoard);
                float requiredZoneStart = centerToBoard - requiredZone / 2f;
                CurrentPosition = requiredZoneStart + Board.BoardIntoWorld(2);
            }

            foreach (GenericShip shipToSetup in PlayerShips)
            {
                if (!shipToSetup.IsSetupPerformed && shipToSetup.State.Initiative == Phases.CurrentSubPhase.RequiredInitiative)
                {
                    Selection.ChangeActiveShip(shipToSetup);

                    Debug.Log("Setup at " + CurrentPosition);

                    int direction = (Phases.CurrentSubPhase.RequiredPlayer == PlayerNo.Player1) ? -1 : 1;
                    Vector3 position = new Vector3(CurrentPosition, 0, Board.BoardIntoWorld(91.44f/2f) - Board.BoardIntoWorld(Board.RANGE_1));
                    CurrentPosition += Board.BoardIntoWorld(6);

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
