using System;
using Xunit;

namespace Tests
{
    public class Tests
    {
        [Fact]
        public void Test1() 
        {
        ConsoleApplication.Program.Main(null);
            Assert.True(true);
        }
    }
}
