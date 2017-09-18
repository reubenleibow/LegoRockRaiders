using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;

public class GridPos
{
	public RockType RockType = RockType.None;
	public RockShape RockShape = RockShape.None;
	public int X = 0;
	public int Y = 0;
	public GameObject gameObj;
	public int AdjacentRocks = 0;

	public bool IsCorner
	{
		get { return RockShape == RockShape.BottomRight || RockShape == RockShape.BottomLeft || RockShape == RockShape.TopRight || RockShape == RockShape.TopLeft; }
	}

	public override string ToString()
	{
		return string.Format("Type={0}, Shape={1}, Pos={2},{3}, Adj={4}", RockType, RockShape, X, Y, AdjacentRocks);
	}
}

public enum RockShape
{
	None = 0,

	Square,
	Top,
	TopRight,
	TopLeft,
	Bottom,
	BottomRight,
	BottomLeft,
	Left,
	Right,
	SquareTopLeft,
	SquareTopRight,
	SquareBottomLeft,
	SquareBottomRight

}

public enum RockType
{
	None,

	LooseRock,
	SoftRock,
	HardRock,
	SolidRock
}

public class Game_Script : MonoBehaviour
{
	private const int Rows = 100;
	private const int Columns = 100;

	public GameObject RockSquare;
	public GameObject RockSlope;
	public GameObject RockCorner;
	public GameObject RockSquareCorner;

	public float RockWidth = 12;
	private float RockHeight = 8;

	public GridPos[,] RockGridNumbers = new GridPos[Rows, Columns];
	//public int[,] RockGridObjects = new int[100, 100];

	// Use this for initialization
	void Start()
	{
		for (int iX = 0; iX < Rows; iX++)
		{
			for (int iY = 0; iY < Columns; iY++)
			{
				RockGridNumbers[iX, iY] = new GridPos { X = iX, Y = iY };
			}
		}

		var map2 = Resources.Load("map2") as Texture2D;
		for (int y = 0; y < Columns && y < map2.height; y++)
		{
			for (int x = 0; x < Rows && x < map2.width; x++)
			{
				var rock = map2.GetPixel(x, y);

				if (rock != Color.white)
				{
					if (rock == Color.black)
					{
						RockGridNumbers[x, y].RockType = RockType.HardRock;
					}
					if (rock == Color.red)
					{
						RockGridNumbers[x, y].RockType = RockType.LooseRock;
					}
					if (rock == new Color(1, 1, 0))
					{
						RockGridNumbers[x, y].RockType = RockType.SoftRock;
					}
				}
			}
		}

		//var map = Resources.Load("map");
		//var lines = map.ToString().Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
		//for (int y = 0; y < Columns && y < lines.Length; y++)
		//{
		//	var line = lines[y];
		//	for (int x = 0; x < Rows && x < line.Length; x++)
		//	{
		//		var rock = line[x].ToString();
		//		RockGridNumbers[x, y].RockType = (RockType)int.Parse(rock);
		//	}
		//}

		//for (int X = 1; X < 3; X++)
		//{
		//	for (int Y = 1; Y < 3; Y++)
		//	{
		//		//var NewRock = Instantiate(Rock,new Vector3(RockWidth*X, RockHeight, RockWidth*Y), Quaternion.identity);
		//		RockGridNumbers[X, Y].RockType = RockType.HardRock;
		//		//	RockGridNumbers[X, Y] = new GridPos { X = X, Y = Y, RockType = RockType.HardRock };

		//	}
		//}

		//RockGridNumbers[3, 3].RockType = RockType.HardRock;
		//RockGridNumbers[3, 4].RockType = RockType.HardRock;
		//RockGridNumbers[3, 5].RockType = RockType.HardRock;
		//RockGridNumbers[3, 6].RockType = RockType.HardRock;

		//RockGridNumbers[4, 3].RockType = RockType.HardRock;
		//RockGridNumbers[4, 4].RockType = RockType.HardRock;
		//RockGridNumbers[4, 5].RockType = RockType.HardRock;
		//RockGridNumbers[4, 6].RockType = RockType.HardRock;

		//RockGridNumbers[5, 3].RockType = RockType.HardRock;
		//RockGridNumbers[5, 4].RockType = RockType.HardRock;
		//RockGridNumbers[5, 5].RockType = RockType.HardRock;
		//RockGridNumbers[5, 6].RockType = RockType.HardRock;

		ResetRockMeshes(0, Rows, 0, Columns);
	}

	// Update is called once per frame
	void Update()
	{
	}

	/// <summary>
	/// Looks at the array `RockGridNumbers` and places the correct mesh
	/// objects in the world.
	/// </summary>
	public void ResetRockMeshes(int MinX, int MaxX, int MinY, int MaxY)
	{
		for (int X = MinX; X < MaxX; X++)
		{
			for (int Y = MinY; Y < MaxY; Y++)
			{
				var curr = RockGridNumbers[X, Y];
				var Up = (Y <= 0) ? RockType.None : RockGridNumbers[X, Y - 1].RockType;
				var Down = (Y >= Columns - 1) ? RockType.None : RockGridNumbers[X, Y + 1].RockType;
				var Left = (X <= 0) ? RockType.None : RockGridNumbers[X - 1, Y].RockType;
				var Right = (X >= Rows - 1) ? RockType.None : RockGridNumbers[X + 1, Y].RockType;

				// set the number of rocks next to this one
				UpdateAdjacentRocks(X, Y);

				// set the rock shape
				if (Up == RockType.None)
				{
					curr.RockShape = RockShape.Top;
					if (Left == RockType.None)
						curr.RockShape = RockShape.TopLeft;
					if (Right == RockType.None)
						curr.RockShape = RockShape.TopRight;
				}
				if (Down == RockType.None)
				{
					curr.RockShape = RockShape.Bottom;
					if (Left == RockType.None)
						curr.RockShape = RockShape.BottomLeft;
					if (Right == RockType.None)
						curr.RockShape = RockShape.BottomRight;
				}
				if (Up != RockType.None && Down != RockType.None)
				{
					if (Right == RockType.None)
						curr.RockShape = RockShape.Right;
					if (Left == RockType.None)
						curr.RockShape = RockShape.Left;
				}
				if (Up != RockType.None && Down != RockType.None && Left != RockType.None && Right != RockType.None)
				{
					curr.RockShape = RockShape.Square;
				}

				// create rock mesh
				if (curr.RockType != RockType.None)
				{
					RecreateRockMesh(X, Y);
				}
			}
		}

		for (int X = Mathf.Max(0, MinX); X <= Mathf.Min(MaxX, Rows - 1); X++)
		{
			for (int Y = Mathf.Max(0, MinY); Y <= Mathf.Min(MaxY, Columns - 1); Y++)
			{
				var curr = RockGridNumbers[X, Y];
				//var corner = (RockShape.TopLeft || RockShape.TopRight)

				if (curr.RockShape == RockShape.Square)
				{
					if (RockGridNumbers[X, Y + 1].IsCorner == true && RockGridNumbers[X + 1, Y].IsCorner == true)
					{
						curr.RockShape = RockShape.SquareTopRight;
						RecreateRockMesh(X, Y);
					}

					if (RockGridNumbers[X, Y + 1].IsCorner == true && RockGridNumbers[X - 1, Y].IsCorner == true)
					{
						curr.RockShape = RockShape.SquareTopLeft;
						RecreateRockMesh(X, Y);

					}

					if (RockGridNumbers[X, Y - 1].IsCorner == true && RockGridNumbers[X + 1, Y].IsCorner == true)
					{
						curr.RockShape = RockShape.SquareBottomRight;
						RecreateRockMesh(X, Y);
					}

					if (RockGridNumbers[X, Y - 1].IsCorner == true && RockGridNumbers[X - 1, Y].IsCorner == true)
					{
						curr.RockShape = RockShape.SquareBottomLeft;
						RecreateRockMesh(X, Y);
					}
				}
			}
		}
	}

	/// <summary>
	/// Given X and Y, create the correct mesh in the world.
	/// </summary>
	public void RecreateRockMesh(int X, int Y)
	{
		var curr = RockGridNumbers[X, Y];

		if (curr.gameObj != null)
		{
			Destroy(curr.gameObj);
		}

		if (curr.RockShape == RockShape.Square)
		{
			curr.gameObj = Instantiate(RockSquare, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
		}

		if (curr.RockShape == RockShape.Top)
		{
			curr.gameObj = Instantiate(RockSlope, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
		}

		if (curr.RockShape == RockShape.TopLeft)
		{
			curr.gameObj = Instantiate(RockCorner, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
			curr.gameObj.transform.eulerAngles = new Vector3(0, 90, 0);
		}

		if (curr.RockShape == RockShape.TopRight)
		{
			curr.gameObj = Instantiate(RockCorner, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
		}

		if (curr.RockShape == RockShape.Bottom)
		{
			curr.gameObj = Instantiate(RockSlope, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
			curr.gameObj.transform.eulerAngles = new Vector3(0, 180, 0);
		}

		if (curr.RockShape == RockShape.BottomLeft)
		{
			curr.gameObj = Instantiate(RockCorner, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
			curr.gameObj.transform.eulerAngles = new Vector3(0, 180, 0);
		}

		if (curr.RockShape == RockShape.BottomRight)
		{
			curr.gameObj = Instantiate(RockCorner, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
			curr.gameObj.transform.eulerAngles = new Vector3(0, 270, 0);
		}

		if (curr.RockShape == RockShape.Right)
		{
			curr.gameObj = Instantiate(RockSlope, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
			curr.gameObj.transform.eulerAngles = new Vector3(0, 270, 0);
		}

		if (curr.RockShape == RockShape.Left)
		{
			curr.gameObj = Instantiate(RockSlope, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
			curr.gameObj.transform.eulerAngles = new Vector3(0, 90, 0);
		}

		if (curr.RockShape == RockShape.SquareBottomLeft)
		{
			curr.gameObj = Instantiate(RockSquareCorner, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
			curr.gameObj.transform.eulerAngles = new Vector3(0, 90, 0);
			//R
		}

		if (curr.RockShape == RockShape.SquareBottomRight)
		{
			curr.gameObj = Instantiate(RockSquareCorner, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
			curr.gameObj.transform.eulerAngles = new Vector3(0, 0, 0);

		}

		if (curr.RockShape == RockShape.SquareTopLeft)
		{
			curr.gameObj = Instantiate(RockSquareCorner, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
			curr.gameObj.transform.eulerAngles = new Vector3(0, 180, 0);

			//R

		}

		if (curr.RockShape == RockShape.SquareTopRight)
		{
			curr.gameObj = Instantiate(RockSquareCorner, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
			curr.gameObj.transform.eulerAngles = new Vector3(0, 270, 0);

		}

		curr.gameObj.GetComponent<Rock_Script>().RockProperties = curr;
	}

	public void OnDestroyRock(GridPos rock)
	{

		this.GetComponent<System_Script>().OnRockDestroyed(rock.gameObj);

		// destroy current rock
		EraseRock(rock.X, rock.Y);

		// reset rock meshes
		var MinX = rock.X - 1;
		var MaxX = rock.X + 2;
		var MinY = rock.Y - 1;
		var MaxY = rock.Y + 2;

		ResetRockMeshes(MinX, MaxX, MinY, MaxY);

	}

	/// <summary>
	/// Destroy a rock in the world and then update adjacent rocks
	/// </summary>
	private void EraseRock(int X, int Y)
	{
		if (X >= 0 && X < Rows && Y >= 0 && Y < Columns)
		{
			var Rks = RockGridNumbers[X, Y];

			if (Rks.RockType != RockType.None)
			{
				Rks.RockType = RockType.None;
				Rks.RockShape = RockShape.None;
				Destroy(Rks.gameObj);
				Rks.gameObj = null;

				UpdateAdjacentRocks(X - 1, X + 1, Y - 1, Y + 1);
			}
		}
	}

	/// <summary>
	/// Update the adjacent rocks, and remove any invalid rock slopes
	/// </summary>
	public void UpdateAdjacentRocks(int MinX, int MaxX, int MinY, int MaxY)
	{
		for (int X = Mathf.Max(0, MinX); X <= Mathf.Min(MaxX, Rows - 1); X++)
		{
			for (int Y = Mathf.Max(0, MinY); Y <= Mathf.Min(MaxY, Columns - 1); Y++)
			{
				UpdateAdjacentRocks(X, Y);

				var curr = RockGridNumbers[X, Y];
				if (curr.AdjacentRocks <= 1)
				{
					EraseRock(X, Y);
				}
			}
		}
	}

	/// <summary>
	/// Updat the number of adjacent rocks
	/// </summary>
	public void UpdateAdjacentRocks(int X, int Y)
	{
		if (X >= 0 && X < Rows && Y >= 0 && Y < Columns)
		{
			var curr = RockGridNumbers[X, Y];

			var Up = (Y <= 0) ? RockType.None : RockGridNumbers[X, Y - 1].RockType;
			var Down = (Y >= Columns - 1) ? RockType.None : RockGridNumbers[X, Y + 1].RockType;
			var Left = (X <= 0) ? RockType.None : RockGridNumbers[X - 1, Y].RockType;
			var Right = (X >= Rows - 1) ? RockType.None : RockGridNumbers[X + 1, Y].RockType;

			// set the number of rocks next to this one
			curr.AdjacentRocks = 0;
			if (Up != RockType.None)
				curr.AdjacentRocks++;
			if (Down != RockType.None)
				curr.AdjacentRocks++;
			if (Left != RockType.None)
				curr.AdjacentRocks++;
			if (Right != RockType.None)
				curr.AdjacentRocks++;
		}
	}
}
