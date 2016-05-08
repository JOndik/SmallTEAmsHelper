using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EA;

namespace BPAddIn.ElementWrappers
{
    class ConstraintWrapper : IEquatable<ConstraintWrapper>
    {
        public EA.Constraint constraint { get; set; }

        public ConstraintWrapper(EA.Constraint constraint)
        {
            this.constraint = constraint;
        }

        public bool Equals(ConstraintWrapper other)
        {
            return constraint.Name.Equals(other.constraint.Name)
                && constraint.Type.Equals(other.constraint.Type)
                && constraint.Status.Equals(other.constraint.Status)
                && constraint.Notes.Equals(other.constraint.Notes);
        }
        public override bool Equals(object other)
        {
            if (other is ConstraintWrapper)
                return this.Equals((ConstraintWrapper)other);
            else
                return false;
        }
        public override int GetHashCode()
        {
            int hashKeyName = constraint.Name == null ? 0 : constraint.Name.GetHashCode();
            int hashKeyType = constraint.Type.GetHashCode();
            int hashKeyStatus = constraint.Status.GetHashCode();
            int hashKeyNotes = constraint.Notes.GetHashCode();

            return hashKeyName ^ hashKeyType ^ hashKeyStatus ^ hashKeyNotes;
        }
    }
}
