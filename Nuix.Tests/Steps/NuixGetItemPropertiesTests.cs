﻿using Reductech.EDR.Core.Steps;

namespace Reductech.EDR.Connectors.Nuix.Tests.Steps;

public partial class
    NuixGetItemPropertiesTests : NuixStepTestBase<NuixGetItemProperties, StringStream>
{
    /// <inheritdoc />
    protected override IEnumerable<NuixIntegrationTestCase> NuixTestCases
    {
        get
        {
            yield return new NuixIntegrationTestCase(
                "Get item properties",
                SetupCase,
                new AssertTrue
                {
                    Boolean = new StringContains
                    {
                        IgnoreCase = Constant(true),
                        Substring =
                            Constant("Character Set	UTF-8	New Folder/data/Theme in Yellow.txt"),
                        String = new NuixGetItemProperties
                        {
                            PropertyRegex = Constant("(.+)"), SearchTerm = Constant("*")
                        }
                    }
                },
                new AssertTrue
                {
                    Boolean = new StringContains
                    {
                        Substring =
                            Constant("Name	Jellyfish.txt	New Folder/data/Jellyfish.txt"),
                        String = new NuixGetItemProperties
                        {
                            PropertyRegex = Constant("(.+)"),
                            ValueRegex    = Constant("(.+fish.+)"),
                            SearchTerm    = Constant("jellyfish"),
                            SortSearch    = Constant(true),
                            SearchOptions = Constant(
                                Entity.Create(("defaultFields", new[] { "name" }))
                            )
                        }
                    }
                },
                CleanupCase
            );
        }
    }
}
