﻿using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Controllers.Base;
using DreamcoreHorrorGameApiServer.Models.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DreamcoreHorrorGameApiServer.Controllers;

[ApiController]
[Route(RouteNames.ApiControllerAction)]
public class AcquiredAbilitiesController : DatabaseEntityController<AcquiredAbility>
{
    public AcquiredAbilitiesController(DreamcoreHorrorGameContext context) : base(context) { }

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.Developer)]
    public override async Task<IActionResult> GetAll()
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .GetAllAsync();

    [HttpGet]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.DeveloperOrPlayer)]
    public override async Task<IActionResult> Get(Guid? id)
        => await RequireHeaders(CorsHeaders.GameClient, CorsHeaders.DeveloperWebApplication)
            .GetAsync(acquiredAbility => acquiredAbility.Id == id);

    [HttpPost]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Create([Bind(
        nameof(AcquiredAbility.Id),
        nameof(AcquiredAbility.PlayerId),
        nameof(AcquiredAbility.AbilityId),
        nameof(AcquiredAbility.AcquirementTimestamp)
    )] AcquiredAbility acquiredAbility)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .CreateAsync(acquiredAbility);

    [HttpPut]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.MediumOrFullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Edit(Guid? id, [Bind(
        nameof(AcquiredAbility.Id),
        nameof(AcquiredAbility.PlayerId),
        nameof(AcquiredAbility.AbilityId),
        nameof(AcquiredAbility.AcquirementTimestamp)
    )] AcquiredAbility acquiredAbility)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .EditAsync(id, acquiredAbility);

    [HttpDelete]
    [Authorize(AuthenticationSchemes = AuthenticationSchemes.Access, Roles = AuthenticationRoles.FullAccessDeveloper)]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> Delete(Guid? id)
        => await RequireHeaders(CorsHeaders.DeveloperWebApplication)
            .DeleteAsync(id);
}