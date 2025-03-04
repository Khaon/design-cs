using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Smart.Design.Razor.TagHelpers.Base;

/// <summary>
/// Base <see cref="ITagHelper"/> class targeting html elements with an id and name attributes.
/// This class is abstract.
/// </summary>
public abstract class BaseTagHelper : TagHelper
{
    protected const string IdAttributeName = "id";
    protected const string NameAttributeName = "name";

    [HtmlAttributeName(IdAttributeName)]
    public string? Id { get; set; }

    [HtmlAttributeName(NameAttributeName)]
    public string? Name { get; set; }

    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext? ViewContext { get; set; }
}
