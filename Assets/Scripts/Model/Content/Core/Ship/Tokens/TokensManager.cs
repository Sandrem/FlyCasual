﻿using System;
using System.Collections.Generic;
using System.Linq;
using Tokens;
using UnityEngine;

namespace Ship
{
    public class TokensManager
    {
        private GenericShip Host;
        private List<GenericToken> AssignedTokens = new List<GenericToken>();
        public GenericToken TokenToAssign;

        public TokensManager(GenericShip host)
        {
            Host = host;
        }

        public List<GenericToken> GetAllTokens()
        {
            return AssignedTokens;
        }

        public bool HasGreenTokens()
        {
            return HasTokenByColor(TokenColors.Green);
        }

        public bool HasTokenByColor(TokenColors tokensColor)
        {
            foreach (GenericToken tok in AssignedTokens)
            {
                if (tok.TokenColor == tokensColor)
                    return true;
            }

            return false;
        }

        public bool HasToken(Type type, char letter = ' ')
        {
            return GetToken(type, letter) != null;
        }

        public int CountTokensByType(Type type)
        {
            return GetAllTokens().Count(n => n.GetType() == type);
        }

        public bool HasToken<T>(char letter = ' ') where T : GenericToken
        {
            return GetToken<T>(letter) != null;
        }

        public int CountTokensByType<T>() where T : GenericToken
        {
            return GetAllTokens().Count(n => n is T);
        }

        public GenericToken GetToken(Type type, char letter = ' ')
        {
            GenericToken result = null;

            foreach (var assignedToken in AssignedTokens)
            {
                if (assignedToken.GetType() == type)
                {
                    if (assignedToken.GetType().BaseType == typeof(GenericTargetLockToken))
                    {
                        if (((assignedToken as GenericTargetLockToken).Letter == letter) || (letter == '*'))
                        {
                            return assignedToken;
                        }
                    }
                    else
                    {
                        return assignedToken;
                    }
                }
            }
            return result;
        }

        public T GetToken<T>(char letter = ' ') where T : GenericToken
        {
            var result = AssignedTokens
                .OfType<T>()
                .Where(t => !(t is GenericTargetLockToken) || letter == '*' || (t as GenericTargetLockToken).Letter == letter)
                .FirstOrDefault();
            return result;
        }

        public List<T> GetTokens<T>(char letter = ' ') where T : GenericToken
        {
            var result = AssignedTokens
                .OfType<T>()
                .Where(t => !(t is GenericTargetLockToken) || letter == '*' || (t as GenericTargetLockToken).Letter == letter)
                .ToList();
            return result;
        }

        public GenericTargetLockToken GetTargetLockToken(char letter)
        {
            return (GenericTargetLockToken)AssignedTokens.Find(n => n.GetType().BaseType == typeof(GenericTargetLockToken) && (n as GenericTargetLockToken).Letter == letter);
        }

        public List<char> GetTargetLockLetterPairsOn(GenericShip targetShip)
        {
            List<char> result = new List<char>();

            List<BlueTargetLockToken> blueTokens = GetTokens<BlueTargetLockToken>('*');

            foreach (BlueTargetLockToken blueToken in blueTokens)
            {
                char foundLetter = (blueToken as BlueTargetLockToken).Letter;

                GenericToken redToken = targetShip.Tokens.GetToken(typeof(RedTargetLockToken), foundLetter);
                if (redToken != null)
                {
                    result.Add(blueToken.Letter);
                }
            }

            return result;
        }

        public void AssignToken(GenericToken token, Action callBack, char letter = ' ')
        {
            TokenToAssign = token;

            Host.CallBeforeAssignToken(
                TokenToAssign,
                delegate { FinalizeAssignToken(callBack); }
            );
        }

        public void AssignToken(Type tokenType, Action callback)
        {
            AssignToken((GenericToken) Activator.CreateInstance(tokenType, Host), callback);
        }

        public void AssignTokens(Func<GenericToken> createToken, int count, Action callback, char letter = ' ')
        {
            if (count > 0)
            {
                count--;
                AssignToken(createToken(), delegate { AssignTokens(createToken, count, callback, letter); });
            }
            else
            {
                callback();
            }
        }

        private void FinalizeAssignToken(Action callback)
        {
            if (TokenToAssign == null)
            {
                callback();
                return;
            }

            AssignedTokens.Add(TokenToAssign);

            TokenToAssign.InitializeTooltip();
            TokenToAssign.WhenAssigned();
            Host.CallOnTokenIsAssigned(TokenToAssign, callback);
        }

        public void RemoveCondition(GenericToken token)
        {
            if (AssignedTokens.Remove(token))
            {
                token.WhenRemoved();
                Host.CallOnConditionIsRemoved(token.GetType());
            }
        }

        public void RemoveToken(Type type, Action callback, char letter = ' ')
        {
            GenericToken assignedToken = GetToken(type, letter);

            if (assignedToken == null)
            {
                callback();
            }
            else
            {
                RemoveToken(assignedToken, callback);
            }
        }

        public void RemoveToken(GenericToken tokenToRemove, Action callback)
        {
            if (Host.CanRemoveToken(tokenToRemove))
            {
                AssignedTokens.Remove(tokenToRemove);

                if (tokenToRemove.GetType().BaseType == typeof(GenericTargetLockToken))
                {
                    GenericShip otherTokenOwner = (tokenToRemove as GenericTargetLockToken).OtherTokenOwner;
                    ActionsHolder.ReleaseTargetLockLetter((tokenToRemove as GenericTargetLockToken).Letter);
                    Type oppositeType = (tokenToRemove.GetType() == typeof(BlueTargetLockToken)) ? typeof(RedTargetLockToken) : typeof(BlueTargetLockToken);

                    char letter = (tokenToRemove as GenericTargetLockToken).Letter;
                    GenericToken otherTargetLockToken = otherTokenOwner.Tokens.GetToken(oppositeType, letter);
                    if (otherTargetLockToken != null)
                    {
                        otherTokenOwner.Tokens.GetAllTokens().Remove(otherTargetLockToken);
                        otherTokenOwner.CallOnRemoveTokenEvent(otherTargetLockToken.GetType());
                    }
                }

                tokenToRemove.WhenRemoved();
                Host.CallOnRemoveTokenEvent(tokenToRemove.GetType());
            }
            Triggers.ResolveTriggers(TriggerTypes.OnTokenIsRemoved, callback);
        }

        public void RemoveAllTokensByType(Type tokenType, Action callback)
        {
            GenericToken tokenToRemove = GetToken(tokenType);
            if (tokenToRemove != null)
            {
                RemoveToken(
                    tokenType,
                    delegate { RemoveAllTokensByType(tokenType, callback); }
                );
            }
            else
            {
                callback();
            }
        }

        public void RemoveTokens(List<GenericToken> tokensToRemove, Action callback)
        {
            if (tokensToRemove != null && tokensToRemove.Count != 0)
            {
                GenericToken tokenToRemove = tokensToRemove.First();
                tokensToRemove.Remove(tokenToRemove);

                RemoveToken(
                    tokenToRemove,
                    delegate { RemoveTokens(tokensToRemove, callback); }
                );
            }
            else
            {
                callback();
            }
        }

        public void RemoveAllTokensByColor(TokenColors color, Action callback)
        {
            GenericToken tokenToRemove = AssignedTokens.FirstOrDefault(token => token.TokenColor == color);
            if (tokenToRemove != null)
            {
                RemoveToken(
                    tokenToRemove,
                    delegate { RemoveAllTokensByColor(color, callback); }
                );
            }
            else
            {
                callback();
            }
        }

        public void SpendToken(Type type, Action callback, char letter = ' ')
        {
            RemoveToken(
                type,
                delegate { Host.CallFinishSpendToken(type, callback); },
                letter
            );
        }

        // CONDITIONS - don't trigger any abilities

        public void RemoveCondition(Type type)
        {
            GenericToken assignedCondition = GetToken(type);
            RemoveCondition(assignedCondition);
        }

        public void AssignCondition(GenericToken token)
        {
            AssignedTokens.Add(token);

            token.InitializeTooltip();
            token.WhenAssigned();
            Host.CallOnConditionIsAssigned(token.GetType());
        }

        public void AssignCondition(Type tokenType)
        {
            GenericToken token = (GenericToken) Activator.CreateInstance(tokenType, Host);
            AssignCondition(token);
        }

        public static TokenColors GetTokenColorByType(Type tokenType)
        {
            GenericToken token = null;
            if (tokenType != typeof(TractorBeamToken))
            {
                token = (GenericToken)Activator.CreateInstance(tokenType, Roster.AllShips.First().Value);
            }
            else
            {
                token = (GenericToken)Activator.CreateInstance(tokenType, Roster.AllShips.First().Value, Roster.Player1);
            }
            return token.TokenColor;
        }

    }
}
