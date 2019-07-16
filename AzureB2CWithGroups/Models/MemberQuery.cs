using Newtonsoft.Json;

namespace AzureB2CWithGroups.Models
{
    /// <summary>
    /// For serializing the request to the isMemberOf query.
    /// </summary>
    public class MemberQuery
    {
        /// <summary>
        /// The group to verify membership against.
        /// </summary>
        public string groupId { get; set; }
        /// <summary>
        /// The object ID to verify against the group.
        /// </summary>
        public string memberId { get; set; }
    }

    public class MemberQueryResponse
    {
        [JsonProperty("odata.metadata")]
        public string odatametadata { get; set; }
        public bool value { get; set; }

    }
}
