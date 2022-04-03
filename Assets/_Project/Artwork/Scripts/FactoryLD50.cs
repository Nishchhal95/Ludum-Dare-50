using HappyFlowGames.General;
using System;
using UnityEngine;

namespace LD50
{
    /// <summary>
    /// Creates, manages, provides entities needed by game.
    /// Requires HappyFlowGames.General.PoolingSimple, HappyFlowGames.General.GeneralObjectCompiled and HappyFlowGames.General.GeneralObjectCompiler
    /// </summary>
    public class FactoryLD50 : MonoBehaviour
    {
        [Header("Base")]
        public int PoolingNumberOfGameObjectsCreatedAtOnce;
        public int PoolingInitialAmount;

        // Good
        [NonSerialized]
        public PoolingSimple PoolGood;
        [Header("Good")]
        public Transform GoodParent;
        public GameObject[] GoodTemplates;

        // Bad
        [NonSerialized]
        public PoolingSimple PoolBad;
        [Header("Bad")]
        public Transform BadParent;
        public GameObject[] BadTemplates;

        // Platform
        [NonSerialized]
        public PoolingSimple PoolPlatform;
        [Header("Platform")]
        public Transform PlatformParent;
        public GameObject [] PlatformTemplates;

        // Misc
        public enum MiscType
        {
            Start = 0,
            Exit = 1
        }
        [NonSerialized]
        public PoolingSimple PoolMisc;
        [Header("Misc")]
        public Transform MiscParent;
        public GameObject[] MiscTemplates;


        // General
        bool allGameObjectsPrecreated;

        #region Init
        void Awake()
        {
            PoolGood = new PoolingSimple(GoodTemplates, GoodParent, "Good", PoolingNumberOfGameObjectsCreatedAtOnce, PoolingInitialAmount);
            PoolBad = new PoolingSimple(BadTemplates, BadParent, "Bad", PoolingNumberOfGameObjectsCreatedAtOnce, PoolingInitialAmount);
            PoolPlatform = new PoolingSimple(PlatformTemplates, PlatformParent, "Platform", PoolingNumberOfGameObjectsCreatedAtOnce, PoolingInitialAmount);
            PoolMisc = new PoolingSimple(MiscTemplates, MiscParent, "Misc", PoolingNumberOfGameObjectsCreatedAtOnce, PoolingInitialAmount);
        }

        /// <summary>
        /// Initialization to be called until returns true.
        /// Fills the gameobject pool in order not to create before game starts.
        /// </summary>
        /// <returns>true when done</returns>
        public bool PreCreateGameObjects()
        {
            bool initalizationCompleted = true; // Using && below ensures that only true if all are true

            initalizationCompleted = initalizationCompleted && PoolGood.PreCreateUpToInitialAmount();
            initalizationCompleted = initalizationCompleted && PoolBad.PreCreateUpToInitialAmount();
            initalizationCompleted = initalizationCompleted && PoolPlatform.PreCreateUpToInitialAmount();
            initalizationCompleted = initalizationCompleted && PoolMisc.PreCreateUpToInitialAmount();

            return initalizationCompleted;
        }

        /// <summary>
        /// Puts back everything
        /// </summary>
        public void PutBackEverything()
        {
            PutBackAllBad();
            PutBackAllGood();
            PutBackAllPlatform();
            PutBackAllMisc();
        }
        #endregion Init

        #region Get
        // Good
        public GameObject GetGood()
        {
            return PoolGood.GetGameObjectAndActivate();
        }

        // Bad
        public GameObject GetBad()
        {
            return PoolBad.GetGameObjectAndActivate();
        }

        // Platform
        public GameObject GetPlatform()
        {
            return PoolPlatform.GetGameObjectAndActivate();

        }

        // Misc
        public GameObject GetMisc(MiscType misctype)
        {
            return PoolMisc.GetGameObjectAndActivate((int)misctype);
        }
        #endregion Get

        #region PutBack
        // Good
        public void PutBackGood(GameObject good)
        {
            PoolGood.DeActivateGameObject(good);
        }
        public void PutBackAllGood()
        {
            PoolGood.DeActivateAll();
        }

        // Bad
        public void PutBackBad(GameObject Bad)
        {
            PoolBad.DeActivateGameObject(Bad);
        }
        public void PutBackAllBad()
        {
            PoolBad.DeActivateAll();
        }

        // Platform
        public void PutBackTilePlatform(GameObject Platform)
        {
            PoolPlatform.DeActivateGameObject(Platform);
        }

        public void PutBackAllPlatform()
        {
            PoolPlatform.DeActivateAll();
        }

        // Misc
        public void PutBackMisc(GameObject Misc)
        {
            PoolMisc.DeActivateGameObject(Misc);
        }

        public void PutBackAllMisc()
        {
            PoolMisc.DeActivateAll();
        }
        #endregion PutBack

        #region Modify
        /// <summary>
        /// Rotate random 90 plus a bit of noise
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="noise"></param>
        private float rotateRandomZ90(GameObject gameObject, float noise = 0f)
        {
            float rotationNoise = UnityEngine.Random.Range(-noise, noise);
            float rotation = (90f + rotationNoise) * UnityEngine.Random.Range(0, 4);
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
            return rotationNoise;
        }

        /// <summary>
        /// Rotate by a fixed amount around Z
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="rotation"></param>
        private void rotateFixedZ(GameObject gameObject, float rotation)
        {
            gameObject.transform.rotation = Quaternion.Euler(0f, 0f, rotation);
        }

        /// <summary>
        /// For a gameobject get sprite renderer and set color
        /// </summary>
        /// <param name="gameObjectWithSprite"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public SpriteRenderer SetColorSpriteGO(GameObject gameObjectWithSprite, Color color)
        {
            if (gameObjectWithSprite == null) return null;
            SpriteRenderer spriteRenderer = gameObjectWithSprite.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null) return null;
            spriteRenderer.color = color;
            return spriteRenderer;
        }

        /// <summary>
        /// For a gameobject get textmesh pro and set color
        /// </summary>
        /// <param name="gameObjectWithTMPro"></param>
        /// <param name="color"></param>
        /// <returns></returns>
        public TMPro.TextMeshPro SetColorTMProGO(GameObject gameObjectWithTMPro, Color color)
        {
            TMPro.TextMeshPro textMeshPro = gameObjectWithTMPro.GetComponent<TMPro.TextMeshPro>();
            textMeshPro.color = color;
            return textMeshPro;
        }

        /// <summary>
        /// For a text mesh pro set color
        /// </summary>
        /// <param name="textMeshPro"></param>
        /// <param name="color"></param>
        public void SetColorTMProGO(TMPro.TextMeshPro textMeshPro, Color color)
        {
            textMeshPro.color = color;
        }
        #endregion Modify    }
    }
}