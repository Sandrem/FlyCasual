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
        private PlayerNo Player { get; set; }
        private Dictionary<int, bool> EncounteredObstacles { get; set; } = new Dictionary<int, bool>();
        private Vector2 BestRange { get; set; } = new Vector2(0, 0);
        private float CenterOfBestRange { get; set; }
        private float FormationWidth { get; set; }
        private int Direction { get; set; }
        private Dictionary<GenericShip, Vector3> PlannedShipPositions = new Dictionary<GenericShip, Vector3>();

        public DeploymentPlans(PlayerNo playerNo)
        {
            Player = playerNo;
            Direction = (playerNo == PlayerNo.Player1) ? 1 : -1;
        }

        public void CreatePlan()
        {
            ScanBoardForObstacles();
            CalculateSafeRanges();
            GenerateShipFormation();
            AdjustShipFormationToSafeZoneCenter();
        }

        public void ScanBoardForObstacles()
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
                    EncounteredObstacles.Add(i, true);
                }
                else
                {
                    EncounteredObstacles.Add(i, false);
                }
            }
        }

        public void CalculateSafeRanges()
        {
            EncounteredObstacles.Add(91, true);

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

            CenterOfBestRange = BestRange.x + (BestRange.y - BestRange.x) / 2f;
        }

        private void GenerateShipFormation()
        {
            List<GenericShip> shipsToSetup = Roster.GetPlayer(Player).Ships.Values.OrderBy(n => n.State.Initiative).ToList();

            float currentPosition = 0;

            // Not small ships - in single row

            foreach (GenericShip notSmallShip in shipsToSetup.Where(n => n.ShipInfo.BaseSize != BaseSize.Small))
            {
                PlannedShipPositions.Add(
                    notSmallShip,
                    new Vector3(currentPosition + Board.BoardIntoWorld(notSmallShip.ShipBase.SHIPSTAND_SIZE_CM / 2f), 0, 0)
                );

                currentPosition += Board.BoardIntoWorld(notSmallShip.ShipBase.SHIPSTAND_SIZE_CM + 2f);
                FormationWidth += notSmallShip.ShipBase.SHIPSTAND_SIZE_CM + 2f;
            }

            // Small ships - can be in 2 rows

            List<GenericShip> smallShips = shipsToSetup.Where(n => n.ShipInfo.BaseSize == BaseSize.Small).ToList();
            float currentPositionSecondRow = currentPosition;
            float secondRowShift = (smallShips.Count % 2 == 0) ? 0 : Board.BoardIntoWorld(3f);

            for (int i = 0; i < smallShips.Count; i++)
            {
                if (!ShouldBeInSecondRow(smallShips[i], i, shipsToSetup))
                {
                    PlannedShipPositions.Add(
                        smallShips[i],
                        new Vector3(currentPosition + Board.BoardIntoWorld(smallShips[i].ShipBase.SHIPSTAND_SIZE_CM / 2f), 0, 0)
                    );

                    currentPosition += Board.BoardIntoWorld(smallShips[i].ShipBase.SHIPSTAND_SIZE_CM + 2f);
                    FormationWidth += smallShips[i].ShipBase.SHIPSTAND_SIZE_CM + 2f;
                }
                else
                {
                    PlannedShipPositions.Add(
                        smallShips[i],
                        new Vector3(
                            currentPositionSecondRow + Board.BoardIntoWorld(smallShips[i].ShipBase.SHIPSTAND_SIZE_CM / 2f) + secondRowShift,
                            0,
                            Direction * (-Board.BoardIntoWorld(smallShips[i].ShipBase.SHIPSTAND_SIZE_CM + 2f))
                        )
                    );

                    currentPositionSecondRow += Board.BoardIntoWorld(smallShips[i].ShipBase.SHIPSTAND_SIZE_CM + 2f);
                }
            }

            FormationWidth -= 2f;
        }

        private bool ShouldBeInSecondRow(GenericShip ship, int count, List<GenericShip> shipsToSetup)
        {
            // only second part of small ships
            if (count + 1 > (shipsToSetup.Count(n => n.ShipInfo.BaseSize == BaseSize.Small) + 1) / 2)
            {
                //if only 3 small ships without larger ships - still 1 row
                if (shipsToSetup.Count(n => n.ShipInfo.BaseSize == BaseSize.Small) < 4 && shipsToSetup.Count(n => n.ShipInfo.BaseSize != BaseSize.Small) == 0)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        private void AdjustShipFormationToSafeZoneCenter()
        {
            if (CenterOfBestRange < FormationWidth / 2f) CenterOfBestRange = FormationWidth / 2f + Board.BoardIntoWorld(2f);
            if (CenterOfBestRange + FormationWidth / 2f > 91.44f) CenterOfBestRange = 91.44f - FormationWidth / 2f - Board.BoardIntoWorld(2f);

            float centerToBoard = Board.BoardIntoWorld(-91.44f / 2f) + (Board.BoardIntoWorld(91.44f) * (CenterOfBestRange / 91.44f));
            float shift = centerToBoard - Board.BoardIntoWorld(FormationWidth / 2f);

            foreach (GenericShip ship in Roster.GetPlayer(Player).Ships.Values)
            {
                PlannedShipPositions[ship] += new Vector3(shift, 0, Board.BoardIntoWorld(Direction * -91.44f / 2f) + Direction * Board.BoardIntoWorld(Board.RANGE_1));
            }
        }

        public void SetupShip()
        {
            List<GenericShip> PlayerShips = Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Values.ToList();

            foreach (GenericShip shipToSetup in PlayerShips)
            {
                if (!shipToSetup.IsSetupPerformed && shipToSetup.State.Initiative == Phases.CurrentSubPhase.RequiredInitiative)
                {
                    Selection.ChangeActiveShip(shipToSetup);

                    GameManagerScript.Wait(
                        0.5f,
                        delegate {
                            GameCommand command = SetupSubPhase.GeneratePlaceShipCommand(
                                shipToSetup.ShipId,
                                PlannedShipPositions[shipToSetup],
                                shipToSetup.GetAngles()
                            );
                            GameMode.CurrentGameMode.ExecuteCommand(command);
                        }
                    );

                    return;
                }
            }

            Phases.Next();
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

            currentDeploymentPlans.CreatePlan();
        }

        public static void SetupShip()
        {
            if (IsSetupOfFirstShip()) ScanDeploymentZone();

            DeploymentPlans currentDeploymentPlans = DeploymentPlans[Phases.CurrentSubPhase.RequiredPlayer];
            currentDeploymentPlans.SetupShip();
        }

        private static bool IsSetupOfFirstShip()
        {
            return !Roster.GetPlayer(Phases.CurrentSubPhase.RequiredPlayer).Ships.Any(n => n.Value.IsSetupPerformed);
        }
    }
}
