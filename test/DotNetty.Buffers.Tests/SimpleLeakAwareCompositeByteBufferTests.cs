﻿// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace DotNetty.Buffers.Tests
{
    using System;
    using System.Collections.Generic;
    using DotNetty.Common;
    using Xunit;

    public class SimpleLeakAwareCompositeByteBufferTests : WrappedCompositeByteBufferTests
    {
        readonly Queue<NoopResourceLeakTracker> trackers = new Queue<NoopResourceLeakTracker>();

        protected virtual Type ByteBufferType => typeof(SimpleLeakAwareByteBuffer);

        protected sealed override IByteBuffer Wrap(CompositeByteBuffer buffer)
        {
            var tracker = new NoopResourceLeakTracker();
            var leakAwareBuf = (WrappedCompositeByteBuffer)this.Wrap(buffer, tracker);
            this.trackers.Enqueue(tracker);
            return leakAwareBuf;
        }

        protected virtual IByteBuffer Wrap(CompositeByteBuffer buffer, IResourceLeakTracker tracker) => new SimpleLeakAwareCompositeByteBuffer(buffer, tracker);

        public override void Dispose()
        {
            base.Dispose();

            for (; ; )
            {
                NoopResourceLeakTracker tracker = null;
                if (this.trackers.Count > 0)
                {
                    tracker = this.trackers.Dequeue();
                }

                if (tracker == null)
                {
                    break;
                }

                Assert.True(tracker.Closed);
            }
        }

        [Fact]
        public void WrapSlice()
        {
            IByteBuffer buf = null;

            try
            {
                buf = this.NewBuffer(8).Slice();
                Assert.IsType(this.ByteBufferType, buf);
            }
            finally
            {
                buf?.Release();
            }
        }

        [Fact]
        public void WrapSlice2()
        {
            IByteBuffer buf = null;

            try
            {
                buf = this.NewBuffer(8).Slice(0, 1);
                Assert.IsType(this.ByteBufferType, buf);
            }
            finally
            {
                buf?.Release();
            }
        }

        [Fact]
        public void WrapReadSlice()
        {
            IByteBuffer buf = null;

            try
            {
                buf = this.NewBuffer(8).ReadSlice(1);
                Assert.IsType(this.ByteBufferType, buf);
            }
            finally
            {
                buf?.Release();
            }
        }

        [Fact]
        public void WrapRetainedSlice()
        {
            IByteBuffer buf = null;
            try
            {
                buf = this.NewBuffer(8).RetainedSlice();
                Assert.IsType(this.ByteBufferType, buf);
            }
            finally
            {
                buf?.Release();
            }

            Assert.True(buf.Release());
        }

        [Fact]
        public void WrapRetainedSlice2()
        {
            IByteBuffer buf = null;
            try
            {
                buf = this.NewBuffer(8).RetainedSlice(0, 1);
                Assert.IsType(this.ByteBufferType, buf);
            }
            finally
            {
                buf?.Release();
            }

            Assert.True(buf.Release());
        }

        [Fact]
        public void WrapReadRetainedSlice()
        {
            IByteBuffer buf = null;
            try
            {
                buf = this.NewBuffer(8).ReadRetainedSlice(1);
                Assert.IsType(this.ByteBufferType, buf);
            }
            finally
            {
                buf?.Release();
            }

            Assert.True(buf.Release());
        }

        [Fact]
        public void WrapDuplicate()
        {
            IByteBuffer buf = null;
            try
            {
                buf = this.NewBuffer(8).Duplicate();
                Assert.IsType(this.ByteBufferType, buf);
            }
            finally
            {
                buf?.Release();
            }
        }

        [Fact]
        public void WrapRetainedDuplicate()
        {
            IByteBuffer buf = null;
            try
            {
                buf = this.NewBuffer(8).RetainedDuplicate();
                Assert.IsType(this.ByteBufferType, buf);
            }
            finally
            {
                buf?.Release();
            }

            Assert.True(buf.Release());
        }
    }
}
