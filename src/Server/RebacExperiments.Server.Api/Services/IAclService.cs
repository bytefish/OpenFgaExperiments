// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using RebacExperiments.Server.Api.Infrastructure.Authorization;
using RebacExperiments.Server.Database.Models;

namespace RebacExperiments.Server.Api.Services
{
    public interface IAclService
    {
        /// <summary>
        /// Checks if a <typeparamref name="TSubjectType"/> is authorized to access an <typeparamref name="TObjectType"/>. 
        /// </summary>
        /// <typeparam name="TObjectType">Object Type</typeparam>
        /// <typeparam name="TSubjectType">Subject Type</typeparam>
        /// <param name="objectId">Object Key</param>
        /// <param name="relation">Relation</param>
        /// <param name="subjectId">SubjectKey</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns><see cref="true"/>, if the <typeparamref name="TSubjectType"/> is authorized; else <see cref="false"/></returns>
        Task<bool> CheckObjectAsync<TObjectType, TSubjectType>(int objectId, string relation, int subjectId, CancellationToken cancellationToken)
            where TObjectType : Entity
            where TSubjectType : Entity;

        /// <summary>
        /// Checks if a <see cref="User"/> is authorized to access an <typeparamref name="TObjectType"/>. 
        /// </summary>
        /// <typeparam name="TObjectType">Object Type</typeparam>
        /// <param name="objectId">Object Key</param>
        /// <param name="relation">Relation</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns><see cref="true"/>, if the <typeparamref name="TSubjectType"/> is authorized; else <see cref="false"/></returns>
        Task<bool> CheckUserObjectAsync<TObjectType>(int userId, int objectId, string relation, CancellationToken cancellationToken)
            where TObjectType : Entity;

        /// <summary>
        /// Checks if a <see cref="User"/> is authorized to access an <typeparamref name="TObjectType"/>. 
        /// </summary>
        /// <typeparam name="TObjectType">Object Type</typeparam>
        /// <param name="context">DbContext</param>
        /// <param name="objectId">Object Key</param>
        /// <param name="relation">Relation</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns><see cref="true"/>, if the <typeparamref name="TSubjectType"/> is authorized; else <see cref="false"/></returns>
        Task<bool> CheckUserObjectAsync<TObjectType>(int userId, TObjectType @object, string relation, CancellationToken cancellationToken)
            where TObjectType : Entity;

        /// <summary>
        /// Returns all <typeparamref name="TObjectType"/> for a given <typeparamref name="TSubjectType"/> and <paramref name="relation"/>.
        /// </summary>
        /// <param name="subjectId">Subject Key to resolve</param>
        /// <param name="relation">Relation between the Object and Subject</param>
        /// <returns>All <typeparamref name="TEntityType"/> the user is related to</returns>
        Task<List<TObjectType>> ListObjectsAsync<TObjectType, TSubjectType>(int subjectId, string relation, CancellationToken cancellationToken)
            where TObjectType : Entity
            where TSubjectType : Entity;


        /// <summary>
        /// Returns all <typeparamref name="TEntityType"/> for a given <paramref name="userId"/> and <paramref name="relation"/>.
        /// </summary>
        /// <param name="userId">UserID</param>
        /// <param name="relation">Relation between the User and a <typeparamref name="TEntityType"/></param>
        /// <returns>All <typeparamref name="TEntityType"/> the user is related to</returns>
        Task<List<TEntityType>> ListUserObjectsAsync<TEntityType>(int userId, string relation, CancellationToken cancellationToken)
            where TEntityType : Entity;

        /// <summary>
        /// Creates a Relationship between a <typeparamref name="TObjectType"/> and a <typeparamref name="TSubjectType"/>.
        /// </summary>
        /// <typeparam name="TObjectType">Type of the Object</typeparam>
        /// <typeparam name="TSubjectType">Type of the Subject</typeparam>
        /// <param name="objectId">Object Entity</param>
        /// <param name="relation">Relation between Object and Subject</param>
        /// <param name="subjectId">Subject Entity</param>
        /// <param name="subjectRelation">Relation to the Subject</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>An awaitable Task</returns>
        Task AddRelationshipAsync<TObjectType, TSubjectType>(int objectId, string relation, int subjectId, string? subjectRelation, CancellationToken cancellationToken = default)
            where TObjectType : Entity
            where TSubjectType : Entity;

        /// <summary>
        /// Deletes a List of Relations.
        /// </summary>
        /// <param name="tuples">Tuples to delete from the Store</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns></returns>
        Task DeleteRelationshipAsync<TObjectType, TSubjectType>(int objectId, string relation, int subjectId, string? subjectRelation, CancellationToken cancellationToken = default)
            where TObjectType : Entity
            where TSubjectType : Entity;

        /// <summary>
        /// Gets all Relationships (until I find a better way of filtering with OData).
        /// </summary>
        /// <param name="filter">Filter to apply</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>All matching stored relationships</returns>
        IQueryable<StoredRelationTuple> GetAllRelationshipsQueryable();

        /// <summary>
        /// Gets all Relationships for a given Store.
        /// </summary>
        /// <param name="storeId">Store</param>
        /// <param name="filter">Filter to apply</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>All matching stored relationships</returns>
        IQueryable<StoredRelationTuple> GetAllRelationshipsByStoreQueryable(string storeId);

        /// <summary>
        /// Creates a List of Relations.
        /// </summary>
        /// <param name="tuples">Tuples to write to the Store</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>An awaitable Task</returns>
        Task AddRelationshipsAsync(ICollection<RelationTuple> tuples, CancellationToken cancellationToken);
        
        /// <summary>
        /// Creates a List of Relations.
        /// </summary>
        /// <param name="tuples">Tuples to write to the Store</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>An awaitable Task</returns>
        Task DeleteRelationshipsAsync(ICollection<RelationTuple> tuples, CancellationToken cancellationToken);

        /// <summary>
        /// Creates a new Object-Relation-User Tuple for the given Object and Subject.
        /// </summary>
        /// <typeparam name="TObjectType">Type of the Object</typeparam>
        /// <typeparam name="TSubjectType">Type of the Subject</typeparam>
        /// <param name="objectId">Object Entity</param>
        /// <param name="relation">Relation between Object and Subject</param>
        /// <param name="subjectId">Subject Entity</param>
        /// <param name="subjectRelation">Relation to the Subject</param>
        /// <returns>Object-Relation-User Tuple</returns>
        RelationTuple GetRelationshipTuple<TObjectType, TSubjectType>(int objectId, string relation, int subjectId, string? subjectRelation)
            where TObjectType : Entity
            where TSubjectType : Entity;

        /// <summary>
        /// Reads all stored Relation Tuples off the Store.
        /// </summary>
        /// <typeparam name="TObjectType">Type of the Object</typeparam>
        /// <param name="objectId">Object Entity</param>
        /// <param name="cancellationToken">Cancellation Token</param>
        /// <returns>All Relationships found in Store</returns>
        Task<List<RelationTuple>> ReadAllRelationshipsByObjectAsync<TObjectType>(int objectId, CancellationToken cancellationToken = default)
            where TObjectType : Entity;

        Task<List<RelationTuple>> ReadAllRelationships(string? @object, string? relation, string? subject, CancellationToken cancellationToken = default);
    }
}