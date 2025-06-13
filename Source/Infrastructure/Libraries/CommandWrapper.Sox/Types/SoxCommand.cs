using CommandWrapper.Core.Abstractions;
using CommandWrapper.Sox.Positions.Effects;
using CommandWrapper.Sox.Positions.FormatOptions;
using CommandWrapper.Sox.Positions.GlobalOptions;
using CommandWrapper.Sox.Positions.Target;

namespace CommandWrapper.Sox.Types;

public sealed class SoxCommand(string executablePath) : Command<SoxProcess>(executablePath)
{
    #region Состояние

    private readonly TargetPosition _input = new TargetPosition(Constants.Priorities.InputFile);
    
    private readonly TargetPosition _output = new TargetPosition(Constants.Priorities.OutputFile);

    private readonly FormatOptionsPosition _inputFormat = new FormatOptionsPosition(Constants.Priorities.InputFormatOptions);

    private readonly FormatOptionsPosition _outputFormat = new FormatOptionsPosition(Constants.Priorities.OutputFormatOptions);

    private readonly EffectsPosition _effects = new EffectsPosition();

    private readonly GlobalOptionsPosition _globalOptions = new GlobalOptionsPosition();

    #endregion

    #region Публичные параметры команды

    public GlobalOptionsPosition GlobalOptions => _globalOptions;

    public FormatOptionsPosition InputFormat => _inputFormat;

    public FormatOptionsPosition OutputFormat => _outputFormat;

    public EffectsPosition Effects => _effects;

    #endregion 

    protected override SortedSet<CommandPosition>? Positions =>
    [
        GlobalOptions,
        InputFormat,
        _input,
        OutputFormat,
        _output,
        Effects
    ];
   
    public override SoxProcess Run()
    {
        return new SoxProcess
        (
            ExecutablePath,
            Arguments
        );
    }
}