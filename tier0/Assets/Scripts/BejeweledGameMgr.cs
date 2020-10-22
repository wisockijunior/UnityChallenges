using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SidiaUnityChallenge
{
    /// <summary>
    /// sample game manager - Bejeweled game
    /// </summary>
    public class BejeweledGameMgr
    {
        // private fields
        private bool gameRunning;
        private int[,] buffer;
        private GameObject[,] bufferGO;
        // ix, iy --> 0..5 (for now, dynamic in the future)
        private int bufferSizeX = 6;
        private int bufferSizeY = 6;

        // internal block move state
        private int moveBlock;
        private GameObject moveBlockGO;
        private Vector3 fromPosition;
        private int iFromX;
        private int iFromY;


        /// <summary>
        /// the game plane in 3D world space
        /// </summary>
        public GameObject GameMgrGo;

        /// <summary>
        /// new blocks starts here
        /// </summary>
        public GameObject PivotChessboardGO;


        /// <summary>
        /// reset the game state, start the game
        /// </summary>
        public void StartNewGame()
        {
            if (!gameRunning)
            {
                gameRunning = true;
                
                resetGame();
            }
        }

        public void StopGame()
        {
            if (gameRunning)
            {
                gameRunning = false;

                quitGame();
            }
        }

        /// <summary>
        /// Initial cleanup
        /// </summary>
        public void Reset()
        {
            // clear all blocks
            InternalClearGameBoard();
        }

        private void quitGame()
        {
            // clear all blocks
            InternalClearGameBoard();
        }

        private void InternalClearGameBoard()
        {
            while (PivotChessboardGO.transform.childCount > 0)
            {
                GameObject go;
                go = PivotChessboardGO.transform.GetChild(0).gameObject;

                GameObject.DestroyImmediate(go);
            }
        }

        /// <summary>
        /// reset the game, fill in the board 
        /// </summary>
        private void resetGame()
        {
            // for now, hardcoded, 5 blocks
            //int blockCount = 5;

            // clear all blocks
            InternalClearGameBoard();

            // new internal buffer, clear
            buffer = new int[bufferSizeX, bufferSizeY];
            bufferGO = new GameObject[bufferSizeX, bufferSizeY];
            // create new blocks
            for (int i = 0; i <= bufferSizeX - 1; i++)
                for (int j = 0; j <= bufferSizeY - 1; j++)
                {
                    // pick random block
                    //int blockIdx = 0;// UnityEngine.Random.Range(0, blockCount-1);//max inclusive
                    //if (blockIdx < 0 || blockIdx > PrefabsMgr.instance.PrefabBlocks.Length - 1)
                    //{
                    //    Debug.LogError("blockIdx out of bounds");
                    //}
                    //else
                    {
                        Vector3 newpos;
                        newpos.x = i;
                        newpos.y = j;
                        newpos.z = 0;

                        // future update: GameObject pooling

                        // Renato - 22/10/2020
                        // only one prefab, for now!
                        GameObject prefabGO = null;// PrefabsMgr.instance.PrefabBlocks[blockIdx];

                        //TODO: provide the game more than one block kind...!
                        // only one block prefab for now
                        prefabGO = PrefabsMgr.instance.blockPrefab;

                        GameObject newBlock;
                        newBlock = GameObject.Instantiate(prefabGO);
                        newBlock.transform.parent = PivotChessboardGO.transform;
                        newBlock.transform.localPosition = newpos;                        
                        newBlock.transform.localRotation = Quaternion.identity;
                        newBlock.transform.localScale = Vector3.one;

                        int matIndex = UnityEngine.Random.Range(0,PrefabsMgr.instance.blockMaterials.Length - 1);
                        Material blockMaterial = PrefabsMgr.instance.blockMaterials[matIndex];
                        newBlock.GetComponent<Renderer>().sharedMaterial = blockMaterial;

                        buffer[i, j] = matIndex;
                        bufferGO[i, j] = newBlock;
                    }
                }
        }

        public bool GetIsGameRunning()
        {
            return gameRunning;
        }

        /// <summary>
        /// handle game internal user interaction
        /// </summary>
        internal void HandleMouseClick()
        {
            if (gameRunning)
            {
                // check the mouse position, finger position on android
                Vector3 localPosition;

                // PC,editor - mouse interaction
                bool mouseDown = Input.GetMouseButton(0);
                bool mouseDownThisFrame = Input.GetMouseButtonDown(0);
                localPosition = Input.mousePosition;
                
                
                // Android, iOS
                if (Input.touchCount > 0)
                {
                    if (Input.GetTouch(0).phase == TouchPhase.Began)
                    {
                        mouseDownThisFrame = true;
                    }
                    if (Input.GetTouch(0).phase == TouchPhase.Moved)
                    {
                        mouseDown = true;
                    }

                    localPosition = Input.GetTouch(0).position;
                }

                // block move
                if (mouseDown)
                {
                    Ray ray;
                    ray = Camera.main.ScreenPointToRay(localPosition);

                    Vector3 fwd = Camera.main.transform.position;
                    Vector3 planePosition = GameMgrGo.transform.position;


                    Plane plane;
                    plane = new Plane(fwd, planePosition);

                    Vector3 worldPos;
                    if (plane.Raycast(ray, out float dist))
                    {
                        // check the raycast target position
                        worldPos = ray.origin + ray.direction * dist;

                        // convert world to local position
                        localPosition = GameMgrGo.transform.InverseTransformPoint(worldPos);


                        //Renato - 22/10/2020
                        // first bug correction --> mouse click pick the wrong block!

                        // get block index (x,y)
                        int ix = Mathf.RoundToInt(localPosition.x);
                        int iy = Mathf.RoundToInt(localPosition.y);
                        
                        if (ix >= 0 && ix <= bufferSizeX - 1 &&
                            iy >= 0 && iy <= bufferSizeY - 1)
                        {
                            int iBlock = buffer[ix, iy];
                                                        
                            //----------------------------------------------------------------
                            // Renato - 22/10/2020 - solved iBlock bug: on index 0 block type
                            //----------------------------------------------------------------
                            
                            if (iBlock > -1)
                            {
                                // block hit

                                //mouse down, start moving block
                                if (mouseDownThisFrame)
                                {
                                    Debug.Log("mouse click --> x:" + ix + " y:" + iy);
                                    

                                    moveBlock = iBlock;
                                    moveBlockGO = bufferGO[ix, iy];
                                    iFromX = ix;
                                    iFromY = iy;

                                    //debug - Bug Solved!
                                    //moveBlockGO.SetActive(false);

                                    // x,y start position
                                    fromPosition = localPosition;
                                }
                            }

                            if (moveBlock > -1)
                            {
                                // moving block
                                Vector3 targetPosition;

                                //target position
                                targetPosition = localPosition;

                                Vector3 deltaPos;
                                deltaPos = targetPosition - fromPosition;

                                //move block, in 2D space, snap to grid
                                deltaPos.x = (int)deltaPos.x;
                                deltaPos.y = (int)deltaPos.y;
                                deltaPos.z = 0;

                                if (deltaPos.x != 0 || deltaPos.y != 0)
                                {

                                    bool bUseAnimation = false;
                                    if (bUseAnimation)
                                    {
                                        // animate block move
                                    }
                                    else
                                    {
                                        //Renato - 22/10/2020
                                        // debug - check block moving accordingly

                                        if (moveBlockGO == null)
                                            Debug.LogError("game object not found");

                                        // check if game object exists
                                        if (moveBlockGO != null)
                                        {
                                            // for now, direct move
                                            Vector3 newPos;
                                            Vector3 fromPos;

                                            fromPos = moveBlockGO.transform.localPosition;
                                            newPos = moveBlockGO.transform.localPosition;
                                            newPos += deltaPos;


                                            // move constraints, 
                                            // do not allow user to move block 
                                            // out of the grid
                                            if (newPos.x < 0)
                                                newPos.x = 0;
                                            if (newPos.y < 0)
                                                newPos.y = 0;
                                            if (newPos.x > bufferSizeX - 1)
                                                newPos.x = bufferSizeX - 1;
                                            if (newPos.y > bufferSizeY - 1)
                                                newPos.y = bufferSizeY - 1;


                                            //debug
                                            //Debug.Log("from position:" + fromPos);
                                            //Debug.Log("to position:" + newPos);

                                            // move block to new position
                                            moveBlockGO.transform.localPosition = newPos;

                                            //---------------------------------
                                            // swap blocks on the new position
                                            //---------------------------------
                                            GameObject moveBlockGO2 = bufferGO[ix, iy];
                                            moveBlockGO2.transform.localPosition = fromPos;


                                            // update internal buffer

                                            // swap blocks
                                            // from position <--> target position
                                            #region swap

                                            int saveBlockIdx = buffer[ix, iy];
                                            GameObject saveBlock = bufferGO[ix, iy];

                                            // target position receive new block
                                            buffer[ix, iy] = iBlock;
                                            bufferGO[ix, iy] = moveBlockGO;

                                            // from position receive swapped block
                                            buffer[iFromX, iFromY] = saveBlockIdx;
                                            bufferGO[iFromX, iFromY] = saveBlock;

                                            Debug.LogWarning("swap");
                                            Debug.Log("from block:" + iFromX);// saveBlockIdx);
                                            Debug.Log("target block:" + ix);// iBlock);

                                            //debug
                                            //Debug.Log("from block:" + moveBlockGO.name, moveBlockGO);// saveBlockIdx);
                                            //if (saveBlock == null)
                                            //    Debug.Log("target block is null");// iBlock);
                                            //else
                                            //    Debug.Log("target block:" + saveBlock.name, saveBlock);// iBlock);

                                            #endregion
                                        }
                                    }

                                    // block moved to new grid position (snaped)
                                    // reset start position from the actual position
                                    fromPosition = targetPosition;
                                }

                            }
                        }

                    }
                }
            }
        }
    }
}