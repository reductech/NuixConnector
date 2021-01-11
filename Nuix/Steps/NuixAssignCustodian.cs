﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Reductech.EDR.Connectors.Nuix.Steps.Meta;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Attributes;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Util;

namespace Reductech.EDR.Connectors.Nuix.Steps
{

/// <summary>
/// Searches a NUIX case with a particular search string and assigns all files it finds to a particular custodian.
/// </summary>
public sealed class NuixAssignCustodianFactory : RubyScriptStepFactory<NuixAssignCustodian, Unit>
{
    private NuixAssignCustodianFactory() { }

    /// <summary>
    /// The instance.
    /// </summary>
    public static RubyScriptStepFactory<NuixAssignCustodian, Unit> Instance { get; } =
        new NuixAssignCustodianFactory();

    /// <inheritdoc />
    public override Version RequiredNuixVersion { get; } = new(3, 6);

    /// <inheritdoc />
    public override IReadOnlyCollection<NuixFeature> RequiredFeatures { get; }
        = new List<NuixFeature> { NuixFeature.ANALYSIS };

    /// <inheritdoc />
    public override string FunctionName => "AssignCustodian";

    /// <inheritdoc />
    public override string RubyFunctionText => @"
    the_case = $utilities.case_factory.open(pathArg)
    log ""Searching for '#{searchArg}'""

    searchOptions = {}
    items = the_case.search(searchArg, searchOptions)
    log ""#{items.length} found""

    j = 0

    items.each {|i|
        if i.getCustodian != custodianArg
            added = i.assignCustodian(custodianArg)
            j += 1
        end
    }

    log ""#{j} items assigned to custodian #{custodianArg}""
    the_case.close";
}

/// <summary>
/// Searches a NUIX case with a particular search string and assigns all files it finds to a particular custodian.
/// </summary>
public sealed class NuixAssignCustodian : RubyScriptStepBase<Unit>
{
    /// <inheritdoc />
    public override IRubyScriptStepFactory<Unit> RubyScriptStepFactory =>
        NuixAssignCustodianFactory.Instance;

    /// <summary>
    /// The path to the case.
    /// </summary>
    [Required]
    [StepProperty(1)]
    [Example("C:/Cases/MyCase")]
    [RubyArgument("pathArg", 1)]
    [Alias("Case")]
    public IStep<StringStream> CasePath { get; set; } = null!;

    /// <summary>
    /// The term to search for.
    /// </summary>
    [Required]
    [StepProperty(2)]
    [Example("*.txt")]
    [RubyArgument("searchArg", 2)]
    [Alias("Search")]
    public IStep<StringStream> SearchTerm { get; set; } = null!;

    /// <summary>
    /// The custodian to assign to found results.
    /// </summary>
    [Required]
    [StepProperty(3)]
    [RubyArgument("custodianArg", 3)]
    public IStep<StringStream> Custodian { get; set; } = null!;
}

}
