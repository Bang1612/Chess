using Unity.Networking.Transport.Utilities;
using UnityEngine;

public class MovePlate : MonoBehaviour
{
    public GameManager gm;
    GameObject reference =null;
    int matrixX;
    int matrixY;
    public bool isAttack =false;

    public void Start()
    {
        if(isAttack){
            gameObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    public void OnMouseUp()
    {
        gm.ApplyMove(new Move(reference.GetComponent<ChessPiece>().Getx(), reference.GetComponent<ChessPiece>().Gety(),
        matrixX,matrixY, isAttack));
    //     if(isAttack){
    //         GameObject square = gm.GetPosition(matrixX,matrixY);
          
    //         if(square.name == "WKing" ){
    //             gm.Winner("Black");
    //         }
    //         if(square.name == "BKing" ){
    //             gm.Winner("White");
    //         }
    //         if (gm.bPieces.Contains(square)) gm.bPieces.Remove(square);
    // else if (gm.wPieces.Contains(square)) gm.wPieces.Remove(square);
    //         square.SetActive(false);
    //     }
        // gm.SetPositionEmpty(reference.GetComponent<ChessPiece>().Getx(), reference.GetComponent<ChessPiece>().Gety());
        // reference.GetComponent<ChessPiece>().Goto(matrixX,matrixY);
        // gm.SetPosition(reference);
        // gm.Nexturn(); 
        reference.GetComponent<ChessPiece>().DestroyMovePlate();
    }
    public void SetCoord(int x, int y){
        matrixX = x;
        matrixY = y;
    }
    public void SetReference(GameObject obj){
        reference=obj;
    }
    public GameObject GetReference(){
        return reference;
    }
}
