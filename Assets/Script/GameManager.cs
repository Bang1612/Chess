using System;
using System.Collections.Generic;
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
    
    public AI.Difficulty aiDifficulty = AI.Difficulty.Dumb;
    public AI ai;
    public bool aiIsBlack = true;
    int wPawny=1;
    int wPiecey=0;
    int bPawny=6;
    int bPieceyP=7;
    
    void Start()
    {
        GameObject.FindGameObjectWithTag("UpperText").GetComponent<Text>().enabled = true;
        GameObject.FindGameObjectWithTag("DownText").GetComponent<Text>().enabled = false;
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
    if (currentPlayer == "Black") {
        // Delay slightly so the player sees the switch
        Invoke(nameof(AIMove), 0.5f);
    }
        GameObject.FindGameObjectWithTag("UpperText").GetComponent<Text>().text=currentPlayer;
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
            SceneManager.LoadScene("Game");
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
        
        GameObject.FindGameObjectWithTag("DownText").GetComponent<Text>().enabled = true;
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
    
    if(m.isAttack){
        GameObject endPiece = GetPosition(m.toX,m.toY);
        if(endPiece.name == "WKing" ){
            Winner("Black");
        }
        if(endPiece.name == "BKing" ){
            Winner("White");
        }
        
        if (bPieces.Contains(endPiece)) bPieces.Remove(endPiece);
    else if (wPieces.Contains(endPiece)) wPieces.Remove(endPiece);
        
        endPiece.SetActive(false);
    }
    SetPositionEmpty(m.fromX,m.fromY);
    startPiece.GetComponent<ChessPiece>().Goto(m.toX,m.toY);
    SetPosition(startPiece);
    Nexturn();

}

public GameManager Clone() {
    // Create a new GameObject instance and copy fields, or
    // manually new‐up a GameManager and deep‐copy arrays & lists.
    var copy = new GameManager();
    copy.currentPlayer = this.currentPlayer;
    copy.gameOver      = this.gameOver;
    copy.positions     = (GameObject[,]) this.positions.Clone();
    copy.bPieces       = new List<GameObject>(this.bPieces);
    copy.wPieces       = new List<GameObject>(this.wPieces);
    return copy;
}

public float Evaluate() {
    float score = 0;
    float fluctuation = UnityEngine.Random.Range(-ai.randomness, +ai.randomness);
    foreach (var p in bPieces) 
        score -= PieceValue(p)*(1f + fluctuation);   // black is negative for AI
    foreach (var p in wPieces) 
        score += PieceValue(p)*(1f + fluctuation);   // white is positive for AI
    
    return score;
}

private int PieceValue(GameObject piece) {
    switch(piece.name) {
        case "BPawn": case "WPawn":   return 1;
        case "BKnight":case "WKnight":return 3;
        case "BBishop":case "WBishop":return 3;
        case "BRook": case "WRook":   return 5;
        case "BQueen":case "WQueen":  return 9;
        case "BKing": case "WKing":   return 0;
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
