// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Management.Automation.Runspaces;
using Empress.Labs.PowerShell.Common.IO;
using Empress.Labs.PowerShell.TestTools.Abstractions;
using Empress.Labs.PowerShell.TestTools.Options;
using Microsoft.PowerShell;

namespace Empress.Labs.PowerShell.TestTools;

/// <inheritdoc cref="IPrepareCmdletInvokation" />
internal sealed class PrepareCmdletInvokation : IPrepareCmdletInvokation {
  private readonly List<string> _modules = [];
  private readonly List<AbsolutePath> _modulesFromPath = [];
  private readonly List<CommandParameter> _parameters = [];
  private readonly List<SessionStateVariableEntry> _variableEntries = [];
  private ExecutionPolicy? _executionPolicy;

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithVariableEntry(SessionStateVariableEntry variableEntry) {
    _variableEntries.Add(variableEntry);

    return this;
  }

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithVariableEntry(string name, object value)
    => WithVariableEntry(new SessionStateVariableEntry(name, value, string.Empty));

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithParameter(CommandParameter commandParameter) {
    _parameters.Add(commandParameter);

    return this;
  }

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithParameter(string key, object value)
    => WithParameter(new CommandParameter(key, value));

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithPSModule(params string[] moduleCollection) {
    _modules.AddRange(moduleCollection);

    return this;
  }

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithPSModuleFromPath(params AbsolutePath[] pathCollection) {
    _modulesFromPath.AddRange(pathCollection);

    return this;
  }

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithExecutionPolicy(ExecutionPolicy executionPolicy) {
    _executionPolicy = executionPolicy;

    return this;
  }

  /// <summary>
  ///   Configures the invokation of a cmdlet using the provided configuration.
  /// </summary>
  /// <param name="configure">The action to prepare the invokation.</param>
  /// <param name="options">The options to be used when invoking the cmdlet.</param>
  public static void Configure(Action<IPrepareCmdletInvokation> configure, out CmdletInvokationOptions options) {
    var prepareCmdletInvokation = new PrepareCmdletInvokation();
    configure(prepareCmdletInvokation);

    options = new CmdletInvokationOptions {
      Modules = prepareCmdletInvokation._modules,
      ModulesFromPath = prepareCmdletInvokation._modulesFromPath,
      Parameters = prepareCmdletInvokation._parameters,
      VariableEntries = prepareCmdletInvokation._variableEntries,
      ExecutionPolicy = prepareCmdletInvokation._executionPolicy
    };
  }
}
