using System.IO;
using UnityEngine;

public class JsonSaveService : ISaveService
{
    private readonly string basePath;

    public JsonSaveService()
    {
        // Сохраняем в стандартную persistent-папку Unity
        basePath = Application.persistentDataPath;
    }

    private string GetPath(string _key) => Path.Combine(basePath, $"{_key}.json");

    public void Save<T>(string _key, T _data)
    {
        var _path = GetPath(_key);
        var _json = JsonUtility.ToJson(_data, true);
        File.WriteAllText(_path, _json);
    }

    public T Load<T>(string _key, T _defaultValue = default)
    {
        var _path = GetPath(_key);

        if (!File.Exists(_path))
        {
            return _defaultValue;
        }
        else
        {
            var _json = File.ReadAllText(_path);
            return JsonUtility.FromJson<T>(_json);
        }
    }

    public void WriteRawText(string _key, string _text)
    {
        var _path = GetPath(_key);
        File.WriteAllText(_path, _text);
    }

    public string ReadRawText(string _key)
    {
        var _path = GetPath(_key);

        if (File.Exists(_path)) 
        {
            return File.ReadAllText(_path);
        }
        else
        {
            return null;
        }
    }

    public void Delete(string _key)
    {
        var _path = GetPath(_key);

        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }
}
