﻿global using mark.davison.berlin.api;
global using mark.davison.berlin.api.Configuration;
global using mark.davison.berlin.api.Data;
global using mark.davison.berlin.api.Ignition;
global using mark.davison.berlin.api.migrations.postgres;
global using mark.davison.berlin.api.migrations.sqlite;
global using mark.davison.berlin.api.persistence;
global using mark.davison.berlin.shared.commands;
global using mark.davison.berlin.shared.constants;
global using mark.davison.berlin.shared.logic.Ignition;
global using mark.davison.berlin.shared.logic.Settings;
global using mark.davison.berlin.shared.models;
global using mark.davison.berlin.shared.models.dtos;
global using mark.davison.berlin.shared.models.Entities;
global using mark.davison.berlin.shared.queries;
global using mark.davison.berlin.shared.validation.Ignition;
global using mark.davison.common.persistence;
global using mark.davison.common.persistence.Configuration;
global using mark.davison.common.server;
global using mark.davison.common.server.abstractions.Authentication;
global using mark.davison.common.server.abstractions.Configuration;
global using mark.davison.common.server.abstractions.CQRS;
global using mark.davison.common.server.abstractions.Health;
global using mark.davison.common.server.abstractions.Identification;
global using mark.davison.common.server.abstractions.Repository;
global using mark.davison.common.server.Authentication;
global using mark.davison.common.server.Configuration;
global using mark.davison.common.server.Endpoints;
global using mark.davison.common.server.Health;
global using mark.davison.common.server.Middleware;
global using mark.davison.common.server.Notifications.Console;
global using mark.davison.common.server.Notifications.Matrix;
global using mark.davison.common.Services;
global using mark.davison.common.source.generators.CQRS;
global using mark.davison.shared.server.services.CommonCandiates;
global using mark.davison.shared.server.services.Helpers;
global using mark.davison.shared.server.services.Igntion;
global using mark.davison.shared.services.Ignition;
global using Microsoft.AspNetCore.Authentication.JwtBearer;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Options;
global using StackExchange.Redis;
global using System.Diagnostics.CodeAnalysis;
