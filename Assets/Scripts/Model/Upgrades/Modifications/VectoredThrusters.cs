using Upgrade;

namespace UpgradesList
{ 
    public class VectoredThrusters : GenericActionBarUpgrade<ActionsList.BarrelRollAction>
    {
        public VectoredThrusters() : base()
        {
            Type = UpgradeSlot.Modification;
            Name = ShortName = "Vectored Thrusters";
            ImageUrl = "https://vignette1.wikia.nocookie.net/xwing-miniatures/images/5/58/Swx53-vectored-thrusters.png";
            Cost = 2;
        }        
    }
}
