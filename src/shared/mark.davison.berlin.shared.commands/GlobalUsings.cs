﻿global using mark.davison.berlin.shared.commands.Scenarios.AddFandom;
global using mark.davison.berlin.shared.commands.Scenarios.AddStory;
global using mark.davison.berlin.shared.commands.Scenarios.DeleteStory;
global using mark.davison.berlin.shared.commands.Scenarios.EditFandom;
global using mark.davison.berlin.shared.commands.Scenarios.EditStory;
global using mark.davison.berlin.shared.commands.Scenarios.Export;
global using mark.davison.berlin.shared.commands.Scenarios.Import;
global using mark.davison.berlin.shared.constants;
global using mark.davison.berlin.shared.logic.Models;
global using mark.davison.berlin.shared.logic.StoryInfo;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.AddFandom;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.AddStory;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.DeleteStory;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.EditFandom;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.EditStory;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Export;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Import;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.UpdateStories;
global using mark.davison.berlin.shared.models.dtos.Shared;
global using mark.davison.berlin.shared.models.Entities;
global using mark.davison.berlin.shared.validation;
global using mark.davison.berlin.shared.validation.Context;
global using mark.davison.common.server.abstractions.Authentication;
global using mark.davison.common.server.abstractions.CQRS;
global using mark.davison.common.server.abstractions.Notifications;
global using mark.davison.common.server.abstractions.Repository;
global using mark.davison.common.server.CQRS;
global using mark.davison.common.server.CQRS.Processors;
global using mark.davison.common.server.CQRS.Validators;
global using mark.davison.common.Services;
global using mark.davison.shared.server.services.Helpers;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using System.Linq.Expressions;
global using System.Text;
