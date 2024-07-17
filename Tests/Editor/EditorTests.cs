using FantasticCore.Runtime.Configuration.Core;
using NUnit.Framework;

namespace FantasticCore.Tests.Editor
{
    public class EditorTests
    {
        [Test, Description("Try get valid project FantasticCoreConfig")]
        public void WhenGetFantasticCoreConfig_ThenIsNotNull()
        {
            // Arrange
            FantasticCoreConfig.GetCurrent();
        }
    }
}