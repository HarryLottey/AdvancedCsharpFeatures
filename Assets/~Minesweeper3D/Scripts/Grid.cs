﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minesweeper3D
{
    public class Grid : MonoBehaviour
    {
        public GameObject blockPrefab;
        // The grid's dimensions
        public int width = 10;
        public int height = 10;
        public int depth = 10;
        public float spacing = 1.2f; // Spacing between each block

        // MD array for storing blocks 
        private Block[,,] blocks;

        // Spawns a block at position and returns block component
        Block SpawnBlock(Vector3 pos)
        {
            GameObject clone = Instantiate(blockPrefab); // spawn clone
            clone.transform.position = pos; // set position
            Block currentBlock = clone.GetComponent<Block>(); // Get block component
            return currentBlock; // Return it
        }

        // Spawns blocks in a grid like fashion
        void GenerateBlocks()
        {
            // Create 3D array to store all the blocks
            blocks = new Block[width, height, depth];

            // Loop through the X, Y and Z axis of the 3D array
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        // Calculate half size using array dimensions
                        Vector3 halfSize = new Vector3(width / 2, height / 2, depth / 2);
                        // Make sure to offset by half (so that elements are centered)
                        halfSize -= new Vector3(0.5f, 0.5f, 0.5f);
                        // Create position for element to pivot around Grid zero
                        Vector3 pos = new Vector3(x - halfSize.x, y - halfSize.y, z - halfSize.z);

                        // Apply spacing
                        pos *= spacing;
                        // Spawn the block at that position
                        Block block = SpawnBlock(pos);
                        // Attach block to grid as a child
                        block.transform.SetParent(transform);
                        // Store array coordinate inside the block itself
                        block.x = x;
                        block.y = y;
                        block.z = z;
                        // Store block in the array coordinates
                        blocks[x, y, z] = block;
                    }
                }
            }
        }

        int GetAdjacentMineCountAt(Block b)
        {
            int count = 0;
            // Loop through all elements and hav eeach axis go between -1 to 1
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    for (int z = -1; z <= 1; z++)
                    {
                        // Calculate adjacent element's index
                        int desiredX = b.x + x;
                        int desiredY = b.y + y;
                        int desiredZ = b.z + z;

                        // Coordinates in range?
                        if (desiredX >= 0 && desiredY >= 0 && desiredZ >= 0 && desiredX < width && desiredY < height && desiredZ < depth)
                        {
                            // Then check for mine
                            Block currentBlock = blocks[desiredX, desiredY, desiredZ];
                            if (currentBlock.isMine)
                            {
                                // Increment the count
                                count++;
                            }
                        }
                    }
                }            
            }
            // Return the total recorded at the end of the algorithm
            return count;
        }

        // Flood Fill function to uncover all the empty elements
        public void FFuncover(int x, int y, int z, bool[,,] visited)
        {
            // Coordinates in Range?
            if (x >= 0 && y >= 0 && z >= 0 && 
                x < width && y < height && z < depth)
            {
                // Visited already?
                if (visited[x, y, z])
                    return;

                // Uncover element
                Block block = blocks[x, y, z];
                int adjacentMines = GetAdjacentMineCountAt(block);
                block.Reveal(adjacentMines);

                // Close to a mine?
                if (adjacentMines > 0)
                    return;

                // Set visited flag
                visited[x, y, z] = true;

                // Perform recursion in each axis to detect adjacent elements
                FFuncover(x - 1, y, z - 1, visited);
                FFuncover(x + 1, y, z - 1, visited);
                FFuncover(x, y-1, z - 1, visited);
                FFuncover(x, y+1, z - 1, visited);
                FFuncover(x, y, z - 1, visited);

                // Z axis
                FFuncover(x - 1, y, z, visited);
                FFuncover(x + 1, y, z, visited);
                FFuncover(x, y - 1, z, visited);
                FFuncover(x, y + 1, z, visited);

                // Back row
                FFuncover(x - 1, y, z + 1, visited);
                FFuncover(x + 1, y, z + 1, visited);
                FFuncover(x, y - 1, z + 1, visited);
                FFuncover(x, y + 1, z + 1, visited);
                FFuncover(x, y, z - 1, visited);
            }
        }

        // Uncovers all mines that are in the grid
        public void UncoverMines()
        {
             // Figure out this thing 

            // Loop through all elements in array
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int z = 0; z < depth; z++)
                    {
                        // Get currentBlock at index
                        Block currentBlock = blocks[x, y, z];
                        // IF currentBlock is a mine
                        if (currentBlock.isMine == true)
                        {
                            // Reveal the mine
                            currentBlock.Reveal(0);
                        }
                    }
                }     
            }
        }


        public void SelectBlock(Block selectedBlock)
        {
            // Reveal the selected block
            int adjacentMines = GetAdjacentMineCountAt(selectedBlock);
            selectedBlock.Reveal(adjacentMines);
            // IF the selectedblock is a mine
            if (selectedBlock.isMine == true)
            {
                // Uncover all other mines
                UncoverMines();
            }
            // ELSE IF there are no adjacent mines
            else if (adjacentMines == 0)
            {
                // Preform Flood Fill algorithm to reveal all empty blocks
                FFuncover(selectedBlock.x,selectedBlock.y,selectedBlock.z, new bool[width, height, depth]); 
            }

                

        }

        // Use this for initialization
        void Start()
        {
            GenerateBlocks();
        }

        // Update is called once per frame
        void Update()
        {
            // IF left mouse button is up
            if (Input.GetMouseButtonDown(0))
            {
                // IF raycast out from camera hits something
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit = new RaycastHit();
                if(Physics.Raycast(ray, out hit, 1000))
                {
                    // Get hit objects block component
                    Block hitBlock = hit.transform.GetComponent<Block>();
                    // CALL SelectBlock() and pass in the hit block
                    SelectBlock(hitBlock);
                }
            }
        }
    }
}
