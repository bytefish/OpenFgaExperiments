// <auto-generated/>
using Microsoft.Kiota.Abstractions.Serialization;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
namespace RebacExperiments.Shared.ApiSdk.Models {
    public class TaskItem : IAdditionalDataHolder, IParsable {
        /// <summary>Stores additional data not described in the OpenAPI description found when deserializing. Can be used for serialization as well.</summary>
        public IDictionary<string, object> AdditionalData { get; set; }
        /// <summary>The assignedTo property</summary>
        public int? AssignedTo { get; set; }
        /// <summary>The completedDateTime property</summary>
        public DateTimeOffset? CompletedDateTime { get; set; }
        /// <summary>The description property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Description { get; set; }
#nullable restore
#else
        public string Description { get; set; }
#endif
        /// <summary>The dueDateTime property</summary>
        public DateTimeOffset? DueDateTime { get; set; }
        /// <summary>The id property</summary>
        public int? Id { get; set; }
        /// <summary>The lastEditedBy property</summary>
        public int? LastEditedBy { get; set; }
        /// <summary>The reminderDateTime property</summary>
        public DateTimeOffset? ReminderDateTime { get; set; }
        /// <summary>The rowVersion property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public byte[]? RowVersion { get; set; }
#nullable restore
#else
        public byte[] RowVersion { get; set; }
#endif
        /// <summary>The taskItemPriority property</summary>
        public TaskItemPriorityEnum? TaskItemPriority { get; set; }
        /// <summary>The taskItemStatus property</summary>
        public TaskItemStatusEnum? TaskItemStatus { get; set; }
        /// <summary>The title property</summary>
#if NETSTANDARD2_1_OR_GREATER || NETCOREAPP3_1_OR_GREATER
#nullable enable
        public string? Title { get; set; }
#nullable restore
#else
        public string Title { get; set; }
#endif
        /// <summary>The validFrom property</summary>
        public DateTimeOffset? ValidFrom { get; set; }
        /// <summary>The validTo property</summary>
        public DateTimeOffset? ValidTo { get; set; }
        /// <summary>
        /// Instantiates a new TaskItem and sets the default values.
        /// </summary>
        public TaskItem() {
            AdditionalData = new Dictionary<string, object>();
        }
        /// <summary>
        /// Creates a new instance of the appropriate class based on discriminator value
        /// </summary>
        /// <param name="parseNode">The parse node to use to read the discriminator value and create the object</param>
        public static TaskItem CreateFromDiscriminatorValue(IParseNode parseNode) {
            _ = parseNode ?? throw new ArgumentNullException(nameof(parseNode));
            return new TaskItem();
        }
        /// <summary>
        /// The deserialization information for the current model
        /// </summary>
        public virtual IDictionary<string, Action<IParseNode>> GetFieldDeserializers() {
            return new Dictionary<string, Action<IParseNode>> {
                {"assignedTo", n => { AssignedTo = n.GetIntValue(); } },
                {"completedDateTime", n => { CompletedDateTime = n.GetDateTimeOffsetValue(); } },
                {"description", n => { Description = n.GetStringValue(); } },
                {"dueDateTime", n => { DueDateTime = n.GetDateTimeOffsetValue(); } },
                {"id", n => { Id = n.GetIntValue(); } },
                {"lastEditedBy", n => { LastEditedBy = n.GetIntValue(); } },
                {"reminderDateTime", n => { ReminderDateTime = n.GetDateTimeOffsetValue(); } },
                {"rowVersion", n => { RowVersion = n.GetByteArrayValue(); } },
                {"taskItemPriority", n => { TaskItemPriority = n.GetEnumValue<TaskItemPriorityEnum>(); } },
                {"taskItemStatus", n => { TaskItemStatus = n.GetEnumValue<TaskItemStatusEnum>(); } },
                {"title", n => { Title = n.GetStringValue(); } },
                {"validFrom", n => { ValidFrom = n.GetDateTimeOffsetValue(); } },
                {"validTo", n => { ValidTo = n.GetDateTimeOffsetValue(); } },
            };
        }
        /// <summary>
        /// Serializes information the current object
        /// </summary>
        /// <param name="writer">Serialization writer to use to serialize this model</param>
        public virtual void Serialize(ISerializationWriter writer) {
            _ = writer ?? throw new ArgumentNullException(nameof(writer));
            writer.WriteIntValue("assignedTo", AssignedTo);
            writer.WriteDateTimeOffsetValue("completedDateTime", CompletedDateTime);
            writer.WriteStringValue("description", Description);
            writer.WriteDateTimeOffsetValue("dueDateTime", DueDateTime);
            writer.WriteIntValue("id", Id);
            writer.WriteIntValue("lastEditedBy", LastEditedBy);
            writer.WriteDateTimeOffsetValue("reminderDateTime", ReminderDateTime);
            writer.WriteByteArrayValue("rowVersion", RowVersion);
            writer.WriteEnumValue<TaskItemPriorityEnum>("taskItemPriority", TaskItemPriority);
            writer.WriteEnumValue<TaskItemStatusEnum>("taskItemStatus", TaskItemStatus);
            writer.WriteStringValue("title", Title);
            writer.WriteDateTimeOffsetValue("validFrom", ValidFrom);
            writer.WriteDateTimeOffsetValue("validTo", ValidTo);
            writer.WriteAdditionalData(AdditionalData);
        }
    }
}