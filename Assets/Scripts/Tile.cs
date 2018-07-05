using UnityEngine;
using System.Collections;

public class Tile : MonoBehaviour {

    public int xIndex;
    public int yIndex;

    GameBoard m_board;



	// Use this for initialization
	void Start () {
	
	}
	
	public void Init(int x, int y, GameBoard board)
    {
        xIndex = x;
        yIndex = y;
        m_board = board;
    }
}
