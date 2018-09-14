﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System;
using System.Collections.Generic;
using FakeItEasy;
using Squidex.Domain.Apps.Core.Tags;
using Squidex.Infrastructure.Queries;
using Xunit;

namespace Squidex.Domain.Apps.Entities.Assets.Queries
{
    public class QueryTagVisitorTests
    {
        private readonly ITagService tagService = A.Fake<ITagService>();
        private readonly Guid appId = Guid.NewGuid();

        [Fact]
        public void Should_normalize_tags()
        {
            A.CallTo(() => tagService.GetTagIdsAsync(appId, TagGroups.Assets, A<HashSet<string>>.That.Contains("tag1")))
                .Returns(new Dictionary<string, string> { ["tag1"] = "normalized1" });

            var source = FilterBuilder.Eq("tags", "tag1");
            var result = QueryTagVisitor.Transform(source, appId, tagService);

            Assert.Equal("tags == normalized1", result.ToString());
        }

        [Fact]
        public void Should_not_not_fail_when_not_found()
        {
            A.CallTo(() => tagService.GetTagIdsAsync(appId, TagGroups.Assets, A<HashSet<string>>.That.Contains("tag1")))
                .Returns(new Dictionary<string, string>());

            var source = FilterBuilder.Eq("tags", "tag1");
            var result = QueryTagVisitor.Transform(source, appId, tagService);

            Assert.Equal("tags == tag1", result.ToString());
        }

        [Fact]
        public void Should_not_normalize_other_field()
        {
            var source = FilterBuilder.Eq("other", "value");
            var result = QueryTagVisitor.Transform(source, appId, tagService);

            Assert.Equal("other == value", result.ToString());

            A.CallTo(() => tagService.GetTagIdsAsync(appId, A<string>.Ignored, A<HashSet<string>>.Ignored))
                .MustNotHaveHappened();
        }
    }
}
