using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Traps : Props
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag != "Player")
            return;
        Character character = other.GetComponent<Character>();

        if (character .CurrentHealth > 0)
        {

            character.TakeDamage();
        }

      //  PieceBoard.instance.pieceSlots[position .x,position .z ].Piece =null ;

        Destroy(gameObject);
    }
}
