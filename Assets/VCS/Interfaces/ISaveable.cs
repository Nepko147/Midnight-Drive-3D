using UnityEngine;

public interface ISaveable
{
    // Уникальный ключ, по которому будут записываться данные этого модуля
    string SaveKey { get; }

    // Метод, который возвращает JSON-текст с данными для сохранения
    string CaptureState();

    // Метод, который принимает сохраненный объект и восстанавливает состояние
    void RestoreState(string _state);

    void SetDefaultState();
}
