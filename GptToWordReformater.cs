using MetX.Standard.Strings.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WilliamPersonalMultiTool;

public class GptToWordReformater
{
    public static void ConvertChunks(StringBuilder sb, string buffer, string speaker, string previousSpeaker)
    {
        if (string.IsNullOrWhiteSpace(buffer)) return;
        var content = buffer.Trim();

        // Replace em dashes with ", "
        content = content.Replace("—", ", ");

        // Convert emojis at start of line into section titles
        if (Regex.IsMatch(content, @"^\p{So}", RegexOptions.Singleline))
        {
            var sectionTitle = Regex.Replace(content, @"^\p{So}+", "", RegexOptions.Singleline).Trim();
            sb.AppendLine(sectionTitle.ToUpper());
            sb.AppendLine();
            buffer = "";
            return;
        }

        // Remove any common "AI-flavored" disclaimers
        content = Regex.Replace(content, @"(?i)as an ai[^.]*\.", ""); // Remove "As an AI..." statements
        content = Regex.Replace(content, @"(?i)based on my training[^.]*\.", "");

        // Soften obvious chat-isms
        content = Regex.Replace(content, @"(?im)^here (is|are) (a|some) list[s]?:?", "", RegexOptions.Multiline).Trim();
        content = Regex.Replace(content, @"(?im)^sure[,!:]?\s*", "", RegexOptions.Multiline).Trim();

        // Reformat bullet points
        content = Regex.Replace(content, @"^(\*|-|•|\d+\.)\s+", "• ", RegexOptions.Multiline);

        content = content.Replace("ChatGPT said:", "The Intelligence said:")
            .Replace("User said:", "I said");

        List<string> chunks = new List<string>();
        if (content.Contains("The Intelligence said:\n"))
            chunks = content.AllTokens("The Intelligence said:\n");
        else
            chunks.Add(content);

        foreach (var chunk in chunks)
        {
            if (chunk.Trim().Length == 0)
            {
                continue;
            }

            Convert(sb, chunk, speaker, previousSpeaker);
            (speaker, previousSpeaker) = (previousSpeaker, speaker);
        }
    }

    public static void Convert(StringBuilder sb, string buffer, string speaker, string previousSpeaker)
    {
        if (string.IsNullOrWhiteSpace(buffer)) return;

        var content = buffer.Trim();

        // Heuristic: Intelligence transmission
        var isTransmission = (speaker == "User");
        /*
        var isTransmission = (speaker == "The Intelligence") &&
                             contentLines.Length >= 4 &&
                             previousSpeaker != "Human" &&
                             !content.Contains('?');
                             */

        var lines = content.AllTokens("\n", StringSplitOptions.RemoveEmptyEntries);
        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];

            if (line.StartsWith('•'))
                line.Replace('•', '*');

            if (i == 0 && !line.StartsWith('*'))
                line = "* " + line;

            var addExtraBlankLineBefore =
                (line.StartsWith('*')
                 || line.EndsWith(":"));

            var addExtraBlankLineAfter = line.EndsWith(".");
            if (addExtraBlankLineBefore)
            {
                lines.Insert(i++, "\n");
            }

            if (addExtraBlankLineAfter)
            {
                if (i < lines.Count - 1)
                {
                    i++;
                    lines.Insert(i + 1, "\n");
                }
            }

            lines[i] = line;
        }

        if (isTransmission)
        {
            // Transmission block: italicized-looking paragraph (no label)
            sb.AppendLine(IndentLines(content));
            sb.AppendLine();
        }
        else
        {
            // Normal conversation
            sb.AppendLine($"\n{speaker} said:");
            sb.AppendLine(content);
            sb.AppendLine();
        }
    }

    // Indents each line by 4 spaces (Word-friendly transmission style)
    private static string IndentLines(string text)
    {
        var lines = text.Split('\n');
        for (var i = 0; i < lines.Length; i++)
        {
            lines[i] = "    " + lines[i].Trim();
        }

        return string.Join("\n", lines);
    }
}