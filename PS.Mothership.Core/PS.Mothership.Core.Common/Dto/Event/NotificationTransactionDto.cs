﻿using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using PS.Mothership.Core.Common.Template.Event;

namespace PS.Mothership.Core.Common.Dto.Event
{
    [DataContract]
    public class NotificationTransactionDto
    {
        [DataMember]
        public Guid NotificationGuid { get; set; }
        [DataMember]
        public Guid EventGuid { get; set; }
        [DataMember]
        public EventNotificationMethodEnum NotificationMethodKey { get; set; }
        [DataMember]
        public DateTimeOffset ScheduledDate { get; set; }
        [DataMember]
        public DateTimeOffset UpdateDate { get; set; }
        [DataMember]
        public Guid UpdateSessionGuid { get; set; }
        [DataMember]
        public EventTransactionDto Event { get; set; }
        [DataMember]
        public IList<EventTransactionDto> EventIsCurrent { get; set; }
    }
}