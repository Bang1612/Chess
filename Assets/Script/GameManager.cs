using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject Piece;
    // public Text turnPlayerText;
    // public Text winnerText;
    // public Text restartText;
    public GameObject[,] positions = new GameObject[8,8];
    public List<GameObject> bPieces = new List<GameObject>();
    public List<GameObject> wPieces = new List<GameObject>();
    public string currentPlayer = "White";
    public bool gameOver = false;
    // public PlayerManager pm;
    public bool isCloned=false;
    public GameObject DownText;
    public AI.Difficulty aiDifficulty = AI.Difficulty.Dumb;
    public AI ai;
    public bool aiIsBlack = true;
    int wPawny=1;
    int wPiecey=0;
    int bPawny=6;
    int bPieceyP=7;
    
    void Start()
    {
        ai = GameObject.Find("AI").GetComponent<AI>();
        aiDifficulty = ai.GetAIType();
        switch (aiDifficulty)
        {
            case AI.Difficulty.Dumb:
            Debug.Log("Easy mode");
            break;
            case AI.Difficulty.Average:
            Debug.Log("Normal mode");
            break;
            case AI.Difficulty.Smart:
            Debug.Log("Hard mode");
            break;
            default:
            break;
        }
        GameObject.FindGameObjectWithTag("UpperText").GetComponent<Text>().enabled = true;
        DownText =GameObject.FindGameObjectWithTag("DownText");
        DownText.SetActive(false);
        GameObject.FindGameObjectWithTag("UpperText").GetComponent<Text>().text=currentPlayer;
        // ai = new AI(aiDifficulty);
        // ai.aiType =AI.Difficulty.Dumb;
        bPieces = new List<GameObject>{
            Create("BPawn",0,bPawny), Create("BPawn",1,bPawny), Create("BPawn",2,bPawny), Create("BPawn",3,bPawny),
            Create("BPawn",4,bPawny), Create("BPawn",5,bPawny), Create("BPawn",6,bPawny), Create("BPawn",7,bPawny),
            Create("BRook",0,bPieceyP), Create("BRook",7,bPieceyP), Create("BKnight",1,bPieceyP), Create("BKnight",6,bPieceyP),
            Create("BBishop",2,bPieceyP), Create("BBishop",5,bPieceyP), Create("BKing",3,bPieceyP), Create("BQueen",4,bPieceyP),
        };
        wPieces = new List<GameObject>{
            Create("WPawn",0,wPawny), Create("WPawn",1,wPawny), Create("WPawn",2,wPawny), Create("WPawn",3,wPawny),
            Create("WPawn",4,wPawny), Create("WPawn",5,wPawny), Create("WPawn",6,wPawny), Create("WPawn",7,wPawny),
            Create("WRook",0,wPiecey), Create("WRook",7,wPiecey), Create("WKnight",1,wPiecey), Create("WKnight",6,wPiecey),
            Create("WBishop",2,wPiecey), Create("WBishop",5,wPiecey), Create("WKing",3,wPiecey), Create("WQueen",4,wPiecey),
        };
        for(int i=0;i<bPieces.Count;i++){
            SetPosition(bPieces[i]);
            SetPosition(wPieces[i]);
        }
    }
    GameObject Create(string name, int x, int y){
        GameObject obj = Instantiate(Piece, new Vector3(0,0,-1), Quaternion.identity);
        ChessPiece cp = obj.GetComponent<ChessPiece>(); 
        cp.name = name;
        cp.Goto(x,y);
        cp.Activated();
        return obj;
    }
    public void SetPosition(GameObject obj){
        ChessPiece cp = obj.GetComponent<ChessPiece>(); 
        positions[cp.Getx(),cp.Gety()] = obj;
    }
    public void SetPositionEmpty(int x, int y){
        positions[x,y]= null;
    }
    public GameObject GetPosition(int x, int y){
        return positions[x,y];
    }
    public bool Available(int x, int y){
        if(x < 0 || y <0 || x >7 || y >7) return false;
        return true;
    }
    public string GetCurrentPlayer(){
        return currentPlayer;
    }
    public bool IsGameOver(){
    return gameOver;
    }
    public void Nexturn(){
        currentPlayer = (currentPlayer == "White") ? "Black" : "White";
        GameObject.FindGameObjectWithTag("UpperText").GetComponent<Text>().text=currentPlayer;
        // schedule AI only on the *real* board, not on clones
        bool aiTurn = (currentPlayer == "Black" && aiIsBlack)
                || (currentPlayer == "White" && !aiIsBlack);

        if (!isCloned && aiTurn)
            Invoke(nameof(AIMove), 0.5f);
        }
    private void AIMove() {
    
        ai.GetBestMove();
    // ApplyAIMove(aiMove);
    //    Nexturn();
    }
    void Update()
    {
        if(gameOver && Input.GetMouseButtonDown(0)){
            gameOver=false;
            // SceneManager.LoadScene("Game");
        }
        // GameObject.FindGameObjectWithTag("UpperText").GetComponent<Text>().text=currentPlayer;
        // if (!gameOver && ((currentPlayer == "Black" && aiIsBlack) || (currentPlayer == "White" && !aiIsBlack)))
        // {
        //     StartCoroutine(AIMakeMove());
        // }
    }
    public void Winner(string winner){
        gameOver=true;
        // GameObject.FindGameObjectWithTag("UpperText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("UpperText").GetComponent<Text>().text="Winner: "+winner;
        
        DownText.SetActive(true);
    }
    public void GetAllLegalMoves(List<Move> moves){
        Debug.Log("Start Getting moves");
        // List<Move> moves = new List<Move>();
        var pieces = (currentPlayer == "Black") ? bPieces : wPieces;
        foreach (GameObject cp in pieces)
        {
            ChessPiece chessPiece =cp.GetComponent<ChessPiece>();
            int x = chessPiece.Getx();
            int y = chessPiece.Gety();
            string name = chessPiece.name;
                 // Pawn logic
        if (name.Contains("Pawn")) {
            int dir = (currentPlayer == "White") ? 1 : -1;
            AddPawnMoves(moves, x, y, dir);
        }
        // Knight logic
        else if (name.Contains("Knight")) {
            AddKnightMoves(moves, x, y);
        }
        // Bishop logic
        else if (name.Contains("Bishop")) {
            AddLineMoves(moves, x, y,  1,  1);
            AddLineMoves(moves, x, y,  1, -1);
            AddLineMoves(moves, x, y, -1,  1);
            AddLineMoves(moves, x, y, -1, -1);
        }
        // Rook logic
        else if (name.Contains("Rook")) {
            AddLineMoves(moves, x, y,  1,  0);
            AddLineMoves(moves, x, y, -1,  0);
            AddLineMoves(moves, x, y,  0,  1);
            AddLineMoves(moves, x, y,  0, -1);
        }
        // Queen logic (rook + bishop)
        else if (name.Contains("Queen")) {
            AddLineMoves(moves, x, y,  1,  0);
            AddLineMoves(moves, x, y, -1,  0);
            AddLineMoves(moves, x, y,  0,  1);
            AddLineMoves(moves, x, y,  0, -1);
            AddLineMoves(moves, x, y,  1,  1);
            AddLineMoves(moves, x, y,  1, -1);
            AddLineMoves(moves, x, y, -1,  1);
            AddLineMoves(moves, x, y, -1, -1);
        }
        // King logic
        else if (name.Contains("King")) {
            AddKingMoves(moves, x, y);
        }
            }
        
        // return moves;
    }

    // 3.1 Pawn Moves
private void AddPawnMoves(List<Move> moves, int x, int y, int dir) {
    // forward
    if (Available(x, y + dir) && GetPosition(x, y + dir) == null)
        moves.Add(new Move(x, y, x, y + dir, false));
    // captures
    foreach (int dx in new int[] { -1, 1 }) {
        int nx = x + dx, ny = y + dir;
        if (Available(nx, ny)) {
            var target = GetPosition(nx, ny);
            if (target != null && target.GetComponent<ChessPiece>().Player != currentPlayer)
                moves.Add(new Move(x, y, nx, ny, true));
        }
    }
}

// 3.2 Knight Moves
private void AddKnightMoves(List<Move> moves, int x, int y) {
    List<(int,int)> deltas = new List<(int, int)> { (1,2),(-1,2),(1,-2),(-1,-2),(2,1),(-2,1),(2,-1),(-2,-1) };
    foreach (var d in deltas) {
        int nx = x + d.Item1, ny = y + d.Item2;
        if (!Available(nx, ny)) continue;
        var target = GetPosition(nx, ny);
        if (target == null)
            moves.Add(new Move(x, y, nx, ny, false));
        else if (target.GetComponent<ChessPiece>().Player != currentPlayer)
            moves.Add(new Move(x, y, nx, ny, true));
    }
}

// 3.3 Sliding Pieces (Rook/Bishop/Queen)
private void AddLineMoves(List<Move> moves, int x, int y, int dx, int dy) {
    int nx = x + dx, ny = y + dy;
    while (Available(nx, ny)) {
        var target = GetPosition(nx, ny);
        if (target == null) {
            moves.Add(new Move(x, y, nx, ny, false));
        } else {
            if (target.GetComponent<ChessPiece>().Player != currentPlayer)
                moves.Add(new Move(x, y, nx, ny, true));
            break;  // blocked
        }
        nx += dx; ny += dy;
    }
}

// 3.4 King Moves
private void AddKingMoves(List<Move> moves, int x, int y) {
    for (int dx = -1; dx <= 1; dx++)
    for (int dy = -1; dy <= 1; dy++) {
        if (dx == 0 && dy == 0) continue;
        int nx = x + dx, ny = y + dy;
        if (!Available(nx, ny)) continue;
        var target = GetPosition(nx, ny);
        if (target == null)
            moves.Add(new Move(x, y, nx, ny, false));
        else if (target.GetComponent<ChessPiece>().Player != currentPlayer)
            moves.Add(new Move(x, y, nx, ny, true));
    }
}

public void ApplyMove(Move m){
    GameObject startPiece = GetPosition(m.fromX,m.fromY);
    if (startPiece == null)
    {
        Debug.LogError($"ApplyMove: no piece at ({m.fromX},{m.fromY})");
        return;
    }
    if(m.isAttack){
        GameObject endPiece = GetPosition(m.toX,m.toY);
        if (endPiece != null)
        {
            if (endPiece.name == "WKing") Winner("Black");
            else if (endPiece.name == "BKing") Winner("White");

            if (bPieces.Contains(endPiece)) bPieces.Remove(endPiece);
            else if (wPieces.Contains(endPiece)) wPieces.Remove(endPiece);

            endPiece.SetActive(false);
        }
        else
        {
            Debug.LogWarning($"ApplyMove: expected to capture at ({m.toX},{m.toY}) but found none");
        }
        
        
    }
    SetPositionEmpty(m.fromX,m.fromY);
    startPiece.GetComponent<ChessPiece>().Goto(m.toX,m.toY);
    SetPosition(startPiece);
    Nexturn();

}

public void VisualApplyMove(Move m)
{
    // grab the “moving” piece reference
    GameObject mover = positions[m.fromX, m.fromY];
    if (mover == null) return;  // sanity check

    // clear the source square
    positions[m.fromX, m.fromY] = null;

    // if it’s a capture, remove that piece from our lists & array
    if (m.isAttack)
    {
        GameObject victim = positions[m.toX, m.toY];
        if (victim != null)
        {
            bPieces.Remove(victim);
            wPieces.Remove(victim);
            positions[m.toX, m.toY] = null;
        }
    }

    // place the mover in its new square
    positions[m.toX, m.toY] = mover;
    
    // update your piece‐lists so that search’s bPieces/wPieces stay in sync
    // (they track GameObject references, not positions, so this is already correct)

    // // flip the “turn” so your GetAllLegalMoves(...) for the next search node sees the right side
    currentPlayer = (currentPlayer == "White") ? "Black" : "White";

}

public GameManager Clone() {
    // Create a new GameObject instance and copy fields, or
    // manually new‐up a GameManager and deep‐copy arrays & lists.
    var copy = new GameManager();
    copy.isCloned = true;
    copy.aiIsBlack = this.aiIsBlack;
    copy.currentPlayer = this.currentPlayer;
    copy.gameOver      = this.gameOver;
    copy.positions     = (GameObject[,]) this.positions.Clone();
    copy.bPieces       = new List<GameObject>(this.bPieces);
    copy.wPieces       = new List<GameObject>(this.wPieces);
    return copy;
}
// public BoardSnapshot ToBoardSnapshot()
// {
//     var snap = new BoardSnapshot();
//     // whose turn?
//     snap.whiteToMove = (currentPlayer == "White");

//     // walk every square
//     for (int x = 0; x < 8; x++)
//     for (int y = 0; y < 8; y++)
//     {
//         GameObject go = positions[x, y];
//         if (go == null)
//         {
//             // empty square
//             snap.board[x, y] = global::Piece.Empty;
//         }
//         else
//         {
//             // piece is alive here—map its name to the enum
//             // strip any "(Clone)" suffix Unity may append
//             string n = go.name;
//             int idx = n.IndexOf('(');
//             if (idx >= 0) n = n.Substring(0, idx);

//             switch (n)
//             {
//                 case "WPawn":   snap.board[x, y] = global::Piece.WPawn;   break;
//                 case "WKnight": snap.board[x, y] = global::Piece.WKnight; break;
//                 case "WBishop": snap.board[x, y] = global::Piece.WBishop; break;
//                 case "WRook":   snap.board[x, y] = global::Piece.WRook;   break;
//                 case "WQueen":  snap.board[x, y] = global::Piece.WQueen;  break;
//                 case "WKing":   snap.board[x, y] = global::Piece.WKing;   break;

//                 case "BPawn":   snap.board[x, y] = global::Piece.BPawn;   break;
//                 case "BKnight": snap.board[x, y] = global::Piece.BKnight; break;
//                 case "BBishop": snap.board[x, y] = global::Piece.BBishop; break;
//                 case "BRook":   snap.board[x, y] = global::Piece.BRook;   break;
//                 case "BQueen":  snap.board[x, y] = global::Piece.BQueen;  break;
//                 case "BKing":   snap.board[x, y] = global::Piece.BKing;   break;

//                 default:
//                     // shouldn't happen, but safe-guard
//                     snap.board[x, y] = global::Piece.Empty;
//                     Debug.LogWarning($"Unknown piece name '{go.name}' at [{x},{y}]");
//                     break;
//             }
//         }
//     }

//     return snap;
// }


public float Evaluate() {
    // 1) Terminal check
    if (IsGameOver()) {
        if(aiIsBlack){
            return bPieces.Contains(GameObject.Find("BKing")) ?  +10000f: -10000f;
            
        }
        else return wPieces.Contains(GameObject.Find("WKing")) ? +10000f : -10000f;
    }

    float material = 0, mobility;

    foreach (var p in wPieces) {
        material += PieceValue(p);
        
    }
    foreach (var p in bPieces) {
        material -= PieceValue(p);
        
    }
    List<Move> temp=new List<Move>();
    GetAllLegalMoves(temp);
    int bMoveCount;
    int wMoveCount;
    if(currentPlayer == "Black"){
        bMoveCount=temp.Count;
        currentPlayer = "White";
        temp=new List<Move>();
        GetAllLegalMoves(temp);
        wMoveCount = temp.Count;
    }
    else{
        wMoveCount=temp.Count;
        currentPlayer = "Black";
        temp=new List<Move>();
        GetAllLegalMoves(temp);
        bMoveCount = temp.Count;
    }
    mobility = 0.1f * (wMoveCount - bMoveCount);

    float raw = material  + mobility;
    return aiIsBlack ? -raw : raw;
}

private int PieceValue(GameObject piece) {
    switch(piece.name) {
        case "BPawn": case "WPawn":   return 1;
        case "BKnight":case "WKnight":return 3;
        case "BBishop":case "WBishop":return 3;
        case "BRook": case "WRook":   return 5;
        case "BQueen":case "WQueen":  return 9;
        case "BKing": case "WKing":   return 10;
    }
    return 0;
}
}

public class Move
{
    public int fromX, fromY, toX, toY;
    public bool isAttack;

    public Move(int fx, int fy, int tx, int ty, bool attack = false)
    {
        fromX = fx;
        fromY = fy;
        toX = tx;
        toY = ty;
        isAttack = attack;
    }
}
