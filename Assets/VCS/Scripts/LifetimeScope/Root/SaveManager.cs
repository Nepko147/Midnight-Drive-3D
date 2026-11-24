using System.Collections.Generic;
using UnityEngine;

public class SaveManager
{
    private readonly ISaveService saveService;
    private readonly List<ISaveable> saveableComponents = new();

    // VContainer передает сюда нужную реализацию ISaveService (например, JsonSaveService)
    public SaveManager(ISaveService _saveService)
    {
        saveService = _saveService;
    }

    public void Register(ISaveable _saveable)
    {
        if (!saveableComponents.Contains(_saveable))
        {
            saveableComponents.Add(_saveable);

            // Как только компонент регистрируется (например, при старте сцены), 
            // мы сразу загружаем его персональное состояние, если оно есть на диске
            LoadComponentState(_saveable);
        }
    }

    public void Unregister(ISaveable _saveable)
    {
        saveableComponents.Remove(_saveable);
    }

    public void SaveGame()
    {
        foreach (var _saveable in saveableComponents)
        {
            string _jsonState = _saveable.CaptureState();
            saveService.WriteRawText(_saveable.SaveKey, _jsonState);
        }

        Debug.Log("[SaveManager] Все модули фреймворка успешно сохранены!");
    }

    public void LoadGame()
    {
        foreach (var _saveable in saveableComponents)
        {
            LoadComponentState(_saveable);
        }

        Debug.Log("[SaveManager] Все модули фреймворка успешно загружены!");
    }

    private void LoadComponentState(ISaveable _saveable)
    {
        // Загружаем данные как строку. Если файла нет, вернется null
        string _savedJson = saveService.ReadRawText(_saveable.SaveKey);

        if (!string.IsNullOrEmpty(_savedJson))
        {
            _saveable.RestoreState(_savedJson);
        }
        else
        {
            _saveable.SetDefaultState();
        }
    }
}