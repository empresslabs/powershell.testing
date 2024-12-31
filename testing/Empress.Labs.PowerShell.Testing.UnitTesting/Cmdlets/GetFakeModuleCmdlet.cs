// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Management.Automation;

namespace Empress.Labs.PowerShell.Testing.UnitTesting.Cmdlets;

[Cmdlet(VerbsCommon.Get, "FakeModule")]
public sealed class GetFakeModuleCmdlet : PSCmdlet {
  private readonly ErrorRecord _errorRecord = new(new Exception("Error message"), "ErrorId", ErrorCategory.InvalidOperation, null);

  [Parameter]
  public SwitchParameter ThrowErrorRecord { get; set; }

  [Parameter]
  public SwitchParameter WriteErrorRecord { get; set; }

  /// <inheritdoc />
  protected override void BeginProcessing() {
    if (!ThrowErrorRecord) {
      return;
    }

    ThrowTerminatingError(_errorRecord);
  }

  /// <inheritdoc />
  protected override void ProcessRecord() {
    if (!WriteErrorRecord) {
      return;
    }

    WriteError(_errorRecord);
  }

  /// <inheritdoc />
  protected override void EndProcessing()
    => WriteObject("FakeModuleCmdlet");
}
