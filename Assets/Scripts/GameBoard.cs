using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameBoard : MonoBehaviour {
    public int width;
    public int height;

    public int borderSize;

    public GameObject tilePrefab;
    public GameObject[] gamePiecePrefabs;

    //adjust speed of movement
    public float swapTime = 0.5f;

    //Declares a 2d array.  The comma indicates the existance of
    //another dimension in the array
    Tile[,] m_allTiles;
    GamePiece[,] m_allGamePieces;

    Tile m_clickedTile;
    Tile m_targetTile;

	// Use this for initialization
	void Start () {
        //Instantiates m_allTiles and sets size to the width and height of board.
        m_allTiles = new Tile[width, height];
        m_allGamePieces = new GamePiece[width, height];
        
        SetupTiles();
        SetupCamera();
        FillRandom();
        //ClearPieceAt(3, 5);
        //ClearPieceAt(5, 6);
    }
	
	void SetupTiles()
    {
        /*
        * This method utilizes the 2d array m_allTiles to
        * add tiles 1 by 1 to the board.
        */
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                GameObject tile = Instantiate (tilePrefab, new Vector3(i, j, 0), Quaternion.identity) as GameObject;

                tile.name = "Tile (" + i + "," + j + ")";

                m_allTiles[i, j] = tile.GetComponent<Tile>();

                tile.transform.parent = transform;

                /*
                 * because the call to the tile class must be made on the
                 * individual tile, we must call Init from within the 
                 * creation loop.  This will record the position of the
                 * tile in respect to the grid being created through
                 * the i and j values.
                */
                m_allTiles[i, j].Init(x:i, y:j, board:this);
            }
        }
    }

    void SetupCamera()
    {
        Camera.main.transform.position = new Vector3((float)(width-1) / 2f, (float)(height-1) / 2f, -10f);

        /*
         * aspect ratio formula is universal 
           aspect ratio = Screen.height/Screen.width
           however, here we invert due to game being mainly vertical.
         */
        float aspectRatio = (float)Screen.width / (float)Screen.height;
        
        /*
         * The following utilize orthographic size, which is half of regular
         * dimensions because ortho size is from center towards outer edge.
         * The formula is (orthosize+borderSize)/aspect ratio
         */
        float verticalSize = ((float) height / 2f + (float) borderSize) / aspectRatio;

        float horizontalSize = ((float)width / 2f + (float)borderSize) / aspectRatio;

        Camera.main.orthographicSize = (verticalSize > horizontalSize) ? verticalSize : horizontalSize;
    }

    GameObject GetRandomGamePiece()
    {
        //Declares an int to choose a random piece from the 
        //gamePiecePrefab array.
        
        int randomIdx = UnityEngine.Random.Range(0, gamePiecePrefabs.Length);

        //Defensive programming, if we forget to set the array in the inspector
        //This snippet will catch that and give us an error
        if (gamePiecePrefabs[randomIdx] == null)
        {
            Debug.LogWarning("BOARD: " + randomIdx + "does not contain a valid GamePiece prefab!!!");
        }

        return gamePiecePrefabs[randomIdx];
    }


    public void PlaceGamePiece(GamePiece gamePiece, int x, int y)
    {
        //If gamePiece is invalid, it barks at you1
        if(gamePiece == null)
        {
            Debug.LogWarning("BOARD: Invalid GamePiece!!!");
            return;
        }

        //Positions the game piece and unrotates and the decided x and y.
        gamePiece.transform.position = new Vector3(x, y, 0);
        gamePiece.transform.rotation = Quaternion.identity;

        //Update the correct element of the 2d array.
        if (IsWithinBounds(x, y))
        {
            m_allGamePieces[x, y] = gamePiece;
        }
        gamePiece.SetCoord(x, y);
        

        //Sends position to GamePiece class to store that data.
        gamePiece.SetCoord(x, y);
    }

    bool IsWithinBounds(int x, int y)
    {
        //Ensures that the x and y are within bounds of array
        return (x >= 0 && x < width && y >= 0 && y < height);
    }
    void FillRandom()
    {
        /*Utilizes double loop to populate a 2d array to place gamePieces
         * in each tile.
         */
        for(int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                //Creates randomPiece gameObject and gives it the piece retrieved from
                //GetRandomGamePiece function.
                GameObject randomPiece = Instantiate(GetRandomGamePiece(), Vector3.zero, Quaternion.identity) as GameObject;

                
                if(randomPiece != null)
                {
                    randomPiece.GetComponent<GamePiece>().Init(this);
                    //Passes GamePiece and coordinates to PlaceGamePiece function to populate board
                    //one by one.
                    PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i, j);
                    randomPiece.transform.parent = transform;
                }
            }
        }
    }

    public void ClickTile(Tile tile)
    {
        if (m_clickedTile == null)
        {
            m_clickedTile = tile;
            Debug.Log("clicked tile: " + tile.name);
        }
    }

    public void DragToTile(Tile tile)
    {
        if (m_clickedTile != null && IsNextTo(tile, m_clickedTile))
        {
            m_targetTile = tile;
        }
    }

    public void ReleaseTile()
    {
        if (m_clickedTile != null && m_targetTile != null)
        {
            SwitchTiles(m_clickedTile, m_targetTile);
        }

        m_clickedTile = null;
        m_targetTile = null;

    }

    void SwitchTiles(Tile clickedTile, Tile targetTile)
    {
        StartCoroutine(SwitchTilesRoutine(clickedTile, targetTile));
    }


    IEnumerator SwitchTilesRoutine(Tile clickedTile, Tile targetTile)
    {
        GamePiece clickedPiece = m_allGamePieces[clickedTile.xIndex, clickedTile.yIndex];
        GamePiece targetPiece = m_allGamePieces[targetTile.xIndex, targetTile.yIndex];
        //add code to switch corresponding Gamepieces

        if (targetPiece != null && clickedPiece != null)
        {
            clickedPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
            targetPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);

            yield return new WaitForSeconds(swapTime);

            List<GamePiece> clickedPieceMatches = FindMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
            List<GamePiece> targetPieceMatches = FindMatchesAt(targetTile.xIndex, targetTile.yIndex);

            if (targetPieceMatches.Count == 0 && clickedPieceMatches.Count == 0)
            {
                clickedPiece.Move(clickedTile.xIndex, clickedTile.yIndex, swapTime);
                targetPiece.Move(targetTile.xIndex, targetTile.yIndex, swapTime);
            }
            else
            {
                yield return new WaitForSeconds(swapTime);
                ClearPieceAt(clickedPieceMatches);
                ClearPieceAt(targetPieceMatches);

                //HighLightMatchesAt(clickedTile.xIndex, clickedTile.yIndex);
                //HighLightMatchesAt(targetTile.xIndex, targetTile.yIndex);
            }

            
        }
    }


    bool IsNextTo(Tile start, Tile end)
    {
        if (Mathf.Abs(start.xIndex - end.xIndex) == 1 && start.yIndex == end.yIndex)
        {
            return true;
        }

        if (Mathf.Abs(start.yIndex - end.xIndex) == 1 && start.xIndex == end.xIndex)
        {
            return true;
        }

        return false;
    }

    List<GamePiece> FindMatches(int startX, int startY, Vector2 searchDirection, int minLength = 3)
    {
        List<GamePiece> matches = new List<GamePiece>();
        GamePiece startPiece = null;

        if (IsWithinBounds(startX, startY))
        {
            startPiece = m_allGamePieces[startX, startY];
        }

        if (startPiece != null)
        {
            matches.Add(startPiece);
        }

        else
        {
            return null;
        }

        int nextX;
        int nextY;

        int maxValue = (width > height) ? width : height;

        for (int i = 1; i < maxValue -1; i++)
        {
            nextX = startX + (int) Mathf.Clamp(searchDirection.x, -1, 1) * i;
            nextY = startY + (int) Mathf.Clamp(searchDirection.y, -1, 1) * i;

            if (!IsWithinBounds(nextX, nextY))
            {
                break;
            }

            GamePiece nextPiece = m_allGamePieces[nextX, nextY];

            if (nextPiece == null)
            {
                break;
            }
            else
            {

                if (nextPiece.matchValue == startPiece.matchValue && !matches.Contains(nextPiece))
                {
                    matches.Add(nextPiece);
                }

                else
                {
                    break;
                }
            }
        }

        if (matches.Count >= minLength)
        {
            return matches;
        }

        return null;


    }

    List<GamePiece> FindVerticalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> upwardMatches = FindMatches(startX, startY, new Vector2(0, 1), 2);
        List<GamePiece> downwardMatches = FindMatches(startX, startY, new Vector2(0, -11), 2);

        if (upwardMatches == null)
        {
            upwardMatches = new List<GamePiece>();
        }

        if(downwardMatches == null)
        {
            downwardMatches = new List<GamePiece>();
        }

        var combinedMatches = upwardMatches.Union(downwardMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;

    }

    List<GamePiece> FindHorizontalMatches(int startX, int startY, int minLength = 3)
    {
        List<GamePiece> rightMatches = FindMatches(startX, startY, new Vector2(1, 0), 2);
        List<GamePiece> leftMatches = FindMatches(startX, startY, new Vector2(-1, 0), 2);

        if (rightMatches == null)
        {
            rightMatches = new List<GamePiece>();
        }

        if (leftMatches == null)
        {
            leftMatches = new List<GamePiece>();
        }

        var combinedMatches = rightMatches.Union(leftMatches).ToList();

        return (combinedMatches.Count >= minLength) ? combinedMatches : null;

    }


    void HighLightTileOff(int x, int y)
    {
        SpriteRenderer spriteRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);

    }

    void HighLightTileOn(int x, int y, Color col)
    {
        SpriteRenderer spriteRenderer = m_allTiles[x, y].GetComponent<SpriteRenderer>();
        spriteRenderer.color = col;
    }


    void HighLightMatches()
    {
        for (int i = 0; i < width; i++)
        {
            for(int j = 0; j < height; j++)
            {
                HighLightMatchesAt(i, j);
            }
        }
    }

    private void HighLightMatchesAt(int x, int y)
    {
        HighLightTileOff(x, y);
        var combinedMatches = FindMatchesAt(x, y);

        if (combinedMatches.Count > 0)
        {
            foreach (GamePiece piece in combinedMatches)
            {
                HighLightTileOn(piece.xIndex, piece.yIndex, piece.GetComponent<SpriteRenderer>().color);
            }
        }
    }

    List<GamePiece> FindMatchesAt(int x, int y, int minLength = 3)
    {
        List<GamePiece> horizMatches = FindHorizontalMatches(x, y, 3);
        List<GamePiece> vertMatches = FindVerticalMatches(x, y, 3);

        if (horizMatches == null)
        {
            horizMatches = new List<GamePiece>();
        }

        if (vertMatches == null)
        {
            vertMatches = new List<GamePiece>();
        }

        var combinedMatches = horizMatches.Union(vertMatches).ToList();
        return combinedMatches;
    }

    void ClearPieceAt(int x, int y)
    {
        GamePiece pieceToClear = m_allGamePieces[x, y];

        if (pieceToClear != null)
        {
            m_allGamePieces[x, y] = null;
            Destroy(pieceToClear.gameObject);
        }

        HighLightTileOff(x, y);
    }

    void ClearBoard()
    {
        for(int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                ClearPieceAt(i, j);
            }
        }
    }

    //OVERLOAD OF CLEARPIECEAT METHOD
    void ClearPieceAt(List<GamePiece> gamePieces)
    {
        foreach (GamePiece piece in gamePieces)
        {
            ClearPieceAt(piece.xIndex, piece.yIndex);
        }
    }
}
