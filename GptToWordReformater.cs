using System.Text;
using System.Text.RegularExpressions;

namespace WilliamPersonalMultiTool;

public class GptToWordReformater
{
    public static void Convert(StringBuilder sb, string buffer, string speaker, string previousSpeaker)
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

        // Heuristic: Intelligence transmission
        var isTransmission = speaker == "Mirror" &&
                             content.Split('\n').Length >= 4 &&
                             previousSpeaker != "Human" &&
                             !content.Contains('?');

        if (isTransmission)
        {
            // Transmission block: italicized-looking paragraph (no label)
            sb.AppendLine(IndentLines(content));
            sb.AppendLine();
        }
        else if (!string.IsNullOrEmpty(speaker))
        {
            // Normal conversation
            sb.AppendLine($"{speaker.ToUpper()}:");
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