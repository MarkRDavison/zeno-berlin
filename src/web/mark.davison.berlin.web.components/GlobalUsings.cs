﻿global using Fluxor;
global using Humanizer;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Export;
global using mark.davison.berlin.shared.models.dtos.Scenarios.Commands.Import;
global using mark.davison.berlin.shared.models.dtos.Shared;
global using mark.davison.berlin.web.components.CommonCandidates.Components.Dropdown;
global using mark.davison.berlin.web.components.CommonCandidates.Confirmation;
global using mark.davison.berlin.web.components.CommonCandidates.FileUpload;
global using mark.davison.berlin.web.components.CommonCandidates.Form;
global using mark.davison.berlin.web.components.CommonCandidates.Navigation;
global using mark.davison.berlin.web.components.Forms.AddFandom;
global using mark.davison.berlin.web.components.Forms.AddStory;
global using mark.davison.berlin.web.components.Forms.AddStoryUpdate;
global using mark.davison.berlin.web.components.Forms.EditFandom;
global using mark.davison.berlin.web.features.Store;
global using mark.davison.berlin.web.features.Store.AuthorListUseCase;
global using mark.davison.berlin.web.features.Store.DashboardListUseCase;
global using mark.davison.berlin.web.features.Store.FandomListUseCase;
global using mark.davison.berlin.web.features.Store.ManageStoryUseCase;
global using mark.davison.berlin.web.features.Store.SharedStoryUseCase;
global using mark.davison.berlin.web.features.Store.StartupUseCase;
global using mark.davison.berlin.web.features.Store.StoryListUseCase;
global using mark.davison.common.client.abstractions.Authentication;
global using mark.davison.common.client.abstractions.Repository;
global using mark.davison.common.CQRS;
global using Microsoft.AspNetCore.Components;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.JSInterop;
global using MudBlazor;
global using MudBlazor.Services;
global using System.Linq.Expressions;
global using System.Text;
global using System.Text.Json;
