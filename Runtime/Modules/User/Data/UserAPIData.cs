using System;

namespace FantasticCore.Runtime.Modules.User.Data
{
    [Serializable]
    public class UserData
    {
        #region Properties

        public string Id { get; set; }

        public string Name { get; set; }

        public string ProfileImageId { get; set; }

        #endregion
    }
}