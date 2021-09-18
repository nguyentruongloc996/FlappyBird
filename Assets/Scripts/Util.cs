using CodeMonkey.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Assets.Scripts
{
    public static class Util
    {

        public enum Scene
        {
            Game,
            LoadingScene
        }

        public static Scene targetScene;

        private static object lockObject = new object();

        static public void Load(Scene scene)
        {
            lock (lockObject)
            {
                //Show loading screen first
                SceneManager.LoadScene(Scene.LoadingScene.ToString());
                targetScene = scene;
            }
        }

        public static void LoadTargerScene()
        {
            lock (lockObject)
            {
                SceneManager.LoadScene(targetScene.ToString());
            } 
        }
    }
}
