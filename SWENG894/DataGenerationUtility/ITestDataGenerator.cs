using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SWENG894.DataGenerationUtility
{
    public interface ITestDataGenerator
    {
        public void GenerateTestData();
        public void RemoveTestData();
    }
}
