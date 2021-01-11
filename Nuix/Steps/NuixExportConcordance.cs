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
/// Exports Concordance for a particular production set.
/// </summary>
public sealed class
    NuixExportConcordanceStepFactory : RubyScriptStepFactory<NuixExportConcordance, Unit>
{
    private NuixExportConcordanceStepFactory() { }

    /// <summary>
    /// The instance.
    /// </summary>
    public static RubyScriptStepFactory<NuixExportConcordance, Unit> Instance { get; } =
        new NuixExportConcordanceStepFactory();

    /// <inheritdoc />
    public override Version RequiredNuixVersion { get; } =
        new(7, 2); //I'm checking the production profile here

    /// <inheritdoc />
    public override IReadOnlyCollection<NuixFeature> RequiredFeatures { get; } =
        new List<NuixFeature>() { NuixFeature.PRODUCTION_SET, NuixFeature.EXPORT_ITEMS };

    /// <inheritdoc />
    public override string FunctionName => "ExportConcordance";

    /// <inheritdoc />
    public override string RubyFunctionText => @"
    the_case = $utilities.case_factory.open(pathArg)

    productionSet = the_case.findProductionSetByName(productionSetNameArg)

    if productionSet == nil
        log ""Could not find production set with name '#{productionSetNameArg.to_s}'""
    elsif productionSet.getProductionProfile == nil
        log ""Production set '#{productionSetNameArg.to_s}' did not have a production profile set.""
    else
        batchExporter = $utilities.createBatchExporter(exportPathArg)


        log 'Starting export.'
        batchExporter.exportItems(productionSet)
        log 'Export complete.'

    end

    the_case.close";
}

/// <summary>
/// Exports Concordance for a particular production set.
/// </summary>
public sealed class NuixExportConcordance : RubyScriptStepBase<Unit>
{
    /// <inheritdoc />
    public override IRubyScriptStepFactory<Unit> RubyScriptStepFactory =>
        NuixExportConcordanceStepFactory.Instance;

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
    /// Where to export the Concordance to.
    /// </summary>
    [Required]
    [StepProperty(2)]
    [RubyArgument("exportPathArg", 2)]
    [Alias("ToDirectory")]
    public IStep<StringStream> ExportPath { get; set; } = null!;

    /// <summary>
    /// The name of the production set to export.
    /// </summary>
    [Required]
    [StepProperty(3)]
    [RubyArgument("productionSetNameArg", 3)]
    [Alias("ProductionSet")]
    public IStep<StringStream> ProductionSetName { get; set; } = null!;
}

}
