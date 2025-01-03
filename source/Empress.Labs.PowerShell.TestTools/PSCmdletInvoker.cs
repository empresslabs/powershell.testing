// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Diagnostics.CodeAnalysis;
using System.Management.Automation.Runspaces;
using System.Reflection;
using Empress.Labs.PowerShell.Common.Extensions;
using Empress.Labs.PowerShell.TestTools.Abstractions;
using Empress.Labs.PowerShell.TestTools.Options;
using Pwsh = System.Management.Automation.PowerShell;

namespace Empress.Labs.PowerShell.TestTools;

/// <summary>
///   Invokes a PowerShell cmdlet to be tested.
/// </summary>
[SuppressMessage("ReSharper", "InconsistentNaming")]
public static class PSCmdletInvoker {
  // Can be any help file name, does not make a difference.
  private const string HELP_FILE_NAME = "Microsoft.Windows.Installer.PowerShell.dll-Help.xml";

  private static Pwsh createShell<TCmdlet>(Action<IPrepareCmdletInvokation>? action = null) where TCmdlet : PSCmdlet {
    var options = CmdletInvokationOptions.Empty;

    if (action is not null) {
      PrepareCmdletInvokation.Configure(action, out options);
    }

    var cmdletAttribute = typeof(TCmdlet).GetCustomAttribute<CmdletAttribute>(true)
                          ?? throw new ArgumentException($"The cmdlet '{typeof(TCmdlet).Name}' does not have the 'CmdletAttribute'.");
    var cmdletName = $"{cmdletAttribute.VerbName}-{cmdletAttribute.NounName}";
    var command = new Command(cmdletName);

    var initialSessionState = InitialSessionState.CreateDefault2();

    initialSessionState.ImportPSModule(options.Modules.ToArray());
    options.ModulesFromPath.ForEach(path => initialSessionState.ImportPSModulesFromPath(path));
    initialSessionState.Variables.Add(options.VariableEntries);
    initialSessionState.Commands.Add(new SessionStateCmdletEntry(cmdletName, typeof(TCmdlet), HELP_FILE_NAME));

    if (OperatingSystem.IsWindows() &&
        options.ExecutionPolicy.HasValue) {
      initialSessionState.ExecutionPolicy = options.ExecutionPolicy.Value;
    }

    var powershell = Pwsh.Create(initialSessionState);
    powershell.Streams.ClearStreams();

    foreach (var parameter in options.Parameters) {
      command.Parameters.Add(parameter);
    }

    powershell.Commands.AddCommand(command);

    return powershell;
  }

  /// <summary>
  ///   Invokes a cmdlet and returns the output.
  /// </summary>
  /// <param name="action">The action to prepare the invokation.</param>
  /// <typeparam name="TCmdlet">The type of the cmdlet to be invoked.</typeparam>
  /// <returns>The output of the cmdlet.</returns>
  /// <exception cref="ArgumentException">The cmdlet does not have the <see cref="CmdletAttribute" />.</exception>
  public static IEnumerable<PSObject> InvokeCollection<TCmdlet>(Action<IPrepareCmdletInvokation>? action = null) where TCmdlet : PSCmdlet {
    using var powershell = createShell<TCmdlet>(action);

    return powershell.Invoke();
  }

  /// <summary>
  ///   Invokes a cmdlet and returns the output.
  /// </summary>
  /// <param name="streams">The data streams of the cmdlet.</param>
  /// <typeparam name="TCmdlet">The type of the cmdlet to be invoked.</typeparam>
  /// <returns>The output of the cmdlet.</returns>
  public static IEnumerable<PSObject> InvokeCollection<TCmdlet>(out PSDataStreams streams) where TCmdlet : PSCmdlet {
    using var powershell = createShell<TCmdlet>();

    var collection = powershell.Invoke();
    streams = powershell.Streams;

    return collection;
  }

  /// <summary>
  ///   Invokes a cmdlet and returns the output.
  /// </summary>
  /// <param name="action">The action to prepare the invokation.</param>
  /// <param name="streams">The data streams of the cmdlet.</param>
  /// <typeparam name="TCmdlet">The type of the cmdlet to be invoked.</typeparam>
  /// <returns>The output of the cmdlet.</returns>
  public static IEnumerable<PSObject> InvokeCollection<TCmdlet>(Action<IPrepareCmdletInvokation> action, out PSDataStreams streams)
    where TCmdlet : PSCmdlet {
    using var powershell = createShell<TCmdlet>(action);

    var collection = powershell.Invoke();
    streams = powershell.Streams;

    return collection;
  }

  /// <summary>
  ///   Invokes a cmdlet and returns the output.
  /// </summary>
  /// <param name="action">The action to prepare the invokation.</param>
  /// <typeparam name="TCmdlet">The type of the cmdlet to be invoked.</typeparam>
  /// <returns>The output of the cmdlet.</returns>
  /// <exception cref="ArgumentException">The cmdlet does not have the <see cref="CmdletAttribute" />.</exception>
  public static PSObject? Invoke<TCmdlet>(Action<IPrepareCmdletInvokation>? action = null) where TCmdlet : PSCmdlet
    => InvokeCollection<TCmdlet>(action).FirstOrDefault();

  /// <summary>
  ///   Invokes a cmdlet and returns the output.
  /// </summary>
  /// <param name="streams">The data streams of the cmdlet.</param>
  /// <typeparam name="TCmdlet">The type of the cmdlet to be invoked.</typeparam>
  /// <returns>The output of the cmdlet.</returns>
  public static PSObject? Invoke<TCmdlet>(out PSDataStreams streams) where TCmdlet : PSCmdlet
    => InvokeCollection<TCmdlet>(out streams).FirstOrDefault();

  /// <summary>
  ///   Invokes a cmdlet and returns the output.
  /// </summary>
  /// <param name="action">The action to prepare the invokation.</param>
  /// <param name="streams">The data streams of the cmdlet.</param>
  /// <typeparam name="TCmdlet">The type of the cmdlet to be invoked.</typeparam>
  /// <returns>The output of the cmdlet.</returns>
  public static PSObject? Invoke<TCmdlet>(Action<IPrepareCmdletInvokation> action, out PSDataStreams streams) where TCmdlet : PSCmdlet
    => InvokeCollection<TCmdlet>(action, out streams).FirstOrDefault();

  /// <summary>
  ///   Invokes a cmdlet and returns a collection of error records.
  /// </summary>
  /// <param name="action">The action to prepare the invokation.</param>
  /// <typeparam name="TCmdlet">The type of the cmdlet to be invoked.</typeparam>
  /// <returns>The error records thrown by the cmdlet; otherwise, an empty collection.</returns>
  /// <exception cref="ArgumentException">The cmdlet does not have the <see cref="CmdletAttribute" />.</exception>
  /// <remarks>
  ///   Works only for those that use <see cref="PSCmdlet.WriteError" /> method, if the cmdlet throws an exception, it will not be caught.
  /// </remarks>
  public static IEnumerable<ErrorRecord> TryInvokeCollection<TCmdlet>(Action<IPrepareCmdletInvokation>? action = null) where TCmdlet : PSCmdlet {
    using var powershell = createShell<TCmdlet>(action);

    powershell.Invoke();

    if (!powershell.HadErrors) {
      return [];
    }

    return powershell
      .Streams
      .Error
      .ReadAll();
  }

  /// <summary>
  ///   Invokes a cmdlet and returns an error record.
  /// </summary>
  /// <param name="action">The action to prepare the invokation.</param>
  /// <typeparam name="TCmdlet">The type of the cmdlet to be invoked.</typeparam>
  /// <returns>The error record thrown by the cmdlet; otherwise, <see langword="null" />.</returns>
  /// <exception cref="ArgumentException">The cmdlet does not have the <see cref="CmdletAttribute" />.</exception>
  /// <remarks>
  ///   Works only for those that use <see cref="PSCmdlet.WriteError" /> method, if the cmdlet throws an exception, it will not be caught.
  /// </remarks>
  public static ErrorRecord? TryInvoke<TCmdlet>(Action<IPrepareCmdletInvokation>? action = null) where TCmdlet : PSCmdlet
    => TryInvokeCollection<TCmdlet>(action).FirstOrDefault();
}
