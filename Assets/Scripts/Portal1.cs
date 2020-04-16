using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal1 : MonoBehaviour,IObstacle 
{
 public    IntVector2 position;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;
        Character character = other.GetComponent<Character >();

        PieceBoard.instance.portals.Remove(this );
        int index = Random.Range(0, PieceBoard.instance.portals.Count );
        PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position.z].Piece.GetComponent<Collider>().enabled = false;

        PieceBoard.instance .pieceSlots[PieceBoard.instance.portals [index].position .x, PieceBoard.instance.portals[index ].position .z].Piece = PieceBoard.instance.pieceSlots[character .position.x, character .position.z].Piece;
        PieceBoard.instance.pieceSlots[character.position.x, character.position.z].Piece = null;

        PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position.z].Piece.Targetposition = PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position.z].position;
        PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position .z ].Piece .slot = PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position.z];
        PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position.z].Piece.GetComponent<Character>().position = PieceBoard.instance.portals[index].position;

        //设置位置，动画
        PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position.z].Piece.transform.position = PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position.z].transform.position;
        PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position.z].Piece.GetComponent<Animator>().SetBool("run",false );
        if (PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position.z].Piece.GetComponent<Character>().alliance == 1)
            PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position.z].Piece.transform.LookAt(PieceBoard .instance .male .transform .position  );

        if (PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position.z].Piece.GetComponent<Character>().alliance == 0)
            PieceBoard.instance.pieceSlots[PieceBoard.instance.portals[index].position.x, PieceBoard.instance.portals[index].position.z].Piece.transform.LookAt(PieceBoard.instance.womma .transform.position);
        //PlayState = PlayState.CharacterMove;
        for (int i = 0; i < PieceBoard.instance.portals.Count  ; i++)
        {
            Destroy(PieceBoard.instance.portals[i].gameObject ,0.3f);
        }
        Destroy(gameObject );
        Debug.LogWarning("eeeeeeeeee");
    }
}
