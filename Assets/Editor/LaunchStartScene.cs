// class doesn't matter, add to anything in the Editor folder
// for any beginners reading, this is c#

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