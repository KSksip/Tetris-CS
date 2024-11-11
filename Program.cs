using Raylib_cs;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq.Expressions;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using System.Timers;
using System.Runtime.Versioning;
using System.Drawing;
using static System.Net.Mime.MediaTypeNames;
using System.Runtime.CompilerServices;



namespace Tetris
{
    class Program
    {

        public static void Main()
        {

            //general vars to be used
            bool gameActivity = false;

            const int blockSide = 20;

            int rotation = 0;


            int curBlockNumCols = 0;
            int curBlockNumRows = 0;

            int blockYPos = 0;
            int blockXPos = 5 - 1;

            int windowWidth = 10 * blockSide + 1 + 200;
            int windowHeight = 24 * blockSide + 1;


            int points = 0;




            //timings

            double fallUpdate = 0;
            double fastFallUpdate = 0;
            double moveLeftUpdate = 0;
            double moveRightUpdate = 0;


            //update cooldown to make game speed playable
            bool eventTriggered(double interval, ref double lastUpdate)
            {
                double currentTime = Raylib.GetTime();
                if (currentTime - lastUpdate >= interval)
                {
                    lastUpdate = currentTime;
                    return true;
                }
                return false;

            }

            //grid stuff
            //block colors
            Raylib_cs.Color[] blockColor = { Raylib_cs.Color.RayWhite, Raylib_cs.Color.Green, Raylib_cs.Color.SkyBlue, Raylib_cs.Color.Blue, Raylib_cs.Color.Orange, Raylib_cs.Color.Yellow, Raylib_cs.Color.Violet, Raylib_cs.Color.Red };

            int numRows = 24;
            int numCols = 10;

            int[,] mainGrid = new int[numRows, numCols];
            int[,] ghostGrid = new int[numRows, numCols];
            int[,] emptyTestGrid = new int[numRows, numCols];

            int cellSize = blockSide;


            //clears selected grid from params
            void initializeGrid(ref int[,] grid)
            {
                for (int row = 0; row < numRows; row++)
                {
                    for (int col = 0; col < numCols; col++)
                    {
                        grid[row, col] = 0;
                        emptyTestGrid[row, col] = 0;
                    }
                }
            }


            //renders the empty grid
            void renderGrid()
            {

                for (int row = 0; row < numRows; row++)
                {
                    for (int col = 0; col < numCols; col++)
                    {
                        int cellValue = mainGrid[row, col];
                        Raylib.DrawRectangle(col * cellSize + 1, row * cellSize + 1, cellSize - 1, cellSize - 1, blockColor[cellValue]);
                    }
                }
            }









            //colors
            Raylib_cs.Color startBtn = Raylib_cs.Color.DarkGray;


            //blocks
            Random randint = new Random();



            void fixBlockArray(ref int[,] block, int rows, int cols)
            {
                for (int i = 0; i < rows; i++)
                {
                    for (int j = 0; j < cols; j++)
                    {
                        block[i, j] = 0;
                    }
                }
            }




            int randomBlockId = randint.Next(1, 8);


            //randomizes block to be used when a block has been placed and on game start
            void randomizeBlock()
            {
                int prevRandomBlockId = randomBlockId;

                randomBlockId = randint.Next(1, 8);

                while (randomBlockId == prevRandomBlockId)
                {
                    randomBlockId = randint.Next(1, 8);
                }

            }


            Vector2 curRotationDisplacement = new Vector2(0, 0);

            //all the block arrays
            int[,] blockGen(int rotationParam, ref Vector2 displacement)
            {

                if (randomBlockId == 1) { return lBlockGen(rotationParam, ref displacement); }

                if (randomBlockId == 2) { return revLBlockGen(rotationParam, ref displacement); }

                if (randomBlockId == 3) { return squareBlockGen(rotationParam, ref displacement); }

                if (randomBlockId == 4) { return tBlockGen(rotationParam, ref displacement); }

                if (randomBlockId == 5) { return zBlockGen(rotationParam, ref displacement); }

                if (randomBlockId == 6) { return revZBlockGen(rotationParam, ref displacement); }

                return iBlockGen(rotationParam, ref displacement);

            }

            int[,] lBlockGen(int rotationParam, ref Vector2 displacement)
            {
                int[,] lBlock = new int[0, 0];

                int color = 3;

                if (rotationParam == 0)
                {
                    curBlockNumCols = 2;
                    curBlockNumRows = 3;

                    lBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref lBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(0, 0);

                    lBlock[0, 0] = color;
                    lBlock[0, 1] = color;
                    lBlock[1, 1] = color;
                    lBlock[2, 1] = color;
                }

                if (rotationParam == 1)
                {
                    curBlockNumCols = 3;
                    curBlockNumRows = 2;

                    lBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref lBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(0, 1);

                    lBlock[0, 0] = color;
                    lBlock[0, 1] = color;
                    lBlock[0, 2] = color;
                    lBlock[1, 0] = color;
                }

                if (rotationParam == 2)
                {
                    curBlockNumCols = 2;
                    curBlockNumRows = 3;

                    lBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref lBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(1, -1);

                    lBlock[0, 0] = color;
                    lBlock[1, 0] = color;
                    lBlock[2, 1] = color;
                    lBlock[2, 0] = color;
                }

                if (rotationParam == 3)
                {
                    curBlockNumCols = 3;
                    curBlockNumRows = 2;

                    lBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref lBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(-1, 0);

                    lBlock[0, 2] = color;
                    lBlock[1, 0] = color;
                    lBlock[1, 1] = color;
                    lBlock[1, 2] = color;
                }

                return lBlock;
            }

            int[,] revLBlockGen(int rotationParam, ref Vector2 displacement)
            {
                int[,] revLBlock = new int[0, 0];

                int color = 4;

                if (rotationParam == 0)
                {
                    curBlockNumCols = 2;
                    curBlockNumRows = 3;

                    revLBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref revLBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(0, 0);

                    revLBlock[0, 1] = color;
                    revLBlock[1, 1] = color;
                    revLBlock[2, 0] = color;
                    revLBlock[2, 1] = color;
                }

                if (rotationParam == 1)
                {
                    curBlockNumCols = 3;
                    curBlockNumRows = 2;

                    revLBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref revLBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(0, 1);

                    revLBlock[0, 0] = color;
                    revLBlock[0, 1] = color;
                    revLBlock[0, 2] = color;
                    revLBlock[1, 2] = color;
                }

                if (rotationParam == 2)
                {
                    curBlockNumCols = 2;
                    curBlockNumRows = 3;

                    revLBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref revLBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(1, -1);

                    revLBlock[0, 1] = color;
                    revLBlock[0, 0] = color;
                    revLBlock[1, 0] = color;
                    revLBlock[2, 0] = color;
                }

                if (rotationParam == 3)
                {
                    curBlockNumCols = 3;
                    curBlockNumRows = 2;

                    revLBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref revLBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(-1, 0);

                    revLBlock[0, 0] = color;
                    revLBlock[1, 0] = color;
                    revLBlock[1, 1] = color;
                    revLBlock[1, 2] = color;
                }

                return revLBlock;
            }

            int[,] squareBlockGen(int rotationParam, ref Vector2 displacement)
            {
                int[,] revLBlock = new int[2, 2];
                curBlockNumCols = 2;
                curBlockNumRows = 2;

                displacement = new Vector2(0, 0);

                int color = 5;

                if (rotationParam >= 0) { revLBlock[0, 0] = color; revLBlock[0, 1] = color; revLBlock[1, 0] = color; revLBlock[1, 1] = color; }

                return revLBlock;
            }

            int[,] tBlockGen(int rotationParam, ref Vector2 displacement)
            {
                int[,] tBlock = new int[0, 0];

                if (rotationParam == 0)
                {
                    curBlockNumCols = 2;
                    curBlockNumRows = 3;

                    tBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref tBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(0, 0);

                    tBlock[0, 1] = 1;
                    tBlock[1, 0] = 1;
                    tBlock[1, 1] = 1;
                    tBlock[2, 1] = 1;
                }

                if (rotationParam == 1)
                {
                    curBlockNumCols = 3;
                    curBlockNumRows = 2;

                    tBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref tBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(0, 1);

                    tBlock[0, 0] = 1;
                    tBlock[0, 1] = 1;
                    tBlock[0, 2] = 1;
                    tBlock[1, 1] = 1;
                }

                if (rotationParam == 2)
                {
                    curBlockNumCols = 2;
                    curBlockNumRows = 3;

                    tBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref tBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(1, -1);

                    tBlock[0, 0] = 1;
                    tBlock[1, 0] = 1;
                    tBlock[1, 1] = 1;
                    tBlock[2, 0] = 1;
                }

                if (rotationParam == 3)
                {
                    curBlockNumCols = 3;
                    curBlockNumRows = 2;

                    tBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref tBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(-1, 0);

                    tBlock[0, 1] = 1;
                    tBlock[1, 0] = 1;
                    tBlock[1, 1] = 1;
                    tBlock[1, 2] = 1;
                }

                return tBlock;
            }

            int[,] zBlockGen(int rotationParam, ref Vector2 displacement)
            {
                int[,] zBlock = new int[3, 3];

                int color = 6;

                if (rotationParam == 0)
                {
                    curBlockNumCols = 2;
                    curBlockNumRows = 3;

                    zBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref zBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(0, 0);

                    zBlock[0, 1] = color;
                    zBlock[1, 0] = color;
                    zBlock[1, 1] = color;
                    zBlock[2, 0] = color;
                }

                if (rotationParam == 1)
                {
                    curBlockNumCols = 3;
                    curBlockNumRows = 2;

                    zBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref zBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(0, 1);

                    zBlock[0, 0] = color;
                    zBlock[0, 1] = color;
                    zBlock[1, 1] = color;
                    zBlock[1, 2] = color;
                }

                if (rotationParam == 2)
                {
                    curBlockNumCols = 2;
                    curBlockNumRows = 3;

                    zBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref zBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(1, -1);

                    zBlock[0, 1] = color;
                    zBlock[1, 0] = color;
                    zBlock[1, 1] = color;
                    zBlock[2, 0] = color;
                }

                if (rotationParam == 3)
                {
                    curBlockNumCols = 3;
                    curBlockNumRows = 2;

                    zBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref zBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(-1, 0);

                    zBlock[0, 0] = color;
                    zBlock[0, 1] = color;
                    zBlock[1, 1] = color;
                    zBlock[1, 2] = color;
                }

                return zBlock;
            }

            int[,] revZBlockGen(int rotationParam, ref Vector2 displacement)
            {
                int[,] revZBlock = new int[3, 3];

                int color = 7;

                if (rotationParam == 0)
                {
                    curBlockNumCols = 2;
                    curBlockNumRows = 3;

                    revZBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref revZBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(0, 0);

                    revZBlock[0, 0] = color;
                    revZBlock[1, 0] = color;
                    revZBlock[1, 1] = color;
                    revZBlock[2, 1] = color;
                }

                if (rotationParam == 1)
                {
                    curBlockNumCols = 3;
                    curBlockNumRows = 2;

                    revZBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref revZBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(0, 1);

                    revZBlock[0, 1] = color;
                    revZBlock[0, 2] = color;
                    revZBlock[1, 0] = color;
                    revZBlock[1, 1] = color;
                }

                if (rotationParam == 2)
                {
                    curBlockNumCols = 2;
                    curBlockNumRows = 3;

                    revZBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref revZBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(1, -1);

                    revZBlock[0, 0] = color;
                    revZBlock[1, 0] = color;
                    revZBlock[1, 1] = color;
                    revZBlock[2, 1] = color;
                }

                if (rotationParam == 3)
                {
                    curBlockNumCols = 3;
                    curBlockNumRows = 2;

                    revZBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref revZBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(-1, 0);

                    revZBlock[0, 1] = color;
                    revZBlock[0, 2] = color;
                    revZBlock[1, 0] = color;
                    revZBlock[1, 1] = color;
                }

                return revZBlock;
            }

            int[,] iBlockGen(int rotationParam, ref Vector2 displacement)
            {
                int[,] iBlock = new int[4, 4];

                if (rotationParam == 0 || rotationParam == 2)
                {
                    curBlockNumCols = 1;
                    curBlockNumRows = 4;

                    iBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref iBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(2, -1);

                    iBlock[0, 0] = 2;
                    iBlock[1, 0] = 2;
                    iBlock[2, 0] = 2;
                    iBlock[3, 0] = 2;
                }

                if (rotationParam == 1 || rotationParam == 3)
                {
                    curBlockNumCols = 4;
                    curBlockNumRows = 1;

                    iBlock = new int[curBlockNumRows, curBlockNumCols];

                    fixBlockArray(ref iBlock, curBlockNumRows, curBlockNumCols);

                    displacement = new Vector2(-2, 2);

                    iBlock[0, 0] = 2;
                    iBlock[0, 1] = 2;
                    iBlock[0, 2] = 2;
                    iBlock[0, 3] = 2;
                }

                return iBlock;
            }
            //end of all the block arrays


            bool isCollidingDown = false;
            bool isCollidingRight = false;
            bool isCollidingLeft = false;


            //moves block back inbound to avoid rendering outside of designated array crash
            void moveBlock()
            {
                if (blockYPos + curBlockNumRows > numRows)
                {
                    blockYPos -= 1;

                    isCollidingDown = true;
                }

                else
                {
                    while (blockXPos < 0)
                    {
                        blockXPos += 1;
                        isCollidingLeft = true;
                    }

                    while (blockXPos + curBlockNumCols > numCols)
                    {
                        blockXPos -= 1;
                        isCollidingRight = true;
                    }
                }

            }


            //refreshes the main grid (removes everything in the empty grid to avoid ghosting)
            void refreshMainGrid()
            {
                initializeGrid(ref mainGrid);
            }



            //renders the falling block to the display grid along with the ghost grid (it renders everything to the empty diplay grid basically)
            void renderFallingBlockToGrid(int xPos, int[,] block, int[,] grid)
            {
                while (blockXPos + curBlockNumCols > numCols)
                {

                    blockXPos -= 1;
                }

                for (int i = 0; i < curBlockNumRows; i++)
                {
                    for (int j = 0; j < curBlockNumCols; j++)
                    {
                        if (block[i, j] != 0)
                        {
                            grid[i + blockYPos, j + xPos] = block[i, j];
                        }
                    }
                }
                renderGhostGrid();
            }

            // save placed blocks
            void savePlacedToGhostGrid()
            {
                renderFallingBlockToGrid(blockXPos, blockGen(rotation, ref curRotationDisplacement), ghostGrid);
            }

            void renderGhostGrid()
            {
                for (int i = 0; i < numRows; i++)
                {
                    for (int j = 0; j < numCols; j++)
                    {
                        if (ghostGrid[i, j] != 0)
                        {
                            mainGrid[i, j] = ghostGrid[i, j];
                        }
                    }
                }
            }


            //checks if block is colliding and places it if so and generates a new block
            void collisionCheck()
            {
                if (isCollidingDown)
                {
                    savePlacedToGhostGrid();
                    newBlock();
                }
            }

            bool rotationLock = false;



            //checks collisions
            void checkBlockOnBlockCollisions(int xPos, int[,] block, int[,] grid)
            {
                moveBlock();
                for (int i = 0; i < curBlockNumRows; i++)
                {
                    // down collision for horizontal iblock due to iblock being 1x4 instead of 2x3
                    if (blockYPos + curBlockNumRows < numRows - 1 && blockXPos >= 0)
                    {
                        if (rotation == 1 || rotation == 3)
                        {
                            if (randomBlockId == 7 && grid[i + blockYPos + 2, 2 + xPos] != 0)
                            {
                                rotationLock = true;

                            }
                        }
                    }

                    for (int j = 0; j < curBlockNumCols; j++)
                    {

                        if (block[i, j] > 0)
                        {
                            //down collision
                            if (blockYPos + curBlockNumRows == numRows)
                            {
                                isCollidingDown = true;


                            }

                            if (blockYPos + curBlockNumRows < numRows && blockXPos >= 0)
                            {
                                if (grid[i + blockYPos + 1, j + xPos] != 0)
                                {
                                    isCollidingDown = true;
                                }
                            }



                            //collision right

                            if (blockXPos + curBlockNumCols < numCols)
                            {

                                if (grid[i + blockYPos, j + xPos + 1] != 0)
                                {
                                    isCollidingRight = true;

                                }
                            }




                            //collision left
                            if (blockXPos > 0)
                            {
                                if (grid[i + blockYPos, j + xPos - 1] != 0)
                                {
                                    isCollidingLeft = true;

                                }
                            }
                        }
                    }
                }
            }

            int hiscore = 0;


            //checks if game is ended
            void isGameOver()
            {
                if (ghostGrid[1, 6] != 0 || ghostGrid[1, 5] != 0)
                {
                    gameActivity = false;

                    if (points > hiscore)
                    {
                        hiscore = points;
                    }
                    points = 0;
                }
            }


            //resets collision statuses so that the vars can be reused in the next cycle
            void resetCollisionStatuses()
            {
                isCollidingDown = false;
                isCollidingLeft = false;
                isCollidingRight = false;
                rotationLock = false;
            }

            int colCheckCount;



            //checks for tetrises and individually completed rows and clears them on top of adding points
            void checkCompletedRows()
            {
                int clearedRows = 0;

                for (int row = 0; row < numRows; row++)
                {
                    colCheckCount = 0;

                    for (int col = 0; col < numCols; col++)
                    {
                        if (ghostGrid[row, col] != 0)
                        {
                            colCheckCount++;
                        }
                    }

                    if (colCheckCount == 10)
                    {
                        clearedRows++;

                        for (int col2 = 0; col2 < numCols; col2++)
                        {
                            ghostGrid[row, col2] = 0;
                        }

                        for (int row2 = row; row2 > 0; row2--)
                        {
                            for (int col = 0; col < numCols; col++)
                            {
                                ghostGrid[row2, col] = ghostGrid[row2 - 1, col];
                            }
                        }
                    }
                }


                //checks if tetris or not
                if (clearedRows <= 3)
                {
                    points += clearedRows * 1000;
                }

                if (clearedRows == 4)
                {
                    points += 10000;
                }

                level();
            }

            double fallSpeed = 0.75;
            int numLevel = 1;



            //checks level and increases difficulty when reached also changes text displaying what level
            void level()
            {
                if (points > 50000)
                {
                    numLevel = 2;
                    fallSpeed = 0.50;
                }

                if (points > 100000)
                {
                    numLevel = 3;
                    fallSpeed = 0.30;
                }

                if (points > 200000)
                {
                    numLevel = 4;
                    fallSpeed = 0.10;
                }
            }



            //choses new block and resets position
            void newBlock()
            {
                if (isCollidingDown)
                {
                    blockYPos = 0;
                    blockXPos = 5 - curBlockNumCols / 2;
                    randomizeBlock();

                }
            }




            //rotate block method
            void rotate()
            {
                if (Raylib.IsKeyPressed(KeyboardKey.W) == true)
                {
                    checkBlockOnBlockCollisions(blockXPos, blockGen(rotation, ref curRotationDisplacement), ghostGrid);

                    if (isCollidingLeft != true && isCollidingRight != true && isCollidingDown != true && blockYPos != 0 && rotationLock == false)
                    {
                        if (rotation < 3) { rotation += 1; }
                        else { rotation = 0; }

                        blockGen(rotation, ref curRotationDisplacement);

                        blockXPos += (int)curRotationDisplacement.X;
                        blockYPos += (int)curRotationDisplacement.Y;

                        moveBlock();
                    }
                    isCollidingDown = false;
                }
            }


            //left block movement
            void moveBlockLeft()
            {
                if (Raylib.IsKeyDown(KeyboardKey.A) == true)
                {

                    checkBlockOnBlockCollisions(blockXPos, blockGen(rotation, ref curRotationDisplacement), ghostGrid);

                    if (isCollidingLeft == false && blockXPos > 0 && blockXPos < numCols)
                    {
                        blockXPos -= 1;
                    }
                }
            }



            //right block movement
            void moveBlockRight()
            {
                if (Raylib.IsKeyDown(KeyboardKey.D) == true)
                {
                    checkBlockOnBlockCollisions(blockXPos, blockGen(rotation, ref curRotationDisplacement), ghostGrid);

                    if (isCollidingRight == false && blockXPos + curBlockNumCols != numCols)
                    {
                        blockXPos += 1;
                    }
                }
            }



            //block fastfall
            void blockSpeed()
            {
                if (Raylib.IsKeyDown(KeyboardKey.S) == true)
                {
                    fallUpdate = Raylib.GetTime();
                    checkBlockOnBlockCollisions(blockXPos, blockGen(rotation, ref curRotationDisplacement), ghostGrid);

                    if (isCollidingDown == false && blockYPos + curBlockNumRows != numRows)
                    {
                        blockYPos += 1;
                    }
                }
            }



            //game pause check
            bool isGamePaused = false;
            void checkGamePause()
            {
                if (Raylib.IsKeyPressed(KeyboardKey.Escape) == true)
                {
                    if (isGamePaused == true)
                    {
                        isGamePaused = false;
                        return;
                    }
                    isGamePaused = true;
                    return;
                }
            }

            //game quit stuff
            bool gameQuitMenu = false;

            void gameQuit()
            {

                if (Raylib.IsKeyPressed(KeyboardKey.Q) == true)
                {
                    gameQuitMenu = true;

                }

                if (gameQuitMenu == true)
                {
                    Raylib.DrawText("Are you sure?", centerObjX(Raylib.MeasureText("Are you sure?", 32)), centerObjY(32) - 20, 32, Raylib_cs.Color.Black);
                    Raylib.DrawText("(Yes) Y/N (No)", centerObjX(Raylib.MeasureText("(Yes) Y/N (No)", 20)), centerObjY(20) + 15, 20, Raylib_cs.Color.DarkGray);
                }

                if (Raylib.IsKeyPressed(KeyboardKey.Y) == true)
                {
                    gameQuitMenu = false;
                    isGamePaused = false;
                    gameActivity = false;

                    if (points > hiscore)
                    {
                        hiscore = points;
                    }
                    points = 0;
                }

                if (Raylib.IsKeyPressed(KeyboardKey.N) == true)
                {
                    gameQuitMenu = false;
                }
            }


            //methods for centering position of items being rendered
            int centerObjX(int objWidth)
            {
                return windowWidth / 2 - objWidth / 2;
            }
            int centerObjY(int objHeight)
            {
                return windowHeight / 2 - objHeight / 2;
            }






            //checks mouse position for buttons
            Vector2 checkMousePos()
            {
                Vector2 mousePos = Raylib.GetMousePosition();
                return mousePos;
            }



            //onclick and hover for buttons
            bool isMouseOver(int startX, int startY, int endX, int endY)
            {
                Vector2 startPoint = new Vector2(startX, startY);
                Vector2 endPoint = new Vector2(endX, endY);

                if (checkMousePos().X > startPoint.X && checkMousePos().Y > startPoint.Y && checkMousePos().X < endPoint.X && checkMousePos().Y < endPoint.Y)
                {
                    Cursor.Current = Cursors.Hand;
                    return true;
                }
                else
                {
                    Cursor.Current = Cursors.Default;
                    return false;
                }
            }

            Raylib.InitWindow(windowWidth, windowHeight, "Tetris Clone");
            Raylib.SetTargetFPS(60);


            while (!Raylib.WindowShouldClose())
            {
                int fps = Raylib.GetFPS();

                Raylib.SetExitKey(KeyboardKey.Null);

                checkMousePos();

                Raylib.BeginDrawing();
                Raylib.ClearBackground(Raylib_cs.Color.RayWhite);

                Raylib.DrawText(fps.ToString(), windowWidth - 20, windowHeight - 20, 10, Raylib_cs.Color.Black);


                //main menu
                if (gameActivity == false)
                {
                    //show hiscore
                    Raylib.DrawText("HiScore: " + hiscore, centerObjX(Raylib.MeasureText("HiScore" + hiscore.ToString(), 30)), 30, 30, Raylib_cs.Color.Black);

                    //title
                    Raylib.DrawText("Basically Tetris", centerObjX(Raylib.MeasureText("Basically Tetris", 40)), 80, 40, Raylib_cs.Color.Black);


                    //start button main menu
                    Raylib.DrawRectangle(centerObjX(100), centerObjY(40) - 70, 100, 40, startBtn);
                    Raylib.DrawText("Start", centerObjX(Raylib.MeasureText("Start", 32)), centerObjY(32) - 70, 32, Raylib_cs.Color.RayWhite);

                    if (isMouseOver(centerObjX(100), centerObjY(40) - 72, centerObjX(90) + 100, centerObjY(40) - 30) == true)
                    {
                        startBtn = Raylib_cs.Color.DarkGray;

                        //onclick event
                        if (Raylib.IsMouseButtonPressed(MouseButton.Left) == true)
                        {
                            //resets everything
                            gameActivity = true;
                            fallSpeed = 0.75;
                            numLevel = 1;
                            blockYPos = 0;
                            initializeGrid(ref mainGrid);
                            initializeGrid(ref ghostGrid);
                            randomizeBlock();
                            resetCollisionStatuses();

                            //dont tick down immediately
                            eventTriggered(fallSpeed, ref fallUpdate);
                        }
                    }
                    else
                    {
                        startBtn = Raylib_cs.Color.Black;
                    }
                }


                //the game
                if (gameActivity == true)
                {

                    //renders game text and info
                    Raylib.DrawText("Score: " + points, 10 * blockSide + 11, 16, 18, Raylib_cs.Color.Black);
                    Raylib.DrawText("Level: " + numLevel, 10 * blockSide + 11, 34, 18, Raylib_cs.Color.Black);


                    Raylib.DrawText("[W] rotate", 10 * blockSide + 11, windowHeight - 120, 18, Raylib_cs.Color.DarkGray);
                    Raylib.DrawText("[A] left", 10 * blockSide + 11, windowHeight - 100, 18, Raylib_cs.Color.DarkGray);
                    Raylib.DrawText("[D] right", 10 * blockSide + 11, windowHeight - 80, 18, Raylib_cs.Color.DarkGray);
                    Raylib.DrawText("[S] fast drop", 10 * blockSide + 11, windowHeight - 60, 18, Raylib_cs.Color.DarkGray);


                    Raylib.DrawText("[Esc] to pause", 10 * blockSide + 11, windowHeight - 20, 18, Raylib_cs.Color.DarkGray);




                    refreshMainGrid();

                    if (isGamePaused == false)
                    {
                        rotate();


                        if (eventTriggered(fallSpeed, ref fallUpdate))
                        {
                            checkBlockOnBlockCollisions(blockXPos, blockGen(rotation, ref curRotationDisplacement), ghostGrid);

                            if (isCollidingDown == false && curBlockNumRows != numRows)
                            {
                                blockYPos += 1;
                            }
                        }

                        if (eventTriggered(0.03, ref fastFallUpdate))
                        {
                            blockSpeed();

                        }

                        collisionCheck();


                        if (eventTriggered(0.06, ref moveLeftUpdate))
                        {
                            moveBlockLeft();
                        }

                        if (eventTriggered(0.06, ref moveRightUpdate))
                        {
                            moveBlockRight();
                        }
                    }


                    renderFallingBlockToGrid(blockXPos, blockGen(rotation, ref curRotationDisplacement), mainGrid);

                    Raylib.DrawRectangle(0, 0, numCols * 20 + 1, windowHeight, Raylib_cs.Color.Black);

                    renderGrid();
                    resetCollisionStatuses();
                    checkCompletedRows();
                    isGameOver();

                    checkGamePause();



                    //pause menu
                    if (isGamePaused)
                    {
                        Raylib.DrawRectangle(0, centerObjY(160), windowWidth, 160, Raylib_cs.Color.Black);
                        Raylib.DrawRectangle(0, centerObjY(150), windowWidth, 150, Raylib_cs.Color.White);


                        //quit menu
                        if (gameQuitMenu == false)
                        {
                            Raylib.DrawText("Game Paused!", centerObjX(Raylib.MeasureText("Game Paused!", 32)), centerObjY(32) - 30, 32, Raylib_cs.Color.Black);
                            Raylib.DrawText("Press [Esc] to resume.", centerObjX(Raylib.MeasureText("Press [esc] to resume.", 20)), centerObjY(20), 20, Raylib_cs.Color.Black);
                            Raylib.DrawText("Press [Q] to quit.", centerObjX(Raylib.MeasureText("Press [Q] to quit.", 20)), centerObjY(20) + 25, 20, Raylib_cs.Color.DarkGray);
                        }

                        gameQuit();

                    }
                }
                Raylib.EndDrawing();
            }
            Raylib.CloseWindow();
        }
    }
}