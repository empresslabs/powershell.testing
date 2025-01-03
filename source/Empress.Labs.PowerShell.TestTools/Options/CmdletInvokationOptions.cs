// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Management.Automation.Runspaces;
using Empress.Labs.PowerShell.Common.IO;
using Microsoft.PowerShell;

namespace Empress.Labs.PowerShell.TestTools.Options;

/// <summary>
///   Invokation options for a cmdlet to be tested.
/// </summary>
public readonly record struct CmdletInvokationOptions {
  /// <summary>
  ///   A read-only instance of the <see cref="CmdletInvokationOptions" /> whose values are all empty or null.
  /// </summary>
  public static readonly CmdletInvokationOptions Empty = new() {
    Modules = [],
    ModulesFromPath = [],
    Parameters = [],
    VariableEntries = []
  };

  public IEnumerable<string> Modules { get; init; }
  public IEnumerable<AbsolutePath> ModulesFromPath { get; init; }
  public IEnumerable<CommandParameter> Parameters { get; init; }
  public IEnumerable<SessionStateVariableEntry> VariableEntries { get; init; }
  public ExecutionPolicy? ExecutionPolicy { get; init; }
}
