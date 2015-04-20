﻿using PS.Mothership.Core.Common.Template.Cust;
using PS.Mothership.Core.Common.Template.Gen;
using System;
using System.Runtime.Serialization;
using System.Collections.Generic;
using PS.Mothership.Core.Common.Dto.Contact;

namespace PS.Mothership.Core.Common.Dto.Merchant
{
    [DataContract]
    public class MerchantDto
    {
        [DataMember]
        public Guid MerchantGuid { get; set; }

        [DataMember]
        public long? V1MerchantId { get; set; }

        [DataMember]
        public string LocatorId { get; set; }

        [DataMember]
        public int ThompsonCodeKey { get; set; }

        [DataMember]
        public long AnnualTurnover { get; set; }

        [DataMember]
        public CustNumberEmployeesEnum NumberEmployeesKey { get; set; }

        [DataMember]
        public GenBusinessLegalTypeEnum BusinessLegalTypeKey { get; set; }
       
        [DataMember]
        public int CurrentTradingBankKey { get; set; }

        [DataMember]
        public GenNoContactReasonEnum CallRestrictedReasonKey { get; set; }

        [DataMember]
        public int EmailRestrictedReasonKey { get; set; }

        [DataMember]
        public Guid AddressGuid { get; set; }

        [DataMember]
        public FullAddressDto Address { get; set; }

        [DataMember]
        public Guid PhoneGuid { get; set; }

        [DataMember]
        public PhoneNumberDto Phone { get; set; }

        [DataMember]
        public Guid FaxGuid { get; set; }

        [DataMember]
        public PhoneNumberDto Fax { get; set; }

        [DataMember]
        public Guid EmailAddressGuid { get; set; }

        [DataMember]
        public string Website { get; set; }

        [DataMember]
        public string CreditPreScreenFlag { get; set; }

        [DataMember]
        public string ExperianBusinessURN { get; set; }

        [DataMember]
        public string ExperianLocationURN { get; set; }

        [DataMember]
        public DateTime? ExperianLastUpdate { get; set; }

        [DataMember]
        public Guid UpdateSessionGuid { get; set; }

        [DataMember]
        public DateTimeOffset UpdateDate { get; set; }

        [DataMember]
        public Guid OwnershipUserGuid { get; set; }

        [DataMember]
        public IList<ContactDto> Contacts { get; set; }

        public MerchantDto()
        {
            this.Contacts = new List<ContactDto>();
        }       
    }
}
