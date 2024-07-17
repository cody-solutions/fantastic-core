using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using NUnit.Framework;
using FantasticCore.Runtime;
using FantasticCore.Runtime.Configuration.Core;

namespace FantasticCore.Tests.PlayMode
{
    [TestFixture]
    public class PlayModeTests : IPrebuildSetup, IPostBuildCleanup
    {
        #region Fields

        private const string PreviousCoreInitializationSaveKey = "PREVIOUS_CORE_INITIALIZATION";
        private const float InitializeCoreTimeOut = 5.0f;
        
        private FantasticCoreConfig _coreConfig;

        #endregion

        public void Setup()
        {
            FantasticCoreConfig coreConfig = FantasticCoreConfig.GetCurrent();
#if UNITY_EDITOR
            UnityEditor.EditorPrefs.SetBool(PreviousCoreInitializationSaveKey, coreConfig.InitializeFantasticCore);
#endif
            coreConfig.SetInitializeFantasticCore(false);
        }
        
        public void Cleanup()
        {
            // ReSharper disable once RedundantAssignment
            bool state = true;
#if UNITY_EDITOR
            state = UnityEditor.EditorPrefs.GetBool(PreviousCoreInitializationSaveKey);
#endif
            FantasticCoreConfig.GetCurrent().SetInitializeFantasticCore(state);
        }

        [SetUp]
        public void SetUp()
        {
            _coreConfig = FantasticCoreConfig.GetCurrent();
        }

        [UnityTest, Description("Load scene 0 then try initialize and wait FantasticCore package")]
        public IEnumerator WhenWaitForCoreInitialization_AndLoadScene0_ThenIsInitializedShouldBeTrue()
        {
            // Arrange
            SceneManager.LoadScene(FantasticProperties.BootSceneBuildIndex);
            
            // Act
            FantasticInstance.InitializeCore(_coreConfig);
            float timer = InitializeCoreTimeOut;
            while (timer > 0.0f && !FantasticInstance.IsInitialized)
            {
                timer -= Time.deltaTime;
                yield return null;
            }
            
            // Assert
            Assert.IsTrue(FantasticInstance.IsInitialized);
        }
    }
}