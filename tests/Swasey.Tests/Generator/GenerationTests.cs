﻿using System;
using System.Linq;
using System.Threading.Tasks;

using Swasey.Tests.Helpers;

using Xunit;

namespace Swasey.Tests.Generator
{
    public class GenerationTests
    {

        [Fact]
        public async Task GenertorWorks()
        {
            var gen = new Swasey(GenerationTestHelper.DefaultGeneratorOptions(DefaultSwaggerJsonCreator.LoadJson));

            await gen.Generate(Fixtures.TestResourceListingUri);
        }

    }
}
