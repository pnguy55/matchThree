using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameBoard : MonoBehaviour {
    public int width;
    public int height;

    public int borderSize;

    public GameObject tilePrefab;

    //Declares a 2d array.  The comma indicates the existance of
    //another dimension in the array
    Tile[,] m_allTiles;

	// Use this for initialization
	void Start () {
        //Instantiates m_allTiles and sets size to the width and height of board.
        m_allTiles = new Tile[width, height];
        SetupTiles();
        SetupCamera();
	}
	
	void SetupTiles()
        /*
         * This method utilizes the 2d array m_allTiles to
         * add tiles 1 by 1 to the board.
         */
    {
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
}
