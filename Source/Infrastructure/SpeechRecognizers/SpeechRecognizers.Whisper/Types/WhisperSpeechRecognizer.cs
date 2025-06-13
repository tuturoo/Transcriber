using Core.Shared.Abstractions;
using Core.SpeechRecognizers.Interfaces;
using Microsoft.Extensions.Logging;
using SpeechRecognizers.Whisper.Models;
using Whisper.net;

namespace SpeechRecognizers.Whisper.Types;

/// <summary>
/// Транскрибация текста через модель Whisper
/// </summary>
/// <param name="whisperSettings">Настройки модели</param>
/// <param name="modelPath">Путь к модели</param>
public sealed class WhisperSpeechRecognizer : ISpeechRecognizer, IDisposable 
{
    private readonly ILogger<WhisperSpeechRecognizer> _logger;

    private readonly WhisperSettings _settings;

    private readonly SemaphoreSlim _semaphoreSlim;

    private WhisperFactory? _factory;

    private WhisperProcessor? _processor;

    public WhisperSpeechRecognizer(ILogger<WhisperSpeechRecognizer> logger, WhisperSettings settings)
    {
        _semaphoreSlim = new SemaphoreSlim(1);

        _logger = logger;
        _settings = settings;
    }
    
    public Task InitAsync(CancellationToken token = default)
    {
        _logger.LogInformation($"Начата инициализация модели Whisper {_settings.ModelPath}, параметры - потоки: {_settings.Threads}, запрос: {_settings.Prompt}, язык: {_settings.Language}");

        _factory = WhisperFactory.FromPath(_settings.ModelPath);

        _logger.LogInformation($"Модель инициализрована");

        _processor = _factory
            .CreateBuilder()
            .WithPrompt(_settings.Prompt)
            .WithThreads(_settings.Threads)
            .WithLanguage(_settings.Language)
            .Build();
        
        return Task.CompletedTask;
    }

    public async Task<string> TranscribeAsync(AudioStream audio, CancellationToken token = default)
    {
        try
        {
            audio.Seek(0, SeekOrigin.Begin);

            float[] frames = new float[audio.TotalFrames];

            await audio.ReadFramesAsync(frames, 0, (int)audio.TotalFrames, token);

            var speechSegments = new LinkedList<string>();

            try
            {
                await _semaphoreSlim.WaitAsync(token);

                await foreach (var segment in _processor!.ProcessAsync(frames, token))
                {
                    _logger.LogInformation($"Получен траскрибированный сегмент: {segment.Text}");

                    speechSegments.AddLast(segment.Text);
                }
            }
            finally
            {
                _semaphoreSlim.Release();
            }

            return string.Join(" ", speechSegments);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Возникла ошибка при транскрибировании сегмента");

            throw;
        }
    }

    public void Dispose()
    {
        _factory?.Dispose();
        _processor?.Dispose();
    }
}