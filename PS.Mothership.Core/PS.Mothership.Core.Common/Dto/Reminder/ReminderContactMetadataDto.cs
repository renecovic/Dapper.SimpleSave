﻿using System;
using System.Runtime.Serialization;
using PS.Mothership.Core.Common.Template.Gen;

namespace PS.Mothership.Core.Common.Dto.Reminder
{
    [DataContract]
    public class ReminderContactMetadataDto
    {
        [DataMember]
        public Guid ContactGuid { get; set; }

        [DataMember]
        public string ContactName { get; set; }

        [DataMember]
        public Guid PhoneNumberGuid { get; set; }

        [DataMember]
        public string PhoneNumber { get; set; }

        [DataMember]
        public GenPhoneNumberTypeEnum PhoneNumberType { get; set; }
    }
}
