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

            // A singleton is a special one-element entity introduced in OData V4. It can be referred by its name from the service
            // root, without having to know its key and without requiring an entity set. You can learn about a Singleton in the 
            // OData Specification at: https://www.odata.org/getting-started/advanced-tutorial/.
            //
            // The OData Documentations specifically deals with the "Me" Endpoint for accessing User Profile data. So we will
            // use a Singleton here as well for resolving User data. This could be specialized, so we do not send out the Hashed
            // Password at all, which is quite a security issue for now ... maybe a DTO makes sense here.
            modelBuilder.Singleton<User>("Me");

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