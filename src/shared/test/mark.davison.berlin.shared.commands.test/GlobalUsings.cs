global using mark.davison.berlin.api.persistence;
global using mark.davison.berlin.shared.commands.Scenarios.AddPotentialStory;
global using mark.davison.berlin.shared.commands.Scenarios.AddStory;
global using mark.davison.berlin.shared.commands.Scenarios.Export;
global using mark.davison.berlin.shared.commands.Scenarios.Import;
global using mark.davison.berlin.shared.commands.Scenarios.MonthlyNotifications;
global using mark.davison.berlin.shared.commands.Scenarios.UpdateStories;
global using mark.davison.berlin.shared.constants;
global using mark.davison.berlin.shared.logic.Models;
global using mark.davison.berlin.shared.logic.Settings;
global using mark.davison.berlin.shared.logic.StoryInfo;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.AddPotentialStory;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.AddStory;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Export;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Import;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.UpdateStories;
global using mark.davison.berlin.shared.models.dtos.Shared;
global using mark.davison.berlin.shared.models.Entities;
global using mark.davison.berlin.shared.server.common.candidate.test.Persistence;
global using mark.davison.berlin.shared.validation;
global using mark.davison.common.CQRS;
global using mark.davison.common.persistence;
global using mark.davison.common.server.abstractions.Authentication;
global using mark.davison.common.server.abstractions.CQRS;
global using mark.davison.common.server.abstractions.Identification;
global using mark.davison.common.server.abstractions.Notifications;
global using mark.davison.common.Services;
global using mark.davison.shared.server.services.Helpers;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;
global using Microsoft.Extensions.Options;
global using Microsoft.VisualStudio.TestTools.UnitTesting;
global using NSubstitute;
