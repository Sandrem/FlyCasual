using Upgrade;
using Actions;
using ActionsList;

namespace UpgradesList.SecondEdition
{
    public class FirstOrderOrdnanceTech : GenericUpgrade
    {
        public FirstOrderOrdnanceTech() : base()
        {
            FromMod = typeof(Mods.ModsList.UnreleasedContentMod);

            UpgradeInfo = new UpgradeCardInfo
            (
                "First Order Ordnance Tech",
                UpgradeType.Gunner,
                cost: 1,
                restriction: new FactionRestriction(Faction.FirstOrder),
                addAction: new ActionInfo(typeof(ReloadAction)),
                addActionLink: new LinkedActionInfo(typeof(ReloadAction), typeof(TargetLockAction), linkedColor: ActionColor.White)
            );

            ImageUrl = "https://i.imgur.com/QNh8vcN.png";
        }
    }
}