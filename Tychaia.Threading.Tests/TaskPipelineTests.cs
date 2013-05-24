//
// This source code is licensed in accordance with the licensing outlined
// on the main Tychaia website (www.tychaia.com).  Changes to the
// license on the website apply retroactively.
//
using System;
using System.Threading;
using Xunit;

namespace Tychaia.Threading.Tests
{
    public class TaskPipelineTests
    {
        [Fact]
        public void TestPipelineBasic()
        {
            var pipeline = new ThreadedTaskPipeline<int>();
            ThreadStart processor = () =>
            {
                pipeline.Connect();
                Assert.Equal(pipeline.Take(), 1);
                Assert.Equal(pipeline.Take(), 2);
                Assert.Equal(pipeline.Take(), 3);
            };
            pipeline.Put(1);
            pipeline.Put(2);
            pipeline.Put(3);
            var thread = new Thread(processor);
            thread.Start();
            thread.Join();
        }

        [Fact]
        public void TestPipelineParallel()
        {
            var pipeline = new ThreadedTaskPipeline<int>();
            ThreadStart processor = () =>
            {
                pipeline.Connect();
                Assert.Equal(pipeline.Take(), 1);
                Assert.Equal(pipeline.Take(), 2);
                Assert.Equal(pipeline.Take(), 3);
            };
            var thread = new Thread(processor);
            thread.Start();
            pipeline.Put(1);
            pipeline.Put(2);
            pipeline.Put(3);
            thread.Join();
        }

        [Fact]
        public void TestPipelineParallelTo1000()
        {
            var random = new Random();
            var pipeline = new ThreadedTaskPipeline<int>();
            var success = true;
            int expected = 0, actual = 0;
            ThreadStart processor = () =>
            {
                pipeline.Connect();
                for (int i = 0; i < 1000; i++)
                {
                    var v = pipeline.Take();
                    if (v != i)
                    {
                        success = false;
                        expected = i;
                        actual = v;
                        break;
                    }
                    Thread.Sleep(random.Next(1, 2));
                }
            };
            var thread = new Thread(processor);
            thread.Start();
            for (int i = 0; i < 1000; i++)
            {
                pipeline.Put(i);
                Thread.Sleep(random.Next(1, 2));
            }
            thread.Join();
            if (!success)
                Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestPipelineParallelTo1000TakeSided()
        {
            var random = new Random();
            var pipeline = new ThreadedTaskPipeline<int>();
            var success = true;
            int expected = 0, actual = 0;
            ThreadStart processor = () =>
            {
                pipeline.Connect();
                for (int i = 0; i < 1000; i++)
                {
                    var v = pipeline.Take();
                    if (v != i)
                    {
                        success = false;
                        expected = i;
                        actual = v;
                        break;
                    }
                    Thread.Sleep(1);
                }
            };
            var thread = new Thread(processor);
            thread.Start();
            for (int i = 0; i < 1000; i++)
            {
                pipeline.Put(i);
                Thread.Sleep(random.Next(1, 2));
            }
            thread.Join();
            if (!success)
                Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestPipelineParallelTo1000PutSided()
        {
            var random = new Random();
            var pipeline = new ThreadedTaskPipeline<int>();
            var success = true;
            int expected = 0, actual = 0;
            ThreadStart processor = () =>
            {
                pipeline.Connect();
                for (int i = 0; i < 1000; i++)
                {
                    var v = pipeline.Take();
                    if (v != i)
                    {
                        success = false;
                        expected = i;
                        actual = v;
                        break;
                    }
                    Thread.Sleep(random.Next(1, 2));
                }
            };
            var thread = new Thread(processor);
            thread.Start();
            for (int i = 0; i < 1000; i++)
            {
                pipeline.Put(i);
                Thread.Sleep(1);
            }
            thread.Join();
            if (!success)
                Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestPipelineParallelTo100()
        {
            var random = new Random();
            var pipeline = new ThreadedTaskPipeline<int>();
            var success = true;
            int expected = 0, actual = 0;
            ThreadStart processor = () =>
            {
                pipeline.Connect();
                for (int i = 0; i < 100; i++)
                {
                    var v = pipeline.Take();
                    if (v != i)
                    {
                        success = false;
                        expected = i;
                        actual = v;
                        break;
                    }
                    Thread.Sleep(random.Next(1, 10));
                }
            };
            var thread = new Thread(processor);
            thread.Start();
            for (int i = 0; i < 100; i++)
            {
                pipeline.Put(i);
                Thread.Sleep(random.Next(1, 10));
            }
            thread.Join();
            if (!success)
                Assert.Equal(expected, actual);
        }
        /*
        [Test]
        public void TestPipelineHighlyParallel()
        {
            var success = true;
            int expected = 0, actual = 0;
            bool[] finished = new bool[10];
            ParameterizedThreadStart container = (index) =>
            {
                finished[(int)index] = true;
                var random = new Random();
                var pipeline = new TaskPipeline<int>();
                ThreadStart processor = () =>
                {
                    pipeline.Connect();
                    for (int i = 0; i < 100; i++)
                    {
                        var v = pipeline.Take();
                        if (v != i)
                        {
                            success = false;
                            expected = i;
                            actual = v;
                            break;
                        }
                        Thread.Sleep(random.Next(1, 10));
                    }
                };
                var thread = new Thread(processor);
                thread.Start();
                for (int i = 0; i < 100; i++)
                {
                    pipeline.Put(i);
                    Thread.Sleep(random.Next(1, 10));
                }
                thread.Join();
            };
            for (int i = 0; i < 100; i++)
                new Thread(container).Start(i);
            while (true)
            {
                bool allFinished = true;
                for (int i = 0; i < 100; i++)
                    allFinished = allFinished && finished[i];
                if (allFinished)
                    break;
            }
            if (!success)
                Assert.AreEqual(expected, actual);
        }
        */
    }
}

