namespace AsyncDataLibrary.Models;

/// <summary>
/// Атрибут, що вказує назву JSON файлу для зберігання даних певної моделі.
/// Застосовується до класів моделей через рефлексію в JsonRepository.
/// </summary>
[AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
public sealed class StorageFileAttribute : Attribute
{
    public string FileName { get; }

    public StorageFileAttribute(string fileName)
    {
        FileName = fileName;
    }
}
