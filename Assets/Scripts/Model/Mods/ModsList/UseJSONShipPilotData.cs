namespace Mods
{
    namespace ModsList
    {
        public class UseJSONShipPilotData : Mod
        {
            public UseJSONShipPilotData()
            {
                Name = "Use JSON Ship/Pilot Data";
                Description = "Load ship and pilot data from JSON files";
                EditionType = typeof(RuleSets.SecondEdition);
            }
        }
    }
}