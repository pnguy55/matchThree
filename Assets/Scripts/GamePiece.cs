using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePiece : MonoBehaviour {

    public int xIndex;
    public int yIndex;

    GameBoard m_board;

    bool m_isMoving = false;

    public InterpType interpolation = InterpType.SmootherStep;

    //Enumerate the types of functions used for interpolation.
    public enum InterpType
    {
        Linear,
        EaseOut,
        EaseIn,
        SmoothStep,
        SmootherStep
    };


	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
        /*
		if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            Move((int)transform.position.x + 2, (int) transform.position.y, 0.5f);
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Move((int)transform.position.x - 2, (int)transform.position.y, 0.5f);
        }
        */
    }

    public void Init(GameBoard board)
    {
        
        m_board = board;
    }

    //Called to set coordinate of piece.
    public void SetCoord(int x, int y)
    {
        xIndex = x;
        yIndex = y;
    }
    //Moves piece by giving the coroutine parameters and starts the coroutine.
    public void Move(int destX, int destY, float timeToMove)
    {
        if (!m_isMoving)
        {
            StartCoroutine(MoveRoutine(new Vector3(destX, destY, 0), timeToMove));

        }
    }

    //This is the coroutine.
    IEnumerator MoveRoutine(Vector3 destination, float timeToMove)
    {

        Vector3 startPosition = transform.position;

        bool reachedDestination = false;

        float elapsedTime = 0f;

        m_isMoving = true;

        //Loop that runs as long as reachedDestination is false.
        while(!reachedDestination)
        {
            //do something useful here

            //If the distance between the position and the destination is small enough
            //this will close the gap and just put in the final destination.
            if (Vector3.Distance(transform.position, destination) < 0.01f)
            {
                reachedDestination = true;

                if (m_board != null)
                {
                    m_board.PlaceGamePiece(this, (int)destination.x, (int)destination.y);
                }

                break;
            }

            //Shows us the elasped time by accessing the deltaTime which is the runtime.
            elapsedTime += Time.deltaTime;

            float t = Mathf.Clamp(elapsedTime / timeToMove, 0f, 1f);

            //Switch function to choose which function based on the one we picked in the beginning
            // of this class.
            switch (interpolation)
            {
                case InterpType.Linear:
                    break;
                case InterpType.EaseOut:
                    t = Mathf.Sin(t * Mathf.PI * 0.5f);
                    break;
                case InterpType.EaseIn:
                    t = 1 - Mathf.Cos(t * Mathf.PI * 0.5f);
                    break;
                case InterpType.SmoothStep:
                    t = t * t * (3 - 2 * t);
                    break;
                case InterpType.SmootherStep:
                    t = t * t * t * (t * (6 - 15) + 10);
                    break;
            }


            //
            //moves the piece by using the Lerp method of Unity's vector 3 class.
            transform.position = Vector3.Lerp(startPosition, destination, t);

            //wait until next frame
            yield return null;
        }
       
        m_isMoving = false;
    }
}
