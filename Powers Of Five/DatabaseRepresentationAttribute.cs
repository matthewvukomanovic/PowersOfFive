using System;
using System.Runtime;

namespace Ross.Infrastructure.Attributes
{
    [AttributeUsage(AttributeTargets.Field)]
    public class DatabaseRepresentationAttribute : Attribute
    {
        // Fields
        public static readonly DatabaseRepresentationAttribute Default = new DatabaseRepresentationAttribute();

        // Methods
        public DatabaseRepresentationAttribute() : this(string.Empty)
        {
        }

        [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
        public DatabaseRepresentationAttribute(string representation)
        {
            RepresentationValue = representation;
        }

        public override bool Equals(object obj)
        {
            if (obj == this)
            {
                return true;
            }
            var attribute = obj as DatabaseRepresentationAttribute;
            return ((attribute != null) && (attribute.Representation == Representation));
        }

        public override int GetHashCode()
        {
            return Representation.GetHashCode();
        }

        public override bool IsDefaultAttribute()
        {
            return Equals(Default);
        }

        // Properties
        protected string RepresentationValue { get; set; }
        public virtual string Representation
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get { return RepresentationValue; }
        }

    }
}
