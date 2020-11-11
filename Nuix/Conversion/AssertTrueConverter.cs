﻿using System.Collections.Generic;
using Reductech.EDR.Connectors.Nuix.Steps.Meta;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.Internal;

namespace Reductech.EDR.Connectors.Nuix.Conversion
{
    internal sealed class AssertTrueConverter : CoreUnitMethodConverter<AssertTrue>
    {
        /// <inheritdoc />
        protected override IEnumerable<(RubyFunctionParameter parameter, IStep argumentProcess)> GetArgumentBlocks(AssertTrue process)
        {
            yield return (TestParameter, process.Test);
        }

        /// <inheritdoc />
        public override string FunctionName => "AssertTrue";

        /// <inheritdoc />
        public override string FunctionText { get; } = $@"
puts ""Assert #{{{TestParameter.ParameterName}}}""
if !{TestParameter.ParameterName}
    raise 'Assertion failed'
end";

        private static readonly RubyFunctionParameter TestParameter
            = new RubyFunctionParameter("testArg", nameof(AssertTrue.Test),  false, null);

        /// <inheritdoc />
        public override IReadOnlyCollection<RubyFunctionParameter> Arguments { get; } = new[] {TestParameter};
    }
}