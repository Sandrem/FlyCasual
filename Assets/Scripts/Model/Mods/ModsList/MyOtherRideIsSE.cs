namespace Mods
{
    namespace ModsList
    {
        public class MyOtherRideIsModSE : Mod
        {
            public MyOtherRideIsModSE()
            {
                Name = "My other ride is...";
                Description = "Darth Vader as TIE Defender pilot\n" +
                              "Maarek Stele as Alpha-class Star Wing pilot\n" +
                              "Corran Horn (with and without Force) and Tycho Celchu as X-Wing pilots";
                EditionType = typeof(Editions.SecondEdition);
            }
        }
    }
}
