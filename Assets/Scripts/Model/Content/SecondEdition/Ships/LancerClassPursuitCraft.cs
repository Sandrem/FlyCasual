namespace Ship
{
    namespace SecondEdition.LancerClassPursuitCraft
    {
        public class LancerClassPursuitCraft : FirstEdition.LancerClassPursuitCraft.LancerClassPursuitCraft
        {
            public LancerClassPursuitCraft() : base()
            {
                ShipInfo.Hull = 8;
                ShipInfo.Shields = 2;

                IconicPilots[Faction.Scum] = typeof(ShadowportHunter);

                ManeuversImageUrl = "https://vignette.wikia.nocookie.net/xwing-miniatures-second-edition/images/5/5b/Maneuver_lancer.png";
            }
        }
    }
}