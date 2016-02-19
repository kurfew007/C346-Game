﻿using UnityEngine;
using System;
using System.Collections.Generic; 		
using Random = UnityEngine.Random; 		//Use the Unity Engine random number generator.

public class BoardManager : MonoBehaviour
{
	[Serializable]
	public class Count
	{
		public int minimum; 			
		public int maximum; 			
			
			
		//Assignment constructor.
		public Count (int min, int max)
		{
			minimum = min;
			maximum = max;
		}
	}
		

	public int columns = 8; 				//Number of columns in game board.
	public int rows = 8;					//Number of rows in game board.
	public Count wallCount = new Count (5, 9);		//Lower and upper limit for shrubs and walls.
	public Count foodCount = new Count (1, 5);		//Lower and upper limit for food items.
	public GameObject exit;					//Prefab to spawn for exit.
	public GameObject[] floorTiles;				//Array of floor prefabs.
	public GameObject[] wallTiles;				//Array of shrub and wall prefabs.
	public GameObject[] foodTiles;				//Array of food prefabs.
	public GameObject[] enemyTiles;				//Array of enemy prefabs.
	public GameObject[] outerWallTiles;			//Array of outer tile prefabs.
		
	private Transform boardHolder;					//Stores a reference to the transform of our Board object.
	private List <Vector3> gridPositions = new List <Vector3> ();	//A list of possible locations to place tiles.
		
		
	//Clears gridPositions and prepares it to generate a new board.
	void InitialiseList ()
	{
		//Clear our list of gridPositions.
		gridPositions.Clear ();
			
		//Loop through x axis (columns).
		for(int x = 1; x < columns-1; x++)
		{
			//Within each column, loop through y axis (rows).
			for(int y = 1; y < rows-1; y++)
			{
				//At each index add a new Vector3 to our list with the x and y coordinates of that position.
				gridPositions.Add (new Vector3(x, y, 0f));
			}
		}
	}
		
		
	//Sets up the outer walls and floor (background) of the game board.
	void BoardSetup ()
	{
		//Instantiate Board and set boardHolder to its transform.
		boardHolder = new GameObject ("Board").transform;
			
		//Loop along x axis, starting from -1 with floor or outerwall edge tiles.
		for(int x = -1; x < columns + 1; x++)
		{
			//Loop along y axis, starting from -1 to place floor or outerwall tiles.
			for(int y = -1; y < rows + 1; y++)
			{
				//Choose a floor tile.
				GameObject toInstantiate = floorTiles[Random.Range (0,floorTiles.Length)];
					
				//Check if current position is at board edge, if so choose an outer wall tile.
				if(x == -1 || x == columns || y == -1 || y == rows)
					toInstantiate = outerWallTiles [Random.Range (0, outerWallTiles.Length)];
					
				//Instantiate the GameObject using the chosen prefab at the current grid position.
				GameObject instance =
					Instantiate (toInstantiate, new Vector3 (x, y, 0f), Quaternion.identity) as GameObject;
					
				//Set the parent of our newly instantiated object instance to boardHolder.
				instance.transform.SetParent (boardHolder);
			}
		}
	}
		
		
	//Returns a random position from our list gridPositions.
	Vector3 RandomPosition ()
	{
		//Random number between 0 and the count of items in gridPositions.
		int randomIndex = Random.Range (0, gridPositions.Count);
			
		//randomIndex from gridPositions.
		Vector3 randomPosition = gridPositions[randomIndex];
			
		//Remove gridPositions[randomIndex] since it already has a tile.
		gridPositions.RemoveAt (randomIndex);
			
		//Return the randomly selected Vector3 position.
		return randomPosition;
	}
		
		
	// Chooses some number, from minimum to maximum, of tiles and places them on the board.
	void LayoutObjectAtRandom (GameObject[] tileArray, int minimum, int maximum)
	{
		//Choose a random number of objects to instantiate within the minimum and maximum limits
		int objectCount = Random.Range (minimum, maximum+1);
			
		//Instantiate objects until the randomly chosen limit objectCount is reached
		for(int i = 0; i < objectCount; i++)
		{
			//Choose a position for randomPosition by getting a random gridPosition
			Vector3 randomPosition = RandomPosition();
				
			//Choose a random tile from tileArray and assign it to tileChoice
			GameObject tileChoice = tileArray[Random.Range (0, tileArray.Length)];
				
			//Instantiate tileChoice at the position returned by RandomPosition with no change in rotation
			Instantiate(tileChoice, randomPosition, Quaternion.identity);
		}
	}
		
		
	//SetupScene initializes the level and calls the previous functions to lay out the game board
	public void SetupScene (int level)
	{
		//Creates the outer walls and floor.
		BoardSetup ();
			
		//Reset our list of gridpositions.
		InitialiseList ();
			
		//Instantiate a random number of wall tiles based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (wallTiles, wallCount.minimum, wallCount.maximum);
			
		//Instantiate a random number of food tiles based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (foodTiles, foodCount.minimum, foodCount.maximum);
			
		//Determine number of enemies based on current level number, based on a logarithmic progression
		int enemyCount = (int)Mathf.Log(level, 2f);
			
		//Instantiate a random number of enemies based on minimum and maximum, at randomized positions.
		LayoutObjectAtRandom (enemyTiles, enemyCount, enemyCount);
			
		//Instantiate the exit tile in the upper right hand corner of our game board
		Instantiate (exit, new Vector3 (columns - 1, rows - 1, 0f), Quaternion.identity);
	}
}
