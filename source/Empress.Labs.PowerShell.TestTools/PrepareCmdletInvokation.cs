// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Management.Automation.Runspaces;
using Empress.Labs.PowerShell.TestTools.Abstractions;

namespace Empress.Labs.PowerShell.TestTools;

/// <inheritdoc cref="IPrepareCmdletInvokation" />
internal sealed class PrepareCmdletInvokation : IPrepareCmdletInvokation {
  private readonly List<CommandParameter> _parameters = [];
  private readonly List<SessionStateVariableEntry> _variableEntries = [];

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

  private void Build(out IEnumerable<SessionStateVariableEntry> variableEntries, out IEnumerable<CommandParameter> parameters)
    => (variableEntries, parameters) = (_variableEntries, _parameters);

  /// <summary>
  ///   Configures the invokation of a cmdlet using the provided configuration.
  /// </summary>
  /// <param name="configure">The action to prepare the invokation.</param>
  /// <param name="variableEntries">The variable entries to be used.</param>
  /// <param name="parameters">The parameters to be used.</param>
  public static void Configure(Action<IPrepareCmdletInvokation> configure, out IEnumerable<SessionStateVariableEntry> variableEntries,
  out IEnumerable<CommandParameter> parameters) {
    var prepare = new PrepareCmdletInvokation();
    configure(prepare);

    prepare.Build(out variableEntries, out parameters);
  }
}
