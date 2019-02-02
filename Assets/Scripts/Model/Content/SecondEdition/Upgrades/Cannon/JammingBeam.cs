using Ship;
using System.Linq;
using Tokens;
using Upgrade;

namespace UpgradesList.SecondEdition
{
    public class JammingBeam : GenericSpecialWeapon
    {
        public JammingBeam() : base()
        {
            UpgradeInfo = new UpgradeCardInfo(
                "Jamming Beam",
                UpgradeType.Cannon,
                cost: 0,
                weaponInfo: new SpecialWeaponInfo(
                    attackValue: 3,
                    minRange: 1,
                    maxRange: 2
                ),
                abilityType: typeof(Abilities.SecondEdition.JammingBeamAbility),
                seImageNumber: 29
            );
        }        
    }
}

namespace Abilities.SecondEdition
{
    public class JammingBeamAbility : Abilities.FirstEdition.JammingBeamAbility
    {

        protected void JammingBeamEffect(object sender, System.EventArgs e)
        {
            int jammingBeamTokens = Combat.DiceRollAttack.Successes;
            Combat.DiceRollAttack.CancelAllResults();
            Combat.DiceRollAttack.RemoveAllFailures();

            Combat.Defender.Tokens.AssignTokens(
                () => new JamToken(Combat.Defender),
                jammingBeamTokens,
                Triggers.FinishTrigger
            );
        }
    }
}