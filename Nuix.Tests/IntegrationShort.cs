﻿using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.Logging;
using Reductech.EDR.Connectors.Nuix.Steps;
using Reductech.EDR.Core;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.Steps;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Xunit;
using static Reductech.EDR.Connectors.Nuix.Tests.Constants;
using static Reductech.EDR.Core.TestHarness.StaticHelpers;

namespace Reductech.EDR.Connectors.Nuix.Tests
{

[Collection("RequiresNuixLicense")]
public partial class IntegrationShortTests
{
    private const string CasePath = @"D:\Shares\Cases\Nuix\IntegrationShort\Case";
    private const string DataPath = @"D:\Shares\Cases\Nuix\IntegrationShort\Data";
    private const string ReportPath = @"D:\Shares\Cases\Nuix\IntegrationShort\Reports";
    private const string ExportPath = @"D:\Shares\Cases\Nuix\IntegrationShort\Export";

    [AutoTheory.GenerateAsyncTheory("NuixIntegration", Category = "IntegrationShort")]

    public IEnumerable<NuixStepTestBase<NuixCreateCase, Unit>.IntegrationTestCase>
        IntegrationTestCases
    {
        get
        {
            var stepTest = new NuixStepTestBase<NuixCreateCase, Unit>.IntegrationTestCase(
                    "Sequence - create, add, ocr, search&tag, report, export",
                    new Sequence<Unit>
                    {
                        InitialSteps = new[]
                        {
                            new DeleteItem { Path = Constant(CasePath) },
                            new DeleteItem { Path = Constant(ReportPath) },
                            new DeleteItem { Path = Constant(ExportPath) },
                            new AssertTrue
                            {
                                Boolean = new Not
                                {
                                    Boolean = new NuixDoesCaseExist
                                    {
                                        CasePath = Constant(CasePath)
                                    }
                                }
                            },
                            // Create and open a case
                            new NuixCreateCase
                            {
                                CaseName     = Constant("IntegrationShort"),
                                CasePath     = Constant(CasePath),
                                Investigator = Constant("InvestigatorA")
                            },
                            new AssertTrue
                            {
                                Boolean = new NuixDoesCaseExist
                                {
                                    CasePath = Constant(CasePath)
                                }
                            },
                            new NuixOpenCase { CasePath = Constant(CasePath) },
                            // Check license and case details
                            new SetVariable<Entity>
                            {
                                Variable = new VariableName("LicenseDetails"),
                                Value    = new NuixGetLicenseDetails()
                            },
                            AssertPropertyValueEquals(
                                "LicenseDetails",
                                "Name",
                                Constant("enterprise-workstation")
                            ),
                            new SetVariable<Entity>
                            {
                                Variable = new VariableName("CaseDetails"),
                                Value    = new NuixGetCaseDetails()
                            },
                            AssertPropertyValueEquals(
                                "CaseDetails",
                                "Name",
                                Constant("IntegrationShort")
                            ),
                            AssertPropertyValueEquals(
                                "CaseDetails",
                                "Investigator",
                                Constant("InvestigatorA")
                            ),
                            // Add loose items
                            new NuixAddItem
                            {
                                Custodian = Constant("EDRM Micro"),
                                Paths     = Array(DataPath),
                                Container = Constant("INT01B0001"),
                                MimeTypeSettings = Array(
                                    Entity.Create(("mimeType", "text/plain"), ("enabled", true)),
                                    // Disable or this extracts a whole bunch of 'application/x-database-table-row'
                                    Entity.Create(
                                        ("mimeType", "text/tab-separated-values"),
                                        ("enabled", true),
                                        ("processEmbedded", false),
                                        ("processText", true)
                                    )
                                ),
                                // Upstream bug in Core. reductech/edr/core#225
                                //CustomMetadata = new CreateEntityStep(
                                //    new ReadOnlyDictionary<EntityPropertyKey, IStep>(
                                //        new Dictionary<EntityPropertyKey, IStep>
                                //        {
                                //            {
                                //                new EntityPropertyKey("EDRVersion"),
                                //                new GetApplicationVersion()
                                //            }
                                //        }
                                //    )
                                //),
                                ProcessingProfileName = Constant("Default"),
                                ProcessingSettings = Constant(
                                    Entity.Create(("calculateAuditedSize", true))
                                )
                            },
                            AssertCount(186, "custodian:\"EDRM Micro\""), AssertCount(2, "*.txt"),
                            //AssertCount(186, "custom-metadata:\"EDRVersion\":*"),
                            // Check audited size
                            AssertEquals(Constant(76858334.0), new NuixGetAuditedSize()),
                            // Add concordance file
                            new NuixAddConcordance
                            {
                                ConcordanceProfileName = Constant("IntegrationTestProfile"),
                                ConcordanceDateFormat  = Constant("yyyy-MM-dd'T'HH:mm:ss.SSSZ"),
                                FilePath               = ConcordancePath,
                                Custodian              = Constant("Reductech EDR"),
                                FolderName             = Constant("INT01B0002")
                            },
                            AssertCount(3, "custodian:\"Reductech EDR\""), AssertCount(4, "*.txt"),
                            // OCR the data
                            AssertCount(1, "deluge"), new NuixPerformOCR
                            {
                                SearchTerm =
                                    Constant(
                                        "mime-type:image/tiff AND \"Disposing-of-Digital-Debris\""
                                    ),
                                OCRProfileName = Constant("Default"),
                                SortSearch     = Constant(true),
                                SearchOptions  = Constant(Entity.Create(("limit", 10)))
                            },
                            AssertCount(2, "deluge"),
                            // Run a search and tag
                            new ForEach<Entity>
                            {
                                Array = Array(
                                    Entity.Create(("SearchTerm", "*.jpg"), ("Tag", "image")),
                                    Entity.Create(("SearchTerm", "blue"),  ("Tag", "colour"))
                                ),
                                Action = new NuixSearchAndTag
                                {
                                    SearchTerm = new EntityGetValue<StringStream>
                                    {
                                        Entity   = GetEntityVariable,
                                        Property = Constant("SearchTerm")
                                    },
                                    Tag = new EntityGetValue<StringStream>
                                    {
                                        Entity   = GetEntityVariable,
                                        Property = Constant("Tag")
                                    }
                                }
                            },
                            new NuixSearchAndExclude
                            {
                                SearchTerm      = Constant("material"),
                                ExclusionReason = Constant("notrelevant"),
                                Tag             = Constant("notrelevant")
                            },
                            AssertCount(2, "tag:notrelevant"),
                            AssertCount(2, "exclusion:notrelevant"),
                            // Create an item set from the tagged items
                            new NuixAddToItemSet
                            {
                                SearchTerm  = Constant("has-exclusion:0 AND tag:*"),
                                ItemSetName = Constant("TaggedItems")
                            },
                            AssertCount(14, "item-set:TaggedItems"),
                            new NuixRemoveFromItemSet
                            {
                                ItemSetName = Constant("TaggedItems"),
                                SearchTerm  = Constant("blue")
                            },
                            AssertCount(11, "item-set:TaggedItems"),
                            // Create a production set from the tagged items
                            new NuixAddToProductionSet
                            {
                                SearchTerm            = Constant("item-set:TaggedItems"),
                                ProductionSetName     = Constant("ExportProduction"),
                                ProductionProfilePath = TestProductionProfilePath
                            },
                            AssertCount(11, "production-set:ExportProduction"),
                            new NuixRemoveFromProductionSet
                            {
                                ProductionSetName = Constant("ExportProduction"),
                                SearchTerm        = Constant("name:IMG_17*")
                            },
                            AssertCount(9, "production-set:ExportProduction"),
                            // Write out a file type report
                            new CreateDirectory { Path = Constant(ReportPath) },
                            new FileWrite
                            {
                                Path = new PathCombine
                                {
                                    Paths = Array(ReportPath, "file-types.txt")
                                },
                                Stream = new NuixCreateReport()
                            },
                            AssertFileContains(ReportPath, "file-types.txt", "*\tkind\t*\t189"),
                            AssertFileContains(
                                ReportPath,
                                "file-types.txt",
                                "EDRM Micro\tkind\t*\t186"
                            ),
                            AssertFileContains(
                                ReportPath,
                                "file-types.txt",
                                "Reductech EDR\tkind\t*\t3"
                            ),
                            // Write out a term list
                            new FileWrite
                            {
                                Path = new PathCombine
                                {
                                    Paths = Array(ReportPath, "terms-list.txt")
                                },
                                Stream = new NuixCreateTermList()
                            },
                            AssertFileContains(ReportPath, "terms-list.txt", "garnethill\t222"),
                            AssertFileContains(ReportPath, "terms-list.txt", "cindyloh3333\t116"),
                            // Create NRT Report
                            new NuixCreateNRTReport
                            {
                                CasePath = Constant(CasePath),
                                NRTPath = Constant(
                                    Path.Join(Nuix8Path, @"user-data\Reports\Case Summary.nrt")
                                ),
                                OutputPath      = Constant(Path.Join(ReportPath, "NRT.pdf")),
                                OutputFormat    = Constant("PDF"),
                                Title           = Constant("A report"),
                                User            = Constant("Investigator"),
                                ApplicationName = Constant("NuixApp")
                            },
                            AssertFileContains(ReportPath, "NRT.pdf", "PDF-1.4"),
                            // Export concordance from the production set
                            new NuixExportConcordance
                            {
                                ProductionSetName = Constant("ExportProduction"),
                                ExportPath        = Constant(ExportPath)
                            },
                            AssertFileContains(
                                ExportPath,
                                "loadfile.dat",
                                "25858867143438cf972761a1e45249fa"
                            ),
                            AssertFileContains(
                                ExportPath,
                                "loadfile.dat",
                                "6b661c59b9cc39b84832e3b7ebee6e93"
                            ),
                            new NuixCloseConnection(),
                            // clean up
                            new DeleteItem { Path = Constant(CasePath) },
                            new DeleteItem { Path = Constant(ReportPath) },
                            new DeleteItem { Path = Constant(ExportPath) },
                            new ForEach<StringStream>
                            {
                                Array = Array(CasePath, ReportPath, ExportPath),
                                Action = AssertDirectoryDoesNotExist(
                                    GetVariable<StringStream>(VariableName.Entity)
                                )
                            }
                        }
                    }
                )
                .WithSettings(
                    NuixSettingsList.First()
                ); // Only run these tests on the latest version of nuix that we support.

            stepTest.OutputLogLevel1 = LogLevel.Debug;
            yield return stepTest;
        }
    }
}

}
