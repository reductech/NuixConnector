﻿using System.Collections.Generic;
using Reductech.EDR.Connectors.Nuix.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Xunit.Abstractions;
using static Reductech.EDR.Connectors.Nuix.Tests.Constants;

namespace Reductech.EDR.Connectors.Nuix.Tests.Steps
{
    public class NuixExtractEntitiesTests : NuixStepTestBase<NuixExtractEntities, Unit>
    {
        /// <inheritdoc />
        public NuixExtractEntitiesTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper)
        {
        }


        /// <inheritdoc />
        protected override IEnumerable<DeserializeCase> DeserializeCases
        {
            get { yield break; }

        }

        /// <inheritdoc />
        protected override IEnumerable<NuixIntegrationTestCase> NuixTestCases {
            get
            {
                yield return new NuixIntegrationTestCase("Extract Entities",
                    DeleteCaseFolder,
                    DeleteOutputFolder,
                    CreateOutputFolder,
                    CreateCase,
                    //Note - we have to add items with a special profile in order to extract entities
                    new NuixAddItem
                    {
                        CasePath = CasePath,
                        Custodian = Constant("Mark"),
                        Path = DataPath,
                        FolderName = Constant("New Folder"),
                        ProcessingProfileName = Constant("ExtractEntities")
                    },
                    new NuixExtractEntities
                    {
                        CasePath = CasePath,
                        OutputFolder = Constant(OutputFolder)
                    },
                    AssertFileContains(OutputFolder, "email.txt", "Marianne.Moore@yahoo.com"),

                    DeleteCaseFolder,
                    DeleteOutputFolder
                );


            } }
    }
}