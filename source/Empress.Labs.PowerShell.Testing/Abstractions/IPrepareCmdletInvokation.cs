// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Management.Automation.Runspaces;

namespace Empress.Labs.PowerShell.Testing.Abstractions;

/// <summary>
///   Prepares the invokation of a cmdlet.
/// </summary>
public interface IPrepareCmdletInvokation {
  /// <summary>
  ///   Add a variable entry to the configuration.
  /// </summary>
  /// <param name="variableEntry">The variable entry to be added.</param>
  /// <returns>The current instance of the configuration.</returns>
  IPrepareCmdletInvokation WithVariableEntry(SessionStateVariableEntry variableEntry);

  /// <summary>
  ///   Add a variable entry to the configuration.
  /// </summary>
  /// <param name="name">The name of the variable.</param>
  /// <param name="value">The value of the variable.</param>
  /// <returns>The current instance of the configuration.</returns>
  IPrepareCmdletInvokation WithVariableEntry(string name, object value);

  /// <summary>
  ///   Add a parameter to the configuration.
  /// </summary>
  /// <param name="commandParameter">The parameter to be added.</param>
  /// <returns>The current instance of the configuration.</returns>
  IPrepareCmdletInvokation WithParameter(CommandParameter commandParameter);

  /// <summary>
  ///   Add a parameter to the configuration.
  /// </summary>
  /// <param name="key">The key of the parameter.</param>
  /// <param name="value">The value of the parameter.</param>
  /// <returns>The current instance of the configuration.</returns>
  IPrepareCmdletInvokation WithParameter(string key, object value);
}
