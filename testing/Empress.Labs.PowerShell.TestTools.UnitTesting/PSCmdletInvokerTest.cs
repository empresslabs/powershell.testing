// Copyright (c) Bruno Sales <me@baliestri.dev>. Licensed under the MIT License.
// See the LICENSE file in the repository root for full license text.

using System.Management.Automation;
using Empress.Labs.PowerShell.TestTools.UnitTesting.Cmdlets;

namespace Empress.Labs.PowerShell.TestTools.UnitTesting;

[TestFixture]
[TestOf(typeof(PSCmdletInvoker))]
public sealed class PSCmdletInvokerTest {
  // TESTING ONLY INVOKE AND TRYINVOKE BECAUSE THOSE CALLS THE COLLECTION TYPE METHODS

  [Test]
  public void Invoke_WithFakeModuleCmdlet_ShouldReturnFakeModuleCmdlet() {
    // Act
    var result = PSCmdletInvoker.Invoke<GetFakeModuleCmdlet>();

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.BaseObject, Is.EqualTo("FakeModuleCmdlet"));
  }

  [Test]
  public void Invoke_WithFakeModuleCmdletAndThrowErrorRecord_ShouldThrowErrorRecord() {
    // Act
    var exception = Assert.Throws<CmdletInvocationException>(() => PSCmdletInvoker.Invoke<GetFakeModuleCmdlet>(prepare =>
      prepare.WithParameter("ThrowErrorRecord", true)));

    // Assert
    Assert.That(exception, Is.Not.Null);
    Assert.Multiple(() => {
      Assert.That(exception.Message, Is.EqualTo("Error message"));
      Assert.That(exception.ErrorRecord, Is.Not.Null);
    });
  }

  [Test]
  public void TryInvoke_WithFakeModuleCmdletAndWriteErrorRecord_ShouldReturnErrorRecord() {
    // Act
    var result = PSCmdletInvoker.TryInvoke<GetFakeModuleCmdlet>(prepare =>
      prepare.WithParameter("WriteErrorRecord", true));

    // Assert
    Assert.That(result, Is.Not.Null);
    Assert.That(result.Exception, Is.Not.Null);
    Assert.That(result.Exception.Message, Is.EqualTo("Error message"));
  }
}
