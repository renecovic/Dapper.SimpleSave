﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace PS.Mothership.Core.Common.Dto
{
    public class UserAssignedAndAvailableRole : Role
    {
        public UserAssignedAndAvailableRole() { }
        public UserAssignedAndAvailableRole(Role role)
            : base(role)
        {

        }
        [DataMember]
        public bool AssignedToUser { get; set; }
    }
}
