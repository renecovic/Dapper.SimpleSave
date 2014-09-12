﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using PS.Mothership.Core.Common.Dto.Message;
using PS.Mothership.Core.Common.Dto.Contact;

namespace PS.Mothership.Core.Common.Dto.Merchant
{
    [DataContract]
    public class ContactDto
    {

        [DataMember]
        public Guid ContactGuid { get; set; }

        [DataMember]
        public int SalutationKey { get; set; }

        [DataMember]
        public string FirstName { get; set; }

        [DataMember]
        public string MiddleInitial { get; set; }

        [DataMember]
        public string Surname { get; set; }

        [DataMember]
        public Guid EmailAddressGuid { get; set; }

        [DataMember]
        public Guid MainPhoneGuid { get; set; }

        [DataMember]
        public Guid MobilePhoneGuid { get; set; }

        [DataMember]
        public EmailAddressDto EmailAddress { get; set; }

        [DataMember]
        public PhoneNumberDto MainPhone { get; set; }

        [DataMember]
        public PhoneNumberDto MobilePhone { get; set; }
    }
}
