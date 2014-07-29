using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Tests
{
    public static class Mock
    {
        public static Mock<T> Strict<T>() where T:class
        {
            return new Mock<T>(MockBehavior.Strict);
        }
    }
}
