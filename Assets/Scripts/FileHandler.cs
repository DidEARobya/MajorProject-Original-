using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;
using Newtonsoft.Json;

public static class FileHandler
{
    public static void SaveToJSON<T>(List<T> toSave, string fileName)
    {
        Debug.Log(GetPath(fileName, true));
        string content = JsonHelper.ToJson<T>(toSave.ToArray());
        WriteFile(GetPath(fileName, true), content);
    }
    public static List<T> ReadFromJSON<T>(string fileName, bool isPersistent)
    {
        string content = ReadFile(GetPath(fileName, isPersistent));

        if(string.IsNullOrEmpty(content) || content == "{}")
        {
            return new List<T>();
        }

        List<T> contentList = JsonHelper.FromJson<T>(content).ToList();

        return contentList;
    }
    private static string GetPath(string fileName, bool persistentData)
    {
        if(persistentData == false)
        {
            return Application.dataPath + "/" + fileName;
        }

        return Application.persistentDataPath + "/" + fileName;
    }
    private static void WriteFile(string path, string content)
    {
        FileStream fileStream = new FileStream(path, FileMode.Create);

        using(StreamWriter writer = new StreamWriter(fileStream))
        {
            writer.Write(content);
        }
    }
    private static string ReadFile(string path)
    {
        if(File.Exists(path))
        {
            using (StreamReader reader = new StreamReader(path))
            {
                string content = reader.ReadToEnd();
                return content;
            }
        }

        return "";
    }
}
public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonConvert.DeserializeObject<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        string json = JsonConvert.SerializeObject(wrapper);
        Debug.Log(json);    
        return json;
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonConvert.SerializeObject(wrapper.Items[0], Formatting.Indented, new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
    }

    [Serializable]
    private class Wrapper<T>
    {
        [SerializeField]
        public T[] Items;
    }
}