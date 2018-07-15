﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {
    public int width;
    public int height;

    public int borderSize;

    public GameObject tilePrefab;
    public GameObject[] gamePiecePrefabs;

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

    internal bool DragToTile(GamePiece gamePiece)
    {
        throw new NotImplementedException();
    }

    internal void ClickTile(GamePiece gamePiece)
    {
        throw new NotImplementedException();
    }

    void PlaceGamePiece(GamePiece gamePiece, int x, int y)
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

        //Sends position to GamePiece class to store that data.
        gamePiece.SetCoord(x, y);
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
                    //Passes GamePiece and coordinates to PlaceGamePiece function to populate board
                    //one by one.
                    PlaceGamePiece(randomPiece.GetComponent<GamePiece>(), i, j);
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
        if (m_clickedTile != null)
        {
            m_targetTile = tile;
        }
    }

    public void ReleaseTile()
    {
        if (m_clickedTile != null && m_targetTile != null)
        {

        }
    }

    void SwitchTiles(Tile clickedTile, Tile targetTile)
    {

        //add code to switch corresponding Gamepieces

        m_clickedTile = null;
        m_targetTile = null;
    }


}
