using Assets.CSharp_vNext_Support.AsyncTools;
using UnityEditor;

namespace Assets.CSharp_vNext_Support.Editor
{
    public static class UnitySchedulerEditor
    {
        [InitializeOnLoadMethod]
        private static void InitializeInEditor()
        {
            UnityScheduler.InitializeInEditor();
            EditorApplication.update += UnityScheduler.ProcessEditorUpdate;
        }
    }
}
