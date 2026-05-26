namespace AsyncDataLibrary.Interfaces;

/// <summary>
/// Контракт серіалізатора даних.
/// Бібліотека не залежить від конкретної реалізації (Text.Json, Newtonsoft, тощо).
/// </summary>
public interface IDataSerializer
{
    /// <summary>Серіалізує об'єкт у рядок JSON.</summary>
    string Serialize<T>(T obj);

    /// <summary>Десеріалізує рядок JSON у об'єкт типу T.</summary>
    T? Deserialize<T>(string json);
}
