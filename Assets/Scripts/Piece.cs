using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Piece : MonoBehaviour
{
    public PieceType PieceType;
    public IntVector2 Targetposition;
    public IntVector2 CurrentPosition;
    public PieceSlot slot;
    public GameObject DestroyVFX;
    public AudioClip DestroySound;
    public bool IscorrectPosition;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    //private void OnMouseDown()
    //{
    //    Debug.LogError("trigger");
    //    slot.Piece = null;
    //    Destroy(gameObject );
    //}
 public     void OnMouseD()
    {
      
        if (!PieceBoard.instance.selectPieces.Contains (this ))
        {
            Debug.LogError("trigger1");
            PieceBoard.instance.selectPieces.Add(this);
        }
       
       // slot.Piece = null;
       // Destroy(gameObject);
    }


    public void Endrag()
    {
        PieceBoard.instance.CheckPiece();
    }
    public void PointExit()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
       
        for (int i = 0; i < PieceBoard .instance .topBoardHeight ; i++)
        {

        }
    }
    private void OnDisable()
    {



    }
}
[Serializable ]
public struct IntVector2
{
   public  int x;
   public  int z;
}
public enum PieceType
{
CUBE,
PIPE,
CYLINDER,
CONE,
TORUS,
None,
OBSTACLE,
SPHERE,
SPHERELOW
}
