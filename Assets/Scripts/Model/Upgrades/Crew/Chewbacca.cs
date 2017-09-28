using Upgrade;

namespace UpgradesList
{
    public class Chewbacca : GenericUpgrade
    {
        public Chewbacca() : base()
        {
            Type = UpgradeType.Crew;
            Name = "Chewbacca";
            Cost = 4;

            isUnique = true;
        }
    }
}
