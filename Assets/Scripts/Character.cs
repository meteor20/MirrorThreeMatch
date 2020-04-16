using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Character : MonoBehaviour,IObstacle  
{
    public bool isCorrect;
    public int alliance;
    public IntVector2 position;
    public NavMeshAgent agent;

    public List<IntVector2> movePositions = new List<IntVector2>();
    public UnityAction unityAction;

    // Start is called before the first frame update
    void Start()
    {
        isCorrect = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (movePositions.Count >0)
        {
            transform.position = Vector3.Lerp(transform .position ,PieceBoard.instance .pieceSlots[movePositions[0].x ,movePositions [0].z ].transform .position ,0.1f);
            if ((transform.position - PieceBoard.instance.pieceSlots[movePositions[0].x, movePositions[0].z].transform.position).magnitude < 0.1f)
            {
                movePositions.RemoveAt(0);
            }
        }
        else
        {
            if (PieceBoard.instance.womma .movePositions.Count == 0 && PieceBoard.instance.male .movePositions .Count ==0)
            {
              //  PieceBoard.instance.PlayState = PlayState.FillPeiceState;
            }
           
        }
    }


    public void SetDestination(Transform transform)
    {
        agent.SetDestination(transform .position );
    }
}
