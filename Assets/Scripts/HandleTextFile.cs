using UnityEngine;
using UnityEditor;
using System.IO;

public static class HandleTextFile
{
    public static void WriteTextContent(string content)
    {
        string path = "Assets/Resources/TestContainer.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(content);
        writer.Close();

        //Re-import the file to update the reference in the editor
        AssetDatabase.ImportAsset(path);
        TextAsset asset = (TextAsset)Resources.Load("TestContainer");
    }

    [MenuItem("Tools/Read content of 'TestContainer.txt'")]
    private static void ReadTextContent()
    {
        string path = "Assets/Resources/TestContainer.txt";

        StreamReader reader = new StreamReader(path);
        Debug.Log(reader.ReadToEnd());
        reader.Close();
    }

    [MenuItem("Tools/Delete content of 'TestContainer.txt'")]
    private static void DeleteTextContent()
    {
        string path = "Assets/Resources/TestContainer.txt";
        File.WriteAllText(path, string.Empty);

        AssetDatabase.ImportAsset(path);
        TextAsset asset = (TextAsset)Resources.Load("TestContainer");

        Debug.Log(asset.text);
    }
}