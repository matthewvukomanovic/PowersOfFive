using System;
using System.Runtime.Serialization;
using Ross.Infrastructure.Interfaces;

namespace Ross.Infrastructure.Core
{
    [DataContract]
    [Serializable]
    public class DependentEntity : Entity
    {
        [DataMember]
        public string DependencyType { get; set; }
        [DataMember]
        public int DependencyTypeId { get; set; }

        [DataMember]
        public string SourceEntityId { get; set; }
    }
}