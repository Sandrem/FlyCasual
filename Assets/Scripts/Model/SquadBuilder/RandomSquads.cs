using Editions;
using Players;
using System.IO;
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
            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/RandomAiSquadrons";
            if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);

            ManagePreGeneratedRandomAiSquads();

            string[] filePaths = Directory.GetFiles(directoryPath);
            if (filePaths.Length != 0)
            {
                int randomFileIndex = UnityEngine.Random.Range(0, filePaths.Length);

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
            string directoryPath = Application.persistentDataPath + "/" + Edition.Current.Name + "/RandomAiSquadrons";

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
