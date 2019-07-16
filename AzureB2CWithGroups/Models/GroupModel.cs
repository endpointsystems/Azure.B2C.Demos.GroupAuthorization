using Newtonsoft.Json;

namespace AzureB2CWithGroups.Models
{
    public class GroupRoot
    {
        [JsonProperty("odata.metadata")]
        public string odatametadata { get; set; }
        public GroupModel[] value { get; set; }
    }
    public class GroupModel
    {
        [JsonProperty("odata.type")]
        public string odataType { get; set; }
        /// <summary>
        /// A string that identifies the object type. For example, for groups the value is always "Group".
        /// </summary>
        public string objectType { get; set; }

        /// <summary>
        /// A Guid that is the unique identifier for the object; for example, 12345678-9abc-def0-1234-56789abcde.
        /// </summary>
        /// <remarks>
        /// Notes: key, immutable, not nullable, unique.
        /// </remarks>
        public string objectId { get; set; }

        /// <summary>
        /// The time at which the directory object was deleted. It only applies to those directory
        /// objects which can be restored. Currently it is only supported for deleted Application objects;
        /// all other entities return null for this property.
        /// </summary>
        public object deletionTimestamp { get; set; }

        /// <summary>
        /// An optional description for the group.
        /// </summary>
        public string description { get; set; }

        /// <summary>
        /// true if this object is synced from an on-premises directory; false if this object was originally synced from an on-premises directory but is no longer synced; null if this object has never been synced from an on-premises directory (default).
        /// </summary>
        public bool? dirSyncEnabled { get; set; }

        /// <summary>
        /// The display name for the group. This property is required when a group is created and it cannot be cleared during updates.
        /// </summary>
        public string displayName { get; set; }

        /// <summary>
        /// Indicates the last time at which the object was synced with the on-premises directory.
        /// </summary>
        public object lastDirSyncTime { get; set; }

        /// <summary>
        /// The SMTP address for the group, for example, "serviceadmins@contoso.onmicrosoft.com".
        /// </summary>
        public string mail { get; set; }

        /// <summary>
        /// Specifies whether the group is mail-enabled. If the securityEnabled property is also true, the group is a mail-enabled security group; otherwise, the group is a Microsoft Exchange distribution group. Only (pure) security groups can be created using Azure AD Graph. For this reason, the property must be set false when creating a group and it cannot be updated using Azure AD Graph.
        /// </summary>
        public bool mailEnabled { get; set; }

        /// <summary>
        /// The mail alias for the group. This property must be specified when a group is created.
        /// </summary>
        public string mailNickname { get; set; }


        public string onPremisesDomainName { get; set; }

        public string onPremisesNetBiosName { get; set; }

        public string onPremisesSamAccountName { get; set; }

        /// <summary>
        /// Contains the on-premises security identifier (SID) for the group that was synchronized from on-premises to the cloud.
        /// </summary>
        public string onPremisesSecurityIdentifier { get; set; }

        /// <summary>
        /// A collection of error details that are preventing this group from being provisioned successfully.
        /// </summary>
        public object[] provisioningErrors { get; set; }

//        /// <summary>
//        /// The preferred language for an Office 365 group. Should follow ISO 639-1 Code; for example "en-US".
//        /// </summary>
//        public string preferredLanguage { get; set; }

        /// <summary>
        /// Notes: not nullable, the any operator is required for filter expressions on multi-valued properties; for more information, see Supported Queries, Filters, and Paging Options.
        /// </summary>
        public string[] proxyAddresses { get; set; }

        /// <summary>
        /// Specifies whether the group is a security group. If the mailEnabled property is also true, the group is a mail-enabled security group; otherwise it is a security group. Only (pure) security groups can be created using Azure AD Graph. For this reason, the property must be set true when creating a group.
        /// </summary>
        public bool securityEnabled { get; set; }

//        /// <summary>
//        /// Specifies an Office 365 group's color theme. Possible values are Teal, Purple, Green, Blue, Pink, Orange or Red.
//        /// </summary>
//        public string theme { get; set; }

    }
}
