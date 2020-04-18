using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal1 : Props ,IImmoveable 
{
    public int Aliance = 1;

    int CheckAliance(int index  )
    {
        if (PieceBoard.instance.portals[index].Aliance == Aliance )
        {
            return index ;
        }
        else
        {
            int indexNext = index +1;
            if (indexNext > PieceBoard.instance.portals.Count * 3)
                return -1;

          return   CheckAliance(indexNext % PieceBoard.instance.portals.Count);
        }
      
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;
        PieceBoard.instance.audioSource.PlayOneShot(GetComponent <Piece >().DestroySound );
        Character character = other.GetComponent<Character >();

        PieceBoard.instance.portals.Remove(this );
        int index = CheckAliance(Random.Range(0, PieceBoard.instance.portals.Count)) ;

        if (index == -1)
            return;

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
   PieceBoard.instance .     PlayState = PlayState.FillPeiceState;
        if (index==0)
        {
            PieceBoard.instance.male.isCorrect = true;
        }
        if (index == 1)
        {
            PieceBoard.instance.womma .isCorrect = true;
        }
        Destroy(PieceBoard.instance.portals[index].gameObject, 0.8f);

        PieceBoard.instance.portals.Remove(PieceBoard.instance.portals[index]);
        Destroy(gameObject );
        Debug.LogWarning("eeeeeeeeee");
    }
}
