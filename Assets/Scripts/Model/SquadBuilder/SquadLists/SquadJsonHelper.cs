using Editions;
using Obstacles;
using Ship;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SquadBuilderNS
{
    public static class SquadJsonHelper
    {
        public static JSONObject GetSquadInJson(SquadList squadList)
        {
            JSONObject squadJson = new JSONObject();
            squadJson.AddField("name", squadList.Name);
            squadJson.AddField("faction", Edition.Current.FactionToXws(squadList.SquadFaction));
            squadJson.AddField("points", squadList.Points);
            squadJson.AddField("version", "0.3.0");

            List<SquadListShip> playerShipConfigs = squadList.Ships;
            JSONObject[] squadPilotsArrayJson = new JSONObject[playerShipConfigs.Count];
            for (int i = 0; i < squadPilotsArrayJson.Length; i++)
            {
                squadPilotsArrayJson[i] = GenerateSquadPilot(playerShipConfigs[i]);
            }
            JSONObject squadPilotsJson = new JSONObject(squadPilotsArrayJson);
            squadJson.AddField("pilots", squadPilotsJson);

            JSONObject squadObstalesArrayJson = new JSONObject(JSONObject.Type.ARRAY);
            for (int i = 0; i < squadList.ChosenObstacles.Count; i++)
            {
                squadObstalesArrayJson.Add(squadList.ChosenObstacles[i].ShortName);
            }

            squadJson.AddField("obstacles", squadObstalesArrayJson);
            squadJson.AddField("description", GetDescriptionOfSquadJson(squadJson));

            return squadJson;
        }

        public static string GetDescriptionOfSquadJson(JSONObject squadJson)
        {
            string result = "";

            try
            {
                if (squadJson.HasField("pilots"))
                {
                    JSONObject pilotJsons = squadJson["pilots"];
                    foreach (JSONObject pilotJson in pilotJsons.list)
                    {
                        if (result != "") result += "\n";

                        string shipNameXws = pilotJson["ship"].str;
                        string shipNameGeneral = SquadBuilder.Instance.Database.AllShips.Find(n => n.ShipNameCanonical == shipNameXws).ShipName;

                        string pilotNameXws = pilotJson["id"].str;
                        string pilotNameGeneral = SquadBuilder.Instance.Database.AllPilots.Find(n => n.PilotNameCanonical == pilotNameXws).PilotName;

                        result += pilotNameGeneral;

                        if (SquadBuilder.Instance.Database.AllPilots.Count(n => n.PilotName == pilotNameGeneral) > 1)
                        {
                            result += " (" + shipNameGeneral + ")";
                        }

                        if (pilotJson.HasField("upgrades"))
                        {
                            try
                            {
                                JSONObject upgradeJsons = pilotJson["upgrades"];
                                foreach (string upgradeType in upgradeJsons.keys)
                                {
                                    JSONObject upgradeNames = upgradeJsons[upgradeType];
                                    foreach (JSONObject upgradeRecord in upgradeNames.list)
                                    {
                                        string upgradeName = SquadBuilder.Instance.Database.AllUpgrades.Find(n => n.UpgradeNameCanonical == upgradeRecord.str).UpgradeName;
                                        result += " + " + upgradeName;
                                    }
                                }
                            }
                            catch (Exception) { }
                        }
                    }
                }

                result = result.Replace("\"", "\\\"");
            }
            catch (Exception)
            {
                Messages.ShowError("Error during creation of description os squadron");
            }

            return result;
        }

        private static JSONObject GenerateSquadPilot(SquadListShip shipHolder)
        {
            JSONObject pilotJson = new JSONObject();
            pilotJson.AddField("id", shipHolder.Instance.PilotNameCanonical);
            //TODO: Restore
            //pilotJson.AddField("points", GetShipCost(shipHolder));
            pilotJson.AddField("ship", shipHolder.Instance.ShipTypeCanonical);

            Dictionary<string, JSONObject> upgradesDict = new Dictionary<string, JSONObject>();
            if (!(shipHolder.Instance.PilotInfo as PilotCardInfo25).IsStandardLayout)
            {
                foreach (var installedUpgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
                {
                    string slotName = Edition.Current.UpgradeTypeToXws(installedUpgrade.UpgradeInfo.UpgradeTypes[0]);
                    if (!upgradesDict.ContainsKey(slotName))
                    {
                        JSONObject upgrade = new JSONObject();
                        upgrade.Add(installedUpgrade.NameCanonical);
                        upgradesDict.Add(slotName, upgrade);
                    }
                    else
                    {
                        upgradesDict[slotName].Add(installedUpgrade.NameCanonical);
                    }
                }
            }
            JSONObject upgradesDictJson = new JSONObject(upgradesDict);
            pilotJson.AddField("upgrades", upgradesDictJson);

            JSONObject vendorJson = new JSONObject();
            JSONObject skinJson = new JSONObject();
            skinJson.AddField("skin", (shipHolder.Instance.PilotInfo as PilotCardInfo25).SkinName);
            vendorJson.AddField("Sandrem.FlyCasual", skinJson);

            pilotJson.AddField("vendor", vendorJson);

            return pilotJson;
        }

        /*public JSONObject GetSquadInJsonCompact(PlayerNo playerNo)
        {
            JSONObject squadJson = new JSONObject();

            JSONObject[] squadPilotsArrayJson = new JSONObject[Ships.Count];
            for (int i = 0; i < squadPilotsArrayJson.Length; i++)
            {
                squadPilotsArrayJson[i] = GenerateSquadPilotCompact(Ships[i]);
            }
            JSONObject squadPilotsJson = new JSONObject(squadPilotsArrayJson);
            squadJson.AddField("pilots", squadPilotsJson);

            return squadJson;
        }

        private JSONObject GenerateSquadPilotCompact(SquadListShip shipHolder)
        {
            JSONObject pilotJson = new JSONObject();
            pilotJson.AddField("n", shipHolder.Instance.PilotNameCanonical);

            string upgradesList = "";
            foreach (var installedUpgrade in shipHolder.Instance.UpgradeBar.GetUpgradesAll())
            {
                upgradesList += installedUpgrade.NameCanonical + " ";
            }
            JSONObject upgradesDictJson = new JSONObject(upgradesList);

            pilotJson.AddField("u", upgradesDictJson);

            return pilotJson;
        }*/

        public static void CreateSquadFromImportedJson(SquadList squad, string jsonString)
        {
            JSONObject squadJson = new JSONObject(jsonString);
            SetPlayerSquadFromImportedJson(squad, squadJson);
        }

        public static void SetPlayerSquadFromImportedJson(SquadList squad, JSONObject squadJson)
        {
            try
            {
                squad.ClearAll();

                if (squadJson.HasField("name"))
                {
                    squad.Name = squadJson["name"].str;
                }

                string factionNameXws = squadJson["faction"].str;
                Faction faction = Edition.Current.XwsToFaction(factionNameXws);
                squad.SquadFaction = faction;

                if (squadJson.HasField("pilots"))
                {
                    JSONObject pilotJsons = squadJson["pilots"];
                    foreach (JSONObject pilotJson in pilotJsons.list)
                    {
                        string shipNameXws = pilotJson["ship"].str;

                        string shipNameGeneral = "";
                        ShipRecord shipRecord = SquadBuilder.Instance.Database.AllShips.FirstOrDefault(n => n.ShipNameCanonical == shipNameXws);
                        if (shipRecord == null)
                        {
                            Messages.ShowError("Cannot find ship: " + shipNameXws);
                            continue;
                        }

                        shipNameGeneral = shipRecord.ShipName;

                        string pilotNameXws = pilotJson["id"].str;
                        PilotRecord pilotRecord = SquadBuilder.Instance.Database.AllPilots.FirstOrDefault(n => n.PilotNameCanonical == pilotNameXws && n.Ship.ShipName == shipNameGeneral && n.PilotFaction == faction);
                        if (pilotRecord == null)
                        {
                            Messages.ShowError("Cannot find pilot: " + pilotNameXws);
                            continue;
                        }

                        GenericShip newShipInstance = (GenericShip)Activator.CreateInstance(Type.GetType(pilotRecord.PilotTypeName));
                        Edition.Current.AdaptShipToRules(newShipInstance);
                        SquadListShip newShip = squad.AddPilotToSquad(newShipInstance);

                        Dictionary<string, string> upgradesThatCannotBeInstalled = new Dictionary<string, string>();

                        if (pilotJson.HasField("upgrades"))
                        {
                            JSONObject upgradeJsons = pilotJson["upgrades"];
                            if (upgradeJsons.keys != null)
                            {
                                foreach (string upgradeType in upgradeJsons.keys)
                                {
                                    JSONObject upgradeNames = upgradeJsons[upgradeType];
                                    foreach (JSONObject upgradeRecord in upgradeNames.list)
                                    {
                                        UpgradeRecord newUpgradeRecord = SquadBuilder.Instance.Database.AllUpgrades.FirstOrDefault(n => n.UpgradeNameCanonical == upgradeRecord.str);
                                        if (newUpgradeRecord == null)
                                        {
                                            Messages.ShowError("Cannot find upgrade: " + upgradeRecord.str);
                                        }

                                        bool upgradeInstalledSucessfully = newShip.InstallUpgrade(upgradeRecord.str, Edition.Current.XwsToUpgradeType(upgradeType));
                                        if (!upgradeInstalledSucessfully && !upgradesThatCannotBeInstalled.ContainsKey(upgradeRecord.str)) upgradesThatCannotBeInstalled.Add(upgradeRecord.str, upgradeType);
                                    }
                                }

                                while (upgradeJsons.Count != 0)
                                {
                                    Dictionary<string, string> upgradesThatCannotBeInstalledCopy = new Dictionary<string, string>(upgradesThatCannotBeInstalled);

                                    bool wasSuccess = false;
                                    foreach (var upgrade in upgradesThatCannotBeInstalledCopy)
                                    {
                                        bool upgradeInstalledSucessfully = newShip.InstallUpgrade(upgrade.Key, Edition.Current.XwsToUpgradeType(upgrade.Value));
                                        if (upgradeInstalledSucessfully)
                                        {
                                            wasSuccess = true;
                                            upgradesThatCannotBeInstalled.Remove(upgrade.Key);
                                        }
                                    }

                                    if (!wasSuccess) break;
                                }
                            }
                        }

                        if ((newShipInstance.PilotInfo as PilotCardInfo25).IsStandardLayout)
                        {
                            foreach (Type upgradeType in newShipInstance.MustHaveUpgrades)
                            {
                                bool upgradeInstalledSucessfully = newShip.InstallUpgrade(upgradeType.ToString());
                                if (!upgradeInstalledSucessfully)
                                {
                                    Messages.ShowError("Cannot install upgrade: " + upgradeType.ToString());
                                }

                            }
                        }

                        if (pilotJson.HasField("vendor"))
                        {
                            JSONObject vendorData = pilotJson["vendor"];
                            if (vendorData.HasField("Sandrem.FlyCasual"))
                            {
                                JSONObject myVendorData = vendorData["Sandrem.FlyCasual"];
                                if (myVendorData.HasField("skin"))
                                {
                                    (newShip.Instance.PilotInfo as PilotCardInfo25).SkinName = myVendorData["skin"].str;
                                }
                            }
                        }
                    }
                }
                else
                {
                    Messages.ShowError("The squad has no pilots");
                }

                if (squadJson.HasField("obstacles"))
                {
                    if (squadJson["obstacles"].Count == 3)
                    {
                        squad.ChosenObstacles.Clear();
                        squad.ChosenObstacles.AddRange(
                            new List<GenericObstacle>()
                            {
                                ObstaclesManager.GetPossibleObstacle(squadJson["obstacles"][0].str),
                                ObstaclesManager.GetPossibleObstacle(squadJson["obstacles"][1].str),
                                ObstaclesManager.GetPossibleObstacle(squadJson["obstacles"][2].str)
                            }
                        );
                    }
                    else
                    {
                        Messages.ShowError("Not enough obstacles in imported XWS, default obstacles are set");
                        squad.SetDefaultObstacles();
                    }
                }
                else
                {
                    squad.SetDefaultObstacles();
                }
            }
            catch (Exception)
            {
                Messages.ShowError("Error during creation of squadron");
            }
        }

        public static void SaveSquadronToFile(SquadList squad, string squadName)
        {
            squad.Name = CleanFileName(squadName);

            // check that directory exists, if not create it
            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/SavedSquadrons";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            string filePath = $"{directoryPath}/{squad.Name}.json";

            if (File.Exists(filePath)) File.Delete(filePath);

            File.WriteAllText(filePath, GetSquadInJson(squad).ToString());
        }

        private static string CleanFileName(string fileName)
        {
            return Path.GetInvalidFileNameChars().Aggregate(fileName, (current, c) => current.Replace(c.ToString(), string.Empty));
        }
    }
}
