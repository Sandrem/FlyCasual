using BoardTools;
using Content;
using Ship;
using SubPhases;
using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using Upgrade;

namespace Ship.SecondEdition.Eta2Actis
{
    public class KitFisto : Eta2Actis
    {
        public KitFisto()
        {
            PilotInfo = new PilotCardInfo25
            (
                "Kit Fisto",
                "Enthusiastic Exemplar",
                Faction.Republic,
                4,
                4,
                8,
                isLimited: true,
                force: 2,
                abilityType: typeof(Abilities.SecondEdition.KitFistoActisAbility),
                extraUpgradeIcons: new List<UpgradeType>
                {
                    UpgradeType.ForcePower,
                    UpgradeType.ForcePower,
                    UpgradeType.Talent,
                    UpgradeType.Astromech,
                    UpgradeType.Modification
                },
                tags: new List<Tags>
                {
                    Tags.Jedi,
                    Tags.LightSide
                }
            );

            ImageUrl = "https://images.squarespace-cdn.com/content/v1/5ce432b1f9d2be000134d8ae/53a01dab-f036-4231-92b6-2f5a2cccd184/SWZ97_KitFistolegal.png";
        }
    }
}

namespace Abilities.SecondEdition
{
    public class KitFistoActisAbility : GenericAbility
    {
        public override void ActivateAbility()
        {
            AddDiceModification(
                "Kit Fisto",
                IsAvailable,
                GetAiPriority,
                DiceModificationType.Change,
                count: 1,
                sidesCanBeSelected: new List<DieSide>() { DieSide.Blank },
                sideCanBeChangedTo: DieSide.Focus,
                isGlobal: true,
                payAbilityCost: PayForceCost
            );
        }

        private bool IsAvailable()
        {
            bool result = false;

            if (Combat.AttackStep == CombatStep.Defence && Tools.IsSameTeam(Combat.Defender, HostShip) && HostShip.State.Force > 0)
            {
                ShotInfoArc arcInfo = new ShotInfoArc(
                    Combat.Defender,
                    Combat.Attacker,
                    HostShip.SectorsInfo.Arcs.First(n => n.Facing == Arcs.ArcFacing.Bullseye)
                );

                if (arcInfo.InArc && arcInfo.Range <= 3) result = true;
            }

            return result;
        }

        private int GetAiPriority()
        {
            int result = 0;

            //want to use it if we have a blank, and a way to use the blank, and a need to do so

            if (Combat.AttackStep == CombatStep.Defence)
            {
                int attackSuccessesCancelable = Combat.DiceRollAttack.SuccessesCancelable;
                int defenceSuccesses = Combat.CurrentDiceRoll.Successes;
                //ship needs help
                if (attackSuccessesCancelable > defenceSuccesses)
                {
                    //ship rolled a blank
                    if (Combat.CurrentDiceRoll.Blanks > 0)
                    {
                        int numFocusTokens = Combat.Defender.Tokens.CountTokensByType(typeof(FocusToken));
                        int numCalculateTokens = Combat.Defender.Tokens.CountTokensByType(typeof(CalculateToken));
                        int numForce = Combat.Defender.State.Force;
                        int ableToUseFocusCount = numFocusTokens + numCalculateTokens + numForce;
                        // ship has a way to use the focus
                        if (ableToUseFocusCount > 0)
                        {
                            result = 100;
                        }
                    }
                }
            }

            if (Editions.Edition.Current is Editions.SecondEdition && Combat.CurrentDiceRoll.Failures == 0) return 0;

            return result;
        }

        public override void DeactivateAbility()
        {
            RemoveDiceModification();
        }

        private void PayForceCost(Action<bool> callback)
        {
            if (HostShip.State.Force > 0)
            {
                IsAbilityUsed = true;
                HostShip.State.SpendForce(1, delegate { callback(true); });
            }
            else
            {
                callback(false);
            }
        }
    }
}
