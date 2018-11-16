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
            }
        }
    }
}