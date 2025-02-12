/// If you will separated to dll you need to have global using for each dll.

// Application
#region Application

global using TaxCalculator;
global using TaxCalculator.Application.Services;

global using TaxCalculator.Domain.Entities;
global using TaxCalculator.Domain.Interfaces;
global using TaxCalculator.Infrastructure.DbContexts;
global using TaxCalculator.Shared.Settings;

#endregion

// NuGet
#region Nugets

global using AutoMapper;
global using Newtonsoft.Json;
global using FluentValidation;
global using FluentValidation.AspNetCore;
global using MicroElements.Swashbuckle.FluentValidation.AspNetCore;

#endregion

global using TaxCalculator.Shared.Constants;


// System
#region System

global using System.Data;
global using Microsoft.Extensions.Options;
global using System.ComponentModel.DataAnnotations;
global using Microsoft.OpenApi.Models;

global using Microsoft.EntityFrameworkCore;
global using System.Reflection;


#endregion