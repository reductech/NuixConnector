﻿using Reductech.EDR.Core.TestHarness;

namespace Reductech.EDR.Connectors.Nuix.Tests.Steps;

public partial class NuixCloseConnectionTests : StepTestBase<NuixCloseConnection, Unit>
{
    protected override IEnumerable<StepCase> StepCases
    {
        get
        {
            yield return new StepCase(
                "Close connection that doesn't exist",
                new NuixCloseConnection(),
                Unit.Default
            );
        }
    }

    protected override IEnumerable<ErrorCase> ErrorCases
    {
        get
        {
            yield break;
        }
    }
}
