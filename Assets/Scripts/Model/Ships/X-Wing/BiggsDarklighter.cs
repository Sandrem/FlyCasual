using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ship
{
    namespace XWing
    {
        public class BiggsDarklighter : XWing
        {
            public BiggsDarklighter() : base()
            {
                PilotName = "Biggs Darklighter";
                ImageUrl = "https://vignette3.wikia.nocookie.net/xwing-miniatures/images/9/90/Biggs-darklighter.png";
                IsUnique = true;
                PilotSkill = 5;
                Cost = 25;
            }

            public override void InitializePilot()
            {
                base.InitializePilot();
                RulesList.TargetIsLegalForShotRule.OnCheckTargetIsLegal += CanPerformAttack;
            }

            public void CanPerformAttack(ref bool result, GenericShip attacker, GenericShip defender)
            {
                bool abilityIsActive = false;
                if (defender.ShipId != this.ShipId)
                {
                    if (defender.Owner.PlayerNo == this.Owner.PlayerNo)
                    {
                        Board.ShipDistanceInformation positionInfo = new Board.ShipDistanceInformation(defender, this);
                        if (positionInfo.Range <= 1)
                        {
                            if (Combat.SecondaryWeapon == null)
                            {
                                if (attacker.InPrimaryWeaponFireZone(this)) abilityIsActive = true;
                            }
                            else
                            {
                                if (Combat.SecondaryWeapon.IsShotAvailable(this)) abilityIsActive = true;
                            }
                        }
                    }
                }

                if (abilityIsActive)
                {
                    if (Roster.GetPlayer(Phases.CurrentPhasePlayer).GetType() == typeof(Players.HumanPlayer))
                    {
                        Game.UI.ShowError("Biggs DarkLighter: You cannot attack target ship");
                    }
                    result = false;
                }
            }
        }
    }
}
