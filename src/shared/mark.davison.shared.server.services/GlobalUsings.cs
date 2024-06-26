﻿global using mark.davison.berlin.api.persistence;
global using mark.davison.berlin.shared.constants;
global using mark.davison.berlin.shared.logic.Models;
global using mark.davison.berlin.shared.logic.StoryInfo;
global using mark.davison.berlin.shared.models.dtos;
global using mark.davison.berlin.shared.models.Entities;
global using mark.davison.berlin.shared.validation;
global using mark.davison.common.CQRS;
global using mark.davison.common.persistence;
global using mark.davison.common.server.abstractions.Authentication;
global using mark.davison.common.server.abstractions.Configuration;
global using mark.davison.common.server.abstractions.CQRS;
global using mark.davison.common.server.CQRS.Processors;
global using mark.davison.common.server.CQRS.Validators;
global using mark.davison.common.Services;
global using mark.davison.shared.server.services.Helpers;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using StackExchange.Redis;
global using System.Diagnostics.CodeAnalysis;
global using System.Text;
global using System.Text.Json;
