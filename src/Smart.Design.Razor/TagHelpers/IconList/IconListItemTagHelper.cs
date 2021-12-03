using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Smart.Design.Razor.TagHelpers.Constants;
using Smart.Design.Razor.TagHelpers.Extensions;
using Smart.Design.Razor.TagHelpers.Icon;

namespace Smart.Design.Razor.TagHelpers.IconList;

[HtmlTargetElement(TagNames.IconListItemTagName, ParentTag = TagNames.IconListTagName, TagStructure = TagStructure.NormalOrSelfClosing)]
public class IconListItemTagHelper : TagHelper
{
    private const string IconAttributeName = "icon";
    private const string LabelAttributeName = "label";

    private readonly IIconListHtmlGenerator _iconListHtmlGenerator;

    [HtmlAttributeName(IconAttributeName)]
    public Image Icon { get; set; }

    [HtmlAttributeName(LabelAttributeName)]
    public string? Label { get; set; }

    public IconListItemTagHelper(IIconListHtmlGenerator iconListHtmlGenerator)
    {
        _iconListHtmlGenerator = iconListHtmlGenerator;
    }

    public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        var iconListItem = await _iconListHtmlGenerator.GenerateIconListItemAsync(Icon, Label).ConfigureAwait(false);

        output.TagMode = TagMode.StartTagAndEndTag;
        output.ClearAndMergeAttributes(iconListItem);
        output.Content.SetHtmlContent(iconListItem.InnerHtml);
    }
}
