// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using RebacExperiments.Server.Api.Infrastructure.Authorization;
using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Infrastructure.OData
{
    public static class ApplicationEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var modelBuilder = new ODataConventionModelBuilder();

            modelBuilder.Namespace = "TaskManagementService";

            modelBuilder.EntitySet<User>("Users");
            modelBuilder.EntitySet<Team>("Teams");
            modelBuilder.EntitySet<Organization>("Organizations");
            modelBuilder.EntitySet<TaskItem>("TaskItems");
            modelBuilder.EnumType<TaskItemStatusEnum>().RemoveMember(TaskItemStatusEnum.None);
            modelBuilder.EnumType<TaskItemPriorityEnum>().RemoveMember(TaskItemPriorityEnum.None);

            // Raw Access to the OpenFGA Tuples
            modelBuilder.EntitySet<StoredRelationTuple>("RelationTuples");

            modelBuilder
                .Function("GetCurrentStoreId")
                .Returns<string>();

            modelBuilder.EntityType<StoredRelationTuple>().Collection
                .Function("GetCurrentRelationTuples")
                .ReturnsFromEntitySet<StoredRelationTuple>("RelationTuples");

            // Authentication
            RegisterSignInUserAction(modelBuilder);
            RegisterSignOutUserAction(modelBuilder);

            // Send as Lower Camel Case Properties, so the JSON looks better:
            modelBuilder.EnableLowerCamelCase();

            return modelBuilder.GetEdmModel();
        }

        private static void RegisterSignInUserAction(ODataConventionModelBuilder modelBuilder)
        {
            var signInUserAction = modelBuilder.Action("SignInUser");

            signInUserAction.HasDescription().HasDescription("SignInUser");

            signInUserAction.Parameter<string>("username").Required();
            signInUserAction.Parameter<string>("password").Required();
            signInUserAction.Parameter<bool>("rememberMe").Required();
        }

        private static void RegisterSignOutUserAction(ODataConventionModelBuilder modelBuilder)
        {
            var signOutUserAction = modelBuilder.Action("SignOutUser");

            signOutUserAction.HasDescription().HasDescription("SignOutUser");
        }
    }
}