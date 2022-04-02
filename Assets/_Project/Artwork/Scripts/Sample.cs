using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LD50
{
    public class Sample : MonoBehaviour
    {
        public FactoryLD50 FactoryLD50;

        bool allGameObjectsPrecreated;

        private void Start()
        {
            allGameObjectsPrecreated = false;
        }

        void Update()
        {
            // This should be at beginning of game, when you initialize stuff
            // FactoryLD50.PreCreateGameObjects() must be called until it returns true, then everything is ready. While false, still things to be done
            if (!allGameObjectsPrecreated) // If still gameobjects to be instanciated
            {
                allGameObjectsPrecreated = FactoryLD50.PreCreateGameObjects();
                return;
            }

            // Creation of gameobjects
            if (Input.GetKeyDown(KeyCode.G))
                FactoryLD50.GetGood();
            if (Input.GetKeyDown(KeyCode.B))
                FactoryLD50.GetBad();
            if (Input.GetKeyDown(KeyCode.P))
                FactoryLD50.GetPlatform();
            if (Input.GetKeyDown(KeyCode.S))
                FactoryLD50.GetMisc(FactoryLD50.MiscType.Start);
            if (Input.GetKeyDown(KeyCode.E))
                FactoryLD50.GetMisc(FactoryLD50.MiscType.Exit);
        }
    }
}