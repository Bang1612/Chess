
using UnityEngine;

public class ChessPiece : MonoBehaviour
{
    public GameManager gm;
    public GameObject movePlate;
    public int xOnBoard =-1;
    public int yOnBoard =-1;

    public string Player;
    public Sprite BlackKing, BlackQueen, BlackRook, BlackBishop, BlackKnight, BlackPawn;
    public Sprite WhiteKing, WhiteQueen, WhiteRook, WhiteBishop, WhiteKnight, WhitePawn;

    void Start()
    {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void Activated(){
        
        SetCordinate();
        switch(this.name){
            case "BQueen":
            this.GetComponent<SpriteRenderer>().sprite = BlackQueen;
            Player="Black";
            break;
            case "BKing":
            this.GetComponent<SpriteRenderer>().sprite = BlackKing;
            Player="Black";
            break;
            case "BRook":
            this.GetComponent<SpriteRenderer>().sprite = BlackRook;
            Player="Black";
            break;
            case "BBishop":
            this.GetComponent<SpriteRenderer>().sprite = BlackBishop;
            Player="Black";
            break;
            case "BKnight":
            this.GetComponent<SpriteRenderer>().sprite = BlackKnight;
            Player="Black";
            break;
            case "BPawn":
            this.GetComponent<SpriteRenderer>().sprite = BlackPawn;
            Player="Black";
            break;

            case "WQueen":
            this.GetComponent<SpriteRenderer>().sprite = WhiteQueen;
            Player="White";
            break;
            case "WKing":
            this.GetComponent<SpriteRenderer>().sprite = WhiteKing;
            Player="White";
            break;
            case "WRook":
            this.GetComponent<SpriteRenderer>().sprite = WhiteRook;
            Player="White";
            break;
            case "WBishop":
            this.GetComponent<SpriteRenderer>().sprite = WhiteBishop;
            Player="White";
            break;
            case "WKnight":
            this.GetComponent<SpriteRenderer>().sprite = WhiteKnight;
            Player="White";
            break;
            case "WPawn":
            this.GetComponent<SpriteRenderer>().sprite = WhitePawn;
            Player="White";
            break;            
        }
    }
    public void SetCordinate(){
        // float xPosition = xOnBoard;
        // float yPosition = yOnBoard;
        // xPosition *=0.87f;
        // yPosition *=0.87f;
        // xPosition += -3.17f;
        // yPosition += -3.17f;
        // this.transform.position = new Vector3(xPosition,yPosition,-1.0f);
        int cor = xOnBoard+ yOnBoard*8;
        GameObject obj = GameObject.Find("WP"+cor.ToString());
        this.transform.position = obj.transform.position;
        // gm.positions[xOnBoard,yOnBoard] = gameObject;
    }

    public int Getx(){
        return xOnBoard;
    }
    public int Gety(){
        return yOnBoard;
    } 
    public void Goto(int? x= null, int? y=null){
        if(x!=null){
            xOnBoard =x.Value;
        }
        if(y!=null){
            yOnBoard = y.Value;
        }
        SetCordinate();
    }
    public void OnMouseUp()
    {
        if(!gm.IsGameOver() && gm.GetCurrentPlayer() == Player){
            Debug.Log("Start spamming MovePlates");
            DestroyMovePlate();
            Debug.Log("MovePlaste destroyed");
            InitialMovePlate();
        }
        else{
            Debug.Log("Not turn Player");
        }
    }
    public void InitialMovePlate(){
        switch(this.name){
            case "BQueen":
            case "WQueen":
                LineMovePlate(1,0);
                LineMovePlate(0,1);
                LineMovePlate(1,1);
                LineMovePlate(-1,0);
                LineMovePlate(0,-1);
                LineMovePlate(-1,-1);
                LineMovePlate(1,-1);
                LineMovePlate(-1,1);
                break;
            case "BKing":
            case "WKing":
                Move1Plate(0,1);
                Move1Plate(1,0);
                Move1Plate(1,1);
                Move1Plate(0,-1);
                Move1Plate(-1,0);
                Move1Plate(-1,-1);
                Move1Plate(-1,1);
                Move1Plate(1,-1);
                break;
            case "BRook":
            case "WRook":
                LineMovePlate(0,1);
                LineMovePlate(1,0);
                LineMovePlate(0,-1);
                LineMovePlate(-1,0);
                break;
            case "BBishop":
            case "WBishop":
                LineMovePlate(1,1);
                LineMovePlate(1,-1);
                LineMovePlate(-1,1);
                LineMovePlate(-1,-1);
                break;
            case "BKnight":
            case "WKnight":
                LMovePlate();
                break;
            case "BPawn":
                PawnMovePlate(xOnBoard,yOnBoard-1);
                break;
            case "WPawn":
                PawnMovePlate(xOnBoard,yOnBoard+1);
                break;

            
        }
    }
    public void PointMovePlate(int x, int y){
        if(gm.Available(x,y)){
            GameObject piece =gm.GetPosition(x,y);
            if(!piece){
                MovePlateSpawn(x,y); 
            }
            else if(piece.GetComponent<ChessPiece>().Player != Player){
                MovePlateAttackSpawn(x,y);
            }
        }
    }
    public void MovePlateSpawn(int x, int y){
        int cor = x +y*8;
        GameObject Des = GameObject.Find("WP"+cor.ToString());
        GameObject spawnMovePlate = Instantiate(movePlate, 
            new Vector3(Des.transform.position.x,Des.transform.position.y,-3),
            Quaternion.identity);
        MovePlate mpScript = spawnMovePlate.GetComponent<MovePlate>();
        mpScript.SetReference(gameObject);
        mpScript.SetCoord(x,y);
    }
    public void MovePlateAttackSpawn(int x, int y){
        int cor = x +y*8;
        GameObject Des = GameObject.Find("WP"+cor.ToString());
        GameObject spawnMovePlate = Instantiate(movePlate, 
            new Vector3(Des.transform.position.x,Des.transform.position.y,-3),
            Quaternion.identity);
            spawnMovePlate.GetComponent<SpriteRenderer>().color = Color.red;
        MovePlate mpScript = spawnMovePlate.GetComponent<MovePlate>();
        mpScript.isAttack= true;
        mpScript.SetReference(gameObject);
        mpScript.SetCoord(x,y);
    }
    public void LineMovePlate(int xIncreasement, int yIncreasement){
        int x =xOnBoard + xIncreasement;
        int y =yOnBoard + yIncreasement;
        while(gm.Available(x,y) && gm.GetPosition(x,y) ==null){
            MovePlateSpawn(x,y);
            x += xIncreasement;
            y+=yIncreasement;
        }
        if(gm.Available(x,y) && gm.GetPosition(x,y).GetComponent<ChessPiece>().Player != Player ){
            MovePlateAttackSpawn(x,y);
        }
    }
    public void LMovePlate(){
        PointMovePlate(xOnBoard+1,yOnBoard+2);
        PointMovePlate(xOnBoard-1,yOnBoard+2);
        PointMovePlate(xOnBoard+1,yOnBoard-2);
        PointMovePlate(xOnBoard-1,yOnBoard-2);
        PointMovePlate(xOnBoard+2,yOnBoard+1);
        PointMovePlate(xOnBoard-2,yOnBoard+1);
        PointMovePlate(xOnBoard+2,yOnBoard-1);
        PointMovePlate(xOnBoard-2,yOnBoard-1);
    }
    public void Move1Plate(int x, int y){
        PointMovePlate(xOnBoard,yOnBoard+1);
        PointMovePlate(xOnBoard+1,yOnBoard);
        PointMovePlate(xOnBoard+1,yOnBoard+1);

        PointMovePlate(xOnBoard,yOnBoard-1);
        PointMovePlate(xOnBoard-1,yOnBoard);
        PointMovePlate(xOnBoard-1,yOnBoard-1);

        PointMovePlate(xOnBoard-1,yOnBoard+1);
        PointMovePlate(xOnBoard+1,yOnBoard-1);
    }
    public void PawnMovePlate(int x, int y){
        if(gm.Available(x,y)){
            if(gm.GetPosition(x,y) == null){
                MovePlateSpawn(x,y);
            }
            if(gm.Available(x+1,y) && gm.GetPosition(x+1,y) != null 
            && gm.GetPosition(x+1,y).GetComponent<ChessPiece>().Player != Player){
                MovePlateAttackSpawn(x+1,y);
            }
            if(gm.Available(x-1,y) && gm.GetPosition(x-1,y) != null 
            && gm.GetPosition(x-1,y).GetComponent<ChessPiece>().Player != Player){
                MovePlateAttackSpawn(x-1,y);
            }
        }
    }
    public void DestroyMovePlate(){
        GameObject[] movePlates = GameObject.FindGameObjectsWithTag("MovePlate");
        for(int i=0;i<movePlates.Length;i++){
            Destroy(movePlates[i]);
        }
    }
    

}
