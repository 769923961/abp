﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Volo.Abp.AspNetCore.Mvc;
using Volo.Abp.AspNetCore.Mvc.UI.Widgets;
using Volo.CmsKit.Reactions;

namespace Volo.CmsKit.Web.Pages.CmsKit.Shared.Components.ReactionSelection
{
    [ViewComponent(Name = "CmsReactionSelection")]
    [Widget(
        ScriptTypes = new[] {typeof(ReactionSelectionScriptBundleContributor)},
        StyleTypes = new[] {typeof(ReactionSelectionStyleBundleContributor)},
        RefreshUrl = "/CmsKitPublicWidgets/ReactionSelection"
    )]
    public class ReactionSelectionViewComponent : AbpViewComponent
    {
        protected IReactionPublicAppService ReactionPublicAppService { get; }

        protected CmsKitUiOptions Options { get; }

        public ReactionSelectionViewComponent(
            IReactionPublicAppService reactionPublicAppService,
            IOptions<CmsKitUiOptions> options)
        {
            ReactionPublicAppService = reactionPublicAppService;
            Options = options.Value;
        }

        public virtual async Task<IViewComponentResult> InvokeAsync(
            string entityType,
            string entityId)
        {
            var result = await ReactionPublicAppService.GetForSelectionAsync(entityType, entityId);

            var viewModel = new ReactionSelectionViewModel
            {
                EntityType = entityType,
                EntityId = entityId,
                Reactions = new List<ReactionViewModel>()
            };

            foreach (var reactionDto in result.Items)
            {
                viewModel.Reactions.Add(
                    new ReactionViewModel //TODO: AutoMap
                    {
                        Name = reactionDto.Reaction.Name,
                        DisplayName = reactionDto.Reaction.DisplayName,
                        Icon = Options.ReactionIcons.GetLocalizedIcon(reactionDto.Reaction.Name),
                        Count = reactionDto.Count,
                        IsSelectedByCurrentUser = reactionDto.IsSelectedByCurrentUser
                    });
            }

            return View("~/Pages/CmsKit/Shared/Components/ReactionSelection/Default.cshtml", viewModel);
        }
    }
}