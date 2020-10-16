using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.XR.WSA.Input;
using Random = UnityEngine.Random;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int Minimum { get; set; }
        public int Maximum { get; set; }

        public Count(int min, int max)
        {
            this.Minimum = min;
            this.Maximum = max;
        }
    }


    public int Columns { get; } = 8;
    public int Rows { get; } = 8;

    public Count WallCount { get; } = new Count(5, 9);
    public Count FoodCount { get; } = new Count(1, 5);
    
    public readonly GameObject exit;
    public readonly GameObject[] floorTiles;
    public readonly GameObject[] wallTiles;
    public readonly GameObject[] foodTiles;
    public readonly GameObject[] enemyTiles;
    public readonly GameObject[] outerWallTiles;

    private Transform boardHolder;
    private readonly List<Vector3> gridPositions = new List<Vector3>();

    void InitializeList()
    {
        gridPositions.Clear();
        for (int x = 1; x < this.Columns - 1; x++)
        {
            for (int y = 1; y < this.Rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y));
            }
        }
    }

    void BoardSetup()
    {
        this.boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < this.Columns + 1; x++)
        {
            for (int y = -1; y < this.Rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];
                if (x == -1 || x == this.Columns || y == -1 || y == this.Rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y), Quaternion.identity);
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);

        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];

            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitializeList();
        LayoutObjectAtRandom(wallTiles, this.WallCount.Minimum, this.WallCount.Maximum);
        LayoutObjectAtRandom(foodTiles, this.FoodCount.Minimum, this.FoodCount.Maximum);

        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);

        Instantiate(exit, new Vector3(this.Columns - 1, this.Rows - 1), Quaternion.identity);
    }
}
