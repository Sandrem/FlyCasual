namespace Mods
{
    namespace ModsList
    {
        public class MyOtherRideIsSEMod : Mod
        {
            public MyOtherRideIsSEMod()
            {
                Name = "My other ride is...";
                Description = "Darth Vader as TIE Defender pilot\n" +
                              "Maarek Stele as Alpha-class Star Wing pilot\n" +
                              "Corran Horn (with and without Force), Hera Syndulla and Tycho Celchu as X-Wing pilots\n" +
                              "Ezra Bridger, Hera Syndulla and Sabine Wren as A-Wing pilots\n" +
                              "Hera Syndulla as B-Wing pilot\n" +
                              "Hera Syndulla as U-Wing pilot\n";
                EditionType = typeof(Editions.SecondEdition);
            }
        }
    }
}
