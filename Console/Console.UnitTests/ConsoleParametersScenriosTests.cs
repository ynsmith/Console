using Meyer.Common.Console.UnitTests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Meyer.Common.Console.UnitTests
{
    [TestClass]
    public class ConsoleParametersScenriosTests
    {
        [TestInitialize]
        public void TestInitialization()
        {
            Program.b = default;
            Program.i = default;
            Program.w = default;
            Program.v = default;
            Program.n = default;
        }

        [TestMethod]
        public void OnlyOrderedConsoleParameters()
        {
            ConsoleParameters options = new ConsoleParameters
            {
                OrderedConsoleParameters =
                {
                    new OrderedEnumConsoleParameter<Verb>(() => Program.v, ""),
                    new OrderedEnumConsoleParameter<Noun>(() => Program.n, "")
                }
            };

            options.Map("get stuff".Split(' '), false);

            Assert.AreEqual(Verb.Get, Program.v);
            Assert.AreEqual(Noun.Stuff, Program.n);
        }

        [TestMethod]
        public void OnlyNamedConsoleParameters()
        {
            ConsoleParameters options = new ConsoleParameters
            {
                NamedConsoleParameters =
                {
                    new NamedConsoleParameter<int>(new[] { "i" }, () => Program.i, ""),
                    new BooleanConsoleParameter(new[] { "b" }, () => Program.b, ""),
                    new ActionConsoleParameter(new[] { "a" }, x => Program.w = x += "w", "")
                }
            };

            options.Map("-i 8 -b -a w".Split(' '), false);

            Assert.AreEqual(8, Program.i);
            Assert.IsTrue(Program.b);
            Assert.AreEqual("ww", Program.w);
        }

        [TestMethod]
        public void MixAndMatchConsoleParameters()
        {
            ConsoleParameters options = new ConsoleParameters
            {
                OrderedConsoleParameters =
                {
                    new OrderedEnumConsoleParameter<Verb>(() => Program.v, "")
                },
                NamedConsoleParameters =
                {
                    new NamedConsoleParameter<int>(new[] { "i" }, () => Program.i, ""),
                    new BooleanConsoleParameter(new[] { "b" }, () => Program.b, ""),
                    new NamedEnumConsoleParameter<Noun>(new[] { "v" }, () => Program.n, ""),
                    new ActionConsoleParameter(new[] { "a" }, x => Program.w = x += "w", "")
                }
            };

            options.Map("get -v stuff -a w -i 8 -b".Split(' '), false);

            Assert.AreEqual(Verb.Get, Program.v);
            Assert.AreEqual(Noun.Stuff, Program.n);
            Assert.AreEqual(8, Program.i);
            Assert.IsTrue(Program.b);
            Assert.AreEqual("ww", Program.w);
        }

        [TestMethod]
        public void MixAndMatchConsoleParameters2()
        {
            ConsoleParameters options = new ConsoleParameters
            {
                OrderedConsoleParameters =
                {
                    new OrderedEnumConsoleParameter<Verb>(() => Program.v, ""),
                    new OrderedEnumConsoleParameter<Noun>(() => Program.n, "")
                },
                NamedConsoleParameters =
                {
                    new NamedConsoleParameter<int>(new[] { "i" }, () => Program.i, ""),
                    new BooleanConsoleParameter(new[] { "b" }, () => Program.b, ""),
                    new ActionConsoleParameter(new[] { "a" }, x => Program.w = x += "w", "")
                }
            };

            options.Map("get stuff -b -a w -i 8".Split(' '), false);

            Assert.AreEqual(Verb.Get, Program.v);
            Assert.AreEqual(8, Program.i);
            Assert.IsTrue(Program.b);
            Assert.AreEqual("ww", Program.w);
        }

        [TestMethod]
        public void DontPassAllOfThem()
        {
            ConsoleParameters options = new ConsoleParameters
            {
                OrderedConsoleParameters =
                {
                    new OrderedEnumConsoleParameter<Verb>(() => Program.v, ""),
                    new OrderedEnumConsoleParameter<Noun>(() => Program.n, "")
                },
                NamedConsoleParameters =
                {
                    new NamedConsoleParameter<int>(new[] { "i" }, () => Program.i, ""),
                    new BooleanConsoleParameter(new[] { "b" }, () => Program.b, ""),
                    new ActionConsoleParameter(new[] { "a" }, x => Program.w = x += "w", "")
                }
            };

            options.Map("get stuff -i 8".Split(' '), false);

            Assert.AreEqual(Verb.Get, Program.v);
            Assert.AreEqual(8, Program.i);
            Assert.IsFalse(Program.b);
            Assert.IsNull(Program.w);
        }

        [TestMethod]
        public void DuplicateValues()
        {
            ConsoleParameters options = new ConsoleParameters
            {
                OrderedConsoleParameters =
                {
                    new OrderedEnumConsoleParameter<Verb>(() => Program.v, ""),
                    new OrderedEnumConsoleParameter<Noun>(() => Program.n, "")
                },
                NamedConsoleParameters =
                {
                    new NamedConsoleParameter<int>(new[] { "i" }, () => Program.i, ""),
                    new NamedConsoleParameter<int>(new[] { "ii" }, () => Program.ii, "")
                }
            };

            options.Map("get stuff -i 8 -ii 8".Split(' '), false);

            Assert.AreEqual(Verb.Get, Program.v);
            Assert.AreEqual(8, Program.i);
            Assert.AreEqual(8, Program.ii);
            Assert.IsFalse(Program.b);
            Assert.IsNull(Program.w);
        }

        [TestMethod]
        public void CanParseMultiple()
        {
            ConsoleParameters options = new ConsoleParameters
            {
                OrderedConsoleParameters =
                {
                    new OrderedEnumConsoleParameter<Verb>(() => Program.v, ""),
                    new OrderedEnumConsoleParameter<Noun>(() => Program.n, "")
                },
                NamedConsoleParameters =
                {
                    new ActionConsoleParameter(new[] { "i" }, x => Program.i += int.Parse(x), ""),
                    new NamedConsoleParameter<int>(new[] { "ii" }, () => Program.ii, "")
                }
            };

            options.Map("get stuff -i 5 -i 4 -ii 8 -i 6".Split(' '), false);

            Assert.AreEqual(15, Program.i);
            Assert.AreEqual(8, Program.ii);
        }

        [TestMethod, ExpectedException(typeof(FormatException))]
        public void WrongDataType()
        {
            ConsoleParameters options = new ConsoleParameters
            {
                OrderedConsoleParameters =
                {
                    new OrderedEnumConsoleParameter<Verb>(() => Program.v, ""),
                    new OrderedEnumConsoleParameter<Noun>(() => Program.n, "")
                },
                NamedConsoleParameters =
                {
                    new NamedConsoleParameter<int>(new[] { "i" }, () => Program.i, ""),
                    new BooleanConsoleParameter(new[] { "b" }, () => Program.b, ""),
                    new ActionConsoleParameter(new[] { "a" }, x => Program.w = x += "w", "")
                }
            };

            options.Map("get stuff -i v -b -a w".Split(' '), false);
        }
    }
}