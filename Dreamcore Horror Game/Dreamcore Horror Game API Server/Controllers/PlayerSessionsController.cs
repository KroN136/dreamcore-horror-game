﻿using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.PropertyPredicates;
using DreamcoreHorrorGameApiServer.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class PlayerSessionsController : DatabaseEntityController<PlayerSession>
{
    public PlayerSessionsController
    (
        DreamcoreHorrorGameContext context,
        IPropertyPredicateValidator propertyPredicateValidator
    )
    : base
    (
        context: context,
        propertyPredicateValidator: propertyPredicateValidator,
        orderBySelectorExpression: playerSession => playerSession.StartTimestamp,
        getAllWithFirstLevelRelationsFunction: async (context) =>
        {
            var gameSessions = await context.GameSessions.ToListAsync();
            var players = await context.Players.ToListAsync();
            var creatures = await context.Creatures.ToListAsync();

            var playerSessions = context.PlayerSessions.AsQueryable();

            await playerSessions.ForEachAsync(playerSession =>
            {
                playerSession.GameSession.PlayerSessions.Clear();
                playerSession.Player.PlayerSessions.Clear();
                playerSession.UsedCreature?.PlayerSessions.Clear();
            });

            return playerSessions;
        },
        setRelationsFromForeignKeysFunction: async (context, playerSession) =>
        {
            var gameSession = await context.GameSessions
                .FindAsync(playerSession.GameSessionId);

            var player = await context.Players
                .FindAsync(playerSession.PlayerId);

            var usedCreature = await context.Creatures
                .FindAsync(playerSession.UsedCreatureId);

            if (gameSession is null || player is null || (usedCreature is null && playerSession.UsedCreatureId is not null))
                throw new InvalidConstraintException();

            playerSession.GameSession = gameSession;
            playerSession.GameSessionId = Guid.Empty;

            playerSession.Player = player;
            playerSession.PlayerId = Guid.Empty;

            if (usedCreature is not null)
            {
                playerSession.UsedCreature = usedCreature;
                playerSession.UsedCreatureId = Guid.Empty;
            }
        }
    )
    { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAll(int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllEntitiesAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAllWithRelations(int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllEntitiesWithRelationsAsync(page, showBy);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public override async Task<IActionResult> Get(Guid? id)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .GetEntityAsync(id);

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayerOrServer)]
    public override async Task<IActionResult> GetWithRelations(Guid? id)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .GetEntityWithRelationsAsync(id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetWhere(PropertyPredicate[] predicateCollection, int page = 0, int showBy = 0)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetEntitiesWhereAsync(predicateCollection, page, showBy);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    public override async Task<IActionResult> Create([Bind(
        nameof(PlayerSession.Id),
        nameof(PlayerSession.GameSessionId),
        nameof(PlayerSession.PlayerId),
        nameof(PlayerSession.StartTimestamp),
        nameof(PlayerSession.EndTimestamp),
        nameof(PlayerSession.IsCompleted),
        nameof(PlayerSession.IsWon),
        nameof(PlayerSession.TimeAlive),
        nameof(PlayerSession.PlayedAsCreature),
        nameof(PlayerSession.UsedCreatureId),
        nameof(PlayerSession.SelfReviveCount),
        nameof(PlayerSession.AllyReviveCount)
    )] PlayerSession playerSession)
        => await RequireHeaders(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .CreateEntityAsync(playerSession);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloperOrServer)]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(PlayerSession.Id),
        nameof(PlayerSession.GameSessionId),
        nameof(PlayerSession.PlayerId),
        nameof(PlayerSession.StartTimestamp),
        nameof(PlayerSession.EndTimestamp),
        nameof(PlayerSession.IsCompleted),
        nameof(PlayerSession.IsWon),
        nameof(PlayerSession.TimeAlive),
        nameof(PlayerSession.PlayedAsCreature),
        nameof(PlayerSession.UsedCreatureId),
        nameof(PlayerSession.SelfReviveCount),
        nameof(PlayerSession.AllyReviveCount)
    )] PlayerSession playerSession)
        => await RequireHeaders(CorsHeaders.GameServer, CorsHeaders.DeveloperWebApplication)
            .EditEntityAsync(id, playerSession);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteEntityAsync(id);
}
