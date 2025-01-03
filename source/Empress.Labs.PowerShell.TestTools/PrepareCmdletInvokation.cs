// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Management.Automation.Runspaces;
using Empress.Labs.PowerShell.Common.IO;
using Empress.Labs.PowerShell.TestTools.Abstractions;

namespace Empress.Labs.PowerShell.TestTools;

/// <inheritdoc cref="IPrepareCmdletInvokation" />
internal sealed class PrepareCmdletInvokation : IPrepareCmdletInvokation {
  public readonly List<string> Modules = [];
  public readonly List<AbsolutePath> ModulesFromPath = [];
  public readonly List<CommandParameter> Parameters = [];
  public readonly List<SessionStateVariableEntry> VariableEntries = [];

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithVariableEntry(SessionStateVariableEntry variableEntry) {
    VariableEntries.Add(variableEntry);

    return this;
  }

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithVariableEntry(string name, object value)
    => WithVariableEntry(new SessionStateVariableEntry(name, value, string.Empty));

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithParameter(CommandParameter commandParameter) {
    Parameters.Add(commandParameter);

    return this;
  }

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithParameter(string key, object value)
    => WithParameter(new CommandParameter(key, value));

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithPSModule(params string[] moduleCollection) {
    Modules.AddRange(moduleCollection);

    return this;
  }

  /// <inheritdoc />
  public IPrepareCmdletInvokation WithPSModuleFromPath(params AbsolutePath[] pathCollection) {
    ModulesFromPath.AddRange(pathCollection);

    return this;
  }

  /// <summary>
  ///   Configures the invokation of a cmdlet using the provided configuration.
  /// </summary>
  /// <param name="configure">The action to prepare the invokation.</param>
  /// <param name="variableEntries">The variable entries to be used.</param>
  /// <param name="parameters">The parameters to be used.</param>
  /// <param name="modules">The modules to be imported.</param>
  /// <param name="modulesFromPath">The modules to be imported from the path.</param>
  public static void Configure(Action<IPrepareCmdletInvokation> configure, out IEnumerable<SessionStateVariableEntry> variableEntries,
  out IEnumerable<CommandParameter> parameters, out IEnumerable<string> modules, out IEnumerable<AbsolutePath> modulesFromPath) {
    var prepareCmdletInvokation = new PrepareCmdletInvokation();
    configure(prepareCmdletInvokation);

    variableEntries = prepareCmdletInvokation.VariableEntries;
    parameters = prepareCmdletInvokation.Parameters;
    modules = prepareCmdletInvokation.Modules;
    modulesFromPath = prepareCmdletInvokation.ModulesFromPath;
  }
}
