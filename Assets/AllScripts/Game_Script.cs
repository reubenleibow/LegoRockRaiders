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
		get { return RockShape == RockShape.BottomRight || RockShape == RockShape.BottomLeft || RockShape == RockShape.TopRight || RockShape == RockShape.TopLeft || RockShape == RockShape.Top || RockShape == RockShape.Bottom || RockShape == RockShape.Left || RockShape == RockShape.Right; }
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
	private Building_System Building_System;

	public GridPos[,] RockGridNumbers = new GridPos[Rows, Columns];

	// Use this for initialization
	void Start()
	{
		Building_System = this.GetComponent<Building_System>();
		var map2 = Resources.Load("map2") as Texture2D;

		//basically set all spaces that should have a rock a rock.

		
		for (int y = 0; y < Columns && y < map2.height; y++)
		{
			for (int x = 0; x < Rows && x < map2.width; x++)
			{
				var CurrentPixelColour = map2.GetPixel(x, y);
				RockGridNumbers[x, y] = new GridPos { X = x, Y = y };

				if (CurrentPixelColour == Color.white)
				{
					RockGridNumbers[x, y].RockType = RockType.None;
				}
				else
				{
					RockGridNumbers[x, y].RockType = RockType.SoftRock;
				}
			}
		}
		RemoveOutStandingRocks(0, Rows, 0, Columns);
		ResetRockMeshes(0, Rows, 0, Columns);
	}

	void Update()
	{
	}

	public void RemoveOutStandingRocks(int MinX, int MaxX, int MinY, int MaxY)
	{
		for (int X = MinX; X < MaxX; X++)
		{
			for (int Y = MinY; Y < MaxY; Y++)
			{
				var curr = RockGridNumbers[X, Y];
				UpdateAdjacentRocks(X, Y);

				if (curr.AdjacentRocks <= 1)
				{
					curr.RockShape = RockShape.None;
					curr.RockType = RockType.None;
				}
			}
		}
	}

	/// <summary>
	/// Looks at the array so far(rock or not) and gives them a more accurate name as well as removing all the rocks that are on its own.
	/// </summary>
	public void ResetRockMeshes(int MinX, int MaxX, int MinY, int MaxY)
	{
		for (int X = MinX; X < MaxX; X++)
		{
			for (int Y = MinY; Y < MaxY; Y++)
			{
				var curr = RockGridNumbers[X, Y];
				var old = RockGridNumbers[X, Y].RockShape;
				var Up = (Y <= 0) ? RockType.None : RockGridNumbers[X, Y - 1].RockType;
				var Down = (Y >= Columns - 1) ? RockType.None : RockGridNumbers[X, Y + 1].RockType;
				var Left = (X <= 0) ? RockType.None : RockGridNumbers[X - 1, Y].RockType;
				var Right = (X >= Rows - 1) ? RockType.None : RockGridNumbers[X + 1, Y].RockType;

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
				if (curr.RockType != RockType.None && curr.RockShape != old)
				{
					RecreateRockMesh(X, Y);
				}
			}
		}

		//creating the square slops
		for (int X = Mathf.Max(0, MinX); X <= Mathf.Min(MaxX, Rows - 1); X++)
		{
			for (int Y = Mathf.Max(0, MinY); Y <= Mathf.Min(MaxY, Columns - 1); Y++)
			{
				var curr = RockGridNumbers[X, Y];

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
		float oldHealth = 0.0f;

		if(curr.gameObj != null)
		{
			oldHealth = curr.gameObj.GetComponent<Work_Script>().Health;
		}

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

		//Square slopes
		if (curr.RockShape == RockShape.SquareBottomLeft)
		{
			curr.gameObj = Instantiate(RockSquareCorner, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
			curr.gameObj.transform.eulerAngles = new Vector3(0, 90, 0);
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
		}

		if (curr.RockShape == RockShape.SquareTopRight)
		{
			curr.gameObj = Instantiate(RockSquareCorner, new Vector3(RockWidth * X, RockHeight, RockWidth * Y), Quaternion.identity) as GameObject;
			curr.gameObj.transform.eulerAngles = new Vector3(0, 270, 0);
		}

		curr.gameObj.GetComponent<Work_Script>().RockProperties = curr;

		if(oldHealth > 0)
		{
			curr.gameObj.GetComponent<Work_Script>().Health = oldHealth;
		}


	}

	public void OnDestroyRock(GridPos rock)
	{
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
	public void EraseRock(int X, int Y)
	{
		if (X >= 0 && X < Rows && Y >= 0 && Y < Columns)
		{
			var Rks = RockGridNumbers[X, Y];

			if (Rks.RockType != RockType.None)
			{
				Rks.gameObj.GetComponent<Work_Script>().Health = 0;
				Rks.RockType = RockType.None;
				Rks.RockShape = RockShape.None;
				Destroy(Rks.gameObj);
				Rks.gameObj = null;

				UpdateAdjacentRocks(X - 1, X + 1, Y - 1, Y + 1);
			}
		}

		//Terrain.activeTerrain.BuildNavMesh();
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

	public void ReUpdateRocks(int MinX, int MaxX, int MinY, int MaxY)
	{
		for (int X = Mathf.Max(0, MinX); X <= Mathf.Min(MaxX, Rows - 1); X++)
		{
			for (int Y = Mathf.Max(0, MinY); Y <= Mathf.Min(MaxY, Columns - 1); Y++)
			{
				var curr = RockGridNumbers[X, Y];

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
}
