using Editions;
using ExtraOptions.ExtraOptionsList;
using Players;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace SquadBuilderNS
{
    public static class RandomSquads
    {
        public static void SetRandomAiSquad()
        {
            var json = GetRandomAiSquad();
            if (json != null)
            {
                Global.SquadBuilder.CurrentPlayer = PlayerNo.Player2;

                Global.SquadBuilder.CurrentSquad.SetPlayerSquadFromImportedJson(json);
            }
            else
            {
                Messages.ShowError("Squadrons for AI aren't found");
            }
        }

        private static JSONObject GetRandomAiSquad()
        {
            string oldDirectoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/RandomAiSquadrons";
            if (Directory.Exists(oldDirectoryPath)) Directory.Move(oldDirectoryPath, oldDirectoryPath + "-IsNotUsedAnymore");

            string directoryPathPrefix = Application.persistentDataPath + "/" + Edition.Current.Name + "/AiSquadrons";
            if (!Directory.Exists(directoryPathPrefix)) Directory.CreateDirectory(directoryPathPrefix);
            string directoryPathDefault = directoryPathPrefix + "/Default";
            if (!Directory.Exists(directoryPathDefault)) Directory.CreateDirectory(directoryPathDefault);

            ManagePreGeneratedRandomAiSquads();

            string directoryPathCustom = directoryPathPrefix + "/Custom";
            if (!Directory.Exists(directoryPathCustom)) Directory.CreateDirectory(directoryPathCustom);

            List<string> filePaths = new List<string>();

            if (!ExtraOptions.ExtraOptionsManager.ExtraOptions[typeof(NoDefaultAiSquadronsExtraOption)].IsOn)
            {
                filePaths.AddRange(Directory.GetFiles(directoryPathDefault).ToList());
            }
            filePaths.AddRange(Directory.GetFiles(directoryPathCustom).ToList());

            if (filePaths.Count != 0)
            {
                int randomFileIndex = UnityEngine.Random.Range(0, filePaths.Count);

                string content = File.ReadAllText(filePaths[randomFileIndex]);
                JSONObject squadJson = new JSONObject(content);

                // filename = Path.GetFileName(filePaths[randomFileIndex]);

                return squadJson;
            }
            else
            {
                return null;
            }
        }

        private static void ManagePreGeneratedRandomAiSquads()
        {
            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/AiSquadrons/Default";

            foreach (var squadron in Edition.Current.PreGeneratedAiSquadrons)
            {
                string filePath = directoryPath + "/" + squadron.Key + ".json";
                if (!DebugManager.NoDefaultAiSquads)
                {
                    File.WriteAllText(filePath, squadron.Value);
                }
                else
                {
                    if (File.Exists(filePath)) File.Delete(filePath);
                }
            }
        }
    }
}
