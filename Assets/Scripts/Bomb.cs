using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : Obstacle 
{

    public override void Start()
    {
        base.Start();

    }
    public override void Death()
    {
        SmallDeath();
    }
    private void BigDeath()
    {
        IntVector2 intVector2 = GetComponent<Props>().position;
        GetComponent<Piece>().slot.GetComponentInChildren<Renderer>().material = PieceBoard.instance.cannotFillMaterial;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                Piece piece = PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece;

                if (piece != null && piece.GetComponent<Obstacle>() != null)
                {
                    piece.GetComponent<Obstacle>().TakeDamage(5);
                }
                else if (piece != null && piece.GetComponent<Props>() == null && piece.GetComponent<Character>() == null)
                {
                    piece.slot.canFillElement = false;
                    piece.slot.GetComponentInChildren<Renderer>().material = PieceBoard.instance.cannotFillMaterial;
                    if (PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece.DestroyVFX)
                        Destroy(Instantiate(PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece.DestroyVFX
                            , PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece.transform.position, Quaternion.identity), 1f);
                    PieceBoard.instance.audioSource.PlayOneShot(PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece.DestroySound);
                    Destroy(PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece.gameObject);

                    PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece = null;
                }
            }
        }
        GetComponentInChildren<Renderer>().enabled = false;
    }

    public void SmallDeath()
    {
        IntVector2 intVector2 = GetComponent<Props>().position;
        GetComponent<Piece>().slot.canFillElement =false ;
        GetComponent<Piece>().slot.GetComponentInChildren<Renderer>().material = PieceBoard.instance.cannotFillMaterial;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                if (Mathf.Abs(i) == Mathf.Abs(j))
                    continue;

                Piece piece = PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece;

                if (piece != null && piece.GetComponent<Obstacle>() != null)
                {
                    piece.GetComponent<Obstacle>().TakeDamage(5);
                }
                else if (piece != null && piece.GetComponent<Props>() == null && piece.GetComponent<Character>() == null)
                {
                    piece.slot.canFillElement = false;
                    piece.slot.GetComponentInChildren<Renderer>().material = PieceBoard.instance.cannotFillMaterial;
                    if (PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece.DestroyVFX)
                        Destroy(Instantiate(PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece.DestroyVFX
                            , PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece.transform.position, Quaternion.identity), 1f);
                    PieceBoard.instance.audioSource.PlayOneShot(PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece.DestroySound);
                    Destroy(PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece.gameObject);

                    PieceBoard.instance.pieceSlots[intVector2.x + i, intVector2.z + j].Piece = null;
                }
            }
        }
        GetComponentInChildren<Renderer>().enabled = false;
    }
}
