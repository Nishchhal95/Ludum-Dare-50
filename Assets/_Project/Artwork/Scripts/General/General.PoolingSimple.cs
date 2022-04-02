using System;
using System.Collections.Generic;
using UnityEngine;

// TODO game over/ end of game back to start. currently only level end7 next level

namespace HappyFlowGames.General
{
    /// <summary>
    /// 2021 by Rafael Hernandez Pereira - Don't redistribute, please ask for permission if you want to use
    /// Maintains a list of gameobjects.
    /// - either with variants
    /// - or identical
    /// Instantiates more if needed.
    /// De-activates not needed ones.
    /// Usage:
    /// - Create instance of the class with the corresponding parameters
    /// - Call PreCreateUpToInitialAmount to prefill the pool
    /// - Call GetGameObjectAndActivate(...) to get random or specific Gameobject
    /// - Call DeActivateGameObject...(...) to put an object back into the pool
    /// - Call Helper methods as needed
    /// </summary>
	public class PoolingSimple
    {
        private string Name;

        private GameObject[] gameObjectTemplates; // The original game objects/ variants
        private Transform parentForGameObjects;

        private int numberOfGameObjectsCreatedAtOnce;
        private int initialAmount;

        private int numberOfVariants; // For better readibility of the code

        private List<GameObject>[] allGameObjects; // Keeps the generated gameobjects
        private List<GameObject>[] notActivatedGameObjects; // For fast access, the not activated ones
        private List<GameObject>[] activatedGameObjects; // For fast access, the activated ones

        #region Assertions
        /// <summary>
        /// Apply a check on all gameobject templates
        /// </summary>
        /// <param name="checkOfGameObject"></param>
        /// <returns>true if all checks are ok</returns>
        public bool AssertionsOnAllGameObjectTemplates (Func<GameObject, bool> checkOfGameObject)
        {
            bool totalCheck = true;

            foreach (GameObject template in gameObjectTemplates)
                totalCheck = checkOfGameObject(template) &&  totalCheck;

            return totalCheck;
        }
        #endregion Assertions

        #region Creation
        /// <summary>
        /// Creation with multiple variants
        /// </summary>
        /// <param name="gameObjectTemplates">These are the templates for instantiation</param>
        /// <param name="parent">Instantiated game objects will be parented to this</param>
        /// <param name="baseName">If null, then the template name is used as base. An index is added as suffix</param>
        /// <param name="numberOfGameObjectsCreatedAtOnce">If not enough exist then within one call (frame) this amount is created</param>
        /// <param name="initialAmount">Number of game objects which will be created initially</param>
        public PoolingSimple(GameObject[] gameObjectTemplates, Transform parent, string baseName, int numberOfGameObjectsCreatedAtOnce, int initialAmount)
        {
            creation(gameObjectTemplates, parent, baseName, numberOfGameObjectsCreatedAtOnce, initialAmount);
        }
        /// <summary>
        /// Creation with no variant
        /// </summary>
        /// <param name="gameObjectTemplates">These are the templates for instantiation</param>
        /// <param name="parent">Instantiated game objects will be parented to this</param>
        /// <param name="baseName">If null, then the template name is used as base. An index is added as suffix</param>
        /// <param name="numberOfGameObjectsCreatedAtOnce">If not enough exist then within one call (frame) this amount is created</param>
        /// <param name="initialAmount">Number of game objects which will be created initially</param>
        public PoolingSimple(GameObject gameObjectTemplate, Transform parent, string baseName, int numberOfGameObjectsCreatedAtOnce, int initialAmount)
        {
            GameObject[] gameObjectTemplates = new GameObject[1];
            gameObjectTemplates[0] = gameObjectTemplate;

            creation(gameObjectTemplates, parent, baseName, numberOfGameObjectsCreatedAtOnce, initialAmount);
        }
        /// <summary>
        /// Actual creation
        /// </summary>
        /// <param name="gameObjectTemplates">These are the templates for instantiation</param>
        /// <param name="parent">Instantiated game objects will be parented to this</param>
        /// <param name="baseName">If null, then the template name is used as base. An index is added as suffix</param>
        /// <param name="numberOfGameObjectsCreatedAtOnce">If not enough exist then within one call (frame) this amount is created</param>
        /// <param name="initialAmount">Number of game objects which will be created initially</param>
        private void creation(GameObject[] gameObjectTemplates, Transform parent, string baseName, int numberOfGameObjectsCreatedAtOnce, int initialAmount)
        {
            this.gameObjectTemplates = gameObjectTemplates;
            this.parentForGameObjects = parent;
            this.Name = baseName;

            this.numberOfGameObjectsCreatedAtOnce = numberOfGameObjectsCreatedAtOnce;
            this.initialAmount = initialAmount;

            this.numberOfVariants = gameObjectTemplates.Length;

            float[] x = new float[3];

            // Create all the lists
            allGameObjects = new List<GameObject>[numberOfVariants];
            notActivatedGameObjects = new List<GameObject>[numberOfVariants];
            activatedGameObjects = new List<GameObject>[numberOfVariants];
            for (int v = 0; v < numberOfVariants; v++)
            {
                allGameObjects[v] = new List<GameObject>();
                notActivatedGameObjects[v] = new List<GameObject>();
                activatedGameObjects[v] = new List<GameObject>();
            }
        }

        /// <summary>
        /// Each time it is called n Gameobject are created of each variant if total initial amount not reached yet.
        /// Usage: Call in update of calling class.
        /// </summary>
        /// <returns>Have all initial Gameobject been precreated</returns>
        public bool PreCreateUpToInitialAmount()
        {
            if (allGameObjects[0].Count >= initialAmount) return true; // All created already before calling this method

            createNGameObjectsButDontActivate();
            return allGameObjects[0].Count >= initialAmount; // Have all been created after call?
        }

        /// <summary>
        /// Pre-create further n gameObjects of each variant without activating
        /// </summary>
        private void createNGameObjectsButDontActivate()
        {
            for (int v = 0; v < numberOfVariants; v++)
                for (int i = 0; i < numberOfGameObjectsCreatedAtOnce; i++)
                {
                    createGameObjectButDontActivate(v);
                }
        }

        /// <summary>
        /// Each time it is called n Gameobject are created of each variant if total initial amount not reached yet.
        /// Usage: Call in update of calling class.
        /// </summary>
        /// <param name="theCreatedOnes">The just created ones for further processing, e.g. initialization</param>
        /// <returns>Have all initial Gameobject been precreated</returns>
        public bool PreCreateUpToInitialAmountAndReturn(out List<GameObject>[] theCreatedOnes)
        {
            if (allGameObjects[0].Count >= initialAmount)
            {
                theCreatedOnes = null;
                return true; // All created already before calling this method
            }

            theCreatedOnes = createNGameObjectsAndReturnButDontActivate();

            return allGameObjects[0].Count >= initialAmount; // Have all been created after call?
        }

        /// <summary>
        /// Pre-create further n gameObjects of each variant without activating
        /// </summary>
        /// <returns>The created ones</returns>
        private List<GameObject>[] createNGameObjectsAndReturnButDontActivate()
        {
            List<GameObject>[] theCreatedOnes = new List<GameObject>[numberOfVariants];

            for (int v = 0; v < numberOfVariants; v++)
            {
                theCreatedOnes[v] = new List<GameObject>();
                for (int i = 0; i < numberOfGameObjectsCreatedAtOnce; i++)
                {
                    theCreatedOnes[v].Add(createGameObjectButDontActivate(v));
                }
            }

            return theCreatedOnes;
        }

        /// <summary>
        /// Create a new game object for variantIndex, but do not activate it
        /// </summary>
        /// <returns></returns>
        private GameObject createGameObjectButDontActivate(int variantIndex)
        {
            GameObject gameObject = GameObject.Instantiate(gameObjectTemplates[variantIndex], parentForGameObjects);

            int id = allGameObjects[variantIndex].Count;
            if (Name == null)
                gameObject.name = gameObject.name.Substring(0, gameObject.name.Length - 7) + " v" + variantIndex.ToString() + " id" + id.ToString(); // Remove "(Clone)" suffix
            else
                gameObject.name = Name + " v" + variantIndex.ToString() + " id" + id.ToString();

            allGameObjects[variantIndex].Add(gameObject);
            notActivatedGameObjects[variantIndex].Add(gameObject);

            gameObject.SetActive(false);

            return gameObject;
        }
        #endregion Creation

        #region Getting
        /// <summary>
        /// Get a game object from the not activate ones.
        /// Create more, if no non activated exists/// </summary>
        /// <param name="variantIndex">-1 for random variant</param>
        /// <returns></returns>
        public GameObject GetGameObjectAndActivate(int variantIndex = -1)
        {
            return GetGameObjectAndActivate(ref variantIndex);
        }

        /// <summary>
        /// Get a game object from the not activate ones and return the variant index, in case it is needed.
        /// Create more, if no non activated exists/// </summary>
        /// <param name="variantIndex">-1 for random variant</param>
        /// <returns></returns>
        public GameObject GetGameObjectAndActivate(ref int variantIndex)
        {
            if (numberOfVariants == 1)
                variantIndex = 0;
            else if (variantIndex == -1)
                variantIndex = UnityEngine.Random.Range(0, numberOfVariants);

            if (notActivatedGameObjects[variantIndex].Count == 0)
                createGameObjectButDontActivate(variantIndex);

            GameObject retrievedGameObject = notActivatedGameObjects[variantIndex][0]; // Of variant the first element [0]
            notActivatedGameObjects[variantIndex].RemoveAt(0);

            activatedGameObjects[variantIndex].Add(retrievedGameObject);

            retrievedGameObject.SetActive(true);

            return retrievedGameObject;
        }
        #endregion Getting

        #region Putting Back/ De-activating

        /// <summary>
        /// Deactivates game object and keeps it for re-activation
        /// </summary>
        /// <param name="gameObjectToBeDeactivated"></param>
        /// <param name="variant">Either variant or -1 if not known</param>
        public void DeActivateGameObject(GameObject gameObjectToBeDeactivated, int variant = -1)
        {
            if (gameObjectToBeDeactivated == null) return;

            if (variant < 0)
            {
                variant = 0;
                if (numberOfVariants > 1)
                    for (variant = 0; variant < numberOfVariants; variant++) // Check which variant this is
                        if (activatedGameObjects[variant].Contains(gameObjectToBeDeactivated))
                            break;
                if (!DnT.Assert(variant < numberOfVariants, "General.PoolingSimple.DeActivateGameObject", "Gameobject {0} does not exist in list of activated gameobjects", gameObjectToBeDeactivated.name))
                    return;
            }

            // Reset rotation and scale TODO implement as well color reset by using a method to be called
            gameObjectToBeDeactivated.transform.rotation = Quaternion.identity;
            gameObjectToBeDeactivated.transform.localScale = new Vector3(1f, 1f, 1f);
            gameObjectToBeDeactivated.transform.SetParent(parentForGameObjects, false);
            //gameObjectToBeDeactivated.transform.parent = parentForGameObjects;
            gameObjectToBeDeactivated.transform.rotation = Quaternion.identity;
            gameObjectToBeDeactivated.transform.localScale = new Vector3(1f, 1f, 1f);

            activatedGameObjects[variant].Remove(gameObjectToBeDeactivated);
            notActivatedGameObjects[variant].Add(gameObjectToBeDeactivated);
            gameObjectToBeDeactivated.SetActive(false);
        }

        /// <summary>
        /// Deactivate all game objects, e.g. for game restart.
        /// </summary>
        public void DeActivateAll()
        {
            for (int v = 0; v < numberOfVariants; v++)
            {
                List<GameObject> tempAsRemovingFromListInLoop = new List<GameObject>();

                foreach (GameObject activatedGameObject in activatedGameObjects[v])
                    tempAsRemovingFromListInLoop.Add(activatedGameObject);

                foreach (GameObject activatedGameObject in tempAsRemovingFromListInLoop)
                    DeActivateGameObject(activatedGameObject);
            }
        }
        #endregion Putting Back/ De-activating

        #region Helper
        /// <summary>
        /// Is the gameobject from this pool
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public bool IsGameObjectFromThisPool(GameObject gameObject)
        {
            for (int v = 0; v < numberOfVariants; v++)
                if (allGameObjects[v].Contains(gameObject))
                    return true;

            return false;
        }

        /// <summary>
        /// Is the gameobject from the active list of this pool
        /// Note: This should be faster than IsGameObjectFromThisPool for activated gameobjects
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public bool IsGameObjectFromActiveListOfThisPool(GameObject gameObject, out int variantIndex)
        {
            for (int v = 0; v < numberOfVariants; v++)
                if (activatedGameObjects[v].Contains(gameObject))
                {
                    variantIndex = v;
                    return true;
                }

            variantIndex = -1;
            return false;
        }
        #endregion Helper

    }
}