﻿using CSharpFunctionalExtensions;
using Reductech.EDR.Connectors.Nuix.Processes.Meta;
using Reductech.EDR.Processes.Internal;

namespace Reductech.EDR.Connectors.Nuix.Conversion
{
    /// <summary>
    /// Converts a core method to a ruby block
    /// </summary>
    internal interface ICoreMethodConverter
    {
        Result<IRubyBlock> Convert(IRunnableProcess process);
    }
}