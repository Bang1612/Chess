using System;
using System.Collections.Generic;
using UnityEngine;

public enum Piece {
    Empty,
    WPawn, WKnight, WBishop, WRook, WQueen, WKing,
    BPawn, BKnight, BBishop, BRook, BQueen, BKing
}

public class BoardSnapshot
{
    public Piece[,] board = new Piece[8,8];
    public bool   whiteToMove = true;

    public BoardSnapshot() { }

    // Deep‐copy constructor
    public BoardSnapshot(BoardSnapshot other)
    {
        whiteToMove = other.whiteToMove;
        for(int x=0; x<8; x++)
            for(int y=0; y<8; y++)
                board[x,y] = other.board[x,y];
    }

    public BoardSnapshot Clone() => new BoardSnapshot(this);

    public bool IsGameOver()
    {
        // e.g. no kings or no legal moves
        bool whiteKing = false, blackKing = false;
        foreach (var p in board)
        {
            if (p == Piece.WKing) whiteKing = true;
            if (p == Piece.BKing) blackKing = true;
        }
        if (!whiteKing || !blackKing) return true;
        return GetAllLegalMoves().Count == 0;
    }

    public float Evaluate()
    {
        // simple material count
        float score = 0;
        for(int x=0; x<8; x++) for(int y=0; y<8; y++)
        {
            switch(board[x,y])
            {
                case Piece.WPawn:   score += 1; break;
                case Piece.WKnight: score += 3; break;
                case Piece.WBishop: score += 3; break;
                case Piece.WRook:   score += 5; break;
                case Piece.WQueen:  score += 9; break;
                case Piece.BPawn:   score -= 1; break;
                case Piece.BKnight: score -= 3; break;
                case Piece.BBishop: score -= 3; break;
                case Piece.BRook:   score -= 5; break;
                case Piece.BQueen:  score -= 9; break;
                default: break;
            }
        }
        return score;
    }

    // VERY similar to your GameManager.GetAllLegalMoves,
    // but using 'board[x,y]' instead of GameObjects.
    public List<Move> GetAllLegalMoves()
    {
        var moves = new List<Move>();
        bool white = whiteToMove;
        var deltasKnight = new (int,int)[]{ (1,2),(-1,2),(1,-2),(-1,-2),(2,1),(-2,1),(2,-1),(-2,-1) };
        var deltasLine   = new (int,int)[]{ (1,0),(-1,0),(0,1),(0,-1),(1,1),(1,-1),(-1,1),(-1,-1) };

        for(int x=0; x<8; x++) for(int y=0; y<8; y++)
        {
            var p = board[x,y];
            if (p == Piece.Empty) continue;
            bool isWhitePiece = ((int)p < (int)Piece.BPawn);
            if (isWhitePiece != white) continue;

            // Pawn
            if (p == (white ? Piece.WPawn : Piece.BPawn))
            {
                int dir = white ? 1 : -1;
                int ny = y + dir;
                if (InRange(x,ny) && board[x,ny] == Piece.Empty)
                    moves.Add(new Move(x,y,x,ny,false));
                foreach (int dx in new[]{ -1,1 })
                {
                    int nx = x+dx;
                    if (InRange(nx,ny) && board[nx,ny]!=Piece.Empty)
                    {
                        bool targetWhite = ((int)board[nx,ny] < (int)Piece.BPawn);
                        if (targetWhite != white)
                            moves.Add(new Move(x,y,nx,ny,true));
                    }
                }
            }
            // Knight
            else if (p == (white ? Piece.WKnight : Piece.BKnight))
            {
                foreach (var d in deltasKnight)
                {
                    int nx = x+d.Item1, ny = y+d.Item2;
                    if (!InRange(nx,ny)) continue;
                    if (board[nx,ny]==Piece.Empty ||
                        (((int)board[nx,ny]< (int)Piece.BPawn) != white))
                        moves.Add(new Move(x,y,nx,ny,board[nx,ny]!=Piece.Empty));
                }
            }
            // Sliding (rook/bishop/queen)
            else if (IsSlidingPiece(p))
            {
                foreach (var d in deltasLine)
                {
                    int nx=x+d.Item1, ny=y+d.Item2;
                    while (InRange(nx,ny))
                    {
                        if (board[nx,ny] == Piece.Empty)
                        {
                            moves.Add(new Move(x,y,nx,ny,false));
                        }
                        else
                        {
                            bool targetWhite = ((int)board[nx,ny] < (int)Piece.BPawn);
                            if (targetWhite != white)
                                moves.Add(new Move(x,y,nx,ny,true));
                            break;
                        }
                        nx += d.Item1; ny += d.Item2;
                    }
                }
            }
            // King (one‐square in all directions)
            else if (p == (white ? Piece.WKing : Piece.BKing))
            {
                for(int dx=-1; dx<=1; dx++) for(int dy=-1; dy<=1; dy++)
                {
                    if (dx==0 && dy==0) continue;
                    int nx=x+dx, ny=y+dy;
                    if (!InRange(nx,ny)) continue;
                    if (board[nx,ny]==Piece.Empty ||
                        (((int)board[nx,ny]< (int)Piece.BPawn) != white))
                        moves.Add(new Move(x,y,nx,ny,board[nx,ny]!=Piece.Empty));
                }
            }
        }
        return moves;
    }

    public void ApplyMove(Move m)
    {
        // Simple array moves & flip side
        board[m.toX,m.toY] = board[m.fromX,m.fromY];
        board[m.fromX,m.fromY] = Piece.Empty;
        whiteToMove = !whiteToMove;
    }

    private bool InRange(int x,int y) => x>=0 && x<8 && y>=0 && y<8;
    private bool IsSlidingPiece(Piece p) =>
        p==Piece.WBishop||p==Piece.WRook||p==Piece.WQueen||
        p==Piece.BBishop||p==Piece.BRook||p==Piece.BQueen;

    // Build a simple string key for your transposition table:
    public override string ToString()
    {
        var sb = new System.Text.StringBuilder(64);
        sb.Append(whiteToMove ? 'W' : 'B');
        for(int y=0;y<8;y++) for(int x=0;x<8;x++)
            sb.Append((int)board[x,y]).Append(',');
        return sb.ToString();
    }
}
