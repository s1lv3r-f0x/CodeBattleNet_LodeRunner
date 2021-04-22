/*-
 * #%L
 * Codenjoy - it's a dojo-like platform from developers to developers.
 * %%
 * Copyright (C) 2018 Codenjoy
 * %%
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as
 * published by the Free Software Foundation, either version 3 of the
 * License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public
 * License along with this program.  If not, see
 * <http://www.gnu.org/licenses/gpl-3.0.html>.
 * #L%
 */
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http.Headers;
using Loderunner.Api;

namespace Loderunner
{
    /// <summary>
    /// This is LoderunnerAI client demo.
    /// </summary>
    internal class MyLoderunnerBot : LoderunnerBase
    {
        private GameBoard gameBoard;
        private BoardPoint myPos;

        public MyLoderunnerBot(string serverUrl)
            : base(serverUrl)
        {
        }

        private BoardPoint GetMinLength(List<BoardPoint> goldList)
        {
            double minLength = Double.MaxValue;
            BoardPoint point = new BoardPoint(0, 0);
            foreach (var goldPoint in goldList)
            {
                if (goldPoint.Y == myPos.Y)
                    return goldPoint;
                var length =
                    Math.Sqrt(Math.Pow(myPos.X - goldPoint.X, 2) + Math.Pow(myPos.Y - goldPoint.Y, 2));
                if (length < minLength)
                {
                    minLength = length;
                    point = goldPoint;
                }
            }
            return point;
        }


        protected override string DoMove(GameBoard gameBoard)
        {
            Console.Clear();
            gameBoard.PrintBoard(); 
            LoderunnerAction action;
            this.gameBoard = gameBoard;
            this.myPos = gameBoard.GetMyPosition();

            var goldPosition = gameBoard.GetGoldPositions();
            var ladderPos = gameBoard.GetLadderPositions();
            var ladderPoint = GetMinLength(ladderPos);
            var point = GetMinLength(goldPosition);
            if (gameBoard.HasLadderAt(myPos.X, myPos.Y))
            {
                return LoderunnerActionToString(LoderunnerAction.GoUp);
            }

            if (gameBoard.HasPipeAt(myPos.X, myPos.Y))
                return LoderunnerActionToString(LoderunnerAction.GoDown);
            if (gameBoard.HasLadderAt(myPos.X, myPos.Y) && gameBoard.HasGoldAt(myPos.X, myPos.Y-1))
                return LoderunnerActionToString(LoderunnerAction.GoUp);
            if (point.Y != myPos.Y)
            {
                if (gameBoard.HasPipeAt(myPos.X, myPos.Y))
                {
                    return LoderunnerActionToString(LoderunnerAction.GoDown);
                }
                if (ladderPoint == myPos)
                {
                    var len = ladderPoint.X - myPos.X;
                    if (len > 0)
                    {
                        Console.WriteLine(LoderunnerAction.GoRight.ToString());
                        return LoderunnerActionToString(LoderunnerAction.GoRight);
                    }
                    if (len < 0)
                    {
                        Console.WriteLine(LoderunnerAction.GoLeft.ToString());
                        return LoderunnerActionToString(LoderunnerAction.GoLeft);
                    }

                    if (len == 0)
                    {
                        Console.WriteLine(LoderunnerAction.GoUp.ToString());
                        return LoderunnerActionToString(LoderunnerAction.GoUp);
                    }
                }
            }

            if (myPos.Y < point.Y)
                action = LoderunnerAction.GoUp;
            if (myPos.X < point.X)
            {
                action = LoderunnerAction.GoRight;
            }
            else
            {
                action = LoderunnerAction.GoLeft;
            }
            Console.WriteLine(action.ToString());
            return LoderunnerActionToString(action);
        }

        
        public void InitiateExit()
        {
            _cts.Cancel();
        }
    }
}
