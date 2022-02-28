using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Smart.Design.Razor.TagHelpers.Icon;

namespace Smart.Design.Razor.TagHelpers.Alert;

public class AlertHtmlGenerator : IAlertHtmlGenerator
{
    /// <inheritdoc />
    public Task<TagBuilder> GenerateAlertAsync(string? title, string? message, AlertStyle alertStyle, Image icon, bool isClosable, bool isLight)
    {
        return GenerateAlertAsync(title, new List<string>() { message ?? string.Empty }, alertStyle, icon, isClosable, isLight);
    }

    private readonly IIconHtmlGenerator _iconHtmlGenerator;

    public AlertHtmlGenerator(IIconHtmlGenerator iconHtmlGenerator)
    {
        _iconHtmlGenerator = iconHtmlGenerator;
    }

    /// <inheritdoc />
    public async Task<TagBuilder> GenerateAlertAsync(string? title, List<string>? messages, AlertStyle alertStyle, Image icon, bool isClosable, bool isLight)
    {
        var alert = new TagBuilder("div");
        alert.AddCssClass("c-alert");
        alert.AddCssClass(AlertCssClassByStyle(alertStyle));

        if (isLight)
        {
            alert.AddCssClass("c-alert--light");
        }

        // First child of a Alert is an icon.
        var iconDiv = await GetIconAsync(alertStyle, icon);
        alert.InnerHtml.AppendHtml(iconDiv);

        // Next comes the body
        var alertBody = new TagBuilder("div");
        alertBody.AddCssClass("c-alert__body");
        alert.InnerHtml.AppendHtml(alertBody);

        // The body has three nested div's.
        var alertText = new TagBuilder("div");
        alertText.AddCssClass("c-alert__text");
        alertBody.InnerHtml.AppendHtml(alertText);
        if (!string.IsNullOrWhiteSpace(title))
        {
            var htmlTitle = new TagBuilder("h4");
            htmlTitle.AddCssClass("c-h4");
            htmlTitle.InnerHtml.Append(title);
            alertText.InnerHtml.AppendHtml(htmlTitle);
        }

        var alertMessage = new TagBuilder("div");
        alertMessage.AddCssClass("c-alert__message");

        alertText.InnerHtml.AppendHtml(alertMessage);

        var alertMessageContent = new TagBuilder("div");
        alertMessageContent.AddCssClass("c-content");

        alertMessage.InnerHtml.AppendHtml(alertMessageContent);

        // The generated HTML differs if there are multiple messages.
        if (messages.Count == 1)
        {
            addSingleMessage(messages, alertMessageContent);
        }
        else if (messages.Count > 1)
        {
            AddMultipleMessages(messages, alertMessageContent);
        }

        if (isClosable)
        {
            alert.InnerHtml.AppendHtml(await GenerateClosingButtonAsync());
            alert.Attributes["data-alert-closable"] = "data-alert-closable";
        }

        return alert;
    }

    private static void addSingleMessage(List<string> messages, TagBuilder alertMessageContent)
    {
        var messageParagraph = new TagBuilder("p");
        messageParagraph.InnerHtml.Append(messages[0]);
        alertMessageContent.InnerHtml.AppendHtml(messageParagraph);
    }

    private static void AddMultipleMessages(List<string> messages, TagBuilder alertMessageContent)
    {
        var messageUl = new TagBuilder("ul");
        foreach (var message in messages)
        {
            var li = new TagBuilder("li");
            li.InnerHtml.Append(message);
            messageUl.InnerHtml.AppendHtml(li);
        }

        alertMessageContent.InnerHtml.AppendHtml(messageUl);
    }

    private string AlertCssClassByStyle(AlertStyle alertStyle)
    {
        return "c-alert--" + alertStyle.ToString().ToLowerInvariant();
    }

    private async Task<TagBuilder> GetIconAsync(AlertStyle alertStyle, Image icon)
    {
        if (icon is not Image.None)
        {
            return await _iconHtmlGenerator.GenerateIconAsync(icon);
        }

        return alertStyle switch
        {
            AlertStyle.Default => await _iconHtmlGenerator.GenerateIconAsync(Image.CircleInformation),
            AlertStyle.Error   => await _iconHtmlGenerator.GenerateIconAsync(Image.CircleError),
            AlertStyle.Warning => await _iconHtmlGenerator.GenerateIconAsync(Image.Warning),
            AlertStyle.Success => await _iconHtmlGenerator.GenerateIconAsync(Image.CircleCheck),
            _                  =>
                throw new ArgumentOutOfRangeException(nameof(alertStyle), alertStyle, $"Unknown handling for {alertStyle}")
        };
    }

    private async Task<TagBuilder> GenerateClosingButtonAsync()
    {
        var button = new TagBuilder("button");
        button.AddCssClass("c-button-link c-button--borderless-muted c-button--icon");

        button.Attributes["type"] = "button";
        button.Attributes["data-alert-close"] = "data-alert-close";
        button.Attributes["aria-label"] = "Close alert";

        var span = new TagBuilder("span");
        span.AddCssClass("c-button__content");

        span.InnerHtml.AppendHtml(await _iconHtmlGenerator.GenerateIconAsync(Image.Close));

        var accessible = new TagBuilder("div");
        accessible.AddCssClass("u-sr-accessible");
        accessible.InnerHtml.Append("Alert sluiten");

        span.InnerHtml.AppendHtml(accessible);

        button.InnerHtml.AppendHtml(span);

        return button;
    }
}
