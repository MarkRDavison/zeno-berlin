namespace mark.davison.berlin.api.fakesite.StoryGeneration;

public sealed class StoryGenerationService : IStoryGenerationService
{
    private readonly IStoryGenerationStateService _storyGenerationStateService;

    public StoryGenerationService(IStoryGenerationStateService storyGenerationStateService)
    {
        _storyGenerationStateService = storyGenerationStateService;
    }

    public async Task<string> GenerateStoryPage(int externalId, int? externalChapterId, CancellationToken cancellationToken)
    {

        await Task.CompletedTask;

        var info = _storyGenerationStateService.RecordGeneration(externalId, externalChapterId);

        var builder = new StringBuilder();
        using (var xml = XmlWriter.Create(builder, new XmlWriterSettings
        {
            Indent = true,
            WriteEndDocumentOnClose = true,
            OmitXmlDeclaration = true,
            ConformanceLevel = ConformanceLevel.Auto
        }))
        {

            xml.WriteDocType("html", null, null, null);

            WriteStartElementWithAttributes(xml, "html", [new("lang", ["en"])], () =>
            {
                WriteStartElementWithAttributes(xml, "head", () =>
                {
                    WriteStartElementWithAttributes(xml, "title", () => xml.WriteString(info.Title));
                });
                WriteStartElementWithAttributes(xml, "body", [new("class", ["logged-out"])], () =>
                {
                    WriteStartElementWithAttributes(xml, "div", [new("id", ["header"])]);
                    WriteStartElementWithAttributes(xml, "div", [new("id", ["inner"])], () =>
                    {
                        WriteStartElementWithAttributes(xml, "div", [
                            new("id", ["main"]),
                        new("class", ["chapters-show", "region"]),
                        new("role", ["main"])
                        ], () =>
                        {
                            WriteStartElementWithAttributes(xml, "div", [new("class", ["work"])], () =>
                            {
                                WriteStartElementWithAttributes(xml, "ul", [
                                    new("class", ["work", "navigation", "actions"]),
                                new("role",["menu"])
                                ], () =>
                                {
                                    WriteStartElementWithAttributes(xml, "li", [
                                        new("class", ["chapter"]),
                                    new("aria-haspopup",["true"])
                                    ], () =>
                                    {
                                        WriteStartElementWithAttributes(xml, "ul", [
                                            new("id", ["chapter_index"]),
                                        new("class",["expandable", "secondary", "hidden"])
                                        ], () =>
                                        {
                                            WriteStartElementWithAttributes(xml, "li", () =>
                                            {
                                                WriteStartElementWithAttributes(xml, "form", () =>
                                                {
                                                    WriteStartElementWithAttributes(xml, "p", () =>
                                                    {
                                                        WriteStartElementWithAttributes(xml, "select", [
                                                            new("name", ["selected_id"]),
                                                            new("id" ,["selected_id"])
                                                        ], () =>
                                                        {
                                                            int chapterIndex = 1;
                                                            foreach (var (chapterName, chapterId) in info.Chapters.Zip(info.ChapterIds))
                                                            {
                                                                var attributes = new List<Tuple<string, List<string>>>
                                                                {
                                                                new("value", [chapterId.ToString()])
                                                                };
                                                                if (externalChapterId == chapterId)
                                                                {
                                                                    attributes.Add(new("selected", ["selected"]));
                                                                }

                                                                WriteStartElementWithAttributes(xml, "option", attributes, () => xml.WriteString($"{chapterIndex}. {chapterName}"));
                                                                chapterIndex++;
                                                            }
                                                        });
                                                    });
                                                });
                                            });
                                        });
                                    });
                                });
                                WriteStartElementWithAttributes(xml, "div", [new("class", ["wrapper"])], () =>
                                {
                                    WriteStartElementWithAttributes(xml, "dl", [new("class", ["work", "meta", "group"])], () =>
                                    {
                                        WriteStartElementWithAttributes(xml, "dt", [new("class", ["fandom", "tags"])], () => xml.WriteString("Fandom:"));
                                        WriteStartElementWithAttributes(xml, "dd", [new("class", ["fandom", "tags"])], () =>
                                        {
                                            WriteStartElementWithAttributes(xml, "ul", [new("class", ["commas"])], () =>
                                            {
                                                foreach (var fandom in info.Fandoms)
                                                {
                                                    var fandomHref = "/tags/" + fandom.Replace(" ", "%20");
                                                    WriteStartElementWithAttributes(xml, "li", () =>
                                                    {
                                                        WriteStartElementWithAttributes(xml, "a", [new("class", ["tag"]), new("href", [fandomHref])], () => xml.WriteString(fandom));
                                                    });
                                                }
                                            });
                                        });
                                        WriteStartElementWithAttributes(xml, "dt", [new("class", ["stats"])], () => xml.WriteString("Stats:"));
                                        WriteStartElementWithAttributes(xml, "dd", [new("class", ["stats"])], () =>
                                        {
                                            WriteStartElementWithAttributes(xml, "dl", [new("class", ["stats"])], () =>
                                            {
                                                WriteStartElementWithAttributes(xml, "dt", [new("class", ["published"])], () => xml.WriteString("Published:"));
                                                WriteStartElementWithAttributes(xml, "dd", [new("class", ["published"])], () => xml.WriteString(info.Published.ToString("yyyy-MM-dd")));
                                                WriteStartElementWithAttributes(xml, "dt", [new("class", ["status"])], () => xml.WriteString("Updated:"));
                                                WriteStartElementWithAttributes(xml, "dd", [new("class", ["status"])], () => xml.WriteString(info.Updated.ToString("yyyy-MM-dd")));
                                                WriteStartElementWithAttributes(xml, "dt", [new("class", ["words"])], () => xml.WriteString("Words:"));
                                                WriteStartElementWithAttributes(xml, "dd", [new("class", ["words"])], () => xml.WriteString(info.Words.ToString("N0")));
                                                WriteStartElementWithAttributes(xml, "dt", [new("class", ["chapters"])], () => xml.WriteString("Chapters:"));
                                                WriteStartElementWithAttributes(xml, "dd", [new("class", ["chapters"])], () => xml.WriteString($"{info.CurrentChapters}/{(info.TotalChapters?.ToString() ?? "?")}"));
                                                WriteStartElementWithAttributes(xml, "dt", [new("class", ["comments"])], () => xml.WriteString("Comments:"));
                                                WriteStartElementWithAttributes(xml, "dd", [new("class", ["comments"])], () => xml.WriteString(info.Comments.ToString("N0")));
                                                WriteStartElementWithAttributes(xml, "dt", [new("class", ["kudos"])], () => xml.WriteString("Kudos:"));
                                                WriteStartElementWithAttributes(xml, "dd", [new("class", ["kudos"])], () => xml.WriteString(info.Kudos.ToString("N0")));
                                                WriteStartElementWithAttributes(xml, "dt", [new("class", ["bookmarks"])], () => xml.WriteString("Bookmarks:"));
                                                WriteStartElementWithAttributes(xml, "dd", [new("class", ["bookmarks"])], () =>
                                                {
                                                    var bookmarksRef = $"/works/{externalId}/bookmarks";
                                                    WriteStartElementWithAttributes(xml, "a", [new("href", [bookmarksRef])], () => xml.WriteString(info.Bookmarks.ToString("N0")));
                                                });
                                                WriteStartElementWithAttributes(xml, "dt", [new("class", ["hits"])], () => xml.WriteString("Hits:"));
                                                WriteStartElementWithAttributes(xml, "dd", [new("class", ["hits"])], () => xml.WriteString(info.Hits.ToString("N0")));
                                            });
                                        });
                                    });
                                });
                                WriteStartElementWithAttributes(xml, "div", [new("id", ["workskin"])], () =>
                                {
                                    WriteStartElementWithAttributes(xml, "div", [new("class", ["preface", "group"])], () =>
                                    {
                                        WriteStartElementWithAttributes(xml, "h2", [new("class", ["title", "heading"])], () => xml.WriteString(info.Title));
                                        WriteStartElementWithAttributes(xml, "h3", [new("class", ["byline", "heading"])], () =>
                                        {
                                            bool firstAuthor = true;
                                            foreach (var author in info.Authors)
                                            {
                                                if (!firstAuthor)
                                                {
                                                    xml.WriteString(", ");
                                                }
                                                WriteStartElementWithAttributes(xml, "a", [
                                                    new("rel", ["author"]),
                                                new("href", [$"/users/{author}/pseuds/{author}"])
                                                    ], () =>
                                                {
                                                    xml.WriteString(author);
                                                });
                                                firstAuthor = false;
                                            }
                                        });
                                        WriteStartElementWithAttributes(xml, "div", [new("class", ["summary", "module"])], () =>
                                        {
                                            WriteStartElementWithAttributes(xml, "h3", [new("class", ["heading"])], () => xml.WriteString("Summary:"));

                                            WriteStartElementWithAttributes(xml, "blockquote", [new("class", ["userstuff"])], () =>
                                            {
                                                foreach (var summaryBlock in info.Summary)
                                                {
                                                    WriteStartElementWithAttributes(xml, "p", () => xml.WriteString(summaryBlock));
                                                }
                                            });
                                        });
                                        WriteStartElementWithAttributes(xml, "div", [new("class", ["notes", "module"])], () =>
                                        {
                                            WriteStartElementWithAttributes(xml, "h3", [new("class", ["heading"])], () => xml.WriteString("Notes:"));

                                            WriteStartElementWithAttributes(xml, "blockquote", [new("class", ["userstuff"])], () =>
                                            {
                                                WriteStartElementWithAttributes(xml, "p", () => xml.WriteString(info.Notes));
                                            });
                                        });
                                    });
                                });
                            });
                        });
                    });
                    WriteStartElementWithAttributes(xml, "div", [new("id", ["footer"])]);
                });
            });

        }

        var res = builder.ToString().Replace("<?xml version=\"1.0\"?>", "");

        return res;
    }

    private static void WriteStartElementWithAttributes(
        XmlWriter writer,
        string element,
        Action? inner = null)
    {
        WriteStartElementWithAttributes(writer, element, null, inner);
    }
    private static void WriteStartElementWithAttributes(
        XmlWriter writer,
        string element,
        List<Tuple<string, List<string>>>? attributes = null,
        Action? inner = null)
    {
        writer.WriteStartElement(element);
        if (attributes != null)
        {
            foreach (var (name, values) in attributes)
            {
                writer.WriteAttributeString(name, string.Join(' ', values));
            }
        }

        inner?.Invoke();

        writer.WriteEndElement();
    }

}