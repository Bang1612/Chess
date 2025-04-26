using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;
using Unity.Netcode;
public class GameManager : NetworkBehaviour
{
    public GameObject Piece;
    public static GameManager instance; 
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
    public static bool  aiEnable = true;
    public GameObject WinPopup;
    int wPawny=1;
    int wPiecey=0;
    int bPawny=6;
    int bPieceyP=7;
    public bool amWhite = true;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        } else {
            instance = this;
        } 
    }
    void Start()
    {
        if (NetworkManager.Singleton.IsHost)
            amWhite = true;    // Host plays White
        else if (IsClient){
            amWhite = false; // Second player is Black
            wPawny=6;
            wPiecey=7;
            bPawny=1;
            bPieceyP=0;
        }
               
        bool Loaded=false;
        // bool SavedFileCheck= File.Exists(Path.Combine(Application.persistentDataPath, "saved_game.json"));
        //Check for save
        if(SaveManager.instance.CurrentSave != null){
            // string path = Path.Combine(Application.persistentDataPath, "saved_game.json");
            // string json = File.ReadAllText(path);
            LoadSave();
            Debug.Log("Save Loaded");
            Loaded=true;
        }
        if(ai.enabled){
            ai=AI.instance;
         // ai = GameObject.Find("AI").GetComponent<AI>();
        aiDifficulty = ai.GetAIType();
        }
        
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
        WinPopup.SetActive(false);
        GameObject.FindGameObjectWithTag("UpperText").GetComponent<Text>().text=currentPlayer;
        // ai = new AI(aiDifficulty);
        // ai.aiType =AI.Difficulty.Dumb;
        
        if(!Loaded){
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
    }
    public void SubmitMove(Move m)
    {
        if (NetworkManager.Singleton.IsListening)
        {
            // we're online: send to server
            SubmitMoveServerRpc(m.fromX, m.fromY, m.toX, m.toY, m.isAttack);
        }
        else
        {
            // offline: just apply locally
            ApplyMove(m);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void SubmitMoveServerRpc(int fx, int fy, int tx, int ty, bool attack)
    {
        var m = new Move(fx, fy, tx, ty, attack);

        // YOU: validate turn / legality here if you like…

        // apply on server
        ApplyMove(m);

        // broadcast to all clients
        BroadcastMoveClientRpc(fx, fy, tx, ty, attack);
    }

    [ClientRpc]
    void BroadcastMoveClientRpc(int fx, int fy, int tx, int ty, bool attack)
    {
        // avoid re-applying on host (it already did ApplyMove)
        if (!IsServer)
        {
            ApplyMove(new Move(fx, fy, tx, ty, attack));
        }
    }
    GameObject Create(string name, int x, int y){
        GameObject obj = Instantiate(Piece, new Vector3(0,0,-1), Quaternion.identity);
        ChessPiece cp = obj.GetComponent<ChessPiece>(); 
        cp.name = name;
        cp.Goto(x,y);
        cp.Activated();

    //     var no = obj.GetComponent<NetworkObject>();
    // no.Spawn();
        return obj;
    }
    public void SetPosition(GameObject obj){
        // Debug.Log("Step 1 in");
        ChessPiece cp = obj.GetComponent<ChessPiece>();
        // Debug.Log("Step 2 in"); 
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
                || (currentPlayer == "White" && !aiIsBlack) ;
        

        if (!isCloned && aiTurn && aiEnable)
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
        WinPopup.SetActive(true);
        GameObject.FindGameObjectWithTag("GameOver").GetComponent<Text>().text="Winner: "+winner;
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
    if(!IsGameOver()){
        Nexturn();
    }
    

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
    copy.aiDifficulty = this.aiDifficulty;
    copy.currentPlayer = this.currentPlayer;
    copy.gameOver      = this.gameOver;
    copy.positions     = (GameObject[,]) this.positions.Clone();
    copy.bPieces       = new List<GameObject>(this.bPieces);
    copy.wPieces       = new List<GameObject>(this.wPieces);
    return copy;
}

public void SaveCreate(){
    SaveData saveData =new SaveData();
    saveData.aiIsBlack = this.aiIsBlack;
    saveData.aiDifficulty = this.aiDifficulty;
    saveData.currentPlayer = this.currentPlayer;
    saveData.gameOver = this.gameObject;
    saveData.positions = SavePosition(this.positions);
    // saveData.bPieces= new List<GameObject>(this.bPieces);
    // saveData.wPieces= new List<GameObject>(this.wPieces);
    // CheckGridContents(saveData.positions);
    // saveData.UseAI = this.aiEnable;
    SaveManager.instance.StoreSave(saveData);

}
public int[,] SavePosition(GameObject[,] positions){
    int[,] res= new int[8,8];
    int Checksum=0;
    for(int x=0;x<8;x++){
        for(int y=0;y<8;y++){
            if(positions[x,y]==null){
                res[x,y]=0;
            }
            //Pawn=1, Rook=2, Knight=3, Bishop=4, Queen=5,King=6, White pos, Black neg
            else{
                switch (positions[x,y].GetComponent<ChessPiece>().name)
                {
                    case "WPawn":
                    res[x,y]=1;
                    Checksum++;
                    break;
                    case "WRook":
                    res[x,y]=2;
                    Checksum++;
                    break;
                    case "WKnight":
                    res[x,y]=3;
                    Checksum++;
                    break;
                    case "WBishop":
                    res[x,y]=4;
                    Checksum++;
                    break;
                    case "WQueen":
                    res[x,y]=5;
                    Checksum++;
                    break;
                    case "WKing":
                    res[x,y]=6;
                    Checksum++;
                    break;

                    case "BPawn":
                    res[x,y]=-1;
                    Checksum++;
                    break;
                    case "BRook":
                    res[x,y]=-2;
                    Checksum++;
                    break;
                    case "BKnight":
                    res[x,y]=-3;
                    Checksum++;
                    break;
                    case "BBishop":
                    res[x,y]=-4;
                    Checksum++;
                    break;
                    case "BQueen":
                    res[x,y]=-5;
                    Checksum++;
                    break;
                    case "BKing":
                    res[x,y]=-6;
                    Checksum++;
                    break;
                    default:
                    res[x,y]=0;
                    break;
                }
            }
        }
    }
    Debug.Log("Total pieces: "+Checksum.ToString() );
    // CheckGridContents(res);
    return res;
}

void CheckGridContents(int[,] grid)
{
    int count=0;
    int rows = grid.GetLength(0); // Get row count
    int cols = grid.GetLength(1); // Get column count

    for (int i = 0; i < rows; i++)
    {
        for (int j = 0; j < cols; j++)
        {
            if(grid[i,j] != 0) count++;
            
        }
    }
    Debug.Log($"Total piece is {count}");
}
public void LoadSave() {
    SaveData source = SaveManager.instance.CurrentSave;
    this.aiIsBlack = source.aiIsBlack;
    this.aiDifficulty = source.aiDifficulty;
    this.currentPlayer = source.currentPlayer;
    this.gameOver = source.gameOver;
    CheckGridContents(source.positions);
    this.bPieces = new List<GameObject>();
    this.wPieces= new List<GameObject>();
    // Array.Fill(this.positions,null);
    for(int x=0;x<8;x++){
        for(int y=0;y<8;y++){
            switch (source.positions[x,y])
            {
                case 0:
                this.positions[x,y]=null;
                break;
                case 1:
                this.wPieces.Add(Create("WPawn",x,y));
                // Debug.Log("Place Piece");
                break;
                case 2:
                this.wPieces.Add(Create("WRook",x,y));
                break;
                case 3:
                this.wPieces.Add(Create("WKnight",x,y));
                break;
                case 4:
                this.wPieces.Add(Create("WBishop",x,y));
                break;
                case 5:
                this.wPieces.Add(Create("WQueen",x,y));
                break;
                case 6:
                this.wPieces.Add(Create("WKing",x,y));
                break;

                case -1:
                this.bPieces.Add(Create("BPawn",x,y));
                // Debug.Log("Place Piece");
                break;
                case -2:
                this.bPieces.Add(Create("BRook",x,y));
                break;
                case -3:
                this.bPieces.Add(Create("BKnight",x,y));
                break;
                case -4:
                this.bPieces.Add(Create("BBishop",x,y));
                break;
                case -5:
                this.bPieces.Add(Create("BQueen",x,y));
                break;
                case -6:
                this.bPieces.Add(Create("BKing",x,y));
                break;
                default:
                break;
            }
        }
    }
    
    Debug.Log("Black: " +bPieces.Count.ToString());
    Debug.Log("White: " +wPieces.Count.ToString());
    for(int i=0;i<this.bPieces.Count;i++){
        if(bPieces[i]==null){
            Debug.Log("Impossible");
        }
        SetPosition(bPieces[i]);
        Debug.Log($"Place {i}");
    }
    for(int i=0;i<this.wPieces.Count;i++){
        SetPosition(wPieces[i]);
    }
}



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
