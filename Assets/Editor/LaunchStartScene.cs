// class doesn't matter, add to anything in the Editor folder
// for any beginners reading, this is c#

using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

namespace Assets.Editor
{
    internal class LaunchStartScene
    {
        [MenuItem("Edit/Play-Stop, But From Prelaunch Scene %0")]
        public static void PlayFromPrelaunchScene()
        {
            using (StreamWriter sw = File.AppendText("log.txt"))
            {
                sw.WriteLine($"Starting up at at {DateTime.Now:HH:mm:ss tt}");
            }

            if (EditorApplication.isPlaying)
            {
                EditorApplication.isPlaying = false;
                return;
            }

            EditorSceneManager.OpenScene("Assets/SceneLoader.unity");
            EditorApplication.isPlaying = true;
        }
    }
}