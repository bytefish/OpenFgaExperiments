// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;
using RebacExperiments.Server.Api.Infrastructure.Authorization;
using RebacExperiments.Server.Api.Models;

namespace RebacExperiments.Server.Api.Infrastructure.OData
{
    public static class ApplicationEdmModel
    {
        public static IEdmModel GetEdmModel()
        {
            var modelBuilder = new ODataConventionModelBuilder();

            modelBuilder.Namespace = "TaskManagementService";

            modelBuilder.ComplexType<RelationTuple>();

            modelBuilder.EntitySet<Team>("Teams");
            modelBuilder.EntitySet<Organization>("Organizations");
            modelBuilder.EntitySet<TaskItem>("TaskItems");
            modelBuilder.EnumType<TaskItemStatusEnum>().RemoveMember(TaskItemStatusEnum.None);
            modelBuilder.EnumType<TaskItemPriorityEnum>().RemoveMember(TaskItemPriorityEnum.None);

            // Authentication
            RegisterSignInUserAction(modelBuilder);
            RegisterSignOutUserAction(modelBuilder);

            // Authorization
            RegisterCreateRelationTupleAction(modelBuilder);
            RegisterDeleteRelationTupleAction(modelBuilder);
            RegisterGetRelationTuplesAction(modelBuilder);

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

        private static void RegisterCreateRelationTupleAction(ODataConventionModelBuilder modelBuilder)
        {
            var signInUserAction = modelBuilder.Action("CreateRelationTuple");

            signInUserAction.HasDescription().HasDescription("CreateRelationTuple");

            signInUserAction.Parameter<RelationTuple>("tuple").Required();
        }

        private static void RegisterDeleteRelationTupleAction(ODataConventionModelBuilder modelBuilder)
        {
            var signInUserAction = modelBuilder.Action("DeleteRelationTuple");

            signInUserAction.HasDescription().HasDescription("DeleteRelationTuple");

            signInUserAction.Parameter<RelationTuple>("tuple").Required();
        }


        private static void RegisterGetRelationTuplesAction(ODataConventionModelBuilder modelBuilder)
        {
            var signInUserAction = modelBuilder.Action("GetRelationTuples");

            signInUserAction.HasDescription().HasDescription("GetRelationTuples");

            signInUserAction.Parameter<RelationTuple>("tuple").Required();
        }
    }
}