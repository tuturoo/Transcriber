namespace CommandWrapper.Sox;

/// <summary>
/// Константы
/// </summary>
public static class Constants
{
    /// <summary>
    /// Порядок аргументов при вызове функции
    /// </summary>
    public enum Priorities : byte
    {
        GlobalOptions = 0,
        InputFormatOptions,
        InputFile,
        OutputFormatOptions,
        OutputFile,
        Effects
    }
}