using Abilities.SecondEdition;
using System.Collections.Generic;
using Upgrade;
using Ship;
using Conditions;
using Tokens;
using Mods.ModsList;
using Mods;

namespace Ship
{
    namespace SecondEdition.NabooRoyalN1Starfighter
    {
        public class PadmeAmidala : NabooRoyalN1Starfighter
        {
            public PadmeAmidala() : base()
            {
                PilotInfo = new PilotCardInfo25
                (
                    "Padmé Amidala",
                    "Aggressive Negotiator",
                    Faction.Republic,
                    4,
                    5,
                    22,
                    isLimited: true,
                    abilityText: "While an enemy ship in your [Front Arc] defends or performs an attack, that ship can modify only 1 [Focus] result (other results can still be modified).",
                    abilityType: typeof(PadmeAmidalaAbility),
                    extraUpgradeIcons: new List<UpgradeType>
                    {
                        UpgradeType.Talent, UpgradeType.Torpedo
                    }
                );

                ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/80/40/8040bcab-ebc0-487e-8dff-bd69da7311dd/swz40_padme-amidala.png";

                if (ModsManager.Mods[typeof(LimitedEditionNabooRoyalN1StarfighterMod)].IsOn)
                {
                    ImageUrl = "https://images-cdn.fantasyflightgames.com/filer_public/e8/c1/e8c1866f-a83a-469f-b2c0-a144c166fced/swzp02_padme-amidala.jpg";
                    ModelInfo.SkinName = "Silver";
                }
            }
        }
    }
}

namespace Abilities.SecondEdition
{
  public class PadmeAmidalaAbility : GenericAbility
  {
    public override void ActivateAbility()
    {
      GenericShip.OnAttackStartAsAttackerGlobal += CheckPadmeAbilityAttacker;
      GenericShip.OnAttackStartAsDefenderGlobal += CheckPadmeAbilityDefender;
    }

    public override void DeactivateAbility()
    {
      GenericShip.OnAttackStartAsAttackerGlobal -= CheckPadmeAbilityAttacker;
      GenericShip.OnAttackStartAsDefenderGlobal -= CheckPadmeAbilityDefender;
      
    }
    public void CheckPadmeAbilityDefender()
    {
      CheckPadmeAbility(false);
    }
    
    public void CheckPadmeAbilityAttacker()
    {
      CheckPadmeAbility(true);
    }
    public void CheckPadmeAbility(bool isAttacker)
    {
      if (!isAttacker &&
          Combat.Defender.Owner != HostShip.Owner && 
          HostShip.SectorsInfo.IsShipInSector(Combat.Defender, Arcs.ArcType.Front))
      {
        PadmeAmidalaCondition condition = new PadmeAmidalaCondition(Combat.Defender, HostShip);
        Combat.Defender.Tokens.AssignCondition(condition);
      }
      if (isAttacker &&
          Combat.Attacker.Owner != HostShip.Owner &&
          HostShip.SectorsInfo.IsShipInSector(Combat.Attacker, Arcs.ArcType.Front))
      {
        PadmeAmidalaCondition condition = new PadmeAmidalaCondition(Combat.Attacker, HostShip);
        Combat.Attacker.Tokens.AssignCondition(condition);
      }
    }
  }
}

namespace Conditions
{
    public class PadmeAmidalaCondition : GenericToken
    {
        bool FocusHasBeenModified = false;

        public PadmeAmidalaCondition(GenericShip host, GenericShip source) : base(host)
        {
            Name = ImageName = "Debuff Token";
            TooltipType = source.GetType();
            Temporary = false;
        }

        public override void WhenAssigned()
        {
            Messages.ShowInfo("Padmé Amidala: " + Host.PilotInfo.PilotName + " can only modify 1 focus result for this attack.");
            FocusHasBeenModified = false;
            
            Host.OnTryDiceResultModification += CheckIfCanChangeDie;
            Host.OnTrySelectDie += CheckIfCanSelectDie;

            Host.OnAttackFinishAsDefender += RemovePadmeAmidalaCondition;
            Host.OnAttackFinishAsAttacker += RemovePadmeAmidalaCondition;
        }

        public void CheckIfCanChangeDie(
          Die die, Abilities.GenericAbility.DiceModificationType modType, DieSide newResult, ref bool isAllowed
        )
        // Add focus modification limitation code here.
        {
          // set FocusHasBeenModified in some check in here
          if (FocusHasBeenModified == true && die.Side == DieSide.Focus)
          {
            isAllowed = false;
            Messages.ShowInfo("Padmé Amidala: Die modification is prevented");
          }
          else if (die.Side == DieSide.Focus)
          {
            FocusHasBeenModified = true;
          }
        }

        public void CheckIfCanSelectDie(
          Die die, ref bool isAllowed
        )
        {
          if (FocusHasBeenModified == true && die.Side == DieSide.Focus)
          {
            isAllowed = false;
            Messages.ShowErrorToHuman("Padmé Amidala: Unable to select focus results");
          }
          else if (die.Side == DieSide.Focus)
          {
            FocusHasBeenModified = true;
          }
        }
        public void RemovePadmeAmidalaCondition(GenericShip ship)
        {
            Host.Tokens.RemoveCondition(this);
        }

        public override void WhenRemoved()
        {
            Messages.ShowInfo("Padmé Amidala: " + Host.PilotInfo.PilotName + "'s ability to modify focus results restored");

            // remove focus modification limitation code here.
            Host.OnTryDiceResultModification -= CheckIfCanChangeDie;
            Host.OnTrySelectDie -= CheckIfCanSelectDie;

            Host.OnAttackFinishAsDefender -= RemovePadmeAmidalaCondition;
            Host.OnAttackFinishAsAttacker -= RemovePadmeAmidalaCondition;
        }
    }
}