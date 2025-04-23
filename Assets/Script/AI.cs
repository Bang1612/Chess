using System.Collections.Generic;
using System;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class AI : MonoBehaviour
{
    public GameManager gm;
    public enum Difficulty { Dumb, Average, Smart }

    private int maxDepth;
    public float randomness; 
    public Difficulty aiType;
    public string aiPlayer;
    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        aiPlayer = gm.aiIsBlack ? "Black" : "White";
    }
    public AI(Difficulty difficulty) {
        aiType = difficulty;
        switch (difficulty) {
            case Difficulty.Dumb:
                maxDepth = 1; randomness = 1f; break;
            case Difficulty.Average:
                maxDepth = 3; randomness = 0.3f; break;
            case Difficulty.Smart:
                maxDepth = 6; randomness = 0f; break;
        }
    }
    public void GetBestMove(){
        if (aiType == Difficulty.Dumb){
            MoveDumb();
             
        }
        else if(aiType == Difficulty.Average){
            GameManager currentState = gm;
            Move move =Minimax(currentState,maxDepth,int.MinValue,int.MaxValue,true).Item2;
            if(move == null){
                MoveDumb();
            }
            else{
                gm.ApplyMove(move);
            }
        }
        else{
            GameManager currentState = gm;
            Move move =Minimax(currentState,maxDepth,int.MinValue,int.MaxValue,true).Item2;
            if(move == null){
                MoveDumb();
            }
            else{
                gm.ApplyMove(move);
            }
        }

        
    }
    public void MoveDumb() {
        
        List<Move> moves= new List<Move>();
        if (gm == null) {
            Debug.Log("GameManager not found!");
        }
        Debug.Log("GetMove");
        gm.GetAllLegalMoves(moves);
        Debug.Log("No of move "+ moves.Count.ToString());
        foreach (Move move in moves)
        {
            GameObject pieceDes= gm.GetPosition(move.toX,move.toY);
            if(pieceDes != null){
                if(pieceDes.name.Contains("King")){
                    gm.ApplyMove(move);
                    return;
                }
            }
            // moves.Add(move);
        }
        int index = UnityEngine.Random.Range(0,moves.Count);
        gm.ApplyMove(moves[index]);
        Debug.Log("Initiate Move");
    }

    public  (float, Move) Minimax(GameManager state, int depth, float alpha, float beta, bool maximizingPlayer)
    {
        
        if (depth == 0 || state.IsGameOver())
            return ( state.Evaluate(), null );     
        Move bestMove = null;
        float bestValue = maximizingPlayer ? float.MinValue : float.MaxValue;
        List<Move> moves= new List<Move>();
        state.GetAllLegalMoves(moves);
        foreach (Move move in moves)
        {
            GameManager newState = state;
            newState.ApplyMove(move);

            float currentValue = Minimax(newState, depth - 1, alpha, beta, !maximizingPlayer).Item1;

            if (maximizingPlayer)
            {
                if (currentValue > bestValue)
                {
                    bestValue = currentValue;
                    bestMove = move;
                    alpha = Math.Max(alpha, bestValue);
                }
            }
            else
            {
                if (currentValue < bestValue)
                {
                    bestValue = currentValue;
                    bestMove = move;
                    beta = Math.Min(beta, bestValue);
                }
            }

            if (beta <= alpha)
                break;
        }

        return (bestValue, bestMove);
    }
    
}
