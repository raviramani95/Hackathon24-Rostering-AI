using eSolver.Entities.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace eSolver.Entities.Constraints
{
    public class ConstraintMNOJTI : ITmpConstraint
    {
        public long? JobStartFrom;
        public long? JobStartTo;
        public long? JobEndFrom;
        public long? JobEndTo;

        public int? MaxCount;
    }
}
