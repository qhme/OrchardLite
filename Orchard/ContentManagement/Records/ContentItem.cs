using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Orchard.ContentManagement.Records
{
    public abstract class ContentItem : IContent
    {
        public virtual int Id { get; set; }

        public override bool Equals(object obj)
        {
            return Equals(obj as ContentItem);
        }

        private static bool IsTransient(ContentItem obj)
        {
            return obj != null && Equals(obj.Id, default(int));
        }

        private Type GetUnproxiedType()
        {
            return GetType();
        }

        public virtual bool Equals(ContentItem other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (!IsTransient(this) &&
                !IsTransient(other) &&
                Equals(Id, other.Id))
            {
                var otherType = other.GetUnproxiedType();
                var thisType = GetUnproxiedType();
                return thisType.IsAssignableFrom(otherType) ||
                        otherType.IsAssignableFrom(thisType);
            }

            return false;
        }

        public override int GetHashCode()
        {
            if (Equals(Id, default(int)))
                return base.GetHashCode();
            return Id.GetHashCode();
        }

        public static bool operator ==(ContentItem x, ContentItem y)
        {
            return Equals(x, y);
        }

        public static bool operator !=(ContentItem x, ContentItem y)
        {
            return !(x == y);
        }

    }
}
