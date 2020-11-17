﻿using System.Collections.Generic;
using Reductech.EDR.Connectors.Nuix.Steps;
using Reductech.EDR.Connectors.Nuix.Steps.Meta.ConnectionObjects;
using Reductech.EDR.Core.Entities;
using Reductech.EDR.Core.Internal;
using Reductech.EDR.Core.TestHarness;
using Reductech.EDR.Core.Util;
using Xunit.Abstractions;
using static Reductech.EDR.Connectors.Nuix.Tests.Constants;

namespace Reductech.EDR.Connectors.Nuix.Tests.Steps
{
    public class NuixAddItemTests : NuixStepTestBase<NuixAddItem, Unit>
    {
        /// <inheritdoc />
        public NuixAddItemTests(ITestOutputHelper testOutputHelper) : base(testOutputHelper) {}


        /// <inheritdoc />
        protected override IEnumerable<StepCase> StepCases
        {
            get
            {
                yield return new NuixStepCase("Add item",
                    new NuixAddItem
                    {
                        CasePath = CasePath,
                        Custodian = Constant("Mark"),
                        Paths = DataPaths,
                        FolderName = Constant("New Folder"),
                        ProcessingSettings = Constant(CreateEntity(("Foo", "Bar")))
                    },
                    Unit.Default,
                    new List<ExternalProcessAction>
                    {
                        new ExternalProcessAction(new ConnectionCommand
                        {
                            Command = "AddToCase",
                            Arguments = new Dictionary<string, object>
                            {
                                {nameof(NuixAddItem.CasePath),  CasePathString},
                                {nameof(NuixAddItem.FolderName), "New Folder"},
                                {nameof(NuixAddItem.Custodian), "Mark"},
                                {nameof(NuixAddItem.Paths), new List<string>{DataPathString}},
                                {nameof(NuixAddItem.ProcessingSettings), CreateEntity(("Foo", "Bar"))}
                            },
                            FunctionDefinition = ""
                        }, new ConnectionOutput
                        {
                            Result = new ConnectionOutputResult{Data = null}
                        })
                    }
                ).WithSettings(UnitTestSettings);


            }
        }

        /// <inheritdoc />
        protected override IEnumerable<SerializeCase> SerializeCases
        {
            get
            {
                var (step, _) = CreateStepWithDefaultOrArbitraryValues();

                yield return new SerializeCase("default",
                    step,
                    @"Do: NuixAddItem
CasePath: 'Bar0'
FolderName: 'Bar3'
Description: 'Bar2'
Custodian: 'Bar1'
Path: 'Bar5'
ProcessingProfileName: 'Bar6'
ProcessingProfilePath: 'Bar7'
ProcessingSettings: (Prop1 = 'Val8',Prop2 = 'Val9')
ParallelProcessingSettings: (Prop1 = 'Val10',Prop2 = 'Val1')
PasswordFilePath: 'Bar4'"

                    );
            }
        }

        /// <inheritdoc />
        protected override IEnumerable<NuixIntegrationTestCase> NuixTestCases {
            get
            {
                yield return new NuixIntegrationTestCase("Add file to case",
                    DeleteCaseFolder,
                    AssertCaseDoesNotExist,
                    CreateCase,
                    AssertCount(0, "*.txt"),
                    new NuixAddItem
                    {
                        CasePath = (CasePath),
                        Custodian = Constant("Mark"),
                        Paths = DataPaths,
                        FolderName = Constant("New Folder")
                    },
                    AssertCount(2, "*.txt"),
                    DeleteCaseFolder);


                yield return new NuixIntegrationTestCase("Add encrypted file to case",
                    DeleteCaseFolder,
                    AssertCaseDoesNotExist,
                    CreateCase,
                    AssertCount(0, "*"),
                    new NuixAddItem
                    {
                        CasePath = (CasePath),
                        Custodian = Constant("Mark"),
                        Paths = EncryptedDataPaths,
                        FolderName = Constant("New Folder"),
                        PasswordFilePath = (PasswordFilePath)
                    },
                    AssertCount(1, "princess"),
                    DeleteCaseFolder
                );

                yield return new NuixIntegrationTestCase("Add file to case with profile",
                    DeleteCaseFolder,
                    AssertCaseDoesNotExist,
                    CreateCase,
                    AssertCount(0, "*.txt"),
                    new NuixAddItem
                    {
                        CasePath = CasePath,
                        Custodian = Constant("Mark"),
                        Paths = DataPaths,
                        FolderName = Constant("New Folder"),
                        ProcessingProfileName = Constant("Default")
                    },
                    AssertCount(2, "*.txt"),
                    DeleteCaseFolder
                );

                yield return new NuixIntegrationTestCase("Add file to case with profile path",
                    DeleteCaseFolder,
                    AssertCaseDoesNotExist,
                    CreateCase,
                    AssertCount(0, "*.txt"),
                    new NuixAddItem
                    {
                        CasePath = CasePath,
                        Custodian = Constant("Mark"),
                        Paths = DataPaths,
                        FolderName = Constant("New Folder"),
                        ProcessingProfilePath = DefaultProcessingProfilePath
                    },
                    AssertCount(2, "*.txt"),
                    DeleteCaseFolder);


                yield return new NuixIntegrationTestCase("Add file to case with processing settings entity",
                    DeleteCaseFolder,
                    AssertCaseDoesNotExist,
                    CreateCase,
                    AssertCount(0, "*.txt"),
                    new NuixAddItem
                    {
                        CasePath = CasePath,
                        Custodian = Constant("Mark"),
                        Paths = DataPaths,
                        FolderName = Constant("New Folder"),
                        ProcessingSettings = new Constant<Entity>(new Entity(new KeyValuePair<string, EntityValue>("processText", EntityValue.Create(true.ToString()))))
                    },
                    AssertCount(2, "*.txt"),
                    DeleteCaseFolder
                );

                yield return new NuixIntegrationTestCase("Add file to case with parallel processing settings entity",
                    DeleteCaseFolder,
                    AssertCaseDoesNotExist,
                    CreateCase,
                    AssertCount(0, "*.txt"),
                    new NuixAddItem
                    {
                        CasePath = CasePath,
                        Custodian = Constant("Mark"),
                        Paths = DataPaths,
                        FolderName = Constant("New Folder"),
                        ParallelProcessingSettings = new Constant<Entity>(new Entity(new KeyValuePair<string, EntityValue>("workerCount", EntityValue.Create(1.ToString()))))
                    },
                    AssertCount(2, "*.txt"),
                    DeleteCaseFolder
                );

                yield return new NuixIntegrationTestCase("Add file to case with mime type settings",
                    DeleteCaseFolder,
                    AssertCaseDoesNotExist,
                    CreateCase,
                    AssertCount(0, "*.txt"),
                    new NuixAddItem
                    {
                        CasePath = CasePath,
                        Custodian = Constant("Mark"),
                        Paths = DataPaths,
                        FolderName = Constant("New Folder"),
                        MimeTypeSettings = Constant(EntityStream.Create(
                            CreateEntity(("mime_tye", "text/plain"), ("enabled", "true")), //These don't really do anything, just tests that it works
                            CreateEntity(("mime_tye", "application/pdf"), ("enabled", "true"))
                            ))
                    },
                    AssertCount(2, "*.txt"),
                    DeleteCaseFolder


                    );

            } }
    }
}