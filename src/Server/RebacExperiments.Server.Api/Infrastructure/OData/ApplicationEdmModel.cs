﻿// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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

            // Configure EntitySets
            modelBuilder.EntitySet<User>("Users");
            modelBuilder.EntitySet<Team>("Teams");
            modelBuilder.EntitySet<Organization>("Organizations");
            modelBuilder.EntitySet<Language>("Languages");
            modelBuilder.EntitySet<TaskItem>("TaskItems");
            modelBuilder.EntitySet<StoredRelationTuple>("RelationTuples");

            // Configure EnumTypes
            modelBuilder.EnumType<TaskItemStatusEnum>().RemoveMember(TaskItemStatusEnum.None);
            modelBuilder.EnumType<TaskItemPriorityEnum>().RemoveMember(TaskItemPriorityEnum.None);

            // Configure EntityTypes
            modelBuilder.EntityType<User>()
                .Ignore(x => x.HashedPassword);

            // Configure Singletons
            modelBuilder.Singleton<User>("Me");
            
            // Configure Unbound Functions
            modelBuilder
                .Function("GetCurrentStoreId")
                .Returns<string>();

            // Configure Bound Functions
            modelBuilder.EntityType<StoredRelationTuple>().Collection
                .Function("GetCurrentRelationTuples")
                .ReturnsFromEntitySet<StoredRelationTuple>("RelationTuples");

            // Authentication
            RegisterSignInUserAction(modelBuilder);
            RegisterSignOutUserAction(modelBuilder);

            // Extenal Authentication
            RegisterLogInGitHubFunction(modelBuilder);
            RegisterSignInGitHubFunction(modelBuilder);

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

        private static void RegisterLogInGitHubFunction(ODataConventionModelBuilder modelBuilder)
        {
            var logInGitHubFunction = modelBuilder.Function("login.github");

            logInGitHubFunction
                .Parameter<string>("redirectUrl")
                .Optional();

            logInGitHubFunction
                .Returns<bool>();

            logInGitHubFunction.HasDescription()
                .HasDescription("Start a Login with GitHub");
        }

        private static void RegisterSignInGitHubFunction(ODataConventionModelBuilder modelBuilder)
        {
            var signInGitHubFunction = modelBuilder.Function("signin.github");

            signInGitHubFunction
                .Parameter<string>("redirectUrl")
                .Optional();

            signInGitHubFunction.Returns<bool>();

            signInGitHubFunction.HasDescription()
                .HasDescription("Called by the GitHub Authentication");
        }

        private static void RegisterSignOutUserAction(ODataConventionModelBuilder modelBuilder)
        {
            var signOutUserAction = modelBuilder.Action("SignOutUser");

            signOutUserAction.HasDescription().HasDescription("SignOutUser");
        }
    }
}