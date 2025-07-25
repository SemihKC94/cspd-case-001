using System.IO;
using UnityEditor;
using UnityEngine;

namespace SKC.Editors
{
    public class SKCEditor : EditorWindow
    {
        [MenuItem("SKC/Delete Save")]
        public static void DeleteSaveFunction()
        {
            string[] filePaths = Directory.GetFiles(Application.persistentDataPath);
            foreach (string filePath in filePaths)
            {
                Debug.Log(filePath);
                File.Delete(filePath);
            }
        }
    }
}
