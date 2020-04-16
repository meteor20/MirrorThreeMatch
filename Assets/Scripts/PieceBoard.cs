using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PlayState
{
    None,
    CharacterMove,
    FillPeiceState,
    GameOver,
    ClearExcessPieceState,
    CanMovePieceState,
}

public class PieceBoard : MonoBehaviour
{
    [Header("General")]
    public GameObject BoardCenterBarrier;
    public Material maleWorldMaterial;
    public Material wommaWorldMaterial;
    public Material notMoveMaterial;
    public Material canMoveMaterial;
    public GameObject pieceBound;
    public float speed = 10;
    public List<Portal1> portals = new List<Portal1>();
    public Obstacle[] obstacles;
    public PlayState PlayState = PlayState.FillPeiceState;
    public Character male;
    public Character womma;

    [Header("Init")]
    public AudioClip SweepSound;
    public AudioSource audioSource;
    [Header("Sound")]

    public int topBoardHeight = 5;
    public int topBoardWith = 6;
    public int bottomBoardHeight = 5;
    public int bottomBoardWith = 6;

    public PieceSlot[] pieceSlotss;
    public PieceSlot[,] pieceSlots;
    public Piece[] pieces;
    public PieceSlot emptyTopPiece;
    public PieceSlot emptyBottomPiece;
    public static PieceBoard instance;

    public List<Piece> selectPieces = new List<Piece>();

    bool canMove;

    private PieceSlot[] tempPieces = new PieceSlot[2];
    // Start is called before the first frame update
    void Start()
    {
      
        pieceBound = Instantiate(pieceBound, Vector3.up * 10, Quaternion.identity);
        Application.targetFrameRate = 30;
        // InitBoard();
        InitBoardAutoed();
        pieceSlots[male.position.x, male.position.z].Piece = male.GetComponent<Piece>();
        pieceSlots[male.position.x, male.position.z].Piece.slot = pieceSlots[male.position.x, male.position.z];
        pieceSlots[male.position.x, male.position.z].Piece.Targetposition = pieceSlots[male.position.x, male.position.z].position;
        pieceSlots[male.position.x, male.position.z].Piece.transform.position = pieceSlots[male.position.x, male.position.z].transform.position;
        pieceSlots[womma.position.x, womma.position.z].Piece = womma.GetComponent<Piece>();
        pieceSlots[womma.position.x, womma.position.z].Piece.slot = pieceSlots[male.position.x, womma.position.z];
        pieceSlots[womma.position.x, womma.position.z].Piece.Targetposition = pieceSlots[womma.position.x, womma.position.z].position;
        pieceSlots[womma.position.x, womma.position.z].Piece.transform.position = pieceSlots[womma.position.x, womma.position.z].transform.position;

        GameObject obstaclesParent = GameObject.Find("Obstacles");
        obstacles = obstaclesParent.GetComponentsInChildren<Obstacle >();
        for (int i = 0; i < obstacles.Length; i++)
        {
            pieceSlots[obstacles[i].position.x, obstacles[i].position.z].Piece = obstacles[i].GetComponent<Piece>();
            pieceSlots[obstacles[i].position.x, obstacles[i].position.z].Piece.slot = pieceSlots[obstacles[i].position.x, obstacles[i].position.z];
            pieceSlots[obstacles[i].position.x, obstacles[i].position.z].Piece.Targetposition = pieceSlots[obstacles[i].position.x, obstacles[i].position.z].position;
            pieceSlots[obstacles[i].position.x, obstacles[i].position.z].Piece.transform.position = pieceSlots[obstacles[i].position.x, obstacles[i].position.z].transform.position;

        }

        for (int i = 0; i < portals.Count; i++)
        {
            pieceSlots[portals[i].position.x, portals[i].position.z].Piece = portals[i].GetComponent<Piece>();
            pieceSlots[portals[i].position.x, portals[i].position.z].Piece.slot = pieceSlots[portals[i].position.x, portals[i].position.z];
            pieceSlots[portals[i].position.x, portals[i].position.z].Piece.Targetposition = pieceSlots[portals[i].position.x, portals[i].position.z].position;
            pieceSlots[portals[i].position.x, portals[i].position.z].Piece.transform.position = pieceSlots[portals[i].position.x, portals[i].position.z].transform.position;

        }

        FillInitPieces();

        instance = this;
        PlayState = PlayState.FillPeiceState;
        m_SinceExchangeTime = Time.time;
        canDestroy = true;
    }


    public void SetPlayState(PlayState State)
    {
        PlayState = State;
    }
    // Update is called once per frame
    void Update()
    {
        if (BoardCenterBarrier == null)
        {
            BoardCenterBarrier = GameObject.Find("frontier");
        }
        Renderer renderer = BoardCenterBarrier.GetComponentInChildren<Renderer>();
        if (PlayState == PlayState.CanMovePieceState)
        {

            renderer.material = canMoveMaterial;
        }
        else
        {
            renderer.material = notMoveMaterial;
        }

        Debug.LogWarning(PlayState + "-" + male.isCorrect + "-" + womma.isCorrect);
        if (Time.time - m_SinceMoveDelayStart > MoveDelayTime)
        {
            UpdatePiecePosition();
        }

        if (PlayState == PlayState.CharacterMove)
        {
            UpdateCharacterPosition();
        }
        else
        {

        }


        RaycastHit hit;
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
        {
            Piece piece = hit.collider.gameObject.GetComponent<Piece>();
            if (piece != null)
            {

                if (Input.GetMouseButton(0))
                {
                    pieceBound.SetActive(true);
                    pieceBound.transform.position = piece.transform.position;
                    if (piece.GetComponent<IObstacle>() == null && !PieceBoard.instance.selectPieces.Contains(piece) && selectPieces.Count < 2)
                    {

                        PieceBoard.instance.selectPieces.Add(piece);
                    }
                }
            }
        }
        if (Input.GetMouseButtonUp(0))
        {

            if (selectPieces.Count > 1 && PlayState == PlayState.CanMovePieceState &&
                (
              (Mathf.Abs(selectPieces[0].Targetposition.x - selectPieces[1].Targetposition.x) <= 1 &&
                 Mathf.Abs(selectPieces[0].Targetposition.z - selectPieces[1].Targetposition.z) < 1)||
                   (Mathf.Abs(selectPieces[0].Targetposition.x - selectPieces[1].Targetposition.x) < 1 &&
                 Mathf.Abs(selectPieces[0].Targetposition.z - selectPieces[1].Targetposition.z) <= 1)
                ))
            {

                pieceBound.SetActive(false);

                audioSource.PlayOneShot(SweepSound);
                ExchangePiece();
                //UpdatePiece();
                //  CheckPiece();
            }
            else
            {
            
                selectPieces.Clear();
            }
        }
        if (Time.time - m_SinceExchangeTime > 1 && PlayState == PlayState.ClearExcessPieceState)//&& canDestroy
        {
            canDestroy = false;
            if (UpdatePiece())
            {
                SetPlayState(PlayState.CharacterMove);
            }
            else
            {
                if (tempPieces[0] != null && tempPieces[1] != null)
                {
                    Piece piece = tempPieces[0].Piece;
                    // Destroy(piece .gameObject );
                    IntVector2 inte = tempPieces[0].Piece.Targetposition;
                    IntVector2 inte2 = tempPieces[1].Piece.Targetposition;
                    //pieceSlots[tempPieces[0].position .x, tempPieces[0].position .z].Piece = pieceSlots[tempPieces[1].position .x, tempPieces[1].position .z].Piece;
                    //pieceSlots[tempPieces[0].position.x, tempPieces[0].position.z].Piece.Targetposition = inte2;
                    //pieceSlots[tempPieces[0].position.x, tempPieces[0].position.z].Piece.slot = tempPieces[1];

                    tempPieces[0].Piece = tempPieces[1].Piece;
                    tempPieces[0].Piece.Targetposition = inte;
                    tempPieces[0].Piece.slot = tempPieces[0];
                    tempPieces[1].Piece = piece;
                    tempPieces[1].Piece.Targetposition = inte2;
                    tempPieces[1].Piece.slot = tempPieces[1];
                    //pieceSlots[tempPieces[1].position.x, tempPieces[1].position.z].Piece = piece;
                    //pieceSlots[tempPieces[1].position.x, tempPieces[1].position.z].Piece.Targetposition = piece.Targetposition ;
                    //pieceSlots[tempPieces[1].position.x, tempPieces[1].position.z].Piece.slot = piece.slot ;
                    Debug.LogError(">>>>>>>>>>>>>>>>>>");
                    tempPieces = new PieceSlot[2];
                }

                if (!UpdateBoard())
                    SetPlayState(PlayState.CanMovePieceState);
                else
                {
                    SetPlayState(PlayState.FillPeiceState);
                }
            }


        }
        if (PlayState == PlayState.FillPeiceState)
        {
            UpdateBoard();
        }
        else
        {
            //male.SetDestination(womma .transform );
            //womma .SetDestination(male .transform);
            //if (male.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathInvalid||
            //    male.agent.pathStatus == UnityEngine.AI.NavMeshPathStatus.PathPartial )
            //{
            //    canMove = false;
            //}
        }

    }


    void FillInitPieces()
    {
        for (int i = 0; i < topBoardWith; i++)
        {
            for (int j = 1; j <= topBoardHeight + bottomBoardHeight; j++)
            {
                int index = Random.Range(0, pieces.Length);
                if (pieceSlots[i, j].Piece == null)
                {

                    pieceSlots[i, j].Piece = Instantiate(get(i, j, index));
                    pieceSlots[i, j].Piece.Targetposition = pieceSlots[i, j].position;
                    pieceSlots[i, j].Piece.slot = pieceSlots[i, j];
                    pieceSlots[i, j].Piece.transform.position = pieceSlots[i, j].transform.position;
                }

            }
        }
    }

    public Piece get(int i, int j, int index)
    {
        int count = 1;

        Piece piecePrefab = pieces[index];

        Piece piece = GetNeighbourPiece(-1, 0, pieceSlots[i, j]);
        if (piece != null)
        {
            if (piece.PieceType == piecePrefab.PieceType)
                count++;
        }
        piece = GetNeighbourPiece(-2, 0, pieceSlots[i, j]);
        if (piece != null)
        {
            if (piece.PieceType == piecePrefab.PieceType)
                count++;
        }

        if (count >= 3)
        {
            index = (index + 1) % pieces.Length;
            if (index / pieces.Length > 2)
            {
                Debug.LogError("DOUB");
                return null;
            }
            Debug.LogError("Change1");
            piecePrefab = get(i, j, index);
        }
        else
        {
            count = 1;
            piece = GetNeighbourPiece(0, -1, pieceSlots[i, j]);
            if (piece != null)
            {
                if (piece.PieceType == piecePrefab.PieceType)
                    count++;
            }
            piece = GetNeighbourPiece(0, -2, pieceSlots[i, j]);
            if (piece != null)
            {
                if (piece.PieceType == piecePrefab.PieceType)
                    count++;
            }
            if (count >= 3)
            {
                index = (index + 1) % pieces.Length;
                if (index / pieces.Length > 2)
                {
                    Debug.LogError("DOUB");
                    return null;
                }

                Debug.LogError("Change2");
                piecePrefab = get(i, j, index);
            }


        }
        return piecePrefab;


    }
    public PieceSlot GetPieceSlot(int x, int y)
    {
        PieceSlot pieceSlot = null;
        if (y > 0)
        {
            return pieceSlots[x, y + bottomBoardHeight];
        }
        if (y < 0)
        {
            return pieceSlots[x, y + bottomBoardHeight + 1];
        }
        return pieceSlot;
    }

    public void InitBoardAutoed()
    {
        pieceSlots = new PieceSlot[topBoardWith, topBoardHeight + bottomBoardHeight + 2];

        for (int i = 0; i < topBoardWith; i++)
        {
            for (int j = 1; j <= topBoardHeight + 1; j++)
            {
                PieceSlot slot = Instantiate(emptyTopPiece);
                slot.GetComponentInChildren<Renderer>().material = maleWorldMaterial;
                Debug.Log(i + "-" + (bottomBoardHeight + j).ToString());
                pieceSlots[i, bottomBoardHeight + j] = slot;
                pieceSlots[i, bottomBoardHeight + j].position.x = i; //(int)pieceSlots[i, bottomBoardHeight + j].transform.position.x;
                pieceSlots[i, bottomBoardHeight + j].position.z = bottomBoardHeight + j;// (int)pieceSlots[i, bottomBoardHeight + j].transform.position.z;
                //pieceSlots[i, bottomBoardHeight + j] = slot;
                slot.transform.position = new Vector3(-((float)topBoardWith) / 2 + 0.5f, 0, bottomBoardHeight) + new Vector3(i, 0, j);
                //if (pieceSlots [i,i ].Piece ==null )
                //{
                // pieces[Random.Range(0, pieces.Length)].gameObiect
                //}
            }

        }
        for (int i = 0; i < bottomBoardWith; i++)
        {
            for (int j = 0; j <= bottomBoardHeight + 1; j++)
            {
                PieceSlot slot = Instantiate(emptyTopPiece);
                slot.GetComponentInChildren<Renderer>().material = wommaWorldMaterial;
                pieceSlots[i, j] = slot;
                pieceSlots[i, j].position.x = i;//(int)pieceSlots[i, j].transform.position.x;
                pieceSlots[i, j].position.z = j;//(int)pieceSlots[i, j].transform.position.z;
                slot.transform.position = new Vector3(-((float)topBoardWith) / 2 + 0.5f, 0, 0) + new Vector3(i, 0, j);
            }

        }
        //for (int i = 0; i < bottomBoardWith; i++)
        //{
        //    for (int j = 0; j < bottomBoardHeight; j++)
        //    {
        //        PieceSlot slot = Instantiate(emptyBottomPiece );
        //        pieceSlots[i, j] = slot;
        //        slot.transform.position = new Vector3 (0.5f,0,-0.5f)+ new Vector3(i, 0, -j);
        //        //if (pieceSlots [i,i ].Piece ==null )
        //        //{
        //        // pieces[Random.Range(0, pieces.Length)].gameObiect
        //        //}
        //    }

        //}
    }

    public void InitBoard()
    {
        pieceSlots = new PieceSlot[topBoardWith, topBoardHeight + bottomBoardHeight + 2];

        for (int i = 0; i < topBoardWith; i++)
        {
            for (int j = 1; j <= topBoardHeight + 1; j++)
            {
                //   PieceSlot slot = Instantiate(emptyTopPiece);
                Debug.Log(i + "-" + (bottomBoardHeight + j).ToString());
                pieceSlots[i, bottomBoardHeight + j] = pieceSlotss[(bottomBoardHeight * (i + 1)) + topBoardWith * i + j];
                pieceSlots[i, bottomBoardHeight + j].position.x = i; //(int)pieceSlots[i, bottomBoardHeight + j].transform.position.x;
                pieceSlots[i, bottomBoardHeight + j].position.z = bottomBoardHeight + j;// (int)pieceSlots[i, bottomBoardHeight + j].transform.position.z;
                //pieceSlots[i, bottomBoardHeight + j] = slot;
                // slot.transform.position = new Vector3(0.5f, 0, 0.5f) + new Vector3(i, 0, j);
                //if (pieceSlots [i,i ].Piece ==null )
                //{
                // pieces[Random.Range(0, pieces.Length)].gameObiect
                //}
            }

        }
        for (int i = 0; i < bottomBoardWith; i++)
        {
            for (int j = 0; j <= bottomBoardHeight + 1; j++)
            {
                pieceSlots[i, j] = pieceSlotss[(bottomBoardHeight * (i)) + topBoardWith * i + j];
                pieceSlots[i, j].position.x = i;//(int)pieceSlots[i, j].transform.position.x;
                pieceSlots[i, j].position.z = j;//(int)pieceSlots[i, j].transform.position.z;

            }

        }
        //for (int i = 0; i < bottomBoardWith; i++)
        //{
        //    for (int j = 0; j < bottomBoardHeight; j++)
        //    {
        //        PieceSlot slot = Instantiate(emptyBottomPiece );
        //        pieceSlots[i, j] = slot;
        //        slot.transform.position = new Vector3 (0.5f,0,-0.5f)+ new Vector3(i, 0, -j);
        //        //if (pieceSlots [i,i ].Piece ==null )
        //        //{
        //        // pieces[Random.Range(0, pieces.Length)].gameObiect
        //        //}
        //    }

        //}
    }




    //fill
    public bool UpdateBoard()
    {
        bool canFill = false;

        for (int p = 0; p < topBoardWith; p++)
        {
            if (GetPieceSlot(p, topBoardHeight).Piece == null)
            {
                GetPieceSlot(p, topBoardHeight + 1).Piece = Instantiate(pieces[Random.Range(0, pieces.Length)], GetPieceSlot(p, topBoardHeight + 1).transform.position, Quaternion.identity);
                GetPieceSlot(p, topBoardHeight + 1).Piece.transform.SetParent(GetPieceSlot(p, topBoardHeight).transform);

                Debug.Log(p + "-" + (topBoardHeight - 1).ToString() + (topBoardHeight - 2).ToString() + "-" + pieceSlots.Length);
                GetPieceSlot(p, topBoardHeight + 1).Piece.Targetposition = GetPieceSlot(p, topBoardHeight).position;
                GetPieceSlot(p, topBoardHeight).Piece = GetPieceSlot(p, topBoardHeight + 1).Piece;
                GetPieceSlot(p, topBoardHeight).Piece.slot = GetPieceSlot(p, topBoardHeight);
                GetPieceSlot(p, topBoardHeight + 1).Piece = null;
                //   pieceSlots[0,0].Piece.Targetposition = pieceSlots[0,0].position;
                Debug.Log(pieceSlots[p, topBoardHeight + 1].name);
                //    Debug.Log(pieceSlots[p, topBoardHeight - 2].name);
                canFill = true;
            }
        }

        for (int p = 0; p < bottomBoardWith; p++)
        {
            if (GetPieceSlot(p, -bottomBoardHeight).Piece == null)
            {
                GetPieceSlot(p, -bottomBoardHeight - 1).Piece = Instantiate(pieces[Random.Range(0, pieces.Length)], GetPieceSlot(p, -bottomBoardHeight - 1).transform.position, Quaternion.identity);
                GetPieceSlot(p, -bottomBoardHeight - 1).Piece.transform.SetParent(GetPieceSlot(p, -bottomBoardHeight).transform);

                GetPieceSlot(p, -bottomBoardHeight - 1).Piece.Targetposition = GetPieceSlot(p, -bottomBoardHeight).position;
                GetPieceSlot(p, -bottomBoardHeight).Piece = GetPieceSlot(p, -bottomBoardHeight - 1).Piece;
                GetPieceSlot(p, -bottomBoardHeight).Piece.slot = GetPieceSlot(p, -bottomBoardHeight);
                GetPieceSlot(p, -bottomBoardHeight - 1).Piece = null;
                canFill = true;
            }
        }

        for (int i = 0; i < topBoardWith; i++)
        {
            for (int j = 1; j < topBoardHeight; j++)
            {

                if (GetPieceSlot(i, j).Piece == null && GetPieceSlot(i, j + 1).Piece != null && GetPieceSlot(i, j + 1).Piece.GetComponent<IObstacle>() == null)
                {
                    GetPieceSlot(i, j + 1).Piece.Targetposition = GetPieceSlot(i, j).position;
                    GetPieceSlot(i, j + 1).Piece.transform.SetParent(GetPieceSlot(i, j).transform);
                    GetPieceSlot(i, j).Piece = GetPieceSlot(i, j + 1).Piece;
                    GetPieceSlot(i, j).Piece.slot = GetPieceSlot(i, j);
                    GetPieceSlot(i, j + 1).Piece = null;
                    canFill = true;
                }
                else if (GetPieceSlot(i, j).Piece == null && GetPieceSlot(i, j + 1).Piece != null && GetPieceSlot(i, j + 1).Piece.GetComponent<IObstacle>() != null)
                {
                    int index = i - 1;
                    if (i == 0)
                    {
                        index = i + 1;
                    }
                    if (GetPieceSlot(index, j + 1).Piece != null && GetPieceSlot(index, j + 1).Piece.GetComponent<IObstacle>() == null)
                    {
                        GetPieceSlot(index, j + 1).Piece.Targetposition = GetPieceSlot(i, j).position;
                        GetPieceSlot(index, j + 1).Piece.transform.SetParent(GetPieceSlot(i, j).transform);
                        GetPieceSlot(i, j).Piece = GetPieceSlot(index, j + 1).Piece;
                        GetPieceSlot(i, j).Piece.slot = GetPieceSlot(i, j);
                        GetPieceSlot(index, j + 1).Piece = null;
                        canFill = true;
                    }
                }

                //PieceSlot slot = Instantiate(emptyBottomPiece);
                //pieceSlots[i, j] = slot;
                //slot.transform.position = new Vector3(0.5f, 0, -0.5f) + new Vector3(i, 0, -j);
                //if (pieceSlots [i,i ].Piece ==null )
                //{
                // pieces[Random.Range(0, pieces.Length)].gameObiect
                //}
            }
        }
        for (int i = 0; i < bottomBoardWith; i++)
        {
            for (int j = -1; j > -bottomBoardHeight; j--)
            {
                if (GetPieceSlot(i, j).Piece == null && GetPieceSlot(i, j - 1).Piece != null && GetPieceSlot(i, j - 1).Piece.GetComponent<IObstacle>() == null)
                {
                    GetPieceSlot(i, j - 1).Piece.Targetposition = GetPieceSlot(i, j).position;
                    GetPieceSlot(i, j - 1).Piece.transform.SetParent(GetPieceSlot(i, j).transform);
                    GetPieceSlot(i, j).Piece = GetPieceSlot(i, j - 1).Piece;
                    GetPieceSlot(i, j).Piece.slot = GetPieceSlot(i, j);
                    GetPieceSlot(i, j - 1).Piece = null;
                    canFill = true;
                }
                else if (GetPieceSlot(i, j).Piece == null && GetPieceSlot(i, j - 1).Piece != null && GetPieceSlot(i, j - 1).Piece.GetComponent<IObstacle>() != null)
                {
                    int index = i - 1;
                    if (i == 0)
                    {
                        index = i + 1;
                    }
                    if (GetPieceSlot(index, j - 1).Piece != null && GetPieceSlot(index, j - 1).Piece.GetComponent<IObstacle>() == null)
                    {
                        GetPieceSlot(index, j - 1).Piece.Targetposition = GetPieceSlot(i, j).position;
                        GetPieceSlot(index, j - 1).Piece.transform.SetParent(GetPieceSlot(i, j).transform);
                        GetPieceSlot(i, j).Piece = GetPieceSlot(index, j - 1).Piece;
                        GetPieceSlot(i, j).Piece.slot = GetPieceSlot(i, j);
                        GetPieceSlot(index, j - 1).Piece = null;
                        canFill = true;
                    }
                }

                //PieceSlot slot = Instantiate(emptyBottomPiece);
                //pieceSlots[i, j] = slot;
                //slot.transform.position = new Vector3(0.5f, 0, -0.5f) + new Vector3(i, 0, -j);
                //if (pieceSlots [i,i ].Piece ==null )
                //{
                // pieces[Random.Range(0, pieces.Length)].gameObiect
                //}
            }
        }





        PlayState = PlayState.ClearExcessPieceState;
        m_SinceExchangeTime = Time.time;
        //if (canDestroy)
        //{
        //    m_SinceExchangeTime = Time.time;
        //    UpdatePiece();
        //}

        //for (int i = 0; i < bottomBoardWith ; i++)
        //{
        //    for (int j = -1; j > -bottomBoardHeight ; j--)
        //    {
        //        if (GetPieceSlot(i, j).Piece == null && GetPieceSlot(i, j - 1).Piece != null)
        //        {
        //            GetPieceSlot(i, j - 1).Piece.Targetposition = GetPieceSlot(i, j).position;
        //            GetPieceSlot(i, j - 1).Piece.transform.SetParent(GetPieceSlot(i, j).transform);
        //            GetPieceSlot(i, j).Piece = GetPieceSlot(i, j - 1).Piece;
        //            GetPieceSlot(i, j - 1).Piece = null;
        //        }

        //    }
        //}
        return canFill;
    }


    public void UpdatePiecePosition()
    {
        for (int i = 0; i < topBoardWith; i++)
        {
            for (int j = 1; j <= topBoardHeight; j++)
            {
                if (GetPieceSlot(i, j).Piece != null && GetPieceSlot(i, j).Piece.GetComponent<IObstacle>() == null)
                {
                    Vector3 tposition = GetPieceSlot(i, j).transform.position;// new Vector3(GetPieceSlot(i, j).Piece.Targetposition.x, 0, GetPieceSlot(i, j).Piece.Targetposition.z);
                    if (GetPieceSlot(i, j).Piece.transform.position != tposition)
                    {
                        GetPieceSlot(i, j).Piece.transform.position = Vector3.Lerp(GetPieceSlot(i, j).Piece.transform.position, tposition, Time.deltaTime * speed);

                        if ((GetPieceSlot(i, j).Piece.transform.position - tposition).magnitude < 0.1f) { }
                        //  canMove = true ;
                        else
                        {
                            // canMove = false;
                        }
                    }
                }
                //else if (GetPieceSlot(i, j).Piece != null && GetPieceSlot(i, j).Piece.GetComponent<IObstacle>() != null&&canMove )
                //{
                //    Vector3 tposition = GetPieceSlot(i, j).transform.position;// new Vector3(GetPieceSlot(i, j).Piece.Targetposition.x, 0, GetPieceSlot(i, j).Piece.Targetposition.z);
                //    if (GetPieceSlot(i, j).Piece.transform.position != tposition)
                //    {
                //        GetPieceSlot(i, j).Piece.transform.position = Vector3.Lerp(GetPieceSlot(i, j).Piece.transform.position, tposition, 0.1f);

                //        if ((GetPieceSlot(i, j).Piece.transform.position - tposition).magnitude < 0.1f) { }
                //        //  canMove = true ;
                //        else
                //        {
                //            // canMove = false;
                //        }
                //    }
                //}

            }
        }

        for (int i = 0; i < bottomBoardWith; i++)
        {
            for (int j = -1; j >= -bottomBoardHeight; j--)
            {
                if (GetPieceSlot(i, j).Piece != null && GetPieceSlot(i, j).Piece.GetComponent<IObstacle>() == null)//&& PlayState == PlayState.FillPeiceState
                {
                    Vector3 tposition = GetPieceSlot(i, j).transform.position;//new Vector3(GetPieceSlot(i, j).Piece.Targetposition.x, 0, GetPieceSlot(i, j).Piece.Targetposition.z);
                    if (GetPieceSlot(i, j).Piece.transform.position != tposition)
                    {
                        GetPieceSlot(i, j).Piece.transform.position = Vector3.Lerp(GetPieceSlot(i, j).Piece.transform.position, tposition, Time.deltaTime * speed);
                    }

                    if ((GetPieceSlot(i, j).Piece.transform.position - tposition).magnitude < 0.1f)

                    { }//canMove = true;
                    else
                    {
                        // canMove = false;
                    }
                }
                //else if (GetPieceSlot(i, j).Piece != null && GetPieceSlot(i, j).Piece.GetComponent<IObstacle>() != null&&canMove)
                //{
                //    Vector3 tposition = GetPieceSlot(i, j).transform.position;//new Vector3(GetPieceSlot(i, j).Piece.Targetposition.x, 0, GetPieceSlot(i, j).Piece.Targetposition.z);
                //    if (GetPieceSlot(i, j).Piece.transform.position != tposition)
                //    {
                //        GetPieceSlot(i, j).Piece.transform.position = Vector3.Lerp(GetPieceSlot(i, j).Piece.transform.position, tposition, 0.1f);
                //    }

                //    if ((GetPieceSlot(i, j).Piece.transform.position - tposition).magnitude < 0.1f)

                //    { }//canMove = true;
                //    else
                //    {
                //        // canMove = false;
                //    }
                //}

            }
        }


    }
    bool moveStop;
    bool hasStop;
    public void UpdateCharacterPosition()
    {
        Vector3 tposition = pieceSlots[male.position.x, male.position.z].transform.position;// new Vector3(GetPieceSlot(i, j).Piece.Targetposition.x, 0, GetPieceSlot(i, j).Piece.Targetposition.z);

        if (pieceSlots[male.position.x, male.position.z].Piece.transform.position != tposition)
        {
            pieceSlots[male.position.x, male.position.z].Piece.transform.position = Vector3.Lerp(pieceSlots[male.position.x, male.position.z].Piece.transform.position, tposition, Time.deltaTime * speed);
            male.GetComponent<Animator>().SetBool("run", true);
            pieceSlots[male.position.x, male.position.z].Piece.transform.LookAt(tposition , Vector3.up);

            if (Vector3.Distance(pieceSlots[male.position.x, male.position.z].Piece.transform.position, tposition) < 0.01f)
            {

                male.GetComponent<Animator>().SetBool("run", false);
                pieceSlots[male.position.x, male.position.z].Piece.transform.position = tposition;
                // Debug.LogError("sssss");

                if (MoveCharacter())
                {
                    // Debug.LogError("sss22222");
                    male.isCorrect = false;
                }
                else
                {
                    if (pieceSlots[male.position.x, male.position.z - 1] == pieceSlots[womma.position.x, womma.position.z] && Time.timeScale != 0)
                    {
                        Debug.LogError("GameOver");
                        PlayState = PlayState.GameOver;
                        return;
                        //Time.timeScale = 0;
                    }
                    pieceSlots[male .position.x, male .position.z].Piece.transform.LookAt(pieceSlots[womma .position.x, womma  .position.z].Piece.transform, Vector3.up);

                    male.isCorrect = true;
                }

                //moveStop = true;
                //hasStop = true;
            }
            else
            {
                //  moveStop = false;
            }

        }



        //womma
        tposition = pieceSlots[womma.position.x, womma.position.z].transform.position;
        if (pieceSlots[womma.position.x, womma.position.z].Piece.transform.position != tposition)
        {
            pieceSlots[womma.position.x, womma.position.z].Piece.transform.position = Vector3.Lerp(pieceSlots[womma.position.x, womma.position.z].Piece.transform.position, tposition, Time.deltaTime * speed);
            pieceSlots[womma.position.x, womma.position.z].Piece.transform.LookAt(tposition, Vector3.up);
            womma.GetComponent<Animator>().SetBool("run", true);
            if (Vector3.Distance(pieceSlots[womma.position.x, womma.position.z].Piece.transform.position, tposition) < 0.01f)
            {
                womma.GetComponent<Animator>().SetBool("run", false);
                if (pieceSlots[male.position.x, male.position.z - 1] == pieceSlots[womma.position.x, womma.position.z] && Time.timeScale != 0)
                {
                    Debug.LogError("GameOver");
                    PlayState = PlayState.GameOver;
                    return;
                    //Time.timeScale = 0;
                }

                pieceSlots[womma.position.x, womma.position.z].Piece.transform.position = tposition;
                // Debug.LogError("sssss11111");
                if (MoveCharacterWomma())
                {
                    //Debug.LogError("sss3333");
                    womma.isCorrect = false;
                }
                else
                {
                    if (male.isCorrect)
                    {
                        //  Debug.LogError("sss44444");
                        PlayState = PlayState.FillPeiceState;
                        pieceSlots[womma.position.x, womma.position.z].Piece.transform.LookAt(pieceSlots[male.position.x, male.position.z].Piece.transform, Vector3.up);
                        womma.isCorrect = true;
                        canDestroy = true;
                        m_SinceExchangeTime = Time.deltaTime;
                    }
                }
                //moveStop = true;
                // hasStop = true ;
            }


        }
        else
        {
            if (male.isCorrect && Vector3.Distance(pieceSlots[male.position.x, male.position.z].Piece.transform.position, pieceSlots[male.position.x, male.position.z].transform.position) < 0.01f)
            {
                //Debug.LogError("sss44444");
                PlayState = PlayState.FillPeiceState;
                canDestroy = true;
            }
        }




        //if (moveStop )
        //{
        //    hasStop = true;
        //    // Debug.LogError("kkkkkk"+ MoveCharacter());
        //    bool h = MoveCharacter();
        //    if (h)
        //    {

        //        Debug.LogError("fhuirghoiraeghp;i");
        //        moveStop = false;
        //        hasStop = false;
        //       PlayState = PlayState.CharacterMove;
        //        // hasStop = false ;
        //    }
        //    if (!h)
        //    {

        //        Debug.LogError("LLLLLL");
        //        //  hasStop = true;
        //        PlayState = PlayState.FillPeiceState;
        //    }
        //    Debug.LogError("YYYYY");
        //}
    }



    public bool CheckPiece()
    {
        Debug.LogError("Exit");
        bool canDestroy = false;
        int count = 1;

        Piece sample = selectPieces[0];
        for (int i = 1; i < selectPieces.Count; i++)
        {
            if (sample.PieceType == selectPieces[i].PieceType)
            {
                count++;
            }
            else
            {
                selectPieces.Clear();
                return false;
            }
        }
        if (count >= 3)
        {
            canDestroy = true;
            for (int i = 0; i < selectPieces.Count; i++)
            {
                Destroy(selectPieces[i].slot.Piece.gameObject);
                selectPieces[i].slot.Piece = null;

            }
        }


        selectPieces.Clear();
        return canDestroy;
    }

    public List<Piece> removedPieces = new List<Piece>();
    private float m_SinceExchangeTime;
    private float m_SinceMoveDelayStart = 0f;
    private bool canDestroy;
    public float MoveDelayTime = 0.75f;


    //添加要删的棋子
    public bool UpdatePiece()
    {
        bool needClear = false;

        for (int i = 0; i < topBoardWith; i++)
        {
            for (int j = 1; j < topBoardHeight + bottomBoardHeight + 1; j++)
            {
                if (pieceSlots[i, j].Piece == null)
                {
                    Debug.LogError("Time.timeScale = 0;");
                    // Time.timeScale = 0;
                    // return true ;
                }
            }
        }




        //canDestroy = false;
        for (int i = 0; i < topBoardWith; i++)
        {
            for (int j = 1; j < topBoardHeight + bottomBoardHeight + 1; j++)
            {

                // int count = 1;

                PieceSlot sampleSlot = pieceSlots[i, j];
                List<Piece> pieces = new List<Piece>();
                int index = i;
                while (index > 0)
                {
                    index--;
                    if (pieceSlots[index, j].Piece == null || sampleSlot.Piece == null)
                    {
                        Debug.LogError(pieceSlots[index, j].position.x + "-" + pieceSlots[index, j].position.z);
                        Debug.LogError(sampleSlot.position.x + "-" + sampleSlot.position.z);
                    }
                    if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IObstacle>() == null && pieceSlots[index, j].Piece != null && pieceSlots[index, j].Piece.PieceType == sampleSlot.Piece.PieceType)
                    {
                        pieces.Add(pieceSlots[index, j].Piece);
                    }
                    else
                    {
                        break;
                    }
                }
                if (pieces.Count < 2)
                {
                    //count = 1;
                    pieces.Clear();
                }
                else
                {
                    for (int a = 0; a < pieces.Count; a++)
                    {
                        if (!removedPieces.Contains(pieces[a]))
                        {
                            // count++;
                            removedPieces.Add(pieces[a]);
                        }

                    }
                    if (!removedPieces.Contains(sampleSlot.Piece))
                    {
                        removedPieces.Add(sampleSlot.Piece);
                    }
                    pieces.Clear();
                }

                pieces.Clear();
                index = j;
                while (index > 1)
                {
                    index--;
                    if (pieceSlots[i, index].Piece == null || sampleSlot.Piece == null)
                    {
                        Debug.LogError(pieceSlots[i, index].position.x + "-" + pieceSlots[i, index].position.z);
                        Debug.LogError(sampleSlot.position.x + "-" + sampleSlot.position.z);
                    }
                    if (sampleSlot.Piece != null && sampleSlot.Piece.GetComponent<IObstacle>() == null && pieceSlots[i, index].Piece != null && pieceSlots[i, index].Piece.PieceType == sampleSlot.Piece.PieceType)
                    {
                        pieces.Add(pieceSlots[i, index].Piece);
                    }
                    else
                    {
                        break;
                    }
                }

                if (pieces.Count >= 2)
                {

                    for (int a = 0; a < pieces.Count; a++)
                    {
                        if (!removedPieces.Contains(pieces[a]))
                        {
                            //  count++;
                            removedPieces.Add(pieces[a]);
                        }

                    }
                    if (!removedPieces.Contains(sampleSlot.Piece))
                    {
                        removedPieces.Add(sampleSlot.Piece);
                    }
                    pieces.Clear();

                }
            }
        }

        if (removedPieces.Count >= 3)//----------------------
        {
            m_SinceExchangeTime = Time.time;
            canDestroy = true;
            CheckPiece(removedPieces);
            m_SinceMoveDelayStart = Time.time;
            tempPieces = new PieceSlot[2];
            needClear = true;
        }
        else
        {

            needClear = false;
        }

        return needClear;

    }


    Piece GetNeighbourPiece(int left, int top, PieceSlot pieceSlot)
    {
        int indexX = pieceSlot.position.x + left;
        int indexY = pieceSlot.position.z + top;
        if (indexX >= 0 && indexX < topBoardWith && indexY >= 1 && indexY <= topBoardHeight + bottomBoardHeight + 1)
        {
            return pieceSlots[indexX, indexY].Piece;
        }
        return null;
    }

    //remove
    public bool CheckPiece(List<Piece> pieces)
    {
        //  Debug.LogError("Exit");
        List<Piece> selectPieces = new List<Piece>();
        selectPieces = pieces;

        #region 检查障碍物
        for (int i = 0; i < selectPieces.Count; i++)
        {
            if (selectPieces[i].slot.Piece != null)
            {
                // pieceSlots[selectPieces[i].slot.position .x, selectPieces[i].slot.position.z]
                int indexX = selectPieces[i].slot.position.x;
                int indexY = selectPieces[i].slot.position.z;
                Piece piece = GetNeighbourPiece(-1, 0, selectPieces[i].slot);
                if (piece != null && piece.GetComponent<Obstacle>() != null&& piece.GetComponent<Obstacle>().TakeDamage ())
                {
                    Debug.LogError("DDDDDD");
                    Destroy(Instantiate(piece.DestroyVFX, piece.transform.position, Quaternion.identity), 1f);
                    audioSource.PlayOneShot(piece.DestroySound);
                    Destroy(piece.GetComponent<Obstacle>().gameObject);
                    pieceSlots[indexX - 1, indexY].Piece = null;
                }
                piece = GetNeighbourPiece(1, 0, selectPieces[i].slot);
                if (piece != null && piece.GetComponent<Obstacle>() != null && piece.GetComponent<Obstacle>().TakeDamage())
                {
                    Debug.LogError("DDDDDD");
                    Destroy(Instantiate(piece.DestroyVFX, piece.transform.position, Quaternion.identity), 1f);

                    audioSource.PlayOneShot(piece.DestroySound);
                    Destroy(piece.GetComponent<Obstacle>().gameObject);
                    pieceSlots[indexX + 1, indexY].Piece = null;
                }
                piece = GetNeighbourPiece(0, -1, selectPieces[i].slot);
                if (piece != null && piece.GetComponent<Obstacle>() != null && piece.GetComponent<Obstacle>().TakeDamage())
                {
                    Debug.LogError("DDDDDD");
                    Destroy(Instantiate(piece.DestroyVFX, piece.transform.position, Quaternion.identity), 1f);
                    audioSource.PlayOneShot(piece.DestroySound);
                    Destroy(piece.GetComponent<Obstacle>().gameObject);
                    pieceSlots[indexX, indexY - 1].Piece = null;
                }
                piece = GetNeighbourPiece(0, 1, selectPieces[i].slot);
                if (piece != null && piece.GetComponent<Obstacle>() != null && piece.GetComponent<Obstacle>().TakeDamage())
                {
                    Debug.LogError("DDDDDD");
                    Destroy(Instantiate(piece.DestroyVFX, piece.transform.position, Quaternion.identity), 1f);
                    audioSource.PlayOneShot(piece.DestroySound);
                    Destroy(piece.GetComponent<Obstacle>().gameObject);
                    pieceSlots[indexX, indexY + 1].Piece = null;
                }
                //  Debug.LogError("ddd");
            }

        }
        #endregion


        //bool canDestroy = false;
        int count = 1;
        for (int i = 0; i < selectPieces.Count; i++)
        {
            if (selectPieces[i].slot.Piece != null)
            {
                GameObject gameObject = selectPieces[i].slot.Piece.gameObject;
                if (selectPieces[i].slot.Piece.DestroyVFX != null)
                    Destroy(Instantiate(selectPieces[i].slot.Piece.DestroyVFX, selectPieces[i].slot.Piece.transform.position, Quaternion.identity), 2);
                audioSource.PlayOneShot(selectPieces[i].slot.Piece.DestroySound);
                selectPieces[i].slot.Piece = null;
                //selectPieces.RemoveAt(0);

                Destroy(gameObject);
            }

        }

        selectPieces.Clear();
        canMove = true;
        MoveCharacter();
        MoveCharacterWomma();
        return false;
    }
    public void ExchangePiece()
    {
        tempPieces[0] = selectPieces[0].slot;
        tempPieces[1] = selectPieces[1].slot;
        Piece firstPiece = selectPieces[0];
        PieceSlot firstSlot = firstPiece.slot;

        Piece secondPiece = selectPieces[1];
        PieceSlot secondSlot = secondPiece.slot;

        Piece tp = firstSlot.Piece;
        firstSlot.Piece = secondPiece;
        firstSlot.Piece.Targetposition = firstSlot.position;
        secondSlot.Piece = tp;
        secondSlot.Piece.Targetposition = secondSlot.position;
        //PieceSlot t = secondSlot;
        firstSlot.Piece.slot = firstSlot;
        secondSlot.Piece.slot = secondSlot;
        //firstSlot = t;
        selectPieces.Clear();
        m_SinceExchangeTime = Time.time;
        PlayState = PlayState.ClearExcessPieceState;
        canDestroy = true;
    }



    public bool MoveCharacter()
    {
        bool canMove1 = false;
        Piece malePiece = male.GetComponent<Piece>();
        Piece wommaPiece = womma.GetComponent<Piece>();

        int indexX = male.position.x - 1;
        PieceSlot closestPiece = pieceSlots[male.position.x, male.position.z];
        IntVector2 intVector2 = new IntVector2();
        intVector2.x = male.position.x;
        intVector2.z = male.position.z;

        float closestDistance = Vector3.Distance(pieceSlots[male.position.x, male.position.z].transform.position, womma.transform.position);
        if (indexX >= 0)
        {

            if (pieceSlots[indexX, male.position.z].Piece == null && Vector3.Distance(pieceSlots[indexX, male.position.z].transform.position, womma.transform.position) < closestDistance)
            {
                closestPiece = pieceSlots[indexX, male.position.z];
                intVector2.x = indexX;
                intVector2.z = male.position.z;
            }
            ////  //检测传送门
            //else if (pieceSlots[indexX, male.position.z].Piece.GetComponent <Portal1>() !=null  )
            //{
            //    intVector2.x = indexX;
            //    intVector2.z = male.position.z;
            //}
        }
        indexX = male.position.x + 1;
        if (indexX < topBoardWith)
        {

            if (pieceSlots[indexX, male.position.z].Piece == null && Vector3.Distance(pieceSlots[indexX, male.position.z].transform.position, womma.transform.position) < closestDistance)
            {
                closestPiece = pieceSlots[indexX, male.position.z];
                intVector2.x = indexX;
                intVector2.z = male.position.z;
            }
            ////  //检测传送门
            //else if (pieceSlots[indexX, male.position.z].Piece.GetComponent<Portal1>() != null)
            //{
            //    intVector2.x = indexX;
            //    intVector2.z = male.position.z;
            //}
        }
        int indexY = male.position.z - 1;
        if (indexY > topBoardHeight)
        {

            if (pieceSlots[male.position.x, indexY].Piece == null && Vector3.Distance(pieceSlots[male.position.x, indexY].transform.position, womma.transform.position) < closestDistance)
            {
                closestPiece = pieceSlots[male.position.x, indexY];
                intVector2.x = male.position.x;
                intVector2.z = indexY;
            }
            ////  //检测传送门
            //else if (pieceSlots[male.position.x, indexY].Piece.GetComponent<Portal1>() != null)
            //{
            //    intVector2.x = male.position.x;
            //    intVector2.z = indexY;
            //}
        }

        indexY = male.position.z + 1;
        if (indexY <= topBoardHeight + bottomBoardHeight + 1)
        {

            if (pieceSlots[male.position.x, indexY].Piece == null && Vector3.Distance(pieceSlots[male.position.x, indexY].transform.position, womma.transform.position) < closestDistance)
            {
                closestPiece = pieceSlots[male.position.x, indexY];
                intVector2.x = male.position.x;
                intVector2.z = indexY;
            }
            ////  //检测传送门
            //else if (pieceSlots[male.position.x, indexY].Piece.GetComponent<Portal1>() != null)
            //{
            //    intVector2.x = male.position.x;
            //    intVector2.z = indexY;
            //}
        }

        //检测传送门



        if (pieceSlots[intVector2.x, intVector2.z].Piece != pieceSlots[male.position.x, male.position.z].Piece)
        {

            pieceSlots[intVector2.x, intVector2.z].Piece = pieceSlots[male.position.x, male.position.z].Piece;
            pieceSlots[male.position.x, male.position.z].Piece = null;
            pieceSlots[intVector2.x, intVector2.z].Piece.Targetposition = pieceSlots[intVector2.x, intVector2.z].position;
            pieceSlots[intVector2.x, intVector2.z].Piece.slot = pieceSlots[intVector2.x, intVector2.z];
            pieceSlots[intVector2.x, intVector2.z].Piece.GetComponent<Character>().position = intVector2;
            // closestPiece.Piece = male .GetComponent<Piece>();
            //  malePiece  .slot.Piece = null;
            // malePiece.slot = pieceSlots[intVector2.x, intVector2.z];
            PlayState = PlayState.CharacterMove;
            canMove1 = true;
        }

        //---------womma
        // indexX = womma .position.x - 1;
        // closestPiece = pieceSlots[womma .position.x, womma .position.z];

        //Debug.LogWarning(closestPiece == womma );
        //IntVector2 v2 = intVector2;
        // closestDistance = Vector3.Distance(pieceSlots[womma .position.x, womma .position.z].transform.position, pieceSlots[v2 .x, v2 .z].transform.position);

        //intVector2.x = womma.position.x;
        //intVector2.z = womma.position.z;
        //if (indexX >= 0)
        //{

        //    if (pieceSlots[indexX, womma .position.z].Piece == null && Vector3.Distance(pieceSlots[indexX, womma .position.z].transform.position, pieceSlots[v2.x, v2.z].transform.position) < closestDistance)
        //    {
        //        closestPiece = pieceSlots[indexX, womma .position.z];
        //        intVector2.x = indexX;
        //        intVector2.z = womma.position.z;
        //    }
        //}
        //indexX = womma .position.x + 1;
        //if (indexX < topBoardWith)
        //{

        //    if (pieceSlots[indexX, womma .position.z].Piece == null && Vector3.Distance(pieceSlots[indexX, womma .position.z].transform.position, pieceSlots[v2.x, v2.z].transform.position) < closestDistance)
        //    {
        //        closestPiece = pieceSlots[indexX, womma .position.z];
        //        intVector2.x = indexX;
        //        intVector2.z = womma.position.z;
        //    }
        //}
        // indexY = womma .position.z - 1;
        //if (indexY > 0)
        //{

        //    if (pieceSlots[womma .position.x, indexY].Piece == null && Vector3.Distance(pieceSlots[womma .position.x, indexY].transform.position, pieceSlots[v2.x, v2.z].transform.position) < closestDistance)
        //    {
        //        closestPiece = pieceSlots[womma .position.x, indexY];
        //        intVector2.x = womma.position.x;
        //        intVector2.z = indexY;
        //    }
        //}

        //indexY = womma .position.z + 1;
        //if (indexY <= topBoardHeight)
        //{

        //    if (pieceSlots[womma .position.x, indexY].Piece == null && Vector3.Distance(pieceSlots[womma .position.x, indexY].transform.position, pieceSlots[v2.x, v2.z].transform.position) < closestDistance)
        //    {
        //        closestPiece = pieceSlots[womma.position.x, indexY];
        //        intVector2.x = womma.position.x;
        //        intVector2.z = indexY;
        //    }
        //}

        //if (pieceSlots[intVector2.x, intVector2.z].Piece!= pieceSlots[womma .position .x , womma.position .z].Piece)//pieceSlots[intVector2.x, intVector2.z].Piece == null || pieceSlots[intVector2.x, intVector2.z].Piece.GetComponent<Character>()==null 
        //{
        //    pieceSlots[intVector2.x, intVector2.z].Piece = pieceSlots[womma.position.x, womma.position.z].Piece;
        //    pieceSlots[womma .position .x, womma.position .z].Piece = null;

        //    pieceSlots[intVector2.x ,intVector2.z ].Piece .Targetposition = pieceSlots[intVector2.x, intVector2.z].position;
        //    pieceSlots[intVector2.x, intVector2.z].Piece.slot = pieceSlots[intVector2.x, intVector2.z];
        //    pieceSlots[intVector2.x, intVector2.z].Piece.GetComponent<Character >().position =intVector2 ;
        //   // Debug.LogError ("是否右路"+ intVector2.x+"--"+ intVector2.z);
        //    // wommaPiece.slot = pieceSlots[intVector2.x, intVector2.z];
        //    PlayState = PlayState.CharacterMove;
        //    canMove1 = true;
        //}


        return canMove1;
    }



    public bool MoveCharacterWomma()
    {
        bool canMove1 = false;
        Piece wommaPiece = womma.GetComponent<Piece>();


        IntVector2 intVector2 = new IntVector2();

        IntVector2 v2 = pieceSlots[male.position.x, male.position.z].position;
        //---------womma




        float closestDistance = Vector3.Distance(pieceSlots[womma.position.x, womma.position.z].transform.position, pieceSlots[v2.x, v2.z].transform.position);

        intVector2.x = womma.position.x;
        intVector2.z = womma.position.z;
        int indexX = womma.position.x - 1;
        if (indexX >= 0)
        {

            if (pieceSlots[indexX, womma.position.z].Piece == null && Vector3.Distance(pieceSlots[indexX, womma.position.z].transform.position, pieceSlots[v2.x, v2.z].transform.position) < closestDistance)
            {

                intVector2.x = indexX;
                intVector2.z = womma.position.z;
            }


        }
        indexX = womma.position.x + 1;
        if (indexX < topBoardWith)
        {

            if (pieceSlots[indexX, womma.position.z].Piece == null && Vector3.Distance(pieceSlots[indexX, womma.position.z].transform.position, pieceSlots[v2.x, v2.z].transform.position) < closestDistance)
            {

                intVector2.x = indexX;
                intVector2.z = womma.position.z;
            }

        }
        int indexY = womma.position.z - 1;
        if (indexY > 0)
        {

            if (pieceSlots[womma.position.x, indexY].Piece == null && Vector3.Distance(pieceSlots[womma.position.x, indexY].transform.position, pieceSlots[v2.x, v2.z].transform.position) < closestDistance)
            {

                intVector2.x = womma.position.x;
                intVector2.z = indexY;
            }

        }

        indexY = womma.position.z + 1;
        if (indexY <= topBoardHeight)
        {

            if (pieceSlots[womma.position.x, indexY].Piece == null && Vector3.Distance(pieceSlots[womma.position.x, indexY].transform.position, pieceSlots[v2.x, v2.z].transform.position) < closestDistance)
            {

                intVector2.x = womma.position.x;
                intVector2.z = indexY;
            }


        }







        indexX = womma.position.x - 1;
        if (indexX >= 0)
        {
            //  //检测传送门
            if (pieceSlots[indexX, womma.position.z].Piece != null && pieceSlots[indexX, womma.position.z].Piece.GetComponent<Portal1>() != null)
            {
                intVector2.x = indexX;
                intVector2.z = womma.position.z;
            }

        }
        indexX = womma.position.x + 1;
        if (indexX < topBoardWith)
        {
            //  //检测传送门
            if (pieceSlots[indexX, womma.position.z].Piece != null && pieceSlots[indexX, womma.position.z].Piece.GetComponent<Portal1>() != null)
            {
                intVector2.x = indexX;
                intVector2.z = womma.position.z;
            }
        }
        indexY = womma.position.z - 1;
        if (indexY > 0)
        {
            //  //检测传送门
            if (pieceSlots[womma.position.x, indexY].Piece != null && pieceSlots[womma.position.x, indexY].Piece.GetComponent<Portal1>() != null)
            {
                intVector2.x = womma.position.x;
                intVector2.z = indexY;
            }
        }

        indexY = womma.position.z + 1;
        if (indexY <= topBoardHeight)
        {
            //  //检测传送门
            if (pieceSlots[womma.position.x, indexY].Piece != null && pieceSlots[womma.position.x, indexY].Piece.GetComponent<Portal1>() != null)
            {
                intVector2.x = womma.position.x;
                intVector2.z = indexY;
            }
        }



        if (pieceSlots[intVector2.x, intVector2.z].Piece == null &&
            pieceSlots[intVector2.x, intVector2.z].Piece != pieceSlots[womma.position.x, womma.position.z].Piece)//pieceSlots[intVector2.x, intVector2.z].Piece == null || pieceSlots[intVector2.x, intVector2.z].Piece.GetComponent<Character>()==null 
        {
            pieceSlots[intVector2.x, intVector2.z].Piece = pieceSlots[womma.position.x, womma.position.z].Piece;
            pieceSlots[womma.position.x, womma.position.z].Piece = null;

            pieceSlots[intVector2.x, intVector2.z].Piece.Targetposition = pieceSlots[intVector2.x, intVector2.z].position;
            pieceSlots[intVector2.x, intVector2.z].Piece.slot = pieceSlots[intVector2.x, intVector2.z];
            pieceSlots[intVector2.x, intVector2.z].Piece.GetComponent<Character>().position = intVector2;
            PlayState = PlayState.CharacterMove;
            canMove1 = true;
        }
        else if (pieceSlots[intVector2.x, intVector2.z].Piece.GetComponent<Portal1>() != null)
        {
            //  pieceSlots[intVector2.x, intVector2.z].Piece = pieceSlots[womma.position.x, womma.position.z].Piece;
            //  pieceSlots[womma.position.x, womma.position.z].Piece = null;

            //pieceSlots[womma.position.x, womma.position.z].Piece.Targetposition = pieceSlots[intVector2.x, intVector2.z].position;

            //pieceSlots[womma.position.x, womma.position.z].Piece.slot = pieceSlots[intVector2.x, intVector2.z];
            //womma.position = intVector2;
            ////pieceSlots[intVector2.x, intVector2.z].Piece.slot = pieceSlots[intVector2.x, intVector2.z];
            ////pieceSlots[intVector2.x, intVector2.z].Piece.GetComponent<Character>().position = intVector2;
            //PlayState = PlayState.CharacterMove;
            //canMove1 = true;




            pieceSlots[intVector2.x, intVector2.z].Piece = pieceSlots[womma.position.x, womma.position.z].Piece;
            pieceSlots[womma.position.x, womma.position.z].Piece = null;

            pieceSlots[intVector2.x, intVector2.z].Piece.Targetposition = pieceSlots[intVector2.x, intVector2.z].position;
            pieceSlots[intVector2.x, intVector2.z].Piece.slot = pieceSlots[intVector2.x, intVector2.z];
            pieceSlots[intVector2.x, intVector2.z].Piece.GetComponent<Character>().position = intVector2;
            PlayState = PlayState.CharacterMove;
            canMove1 = true;
            Debug.LogWarning("wwwwwwwwwwwwwwwww");
        }


        return canMove1;
    }

    IEnumerator StayOneMinate()
    {
        yield return new WaitForSeconds(1);
        // PlayState = PlayState.FillPeiceState;
    }
}
