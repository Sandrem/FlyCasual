using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class TractorBeam : GenericSpecialWeapon
    {
        public TractorBeam() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Tractor Beam",
                UpgradeType.Cannon,
                cost: 2,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 3
                ),
                abilityType: typeof(Abilities.SecondEdition.TractorBeamAbility),
                seImageNumber: 30
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class TractorBeamAbility : Abilities.FirstEdition.TractorBeamAbility
    {
        protected override void TractorBeamEffect(object sender, System.EventArgs e)
        {
            int tractorBeamTokens = Combat.DiceRollAttack.Successes;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignTokens(() => new TractorBeamToken(Combat.Defender, Combat.Attacker.Owner), tractorBeamTokens, Triggers.FinishTrigger);
        }
    }
}