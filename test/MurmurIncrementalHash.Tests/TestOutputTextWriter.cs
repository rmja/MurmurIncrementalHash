using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit.Abstractions;

namespace MurmurIncrementalHash.Tests
{
    internal class TestOutputTextWriter : TextWriter
    {
        private readonly ITestOutputHelper _output;

        public override Encoding Encoding => Encoding.UTF8;

        public TestOutputTextWriter(ITestOutputHelper output) => _output = output;

        public override void WriteLine(string? message) => _output.WriteLine(message);

        public override void WriteLine(string format, params object?[] args) => _output.WriteLine(format, args);

        public override void Write(char value) => throw new NotSupportedException();
    }
}
