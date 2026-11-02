using UnityEngine;

public interface ISaveService
{
    // Метод для сохранения данных любого типа T по строковому ключу
    void Save<T>(string _key, T _data);

    // Метод для загрузки данных типа T. 
    // Если файла нет, возвращается переданное дефолтное значение
    T Load<T>(string _key, T _defaultValue = default);

    void WriteRawText(string _key, string _text);
    string ReadRawText(string _key);

    // Метод для удаления файла сохранения (пригодится для функции "Сброс прогресса")
    void Delete(string _key);
}
