﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Reductech.EDR.Connectors.Nuix.processes.meta;
using Reductech.EDR.Processes;
using Reductech.EDR.Processes.Attributes;
using Reductech.EDR.Processes.Internal;

namespace Reductech.EDR.Connectors.Nuix.processes
{

    /// <summary>
    /// Migrates a case to the latest version if necessary.
    /// </summary>
    public sealed class NuixMigrateCaseProcessFactory : RubyScriptProcessFactory<NuixMigrateCase, Unit>
    {
        private NuixMigrateCaseProcessFactory() { }

        /// <summary>
        /// The instance.
        /// </summary>
        public static RubyScriptProcessFactory<NuixMigrateCase, Unit> Instance { get; } = new NuixMigrateCaseProcessFactory();

        /// <inheritdoc />
        public override Version RequiredVersion { get; } = new Version(3, 0);

        /// <inheritdoc />
        public override IReadOnlyCollection<NuixFeature> RequiredFeatures { get; } = new List<NuixFeature>();

        /// <inheritdoc />
        public override string MethodName => "MigrateCase";

        /// <inheritdoc />
        public override string ScriptText => @"
    puts ""Opening Case, migrating if necessary""

    options = {migrate: true}

    the_case = utilities.case_factory.open(pathArg, options)";
    }


    /// <summary>
    /// Migrates a case to the latest version if necessary.
    /// </summary>
    public sealed class NuixMigrateCase : RubyScriptProcessUnit
    {
        /// <inheritdoc />
        public override IRubyScriptProcessFactory<Unit> RubyScriptProcessFactory => NuixMigrateCaseProcessFactory.Instance;

        /// <summary>
        /// The path to the case.
        /// </summary>
        [Required]
        [RunnableProcessProperty]
        [Example("C:/Cases/MyCase")]
        [RubyArgument("pathArg", 1)]

        public IRunnableProcess<string> CasePath { get; set; } = null!;
    }
}