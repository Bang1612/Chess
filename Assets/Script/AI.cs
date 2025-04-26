using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
public class AI : MonoBehaviour
{
    public enum Difficulty { Dumb, Average, Smart }

    [Header("AI Settings")]
    public static Difficulty aiType;
    public int    maxDepth  = 7;       // deepest search
    public float  maxTime   = 20f;      // seconds before stopping ID
    [Range(0f,1f)]
    public float  randomness = 0.3f;   // for Average level

    [HideInInspector] public Move bestMove;
    public static AI instance; 
    public GameManager      gm;
    Dictionary<string,TTNode> tTable = new Dictionary<string,TTNode>();

    // --- Transposition Table Node ---
    class TTNode {
        public int    depth;
        public float  value;
        public TTFlag flag;
    }
    enum TTFlag { Exact, LowerBound, UpperBound }

    void Awake()
    {

        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        } else {
            instance = this;
        }
        // if(File.Exists(Path.Combine(Application.persistentDataPath, "saved_game.json"))){
            
        // }
    }
    public Difficulty GetAIType(){
        return aiType;
    }
    void Start()
    {
        gm = GameManager.instance;
    }
    /// <summary>
    /// Call this when it’s the AI’s turn.
    /// Starts iterative deepening in the background.
    /// </summary>
    public void GetBestMove()
    {
        // Dumb = purely random
        if (aiType == Difficulty.Dumb)
        {
            MoveDumb();
            return;
        }
        else
        // Otherwise run iterative deepening & pick bestMove
        StartCoroutine(IterativeDeepeningSearch(gm.Clone()));
    }

    void MoveDumb()
    {
        var moves = new List<Move>();
        gm.GetAllLegalMoves(moves);
        bestMove = moves[UnityEngine.Random.Range(0, moves.Count)];
        gm.ApplyMove(bestMove);
        // gm.Nexturn();
    }

    /// <summary>
    /// Iterative deepening from depth=1 up to maxDepth or until maxTime is exhausted.
    /// </summary>
    IEnumerator IterativeDeepeningSearch(GameManager rootState)
    {
        float start = Time.unscaledTime;
        for (int depth = 1; depth <= maxDepth; depth++)
        {
            yield return StartCoroutine(MinimaxAB(rootState, depth));

            // time check
            if (Time.unscaledTime - start > maxTime)
                break;
        }

        // finally apply bestMove
        gm.ApplyMove(bestMove);
        // gm.Nexturn();
    }

    /// <summary>
    /// Performs one fixed‐depth root search, populating bestMove.
    /// </summary>
    IEnumerator MinimaxAB(GameManager rootState, int depth)
    {
        float alpha = float.MinValue, beta = float.MaxValue;
        float bestScore = float.MinValue;
        Move   localBest = null;

        var moves = new List<Move>();
        rootState.GetAllLegalMoves(moves);
        
        for (int i = 0; i < moves.Count; i++)
        {
            var child = rootState.Clone();
            child.VisualApplyMove(moves[i]);

            float score = AlphaBeta(child, depth - 1, alpha, beta, false);

            if (score > bestScore)
            {
                bestScore = score;
                localBest = moves[i];
            }

            alpha = Mathf.Max(alpha, bestScore);
            if (beta <= alpha)
                break;  // prune

            yield return null;  // let Unity breathe
        }

        bestMove = localBest;
    }

    /// <summary>
    /// Minimax with Alpha‐Beta and Transposition Table.
    /// </summary>
    float AlphaBeta(GameManager state, int depth, float alpha, float beta, bool maxPlayer)
    {
        // 1) TT lookup
        string key = state.ToString(); // implement a compact board‐string in GameManager
        if (tTable.TryGetValue(key, out var entry) && entry.depth >= depth)
        {
            switch (entry.flag)
            {
                case TTFlag.Exact:      return entry.value;
                case TTFlag.LowerBound: alpha = Mathf.Max(alpha, entry.value); break;
                case TTFlag.UpperBound: beta  = Mathf.Min(beta,  entry.value); break;
            }
            if (beta <= alpha)
                return entry.value;
        }

        // 2) Leaf or terminal?
        if (depth == 0 || state.IsGameOver())
            return state.Evaluate();

        float bestVal = maxPlayer ? float.MinValue : float.MaxValue;
        var   moves   = new List<Move>();
        state.GetAllLegalMoves(moves);

        // 3) Explore children
        foreach (var m in moves)
        {
            var child = state.Clone();
            child.VisualApplyMove(m);

            float score = AlphaBeta(child, depth - 1, alpha, beta, !maxPlayer);

            if (maxPlayer)
            {
                if (score > bestVal) bestVal = score;
                alpha = Mathf.Max(alpha, bestVal);
            }
            else
            {
                if (score < bestVal) bestVal = score;
                beta  = Mathf.Min(beta, bestVal);
            }
            if (beta <= alpha) break;  // prune
        }

        // 4) Store in TT
        var flag = (bestVal <= alpha) ? TTFlag.UpperBound
                 : (bestVal >= beta)  ? TTFlag.LowerBound
                 : TTFlag.Exact;

        tTable[key] = new TTNode { depth = depth, value = bestVal, flag = flag };

        return bestVal;
    }
}

















// using System.Collections.Generic;
// using System;
// using Unity.VisualScripting.Antlr3.Runtime;
// using UnityEngine;

// public class AI:MonoBehaviour  
// {
//     public GameManager gm;
//     public enum Difficulty { Dumb, Average, Smart }

//     private int maxDepth;
//     public float randomness; 
//     public static Difficulty aiType;
//     public string aiPlayer;
//     void Awake()
//     {
//         gm = GameObject.Find("GameManager").GetComponent<GameManager>();
//         aiPlayer = gm.aiIsBlack ? "Black" : "White";
//     }
//     public Difficulty GetAIType(){
//         return aiType;
//     }
//     public AI() {
        
//         switch (aiType) {
//             case Difficulty.Dumb:
//                 maxDepth = 1; randomness = 1f; break;
//             case Difficulty.Average:
//                 maxDepth = 3; randomness = 0.3f; break;
//             case Difficulty.Smart:
//                 maxDepth = 6; randomness = 0f; break;
//         }
//     }
//     public void GetBestMove(){
//         if (aiType == Difficulty.Dumb){
//             MoveDumb();
             
//         }
//         else if(aiType == Difficulty.Average){
//             GameManager currentState = gm;
//             Move move =Minimax(currentState,maxDepth,int.MinValue,int.MaxValue,true).Item2;
//             if(move == null){
//                 MoveDumb();
//             }
//             else{
//                 gm.ApplyMove(move);
//             }
//         }
//         else{
//             GameManager currentState = gm;
//             Move move =Minimax(currentState,maxDepth,int.MinValue,int.MaxValue,true).Item2;
//             if(move == null){
//                 MoveDumb();
//             }
//             else{
//                 gm.ApplyMove(move);
//             }
//         }

        
//     }
//     public void MoveDumb() {
        
//         List<Move> moves= new List<Move>();
//         if (gm == null) {
//             Debug.Log("GameManager not found!");
//         }
//         Debug.Log("GetMove");
//         gm.GetAllLegalMoves(moves);
//         Debug.Log("No of move "+ moves.Count.ToString());
//         foreach (Move move in moves)
//         {
//             GameObject pieceDes= gm.GetPosition(move.toX,move.toY);
//             if(pieceDes != null){
//                 if(pieceDes.name.Contains("King")){
//                     gm.ApplyMove(move);
//                     return;
//                 }
//             }
//             // moves.Add(move);
//         }
//         int index = UnityEngine.Random.Range(0,moves.Count);
//         gm.ApplyMove(moves[index]);
//         Debug.Log("Initiate Move");
//     }

//     public  (float, Move) Minimax(GameManager state, int depth, float alpha, float beta, bool maximizingPlayer)
//     {
        
//         if (depth == 0 || state.IsGameOver())
//             return ( state.Evaluate(), null );     
//         Move bestMove = null;
//         float bestValue = maximizingPlayer ? float.MinValue : float.MaxValue;
//         List<Move> moves= new List<Move>();
//         state.GetAllLegalMoves(moves);
//         foreach (Move move in moves)
//         {
//             GameManager newState = state;
//             newState.ApplyMove(move);

//             float currentValue = Minimax(newState, depth - 1, alpha, beta, !maximizingPlayer).Item1;

//             if (maximizingPlayer)
//             {
//                 if (currentValue > bestValue)
//                 {
//                     bestValue = currentValue;
//                     bestMove = move;
//                     alpha = Math.Max(alpha, bestValue);
//                 }
//             }
//             else
//             {
//                 if (currentValue < bestValue)
//                 {
//                     bestValue = currentValue;
//                     bestMove = move;
//                     beta = Math.Min(beta, bestValue);
//                 }
//             }

//             if (beta <= alpha)
//                 break;
//         }

//         return (bestValue, bestMove);
//     }
    
// }
