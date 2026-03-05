global using Microsoft.AspNetCore.Builder;
global using Microsoft.AspNetCore.Http;
global using Microsoft.AspNetCore.Routing;
global using Microsoft.Extensions.DependencyInjection;
global using Microsoft.Extensions.Logging;

global using Core.Model.TypeId;
global using Core.Model.Delta;
global using Questionnaire.Contract.Models;
global using Questionnaire.Contract.Handlers;
global using Questionnaire.Requests;

global using QuestionnaireId = Core.Model.TypeId.StringId<Questionnaire.Contract.Models.Questionnaire>;
global using QuestionId = Core.Model.TypeId.StringId<Questionnaire.Contract.Models.Question>;
