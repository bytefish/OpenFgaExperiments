// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Infrastructure.OData
{
    public static class ApplicationEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var modelBuilder = new ODataConventionModelBuilder();

            modelBuilder.Namespace = "TaskManagementService";

            modelBuilder.EntitySet<TaskItem>("UserTasks");
            modelBuilder.EntitySet<RelationTuple>("RelationTuples");

            modelBuilder.EnumType<TaskItemStatusEnum>().RemoveMember(TaskItemStatusEnum.None);
            modelBuilder.EnumType<TaskItemPriorityEnum>().RemoveMember(TaskItemPriorityEnum.None);

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